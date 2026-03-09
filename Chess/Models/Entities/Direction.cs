namespace Chess
{
    public class Direction(int rowDelta, int columnDelta)
    {
        public readonly static Direction North = new(-1, 0); // Bắc
        public readonly static Direction South = new(1, 0);   // Nam
        public readonly static Direction East = new(0, 1);    // Đông
        public readonly static Direction West = new(0, -1);   // Tây
        public readonly static Direction NorthEast = new(-1, 1);  // Đông Bắc
        public readonly static Direction NorthWest = new(-1, -1); // Tây Bắc
        public readonly static Direction SouthEast = new(1, 1);   // Đông Nam
        public readonly static Direction SouthWest = new(1, -1); // Tây Nam
        public int RowDelta { get; } = rowDelta;
        public int ColumnDelta { get; } = columnDelta;
        public static Direction operator +(Direction dir1, Direction dir2)
        {
            return new Direction(dir1.RowDelta + dir2.RowDelta, dir1.ColumnDelta + dir2.ColumnDelta);
        }
        public static Direction operator *(int scalar, Direction dir)
        {
            return new Direction(scalar * dir.RowDelta, scalar * dir.ColumnDelta);
        }
    }
}
