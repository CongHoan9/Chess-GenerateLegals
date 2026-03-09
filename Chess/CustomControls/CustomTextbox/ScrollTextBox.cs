using System.Windows.Controls;
using System.Windows.Input;

namespace Chess
{
    public class ScrollTextBox : TextBox
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Loaded += (_, _) => { IsReadOnly = true; };
        }
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (e.Delta != 0)
            {
                if (GetTemplateChild("PART_ContentHost") is ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
                    e.Handled = true;
                }
            }
        }
    }
}
