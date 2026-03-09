namespace Chess
{
    public interface IBoard_Bit
    {
        public ref Piece_Bit this[Position position] => ref Piece_Bits[position.Row, position.Column];
        public Dictionary<Piece_Color, Dictionary<Piece_Bit, int>> PieceCount { get; set; }
        public Dictionary<Piece_Color, Position> KingInCheck { get; set; }
        public Position EnPassantTarget { get; set; }
        public Piece_Bit[,] Piece_Bits { get; set; }
        public bool IsInSide(Position position);
    }
}
