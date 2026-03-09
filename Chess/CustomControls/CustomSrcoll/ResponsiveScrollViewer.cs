using System.Windows;
using System.Windows.Controls;

namespace Chess
{
    public class ResponsiveScrollViewer : ScrollViewer
    {
        public static readonly DependencyProperty ResponsiveSizeProperty = DependencyProperty.Register(nameof(ResponsiveSize), typeof(double), typeof(ResponsiveScrollViewer), new(double.NaN));
        public double ResponsiveSize
        {
            get => (double)GetValue(ResponsiveSizeProperty);
            set => SetValue(ResponsiveSizeProperty, value);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (!double.IsNaN(ActualWidth) && !double.IsNaN(ResponsiveSize))
            {
                VerticalScrollBarVisibility = ActualWidth < ResponsiveSize ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Disabled;
            }
        }
    }
}
