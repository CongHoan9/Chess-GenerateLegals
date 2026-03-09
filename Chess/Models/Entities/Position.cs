namespace Chess
{
    public class Position(int column, int row) : Binding // ReferenceEquals
    {
        public int Column
        {
            get => GetInt(column, 0);
            set
            {
                if (column != value)
                {
                    column = value;
                    OnPropertyChanged(nameof(Column));
                }
            }
        }
        public int Row
        {
            get => GetInt(row, 0);
            set
            {
                if (row != value)
                {

                    row = value;
                    OnPropertyChanged(nameof(Row));
                }
            }
        }
        public static Position operator +(Position position, Direction direction)
        {
            return new(position.Row + direction.RowDelta, position.Column + direction.ColumnDelta);
        }
        public static bool operator ==(Position position, Position other)
        {
            return position != null && other != null && position.Row == other.Row & position.Column == other.Column;
        }
        public static bool operator !=(Position position, Position other)
        {
            return !(position == other);
        }
        public static bool operator <(Position position, Position other)
        {
            return (position.Row < other.Row) && (position.Column < other.Column);
        }
        public static bool operator >(Position position, Position other)
        {
            return (position.Row > other.Row) && (position.Column > other.Column);
        }
        public override string ToString()
        {
            return $"{(char)('a' + Column)}{Row}";
        }
        public override bool Equals(object obj)
        {
            return obj is Position other && other == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Column, Row);
        }
    }
}
