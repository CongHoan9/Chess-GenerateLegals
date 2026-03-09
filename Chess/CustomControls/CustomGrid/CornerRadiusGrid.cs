
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Chess
{
    public class CornerRadiusGrid : Grid
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(double), typeof(CornerRadiusGrid), new(0.0, OnCornerRadiusChanged));
        public double CornerRadius
        {
            get => (double)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CornerRadiusGrid grid)
            {
                grid.UpdateClip();
            }
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateClip();
        }
        private void UpdateClip()
        {
            if (ActualWidth > 0 && ActualHeight > 0)
            {
                Clip = new RectangleGeometry
                {
                    Rect = new Rect(0, 0, ActualWidth, ActualHeight),
                    RadiusX = CornerRadius,
                    RadiusY = CornerRadius
                };
            }
        }
    }
}
