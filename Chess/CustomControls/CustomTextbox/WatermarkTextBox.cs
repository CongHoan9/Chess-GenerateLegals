using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess
{
    public class WatermarkTextBox : TextBox
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof(Watermark), typeof(string), typeof(WatermarkTextBox), new("Nhập"));
        public string Watermark
        {
            get => (string)GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }
        protected readonly SolidColorBrush WatermarkColor = new(Color.FromRgb(128, 128, 128));
        protected readonly SolidColorBrush InputColor = new(Color.FromRgb(48, 48, 48));
        protected virtual bool IsWatermarkShowing => Text == Watermark && Foreground == WatermarkColor;
        protected virtual bool ShouldShowWatermark => string.IsNullOrWhiteSpace(Text) || string.IsNullOrEmpty(Text);
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Loaded += (_, _) =>
            {
                if (ShouldShowWatermark)
                {
                    Text = Watermark;
                    Foreground = WatermarkColor;
                }
            };
        }
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (IsWatermarkShowing)
            {
                SetupToInput(e);
            }
        }
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (IsWatermarkShowing)
            {
                SetupToInput(e);
            }
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (ShouldShowWatermark)
            {
                ShowWatermark();
            }
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (IsWatermarkShowing)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (e.Key == Key.A || e.Key == Key.X || e.Key == Key.End || e.Key == Key.C || e.Key == Key.V)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                if (e.Key == Key.Enter || e.Key == Key.End || e.Key == Key.Home || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right)
                {
                    e.Handled = true;
                    return;
                }
            }
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (IsWatermarkShowing)
            {
                HideWatermark();
            }
            base.OnPreviewTextInput(e);
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            string trimmed = Text.TrimStart();
            if (Text != trimmed)
            {
                Text = trimmed;
            }
            if (ShouldShowWatermark && Foreground != WatermarkColor)
            {
                ShowWatermark();
            }
        }
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);
            if (IsWatermarkShowing)
            {
                HideWatermark();
            }
        }
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (e.Data.GetData(DataFormats.Text) is string input)
            {
                Text = input;
                Foreground = InputColor;
            }
            else
            {
                ShowWatermark();
            }
        }
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            if (e.Data.GetData(DataFormats.Text) is string input)
            {
                Text = input;
            }
        }
        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            ShowWatermark();
        }
        protected void SetupToInput(RoutedEventArgs e)
        {
            Dispatcher.InvokeAsync(() => { CaretIndex = 0; SelectionLength = 0; });
            e.Handled = true;
            Focus();
        }
        protected void ShowWatermark()
        {
            Text = Watermark;
            Foreground = WatermarkColor;
        }
        protected void HideWatermark()
        {
            Text = string.Empty;
            Foreground = InputColor;
        }
    }
}