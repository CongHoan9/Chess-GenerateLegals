using System.Windows.Controls;
using System.Windows.Input;

namespace Chess
{
    public class DoubleComboBox : ComboBox
    {
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            IsDropDownOpen = true;
        }
    }
}
