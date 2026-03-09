using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chess
{
    public static class Attacks
    {
        private static readonly ulong[] KnightAttacks = new ulong[64];
        private static readonly ulong[] KingAttacks = new ulong[64];
        private static readonly ulong[,] PawnAttacks = new ulong[2, 64];
        private static readonly ulong[,] BetweenBB = new ulong[64, 64];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Attacks()
        {
            InitKnightAttacks();
            InitKingAttacks();
            InitPawnAttacks();
            InitBetweenBB();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitKnightAttacks()
        {
            int[] dr = [2, 2, 1, 1, -1, -1, -2, -2];
            int[] df = [1, -1, 2, -2, 2, -2, 1, -1];
            for (int square = 0; square < 64; square++)
            {
                ulong attacks = 0;
                int rank = square >> 3;
                int f = square & 7;

                for (int i = 0; i < 8; i++)
                {
                    int rrank = rank + dr[i];
                    int ff = f + df[i];
                    if ((uint)rrank < 8 && (uint)ff < 8)
                    {
                        attacks |= 1UL << (rrank * 8 + ff);
                    }
                }
                KnightAttacks[square] = attacks;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitKingAttacks()
        {
            for (int square = 0; square < 64; square++)
            {
                ulong attacks = 0;
                int rank = square >> 3;
                int f = square & 7;

                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int df = -1; df <= 1; df++)
                    {
                        if (dr == 0 && df == 0)
                        {
                            continue;
                        }
                        int rrank = rank + dr, ff = f + df;
                        if (rrank >= 0 && rrank < 8 && ff >= 0 && ff < 8)
                        {
                            attacks |= 1UL << (rrank * 8 + ff);
                        }
                    }
                }
                KingAttacks[square] = attacks;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitPawnAttacks()
        {
            for (int square = 0; square < 64; square++)
            {
                PawnAttacks[0, square] = GetPawnAttacksInternal(Piece_Color.White, square);
                PawnAttacks[1, square] = GetPawnAttacksInternal(Piece_Color.Black, square);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitBetweenBB()
        {
            for (int from = 0; from < 64; from++)
            {
                int rank1 = from >> 3;
                int f1 = from & 7;
                for (int to = 0; to < 64; to++)
                {
                    if (from == to)
                    {
                        BetweenBB[from, to] = 0UL;
                        continue;
                    }
                    int rank2 = to >> 3;
                    int f2 = to & 7;
                    int dr = Math.Sign(rank2 - rank1);
                    int df = Math.Sign(f2 - f1);
                    if (rank1 != rank2 && f1 != f2 && Math.Abs(rank2 - rank1) != Math.Abs(f2 - f1))
                    {
                        BetweenBB[from, to] = 0UL;
                        continue;
                    }
                    ulong bb = 0UL;
                    int rank = rank1 + dr;
                    int f = f1 + df;
                    while (rank != rank2 || f != f2)
                    {
                        bb |= 1UL << (rank * 8 + f);
                        rank += dr;
                        f += df;
                    }
                    BetweenBB[from, to] = bb;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PawnAttacksWhiteBB(ulong pawns)
        {
            return ((pawns & ~Mask.FileA) << 7) | ((pawns & ~Mask.FileH) << 9);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PawnAttacksBlackBB(ulong pawns)
        {
            return ((pawns & ~Mask.FileH) >> 7) | ((pawns & ~Mask.FileA) >> 9);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetPawnAttacks(Piece_Color color, int square)
        {
            return PawnAttacks[(int)color, square];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong GetPawnAttacksInternal(Piece_Color color, int square)
        {
        //    if (square < 0 || square > 63)
        //    {
        //        return 0UL;
        //    }
            ulong attacks = 0UL;
            int rank = square >> 3;
            int f = square & 7;
            if (color == Piece_Color.White)
            {
                if (rank < 7)
                {
                    if (f > 0)
                    {
                        attacks |= 1UL << (square + 7);
                    }
                    if (f < 7)
                    {
                        attacks |= 1UL << (square + 9);
                    }
                }
            }
            else
            {
                if (rank > 0)
                {
                    if (f > 0)
                    {
                        attacks |= 1UL << (square - 9);
                    }
                    if (f < 7)
                    {
                        attacks |= 1UL << (square - 7);
                    }
                }
            }
            return attacks;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKnightAttacks(int square)
        {
            if (square < 0 || square > 63)
            {
                return 0UL;
            }
            return KnightAttacks[square];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKingAttacks(int square)
        {
            if (square < 0 || square > 63)
            {
                return 0UL;
            }
            return KingAttacks[square];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetBetweenMask(int from, int to)
        {
            return BetweenBB[from, to];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PinnedInfo GetPinnedInfo(Board_Bit board, Piece_Color side)
        {
            PinnedInfo info = new();
            if (!Support_Bit.TryGetSingleSquare(board.GetPieceBB(Generate.GetKing(side)), out int kingsquare))
            {
                return info;
            }
            Piece_Color opp = side ^ Piece_Color.Black;
            ulong ownOcc = side == Piece_Color.White ? board.WhiteOcc : board.BlackOcc;
            // Orthogonal pinners (rooks + queens)
            ulong sliders = board.GetPieceBB(Generate.GetRook(opp)) | board.GetPieceBB(Generate.GetQueen(opp));
            while (Support_Bit.TryPopLsb(ref sliders, out int pinnerSq))
            {
                int kr = kingsquare >> 3;
                int kf = kingsquare & 7;
                int pr = pinnerSq >> 3;
                int pf = pinnerSq & 7;
                if (kr == pr || kf == pf) // aligned orthogonally
                {
                    ulong between = GetBetweenMask(kingsquare, pinnerSq) & board.Occupied;
                    if (BitOperations.PopCount(between) == 1 && (between & ownOcc) != 0)
                    {
                        if (Support_Bit.TryGetSingleSquare(between, out int pinnedSq))
                        {
                            info.Pinned |= (1UL << pinnedSq);
                            info.Pinner[pinnedSq] = pinnerSq;
                        }
                    }
                }
            }
            // Diagonal pinners (bishops + queens)
            sliders = board.GetPieceBB(Generate.GetBishop(opp)) | board.GetPieceBB(Generate.GetQueen(opp));
            while (Support_Bit.TryPopLsb(ref sliders, out int pinnerSq))
            {
                int kr = kingsquare >> 3;
                int kf = kingsquare & 7;
                int pr = pinnerSq >> 3;
                int pf = pinnerSq & 7;
                int dr = Math.Abs(kr - pr);
                int df = Math.Abs(kf - pf);
                if (dr == df && dr != 0) 
                {
                    ulong between = GetBetweenMask(kingsquare, pinnerSq) & board.Occupied;
                    if (BitOperations.PopCount(between) == 1 && (between & ownOcc) != 0)
                    {
                        if (Support_Bit.TryGetSingleSquare(between, out int pinnedSq))
                        {
                            info.Pinned |= (1UL << pinnedSq);
                            info.Pinner[pinnedSq] = pinnerSq;
                        }
                    }
                }
            }
            return info;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOurPiece(Piece_Bit piece, Piece_Color us)
        {
            return us == Piece_Color.White ? (int)piece >= 1 && (int)piece <= 6 : (int)piece >= 7 && (int)piece <= 12;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetAttackedSquares(Board_Bit board, Piece_Color color, ulong occ = 0UL)
        {
            if (occ == 0UL)
            {
                occ = board.Occupied; // default: dùng occ hiện tại
            }
            ulong attacked = 0UL;
            // Pawn attacks (NO LOOP)
            ulong pawns = board.GetPieceBB(Generate.GetPawn(color));
            attacked |= color == Piece_Color.White ? PawnAttacksWhiteBB(pawns) : PawnAttacksBlackBB(pawns);
            // Knight attacks
            ulong knights = board.GetPieceBB(Generate.GetKnight(color));
            while (knights != 0)
            {
                if (Support_Bit.TryPopLsb(ref knights, out int square))
                {
                    attacked |= KnightAttacks[square];
                }
            }
            // King attacks
            ulong kings = board.GetPieceBB(Generate.GetKing(color));
            while (kings != 0)
            {
                if (Support_Bit.TryPopLsb(ref kings, out int square))
                {
                    attacked |= KingAttacks[square];
                }
            }
            // Bishop + Queen (diagonal) – dùng occ tùy chỉnh
            ulong bishops = board.GetPieceBB(Generate.GetBishop(color)) | board.GetPieceBB(Generate.GetQueen(color));
            while (bishops != 0)
            {
                if (Support_Bit.TryPopLsb(ref bishops, out int square))
                {
                    attacked |= Magic.GetBishopAttacks(square, occ);
                }
            }
            // Rook + Queen (straight) – dùng occ tùy chỉnh
            ulong rooks = board.GetPieceBB(Generate.GetRook(color)) | board.GetPieceBB(Generate.GetQueen(color));
            while (rooks != 0)
            {
                if (Support_Bit.TryPopLsb(ref rooks, out int square))
                {
                    attacked |= Magic.GetRookAttacks(square, occ);
                }
            }
            return attacked;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AttackersToMasked(Board_Bit board, int sq, Piece_Color color, ulong occ)
        {
            ulong attackers = 0;

            // Pawn attackers (đúng hướng attacker)
            attackers |= board.GetPieceBB(Generate.GetPawn(color))
                         & Attacks.GetPawnAttacks(color, sq);  // attacks TO sq
            // Knight
            attackers |= board.GetPieceBB(Generate.GetKnight(color))
                         & Attacks.GetKnightAttacks(sq);
            // King (nếu cần)
            attackers |= board.GetPieceBB(Generate.GetKing(color))
                         & Attacks.GetKingAttacks(sq);
            // Bishop + Queen
            attackers |= (board.GetPieceBB(Generate.GetBishop(color)) | board.GetPieceBB(Generate.GetQueen(color)))
                         & Magic.GetBishopAttacks(sq, occ);
            // Rook + Queen
            attackers |= (board.GetPieceBB(Generate.GetRook(color)) | board.GetPieceBB(Generate.GetQueen(color)))
                         & Magic.GetRookAttacks(sq, occ);
            return attackers;
        }
    }
}