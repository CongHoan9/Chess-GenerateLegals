using System.Windows;
using System.Windows.Controls;
namespace Chess
{
    public abstract class SetLayoutChildrenCanvas : Canvas
    {
        protected abstract void CanChinhViTri(FrameworkElement child);
        protected virtual bool KiemTraNaN(FrameworkElement child)
        {
            return double.IsNaN(ActualWidth) || double.IsNaN(ActualHeight) || double.IsNaN(child.ActualWidth) || double.IsNaN(child.ActualHeight);
        }
    }
}
