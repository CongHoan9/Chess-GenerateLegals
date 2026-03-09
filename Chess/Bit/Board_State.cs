namespace Chess
{
    public struct Board_State
    {
        public Move Move;
        public int HalfMove;
        public int FullMove;
        public ulong Zobrist;
        public byte Castling;
        public int EnPassant;
        public Piece_Bit Moved;
        public Piece_Bit Captured;
        public int CapturedSquare;
        public Piece_Color Current;
    }
}
