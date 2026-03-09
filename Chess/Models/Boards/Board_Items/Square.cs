namespace Chess
{
    public class Square(Position position, Piece_View piece) : Cell(position)
    {
        public virtual Piece_View Piece
        {
            get => piece;
            set
            {
                if (piece != value)
                {
                    piece = value;
                    OnPropertyChanged(nameof(Piece));
                }
            }
        }
    }
}
