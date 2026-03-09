using System.Windows.Controls;
namespace Chess
{
    public class ZoomScrollViewer : ScrollViewer
    {
        private ScrollBarVisibility OldVertical;
        private ScrollBarVisibility OldHorizontal;
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            OldVertical = VerticalScrollBarVisibility;
            OldHorizontal = HorizontalScrollBarVisibility;
        }
        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
            bool canScrollVertically = ExtentHeight >= ExtentWidth;
            VerticalScrollBarVisibility = canScrollVertically ? OldVertical : ScrollBarVisibility.Disabled;
            HorizontalScrollBarVisibility = canScrollVertically ? ScrollBarVisibility.Disabled : OldHorizontal;
        }
    }
}
