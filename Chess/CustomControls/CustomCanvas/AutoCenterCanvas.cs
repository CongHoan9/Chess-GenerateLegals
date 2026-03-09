using System.Windows;
namespace Chess
{
    public class AutoCenterCanvas : SetLayoutChildrenCanvas
    {
        public static readonly DependencyProperty AutoHorizontalProperty = DependencyProperty.Register("AutoHorizontal", typeof(bool), typeof(AutoCenterCanvas), new(false));
        public static readonly DependencyProperty AutoVerticalProperty = DependencyProperty.Register("AutoVertical", typeof(bool), typeof(AutoCenterCanvas), new(false));
        public bool AutoHorizontal
        {
            get => (bool)GetValue(AutoHorizontalProperty);
            set => SetValue(AutoHorizontalProperty, value);
        }
        public bool AutoVertical
        {
            get => (bool)GetValue(AutoVerticalProperty);
            set => SetValue(AutoVerticalProperty, value);
        }
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
                if (AutoHorizontal)
                {
                    SetLeft(child, (ActualWidth - child.ActualWidth) / 2);
                }
                if (AutoVertical)
                {
                    SetTop(child, (ActualHeight - child.ActualHeight) / 2);
                }
            }
        }
    }
}