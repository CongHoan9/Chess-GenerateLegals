using System.Windows;
namespace Chess
{
    public class AutoBoundCanvas : SetLayoutChildrenCanvas
    {
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (visualAdded is FrameworkElement addedElement)
            {
                addedElement.LayoutUpdated += (_, _) => CanChinhViTri(addedElement);
            }
            if (visualRemoved is FrameworkElement removedElement)
            {
                removedElement.LayoutUpdated -= (_, _) => CanChinhViTri(removedElement);
            }
        }
        protected override void CanChinhViTri(FrameworkElement child)
        {
            if (!KiemTraNaN(child))
            {
                Thickness margin = child.Margin;
                SetLeft(child, Math.Max(0, Math.Min(GetLeft(child), ActualWidth - (child.ActualWidth + margin.Right * 2))));
                SetTop(child, Math.Max(0, Math.Min(GetTop(child), ActualHeight - (child.ActualHeight + margin.Bottom * 2))));
            }
        }
    }
}
