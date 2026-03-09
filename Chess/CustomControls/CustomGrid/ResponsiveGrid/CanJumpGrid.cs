using System.Windows;
using System.Windows.Controls;

namespace Chess
{
    public class CanJumpGrid : Grid
    {
        public static readonly DependencyProperty ResponsiveSizeProperty = DependencyProperty.Register(nameof(ResponsiveSize), typeof(double), typeof(CanJumpGrid), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty NewColumnProperty = DependencyProperty.Register(nameof(NewColumn), typeof(int), typeof(CanJumpGrid), new PropertyMetadata(0));
        public static readonly DependencyProperty NewRowProperty = DependencyProperty.Register(nameof(NewRow), typeof(int), typeof(CanJumpGrid), new PropertyMetadata(0));
        public double ResponsiveSize
        {
            get => (double)GetValue(ResponsiveSizeProperty);
            set => SetValue(ResponsiveSizeProperty, value);
        }
        public int NewColumn
        {
            get => (int)GetValue(NewColumnProperty);
            set => SetValue(NewColumnProperty, value);
        }
        public int NewRow
        {
            get => (int)GetValue(NewRowProperty);
            set => SetValue(NewRowProperty, value);
        }
        private int OldColumn;
        private int OldRow;
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            OldRow = GetRow(this);
            OldColumn = GetColumn(this);
            Loaded += (_, _) =>
            {
                if (Parent is FrameworkElement grid)
                {
                    grid.SizeChanged += CheckToJump;
                }
            };
        }
        private void CheckToJump(object sender, SizeChangedEventArgs e)
        {
            double size = e.NewSize.Width;
            if (!double.IsNaN(size) && !double.IsNaN(ResponsiveSize))
            {
                if (size < ResponsiveSize)
                {
                    SetColumn(this, NewColumn);
                    SetRow(this, NewRow);
                }
                else
                {
                    SetColumn(this, OldColumn);
                    SetRow(this, OldRow);
                }
            }
        }
    }
}
