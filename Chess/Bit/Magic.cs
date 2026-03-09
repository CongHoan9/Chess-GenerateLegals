using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Chess
{
    public static class Magic
    {
        private static readonly ulong[] RookMagics =
        [
            0x0080001020400080UL, 0x0040001000200040UL, 0x0080081000200080UL, 0x0080040800100080UL,
            0x0080020400080080UL, 0x0080010200040080UL, 0x0080008001000200UL, 0x0080002040800100UL,
            0x0000800020400080UL, 0x0000400020005000UL, 0x0000801000200080UL, 0x0000800800100080UL,
            0x0000800400080080UL, 0x0000800200040080UL, 0x0000800100020080UL, 0x0000800040800100UL,
            0x0000208000400080UL, 0x0000404000201000UL, 0x0000808010002000UL, 0x0000808008001000UL,
            0x0000808004000800UL, 0x0000808002000400UL, 0x0000010100020004UL, 0x0000020000408104UL,
            0x0000208080004000UL, 0x0000200040005000UL, 0x0000100080200080UL, 0x0000080080100080UL,
            0x0000040080080080UL, 0x0000020080040080UL, 0x0000010080800200UL, 0x0000800080004100UL,
            0x0000204000800080UL, 0x0000200040401000UL, 0x0000100080802000UL, 0x0000080080801000UL,
            0x0000040080800800UL, 0x0000020080800400UL, 0x0000020001010004UL, 0x0000800040800100UL,
            0x0000204000808000UL, 0x0000200040008080UL, 0x0000100020008080UL, 0x0000080010008080UL,
            0x0000040008008080UL, 0x0000020004008080UL, 0x0000010002008080UL, 0x0000004081020004UL,
            0x0000204000800080UL, 0x0000200040008080UL, 0x0000100020008080UL, 0x0000080010008080UL,
            0x0000040008008080UL, 0x0000020004008080UL, 0x0000800100020080UL, 0x0000800041000080UL,
            0x0000102040800101UL, 0x0000102040008101UL, 0x0000081020004101UL, 0x0000040810002101UL,
            0x0001000204080011UL, 0x0001000204000801UL, 0x0001000082000401UL, 0x0000002040810402UL
        ];
        private static readonly ulong[] BishopMagics =
        [
            0x0002020202020200UL, 0x0002020202020000UL, 0x0004010202000000UL, 0x0004040080000000UL,
            0x0001104000000000UL, 0x0000821040000000UL, 0x0000410410400000UL, 0x0000104104104000UL,
            0x0000040404040400UL, 0x0000020202020200UL, 0x0000040102020000UL, 0x0000040400800000UL,
            0x0000011040000000UL, 0x0000008210400000UL, 0x0000004104104000UL, 0x0000002082082000UL,
            0x0004000808080800UL, 0x0002000404040400UL, 0x0001000202020200UL, 0x0000800802004000UL,
            0x0000800400A00000UL, 0x0000200100884000UL, 0x0000400082082000UL, 0x0000200041041000UL,
            0x0002080010101000UL, 0x0001040008080800UL, 0x0000208004010400UL, 0x0000404004010200UL,
            0x0000840000802000UL, 0x0000404002011000UL, 0x0000808001041000UL, 0x0000404000820800UL,
            0x0001041000202000UL, 0x0000820800101000UL, 0x0000104400080800UL, 0x0000020080080080UL,
            0x0000404040040100UL, 0x0000808100020100UL, 0x0001010100020800UL, 0x0000808080010400UL,
            0x0000820820004000UL, 0x0000410410002000UL, 0x0000082088001000UL, 0x0000002011000800UL,
            0x0000080100400400UL, 0x0001010101000200UL, 0x0002020202000400UL, 0x0001010101000200UL,
            0x0000410410400000UL, 0x0000208208200000UL, 0x0000002084100000UL, 0x0000000020880000UL,
            0x0000001002020000UL, 0x0000040408020000UL, 0x0004040404040000UL, 0x0002020202020000UL,
            0x0000104104104000UL, 0x0000002082082000UL, 0x0000000020841000UL, 0x0000000000208800UL,
            0x0000000010020200UL, 0x0000000404080200UL, 0x0000040404040400UL, 0x0002020202020200UL
        ];
        private static readonly int[] RookShift = new int[64];
        private static readonly int[] BishopShift = new int[64];
        private static readonly ulong[] RookMasks = new ulong[64];
        private static readonly ulong[] BishopMasks = new ulong[64];
        private static readonly ulong[,] RookAttacks = new ulong[64, 1 << 14];
        private static readonly ulong[,] BishopAttacks = new ulong[64, 1 << 12];
        static Magic()
        {
            InitMasksAndShifts();
            InitRookAttacks();
            InitBishopAttacks();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitMasksAndShifts()
        {
            for (int sq = 0; sq < 64; sq++)
            {
                RookMasks[sq] = ComputeRookMask(sq);
                BishopMasks[sq] = ComputeBishopMask(sq);
                RookShift[sq] = 64 - BitOperations.PopCount(RookMasks[sq]);
                BishopShift[sq] = 64 - BitOperations.PopCount(BishopMasks[sq]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ComputeRookMask(int sq)
        {
            ulong mask = 0UL;
            int rank = sq >> 3;
            int file = sq & 7;
            for (int r = rank + 1; r < 7; r++)
            {
                mask |= 1UL << (r * 8 + file);
            }    
            for (int r = rank - 1; r > 0; r--)
            {
                mask |= 1UL << (r * 8 + file);
            }    
            for (int f = file + 1; f < 7; f++)
            {
                mask |= 1UL << (rank * 8 + f);
            }
            for (int f = file - 1; f > 0; f--)
            {
                mask |= 1UL << (rank * 8 + f);
            }
            return mask;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ComputeBishopMask(int sq)
        {
            ulong mask = 0UL;
            int rank = sq >> 3;
            int file = sq & 7;
            for (int r = rank + 1, f = file + 1; r < 7 && f < 7; r++, f++)
            {
                mask |= 1UL << (r * 8 + f);
            }
            for (int r = rank + 1, f = file - 1; r < 7 && f > 0; r++, f--)
            {
                mask |= 1UL << (r * 8 + f);
            }
            for (int r = rank - 1, f = file + 1; r > 0 && f < 7; r--, f++)
            {
                mask |= 1UL << (r * 8 + f);
            }   
            for (int r = rank - 1, f = file - 1; r > 0 && f > 0; r--, f--)
            {
                mask |= 1UL << (r * 8 + f);
            }    
            return mask;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitRookAttacks()
        {
            for (int sq = 0; sq < 64; sq++)
            {
                ulong mask = RookMasks[sq];
                int bits = BitOperations.PopCount(mask);
                int entries = 1 << bits;
                for (int i = 0; i < entries; i++)
                {
                    ulong occ = SetOccupancy(i, mask);
                    ulong attacks = ComputeRookAttacks(sq, occ);
                    int key = (int)((occ * RookMagics[sq]) >> RookShift[sq]);
                    RookAttacks[sq, key] = attacks;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitBishopAttacks()
        {
            for (int sq = 0; sq < 64; sq++)
            {
                ulong mask = BishopMasks[sq];
                int bits = BitOperations.PopCount(mask);
                int entries = 1 << bits;
                for (int i = 0; i < entries; i++)
                {
                    ulong occ = SetOccupancy(i, mask);
                    ulong attacks = ComputeBishopAttacks(sq, occ);
                    int key = (int)((occ * BishopMagics[sq]) >> BishopShift[sq]);
                    BishopAttacks[sq, key] = attacks;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ComputeRookAttacks(int sq, ulong occ)
        {
            ulong attacks = 0UL;
            int rank = sq >> 3;
            int file = sq & 7;
            for (int r = rank + 1; r < 8; r++)
            {
                int s = r * 8 + file;
                attacks |= 1UL << s;
                if ((occ & (1UL << s)) != 0)
                {
                    break;
                }
            }
            for (int r = rank - 1; r >= 0; r--)
            {
                int s = r * 8 + file;
                attacks |= 1UL << s;
                if ((occ & (1UL << s)) != 0)
                {
                    break;
                }
            }
            for (int f = file + 1; f < 8; f++)
            {
                int s = rank * 8 + f;
                attacks |= 1UL << s;
                if ((occ & (1UL << s)) != 0)
                {
                    break;
                }
            }
            for (int f = file - 1; f >= 0; f--)
            {
                int s = rank * 8 + f;
                attacks |= 1UL << s;
                if ((occ & (1UL << s)) != 0)
                {
                    break;
                }
            }
            return attacks;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ComputeBishopAttacks(int sq, ulong occ)
        {
            ulong attacks = 0UL;
            int rank = sq >> 3;
            int file = sq & 7;
            for (int r = rank + 1, f = file + 1; r < 8 && f < 8; r++, f++)
            {
                int s = r * 8 + f;
                attacks |= 1UL << s;
                if ((occ & (1UL << s)) != 0)
                {
                    break;
                }
            }
            for (int r = rank + 1, f = file - 1; r < 8 && f >= 0; r++, f--)
            {
                int s = r * 8 + f;
                attacks |= 1UL << s;
                if ((occ & (1UL << s)) != 0)
                {
                    break;
                }
            }
            for (int r = rank - 1, f = file + 1; r >= 0 && f < 8; r--, f++)
            {
                int s = r * 8 + f;
                attacks |= 1UL << s;
                if ((occ & (1UL << s)) != 0)
                {
                    break;
                }
            }
            for (int r = rank - 1, f = file - 1; r >= 0 && f >= 0; r--, f--)
            {
                int s = r * 8 + f;
                attacks |= 1UL << s;
                if ((occ & (1UL << s)) != 0)
                {
                    break;
                }
            }
            return attacks;
        }
        private static ulong SetOccupancy(int index, ulong mask)
        {
            ulong occ = 0;
            int bit = 0;
            while (Support_Bit.TryPopLsb(ref mask, out int square))
            {
                if ((index & (1 << bit)) != 0)
                {
                    occ |= 1UL << square;
                }
                bit++;
            }
            return occ;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetRookAttacks(int square, ulong occ)
        {
            occ &= RookMasks[square];
            ulong key = (occ * RookMagics[square]) >> RookShift[square];
            return RookAttacks[square, key];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetBishopAttacks(int square, ulong occ)
        {
            occ &= BishopMasks[square];
            ulong key = (occ * BishopMagics[square]) >> BishopShift[square];
            return BishopAttacks[square, key];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetQueenAttacks(int square, ulong occ)
        {
            return GetRookAttacks(square, occ) | GetBishopAttacks(square, occ);
        }
    }
}
