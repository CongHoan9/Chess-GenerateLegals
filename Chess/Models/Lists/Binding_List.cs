using System.Collections.ObjectModel;
namespace Chess
{
    public abstract class Binding_List<T> : ObservableCollection<T>
    {
        public Binding_List() { }
        public Binding_List(IEnumerable<T> items) : base(items) { }
    }
}
