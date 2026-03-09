using System.ComponentModel;
namespace Chess
{
    public abstract class Item_List<T> : Reusable_List<T> where T : Item
    {
        public event Action OnReset;
        public abstract bool IsInItemside(T item);
        protected override void InsertItem(int index, T item)
        {
            if (IsInItemside(item))
            {
                base.InsertItem(index, item);
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }
        protected override void RemoveItem(int index)
        {
            this[index].PropertyChanged -= OnItemPropertyChanged;
            base.RemoveItem(index);
        }
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is T item && e.PropertyName == nameof(Cell.Position) && !IsInItemside(item))
            {
                Remove(item);
            }
        }
    }
}
