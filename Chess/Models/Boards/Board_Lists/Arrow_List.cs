namespace Chess
{
    public class Arrow_List(Func<Position, bool> isinside) : Item_List<Arrow>
    {
        public override bool IsInItemside(Arrow item) => isinside(item.Start) && isinside(item.End);
    }
}
