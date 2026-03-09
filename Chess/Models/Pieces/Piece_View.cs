using System;

namespace Chess
{
    public abstract class Piece_View(Piece_Bit piece, int movescount = 0) : Binding
    {
        public Piece_Bit PieceType => piece;
        public virtual int MovesCount
        {
            get => GetInt(movescount);
            set
            {
                movescount = value;
                OnPropertyChanged(nameof(MovesCount));
            }
        }
    }
}
