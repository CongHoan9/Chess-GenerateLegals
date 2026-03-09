using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
namespace Chess
{
    public class ResponsiveUniformGrid : UniformGrid
    {
        public static readonly DependencyProperty ResponsiveColumsProperty = DependencyProperty.Register("ResponsiveColums", typeof(string), typeof(ResponsiveUniformGrid), new(""));
        public static readonly DependencyProperty ResponsiveRowsProperty = DependencyProperty.Register("ResponsiveRows", typeof(string), typeof(ResponsiveUniformGrid), new(""));
        public string ResponsiveColums
        {
            get => (string)GetValue(ResponsiveColumsProperty);
            set => SetValue(ResponsiveColumsProperty, value);
        }
        public string ResponsiveRows
        {
            get => (string)GetValue(ResponsiveRowsProperty);
            set => SetValue(ResponsiveRowsProperty, value);
        }
        private List<(double, int)> ColumnRules = [];
        private List<(double, int)> RowRules = [];
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            RowRules = ParseRules(ResponsiveRows);
            ColumnRules = ParseRules(ResponsiveColums);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (ColumnRules.Count > 0)
            {
                Columns = GetResponsiveValue(sizeInfo.NewSize.Width, ColumnRules);
            }
            if (RowRules.Count > 0)
            {
                Rows = GetResponsiveValue(sizeInfo.NewSize.Height, RowRules);
            }
        }
        private static List<(double, int)> ParseRules(string input)
        {
            List<(double, int)> result = [];
            if (!string.IsNullOrWhiteSpace(input))
            {
                var parts = input.Split(',');
                foreach (var part in parts)
                {
                    var items = part.Split(':');
                    if (items.Length == 2 &&
                        double.TryParse(items[0].Trim(), out double threshold) &&
                        int.TryParse(items[1].Trim(), out int value))
                    {
                        result.Add(new(threshold, value));
                    }
                }
                result.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            }
            return result;
        }
        private static int GetResponsiveValue(double size, List<(double, int)> rules)
        {
            foreach (var rule in rules)
            {
                if (size < rule.Item1)
                {
                    return rule.Item2;
                }
            }
            return 0;
        }
    }
}
