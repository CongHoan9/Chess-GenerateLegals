namespace Chess
{
    public class Square_List(IBoard_Bit board_bit) : Cell_List<Square>(board_bit.IsInSide)
    {
        protected override void InsertItem(int index, Square square)
        {
            base.InsertItem(index, square);
            if (square.Piece is Piece_View piece)
            {
                //board_bit[square.Position] = piece
            }
        }
        protected override void RemoveItem(int index)
        {
            if (this[index] is Square square && board_bit.IsInSide(square.Position))
            {
                board_bit[square.Position] = new Piece_Bit();
            }
            base.RemoveItem(index);
        }
        protected override void ClearItems()
        {
            Array.Clear(board_bit.Piece_Bits);
            base.ClearItems();
        }
    }
}
