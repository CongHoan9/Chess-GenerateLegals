using System.Numerics;
using System.Runtime.CompilerServices;
namespace Chess
{
    public struct PinnedInfo
    {
        public ulong Pinned;
        public int[] Pinner;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PinnedInfo()
        {
            Pinned = 0UL;
            Pinner = new int[64];
            Array.Fill(Pinner, -1);
        }
    }
    public static class Generate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GenerateMoves(Board_Bit board, Span<Move> moves, bool capturesonly = false)
        {
            Piece_Color us = board.Curent;
            Piece_Color them = us ^ Piece_Color.Black;
            int opponentKingSq = BitOperations.TrailingZeroCount(board.GetPieceBB(GetKing(them)));
            ulong opponentKingMask = 1UL << opponentKingSq;
            ulong checkers = GetCheckers(board, us);
            int checkCount = BitOperations.PopCount(checkers);
            int kingsquare = BitOperations.TrailingZeroCount(board.GetPieceBB(GetKing(us)));
            ulong blockMask = 0;
            if (checkCount == 1 && Support_Bit.TryGetSingleSquare(checkers, out int checkerSq))
            {
                blockMask = Attacks.GetBetweenMask(kingsquare, checkerSq) | (1UL << checkerSq);
            }
            ulong ownOcc = us == Piece_Color.White ? board.WhiteOcc : board.BlackOcc;
            ulong theirOcc = board.Occupied ^ ownOcc;
            ulong targetmask = capturesonly ? theirOcc : (checkCount == 1 ? blockMask : ~ownOcc);
            targetmask &= ~opponentKingMask;
            Generate_Context context = new(board, moves, Attacks.GetPinnedInfo(board, us), checkers, kingsquare, targetmask, capturesonly);
            if (checkCount >= 2)
            {
                GenerateKingMoves(ref context);
                return context.Count;
            }
            GeneratePawnMoves(ref context);
            GenerateRookMoves(ref context);
            GenerateKingMoves(ref context);
            GenerateQueenMoves(ref context);
            GenerateBishopMoves(ref context);
            GenerateKnightMoves(ref context);
            GenerateCastlingMoves(ref context);
            GenerateEnPassantMoves(ref context);
            return context.Count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateKnightMoves(ref Generate_Context context)
        {
            ulong pieces = context.Board.GetPieceBB(GetKnight(context.Us));
            while (Support_Bit.TryPopLsb(ref pieces, out int from))
            {
                ulong pinMask = GetPinnedAllowedMask(ref context, from);
                ulong attacks = Attacks.GetKnightAttacks(from) & context.TargetMask & pinMask;
                AddMovesFromAttacks(ref context, from, attacks);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddMovesFromAttacks(ref Generate_Context context, int from, ulong attacks)
        {
            while (Support_Bit.TryPopLsb(ref attacks, out int to))
            {
                Move_Flags flag = (context.TheirOcc & (1UL << to)) != 0 ? Move_Flags.Capture : Move_Flags.Quiet;
                context.Add(new Move(from, to, (int)flag));
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateQueenMoves(ref Generate_Context context)
        {
            ulong pieces = context.Board.GetPieceBB(GetQueen(context.Us));
            while (Support_Bit.TryPopLsb(ref pieces, out int from))
            {
                ulong pinMask = GetPinnedAllowedMask(ref context, from);
                ulong attacks = Magic.GetQueenAttacks(from, context.Occupied) & context.TargetMask & pinMask;
                AddMovesFromAttacks(ref context, from, attacks);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateBishopMoves(ref Generate_Context context)
        {
            ulong pieces = context.Board.GetPieceBB(GetBishop(context.Us));
            while (Support_Bit.TryPopLsb(ref pieces, out int from))
            {
                ulong pinMask = GetPinnedAllowedMask(ref context, from);
                ulong attacks = Magic.GetBishopAttacks(from, context.Occupied) & context.TargetMask & pinMask;
                AddMovesFromAttacks(ref context, from, attacks);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateRookMoves(ref Generate_Context context)
        {
            ulong pieces = context.Board.GetPieceBB(GetRook(context.Us));
            while (Support_Bit.TryPopLsb(ref pieces, out int from))
            {
                ulong pinMask = GetPinnedAllowedMask(ref context, from);
                ulong attacks = Magic.GetRookAttacks(from, context.Occupied) & context.TargetMask & pinMask;
                AddMovesFromAttacks(ref context, from, attacks);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateKingMoves(ref Generate_Context context)
        {
            int from = context.Kingsquare;
            ulong candidates = Attacks.GetKingAttacks(from) & ~context.OwnOcc;
            while (Support_Bit.TryPopLsb(ref candidates, out int to))
            {
                // occ sau khi king di chuyển đến to (xóa from, thêm/set to)
                ulong occ_new = context.Occupied ^ (1UL << from) ^ (1UL << to);
                // Safe nếu không bị enemy attack tại vị trí mới
                if (context.Board.AttackersTo(to, context.Them, occ_new) == 0)
                {
                    Move_Flags flag = ((1UL << to) & context.TheirOcc) != 0 ? Move_Flags.Capture : Move_Flags.Quiet;
                    context.Add(new Move(from, to, (int)flag));
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateCastlingMoves(ref Generate_Context ctx)
        {
            if (ctx.Checkers != 0) return; 

            int kingFrom = ctx.Kingsquare;
            Piece_Color us = ctx.Us;
            Piece_Color them = ctx.Them;
            byte rights = ctx.Board.CastlingRights;
            if (us == Piece_Color.White)
            {
                if (kingFrom != 4) return;
                if (ctx.Board.AttackersTo(4, them, ctx.Occupied) != 0)
                    return;
                // Kingside
                if ((rights & 1) != 0 && (ctx.Occupied & 0x60UL) == 0) // f1 g1 empty
                {
                    ulong occNew = ctx.Occupied ^ (1UL << 4) ^ (1UL << 7) ^ (1UL << 6) ^ (1UL << 5);
                    if (ctx.Board.AttackersTo(6, them, occNew) == 0 && ctx.Board.AttackersTo(5, them, occNew) == 0)
                        ctx.Add(new Move(4, 6, (int)Move_Flags.CastleKS));
                }
                // Queenside
                if ((rights & 2) != 0 && (ctx.Occupied & 0x0EUL) == 0) // b1 c1 d1 empty
                {
                    ulong occNew = ctx.Occupied ^ (1UL << 4) ^ (1UL << 0) ^ (1UL << 2) ^ (1UL << 3);
                    if (ctx.Board.AttackersTo(2, them, occNew) == 0 && ctx.Board.AttackersTo(3, them, occNew) == 0)
                        ctx.Add(new Move(4, 2, (int)Move_Flags.CastleQS));
                }
            }
            else // Black
            {
                if (kingFrom != 60) return;
                if (ctx.Board.AttackersTo(60, them, ctx.Occupied) != 0)
                    return;
                // Kingside
                if ((rights & 4) != 0 && (ctx.Occupied & (0x60UL << 56)) == 0)
                {
                    ulong occNew = ctx.Occupied ^ (1UL << 60) ^ (1UL << 63) ^ (1UL << 62) ^ (1UL << 61);
                    if (ctx.Board.AttackersTo(62, them, occNew) == 0 && ctx.Board.AttackersTo(61, them, occNew) == 0)
                        ctx.Add(new Move(60, 62, (int)Move_Flags.CastleKS));
                }

                // Queenside
                if ((rights & 8) != 0 && (ctx.Occupied & (0x0EUL << 56)) == 0)
                {
                    ulong occNew = ctx.Occupied ^ (1UL << 60) ^ (1UL << 56) ^ (1UL << 58) ^ (1UL << 59);
                    if (ctx.Board.AttackersTo(58, them, occNew) == 0 && ctx.Board.AttackersTo(59, them, occNew) == 0)
                        ctx.Add(new Move(60, 58, (int)Move_Flags.CastleQS));
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GeneratePawnMoves(ref Generate_Context context)
        {
            Piece_Color us = context.Us;
            int dir = us == Piece_Color.White ? 8 : -8;
            bool isWhite = us == Piece_Color.White;
            int promotionRankMin = isWhite ? 56 : 0;
            int promotionRankMax = isWhite ? 63 : 7;
            int doublePushFromMin = isWhite ? 8 : 48;
            int doublePushFromMax = isWhite ? 15 : 55;
            ulong pawns = context.Board.GetPieceBB(Generate.GetPawn(us));
            ulong empty = ~context.Occupied & Mask.Board; // explicit 64-bit safe
            while (Support_Bit.TryPopLsb(ref pawns, out int from))
            {
                ulong pinMask = GetPinnedAllowedMask(ref context, from);
                // ===== Single push =====
                int to1 = from + dir;
                if ((uint)to1 < 64)
                {
                    ulong to1Bit = 1UL << to1;
                    if ((to1Bit & empty & context.TargetMask & pinMask) != 0)
                    {
                        bool isPromo = (uint)to1 >= promotionRankMin && (uint)to1 <= promotionRankMax;
                        if (isPromo)
                        {
                            AddPromotionMoves(ref context, from, to1, Move_Flags.Quiet);
                        }
                        else
                        {
                            context.Add(new Move(from, to1, (int)Move_Flags.Quiet));
                        }
                    }
                    // ===== Double push (only from starting rank) =====
                    if ((uint)from >= doublePushFromMin && (uint)from <= doublePushFromMax)
                    {
                        int to2 = from + (dir << 1);
                        if ((uint)to2 < 64)
                        {
                            ulong midBit = 1UL << to1;
                            ulong to2Bit = 1UL << to2;
                            if ((midBit & empty) != 0 && (to2Bit & empty & context.TargetMask & pinMask) != 0)
                            {
                                context.Add(new Move(from, to2, (int)Move_Flags.DoublePush));
                            }
                        }
                    }
                }
                // ===== Captures (left + right) + promotion capture =====
                ulong capTargets = Attacks.GetPawnAttacks(us, from) & context.TheirOcc & context.TargetMask & pinMask;
                while (Support_Bit.TryPopLsb(ref capTargets, out int to))
                {
                    bool isPromo = (uint)to >= promotionRankMin && (uint)to <= promotionRankMax;
                    if (isPromo)
                    {
                        AddPromotionMoves(ref context, from, to, Move_Flags.Capture);
                    }
                    else
                    {
                        context.Add(new Move(from, to, (int)Move_Flags.Capture));
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GenerateEnPassantMoves(ref Generate_Context context)
        {
            if (context.EpBB == 0)
            {
                return;
            }
            if (Support_Bit.TryGetSingleSquare(context.EpBB, out int to))
            {
                ulong toBit = 1UL << to;
                if ((toBit & context.TargetMask) == 0)
                {
                    return;
                }
                ulong capturers = Attacks.GetPawnAttacks(context.Them, to) & context.Board.GetPieceBB(GetPawn(context.Us));
                while (Support_Bit.TryPopLsb(ref capturers, out int from))
                {
                    ulong pinMask = GetPinnedAllowedMask(ref context, from);
                    if ((toBit & pinMask) != 0 && !IsIllegalEnPassant(ref context, from, to))
                    {
                        context.Add(new Move(from, to, (int)Move_Flags.EnPassant));
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsIllegalEnPassant(ref Generate_Context context, int from, int to)
        {
            int capturedSq = to + (context.Us == Piece_Color.White ? -8 : 8);
            int kingSq = context.Kingsquare;
            int epRank = capturedSq >> 3;
            if ((kingSq >> 3) != epRank)
            {
                return false;
            }
            ulong occ = (context.Occupied ^ (1UL << from) ^ (1UL << capturedSq)) | (1UL << to);
            ulong possibleAttackers = Magic.GetRookAttacks(kingSq, occ) & context.TheirOcc;
            ulong rq = context.Board.GetPieceBB(GetRook(context.Them)) | context.Board.GetPieceBB(GetQueen(context.Them));
            return (possibleAttackers & rq) != 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetPinnedAllowedMask(ref Generate_Context context, int from)
        {
            if (((1UL << from) & context.PinnedInfo.Pinned) == 0)
            {
                return ~0UL;
            }
            int pinner = context.PinnedInfo.Pinner[from];
            if (pinner < 0)
            {
                return 0UL;
            }
            return Attacks.GetBetweenMask(context.Kingsquare, pinner) | (1UL << pinner);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddPromotionMoves(ref Generate_Context context, int from, int to, Move_Flags baseFlag)
        {
            int flags = (int)baseFlag | (int)Move_Flags.Promotion;
            context.Add(new Move(from, to, flags, GetQueen(context.Us)));
            context.Add(new Move(from, to, flags, GetRook(context.Us)));
            context.Add(new Move(from, to, flags, GetBishop(context.Us)));
            context.Add(new Move(from, to, flags, GetKnight(context.Us)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetCheckers(Board_Bit board, Piece_Color color) // color = bên king
        {
            Piece_Color opponent = color ^ Piece_Color.Black;
            int kingSq = BitOperations.TrailingZeroCount(board.GetPieceBB(GetKing(color)));
            ulong occ = board.Occupied;
            ulong checkers = 0;
            checkers |= Attacks.GetPawnAttacks(opponent, kingSq) & board.GetPieceBB(GetPawn(opponent));
            checkers |= Attacks.GetKnightAttacks(kingSq) & board.GetPieceBB(GetKnight(opponent));
            checkers |= Magic.GetBishopAttacks(kingSq, occ) & (board.GetPieceBB(GetBishop(opponent)) | board.GetPieceBB(GetQueen(opponent)));
            checkers |= Magic.GetRookAttacks(kingSq, occ) & (board.GetPieceBB(GetRook(opponent)) | board.GetPieceBB(GetQueen(opponent)));
            return checkers;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece_Bit GetPawn(Piece_Color color)
        {
            return color == Piece_Color.White ? Piece_Bit.WPawn : Piece_Bit.BPawn;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece_Bit GetKnight(Piece_Color color)
        {
            return color == Piece_Color.White ? Piece_Bit.WKnight : Piece_Bit.BKnight;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece_Bit GetBishop(Piece_Color color)
        {
            return color == Piece_Color.White ? Piece_Bit.WBishop : Piece_Bit.BBishop;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece_Bit GetRook(Piece_Color color)
        {
            return color == Piece_Color.White ? Piece_Bit.WRook : Piece_Bit.BRook;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece_Bit GetQueen(Piece_Color color)
        {
            return color == Piece_Color.White ? Piece_Bit.WQueen : Piece_Bit.BQueen;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece_Bit GetKing(Piece_Color color)
        {
            return color == Piece_Color.White ? Piece_Bit.WKing : Piece_Bit.BKing;
        }
    }
}