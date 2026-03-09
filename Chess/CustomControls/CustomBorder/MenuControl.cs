using System.Windows.Controls;
using System.Windows.Input;
namespace Chess
{
    public class MenuControl : Border
    {
        protected int Count = 0;
        protected DateTime Time = DateTime.MinValue;
        private bool _IsShow = false;
        public bool IsShow
        {
            get => _IsShow;
            set
            {
                _IsShow = value;
                if (_IsShow)
                {
                    ClearValue(MaxHeightProperty);
                    ClearValue(MaxWidthProperty);
                }
                else
                {
                    MaxHeight = 50;
                    MaxWidth = 50;
                }
            }
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            MinHeight = 50; 
            MinWidth = 50; 
            MaxHeight = 50; 
            MaxWidth = 50;
        }
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if ((DateTime.Now - Time) < TimeSpan.FromMilliseconds(400))
            {
                Count++;
            }
            else
            {
                Count = 1;
            }
            Time = DateTime.Now;
            if (Count >= 2)
            {
                IsShow = !IsShow;
                Count = 0;
            }
        }
    }
}
