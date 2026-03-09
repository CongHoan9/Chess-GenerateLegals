using System.Runtime.CompilerServices;

namespace Chess
{
    public static class Search
    {
        private static readonly int[] PieceValues =
        [
            0,100,300,300,500,900,0,
            100,300,300,500,900,0
        ];
        private const int MaxPly = 512;
        private const int MateScore = 100000;
        private const int MaxHistory = 16384;
        private const int TableSize = 1 << 22;
        private const ulong TableMask = TableSize - 1;
        private static readonly int[] MVVLVA = new int[13 * 13];
        public static readonly int[,] History = new int[2, 64 * 64];
        private static readonly Move[,] Killer = new Move[MaxPly, 2];
        private static readonly TTEntry[] TT = new TTEntry[TableSize];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Search()
        {
            for (int a = 0; a < 13; a++)
            {
                for (int v = 0; v < 13; v++)
                {
                    MVVLVA[a * 13 + v] = PieceValues[v] * 32 - PieceValues[a];
                }
            }
        }
        struct TTEntry
        {
            public ulong Key;
            public int Score;
            public byte Flag;
            public sbyte Depth;
            public Move bestmove;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PieceValue(Piece_Bit piece)
        {
            int index = (int)piece;
            if ((uint)index >= PieceValues.Length)
            {
                return 0;
            }
            return PieceValues[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Move Root(Board_Bit board, int depth)
        {
            Span<Move> moves = stackalloc Move[218];
            int count = Generate.GenerateMoves(board, moves);
            if (count == 0)
            {
                return default;
            }
            Span<int> scores = stackalloc int[218];
            StagedMovePicker picker = new(board, moves, count, scores, default, default, default);
            int alpha = -MateScore;
            int beta = MateScore;
            int bestscore = -MateScore;
            Move bestmove = default;
            while (picker.Next(out Move move))
            {
                board.MakeMove(move);
                int score = -AlphaBeta(board, depth - 1, -beta, -alpha, 1);
                board.UnMakeMove();
                if (score > bestscore)
                {
                    bestscore = score;
                    bestmove = move;
                }
                if (score > alpha)
                {
                    alpha = score;
                }
            }
            return bestmove;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlphaBeta(Board_Bit board, int depth, int alpha = -1_000_000, int beta = 1_000_000, int ply = 0)
        {
            if (depth <= 0 || ply >= MaxPly)
            {
                return Evaluate(board);
                //return Quiescence(board, alpha, beta, ply);
            }
            ulong key = board.Zobrist_Instant;
            uint index = (uint)key ^ (uint)(key >> 32);
            index &= (uint)TableMask;
            ref TTEntry entry = ref TT[index];
            Move ttMove = default;
            if (entry.Key == key)
            {
                ttMove = entry.bestmove;
                if (entry.Depth >= depth)
                {
                    if (entry.Flag == 0)
                    {
                        return entry.Score;
                    }
                    if (entry.Flag == 1 && entry.Score >= beta)
                    {
                        return entry.Score;
                    }
                    if (entry.Flag == 2 && entry.Score <= alpha)
                    {
                        return entry.Score;
                    }
                }
            }
            Span<Move> moves = stackalloc Move[218];
            int count = Generate.GenerateMoves(board, moves);
            if (count == 0)
            {
                return board.InCheck(board.Curent) ? -MateScore + ply : 0;
            }
            Span<int> scores = stackalloc int[218];
            StagedMovePicker picker = new(board, moves, count, scores, ttMove, Killer[ply, 0], Killer[ply, 1]);
            int originalAlpha = alpha;
            int bestscore = -MateScore;
            Move bestmove = default;
            int movesTried = 0;
            bool first = true;
            while (picker.Next(out Move move))
            {
                movesTried++;
                board.MakeMove(move);
                bool quiet = !move.IsCapture && !move.IsPromotion;
                int reduction = 0;
                if (!first && depth >= 3 && movesTried >= 4 && quiet)
                {
                    reduction = 1;
                }
                int score;
                if (first)
                {
                    score = -AlphaBeta(board, depth - 1, -beta, -alpha, ply + 1);
                }
                else
                {
                    score = -AlphaBeta(board, depth - 1 - reduction, -alpha - 1, -alpha, ply + 1);
                    if (score > alpha)
                    {
                        score = -AlphaBeta(board, depth - 1, -beta, -alpha, ply + 1);
                    }
                }
                board.UnMakeMove();
                if (score >= beta)
                {
                    if (quiet)
                    {
                        Killer[ply, 1] = Killer[ply, 0];
                        Killer[ply, 0] = move;
                        int idx = move.From * 64 + move.To;
                        int c = (int)board.Curent;
                        History[c, idx] += depth * depth;
                        if (History[c, idx] > MaxHistory)
                        {
                            History[c, idx] = MaxHistory;
                        }
                    }
                    Store(key, (sbyte)depth, beta, 1, move);
                    return beta;
                }
                if (score > bestscore)
                {
                    bestscore = score;
                    bestmove = move;
                }
                if (score > alpha)
                {
                    alpha = score;
                }
                first = false;
            }
            byte flag; 
            if (bestscore <= originalAlpha)
            {
                flag = 2; // UpperBound
            } 
            else if (bestscore >= beta) 
            { 
                flag = 1; // LowerBound
            } 
            else 
            { 
                flag = 0; // Exact
            }
            Store(key, (sbyte)depth, bestscore, flag, bestmove);
            return bestscore;
        }
        public ref struct StagedMovePicker
        {
            private readonly Span<Move> Moves;
            private readonly Span<int> Scores;
            private readonly Board_Bit Board;
            private readonly Move HashMove;
            private readonly Move Killer1;
            private readonly Move Killer2;
            private readonly int Count;
            private PickerPhase Phase;
            private int Current;
            private enum PickerPhase
            {
                Hash,
                Captures,
                Killers,
                Quiets,
                Done
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public StagedMovePicker(Board_Bit board, Span<Move> moves, int count, Span<int> scores, Move hashMove, Move killer1, Move killer2)
            {
                Scores = scores[..count];
                Phase = PickerPhase.Hash;
                Moves = moves[..count];
                HashMove = hashMove;
                Killer1 = killer1;
                Killer2 = killer2;
                Board = board;
                Count = count;
                Current = 0;
                ScoreAllMoves();
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private readonly void ScoreAllMoves()
            {
                int color = (int)Board.Curent;
                for (int i = 0; i < Count; i++)
                {
                    Move m = Moves[i];
                    if (!HashMove.IsNull && m.Value == HashMove.Value)
                    {
                        Scores[i] = 2_000_000;
                        continue;
                    }
                    if (m.IsCapture || m.IsEnPassant || m.IsPromotion)
                    {
                        int victim = GetVictimValue(m);
                        int attacker = (int)Board[m.From];
                        Scores[i] = 1_000_000 + MVVLVA[attacker * 13 + victim];

                        if (m.IsPromotion)
                        {
                            Scores[i] += PieceValues[(int)m.Promotion] * 10;
                        }
                    }
                    else
                    {
                        int idx = m.From * 64 + m.To;
                        Scores[i] = History[color, idx];
                        if (m.Equals(Killer1))
                        {
                            Scores[i] += 900_000;
                        }
                        else if (m.Equals(Killer2))
                        {
                            Scores[i] += 800_000;
                        }
                        if (m.IsPromotion)
                        {
                            Scores[i] += 9_000; // ưu tiên promotion quiet
                        }
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private readonly int GetVictimValue(Move m)
            {
                if (m.IsEnPassant)
                {
                    return (int)(Board.Curent == Piece_Color.White ? Piece_Bit.BPawn : Piece_Bit.WPawn);
                }
                if (m.IsCapture)
                {
                    return (int)Board[m.To];
                }
                // Promotion quiet
                return (int)Piece_Bit.WPawn; // dummy
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private readonly void Swap(int a, int b)
            {
                (Moves[a], Moves[b]) = (Moves[b], Moves[a]);
                (Scores[a], Scores[b]) = (Scores[b], Scores[a]);
            }
            public bool Next(out Move move)
            {
                move = default;

                if (Current >= Count)
                {
                    Phase = PickerPhase.Done;
                    return false;
                }
                switch (Phase)
                {
                    case PickerPhase.Hash:
                        // Tìm và đưa hash move lên đầu
                        for (int i = Current; i < Count; i++)
                        {
                            if (Moves[i].Value == HashMove.Value)
                            {
                                Swap(i, Current);
                                move = Moves[Current++];
                                Phase = PickerPhase.Captures;
                                return true;
                            }
                        }
                        Phase = PickerPhase.Captures;
                        goto case PickerPhase.Captures;
                    case PickerPhase.Captures:
                        // Chọn capture/promotion/en-passant tốt nhất còn lại (theo MVV-LVA)
                        int bestCapIdx = -1;
                        int bestCapScore = -1;
                        for (int i = Current; i < Count; i++)
                        {
                            if (Moves[i].IsCapture || Moves[i].IsEnPassant || Moves[i].IsPromotion)
                            {
                                if (Scores[i] > bestCapScore)
                                {
                                    bestCapScore = Scores[i];
                                    bestCapIdx = i;
                                }
                            }
                        }
                        if (bestCapIdx != -1)
                        {
                            Swap(bestCapIdx, Current);
                            move = Moves[Current++];
                            return true;
                        }
                        // Hết capture → sang Killers
                        Phase = PickerPhase.Killers;
                        goto case PickerPhase.Killers;

                    case PickerPhase.Killers:
                        // Thử Killer1
                        if (!Killer1.IsNull)
                        {
                            for (int i = Current; i < Count; i++)
                            {
                                if (Moves[i].Value == Killer1.Value)
                                {
                                    Swap(i, Current);
                                    move = Moves[Current++];
                                    return true;
                                }
                            }
                        }
                        // Thử Killer2
                        if (!Killer2.IsNull)
                        {
                            for (int i = Current; i < Count; i++)
                            {
                                if (Moves[i].Value == Killer2.Value)
                                {
                                    Swap(i, Current);
                                    move = Moves[Current++];
                                    return true;
                                }
                            }
                        }
                        Phase = PickerPhase.Quiets;
                        goto case PickerPhase.Quiets;

                    case PickerPhase.Quiets:
                        int bestQuietIdx = -1;
                        int bestQuietScore = -1;
                        for (int i = Current; i < Count; i++)
                        {
                            if (!Moves[i].IsCapture && !Moves[i].IsEnPassant && !Moves[i].IsPromotion)
                            {
                                if (Scores[i] > bestQuietScore)
                                {
                                    bestQuietScore = Scores[i];
                                    bestQuietIdx = i;
                                }
                            }
                        }
                        if (bestQuietIdx != -1)
                        {
                            Swap(bestQuietIdx, Current);
                            move = Moves[Current++];
                            return true;
                        }
                        Phase = PickerPhase.Done;
                        return false;
                }

                return false;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Store(ulong key, sbyte depth, int score, byte flag, Move best)
        {
            uint index = (uint)key ^ (uint)(key >> 32);
            index &= (uint)TableMask;
            ref TTEntry e = ref TT[index];
            if (depth >= e.Depth || e.Key != key)
            {
                e.Key = key;
                e.Depth = depth;
                e.Score = score;
                e.Flag = flag;
                e.bestmove = best;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Evaluate(Board_Bit b)
        {
            int s = 0;
            s += 100 * b.Count(Piece_Bit.WPawn);
            s += 300 * b.Count(Piece_Bit.WKnight);
            s += 300 * b.Count(Piece_Bit.WBishop);
            s += 500 * b.Count(Piece_Bit.WRook);
            s += 900 * b.Count(Piece_Bit.WQueen);
            s -= 100 * b.Count(Piece_Bit.BPawn);
            s -= 300 * b.Count(Piece_Bit.BKnight);
            s -= 300 * b.Count(Piece_Bit.BBishop);
            s -= 500 * b.Count(Piece_Bit.BRook);
            s -= 900 * b.Count(Piece_Bit.BQueen);
            return b.Curent == Piece_Color.White ? s : -s;
        }
        public static ulong Perft(Board_Bit board, int depth, bool divide = false)
        {
            if (depth == 0)
            {
                return 1;
            }
            Span<Move> moves = stackalloc Move[218];
            int moveCount = Generate.GenerateMoves(board, moves);
            ulong nodes = 0;
            for (int i = 0; i < moveCount; i++)
            {
                Move move = moves[i];
                board.MakeMove(move);
                ulong count = Perft(board, depth - 1, false);
                nodes += count;
                if (divide)
                {
                    Console.WriteLine($"{i + 1}. Move {move}: {count:N0} nodes");
                }
                board.UnMakeMove();
            }
            return nodes;
        }
    }
}
