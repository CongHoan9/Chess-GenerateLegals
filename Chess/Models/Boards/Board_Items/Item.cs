using System.ComponentModel;
using System.Windows;

namespace Chess
{
    public abstract class Item(Visibility visibility = Visibility.Visible) : Binding
    {
        public Visibility Visibility
        {
            get => visibility;
            set
            {
                if (visibility != value)
                {
                    visibility = value;
                    OnPropertyChanged(nameof(Visibility));
                }
            }
        }
    }
}
