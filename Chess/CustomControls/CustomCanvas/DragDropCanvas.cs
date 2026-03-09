using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
namespace Chess
{
    public class DragDropCanvas : AutoBoundCanvas
    {
        private FrameworkElement _Drag;
        protected FrameworkElement Drag
        {
            get => _Drag;
            set
            {
                if (value is FrameworkElement newdrag)
                {
                    SetPoint(newdrag);
                }
                _Drag = value;
            }
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Background = null;
            AllowDrop = true;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetPoint(Drag);
            }
        }
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded is FrameworkElement drag)
            {
                Drag = drag;
            }
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.OriginalSource is FrameworkElement child)
            {
                Drag = child;
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Background = null;
            base.OnMouseLeftButtonUp(e);
            Drag = null;
        }
        protected virtual void SetPoint(FrameworkElement drag)
        {
            if (drag != null)
            {
                Point locate = Mouse.GetPosition(this); 
                Thickness margin = drag.Margin;
                SetTop(drag, locate.Y - margin.Top - 15);
                SetLeft(drag, locate.X - (drag.ActualWidth / 2) - margin.Left);
                if (Background != Brushes.Transparent)
                {
                    Background = Brushes.Transparent;
                }
                if (Mouse.Captured != drag)
                {
                    drag.CaptureMouse();
                }
                drag.PreviewMouseLeftButtonUp += (s, e) =>
                {
                    Background = null;
                    drag.ReleaseMouseCapture();
                };
            }
        }
    }
}
