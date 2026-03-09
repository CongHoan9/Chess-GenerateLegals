namespace Chess
{
    public class Reusable_List<T> : Binding_List<T>
    {
        protected readonly Stack<T> Saves = [];
        protected override void RemoveItem(int index)
        {
            try
            {
                if (Items[index] is T item)
                {
                    Saves.Push(item);
                }
            }
            catch { }
            base.RemoveItem(index);
        }
        public virtual void Add(Func<T> create, Action<T> configure)
        {
            if (!Saves.TryPop(out T item))
            {
                if (create == null)
                {
                    return;
                }
                item = create();
            }
            configure?.Invoke(item);
            base.Add(item);
        }
    }
}
