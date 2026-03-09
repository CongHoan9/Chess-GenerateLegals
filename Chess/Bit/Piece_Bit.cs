using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chess
{
    public enum Piece_Color : byte
    {
        White,
        Black,
    }
    public enum Piece_Bit : byte
    {
        None = 0,
        WPawn, WKnight, WBishop, WRook, WQueen, WKing,
        BPawn, BKnight, BBishop, BRook, BQueen, BKing
    }
}
