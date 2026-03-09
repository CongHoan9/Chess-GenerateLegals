namespace Chess
{
    public class Arrow(Position start, Position end) : Item
    {
        public virtual Position Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged(nameof(Start));
            }
        }
        public virtual Position End
        {
            get => end;
            set
            {
                end = value;
                OnPropertyChanged(nameof(End));
            }
        }
    }
}
