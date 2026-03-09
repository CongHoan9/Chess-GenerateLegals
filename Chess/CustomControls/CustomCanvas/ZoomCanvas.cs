using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace Chess
{
    public class ZoomCanvas : Canvas
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            RenderTransform = new ScaleTransform(1, 1);
            RenderTransformOrigin = new(.5, .5);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            double zoom = e.Delta > 0 ? 1.1 : 1 / 1.1;
            if (RenderTransform is ScaleTransform scale)
            {
                scale.ScaleX *= zoom;
                scale.ScaleY *= zoom;
            }
            else
            {
                RenderTransform = new ScaleTransform(zoom, zoom);
            }
            Console.WriteLine(123);
        }
    }
}
