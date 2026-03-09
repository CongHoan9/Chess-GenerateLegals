namespace Chess
{
    public abstract class Cell_List<T>(Func<Position, bool> isinside) : Item_List<T> where T : Cell
    {
        public virtual Cell this[Position position] => this.FirstOrDefault(item => item.Position == position);
        public override bool IsInItemside(T item) => isinside(item.Position);
    }
}
