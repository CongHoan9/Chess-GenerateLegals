using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chess
{
    public static class Support_Bit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryPopLsb(ref ulong bb, out int sq)
        {
            if (bb == 0)
            {
                sq = -1;
                return false;
            }
            sq = BitOperations.TrailingZeroCount(bb);
            bb &= bb - 1;
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetSingleSquare(ulong bb, out int sq)
        {
            if (bb == 0)
            {
                sq = -1;
                return false;
            }
            sq = BitOperations.TrailingZeroCount(bb);
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece_Bit CharToPiece(char c) => c switch
        {
            'P' => Piece_Bit.WPawn,
            'N' => Piece_Bit.WKnight,
            'B' => Piece_Bit.WBishop,
            'R' => Piece_Bit.WRook,
            'Q' => Piece_Bit.WQueen,
            'K' => Piece_Bit.WKing,
            'p' => Piece_Bit.BPawn,
            'n' => Piece_Bit.BKnight,
            'b' => Piece_Bit.BBishop,
            'r' => Piece_Bit.BRook,
            'q' => Piece_Bit.BQueen,
            'k' => Piece_Bit.BKing,
            _ => Piece_Bit.None
        };
    }
}
