namespace Chess
{
    public abstract class Board(IBoard_Bit board_bit, double rotate = 0) : Binding
    {
        public virtual double Rotate
        {
            get => rotate;
            set
            {
                rotate = value;
                OnPropertyChanged(nameof(Rotate));
            }
        }
        public readonly Square_List Squares = new(board_bit);
        public readonly Arrow_List Arrows = new(board_bit.IsInSide);
    }
}
