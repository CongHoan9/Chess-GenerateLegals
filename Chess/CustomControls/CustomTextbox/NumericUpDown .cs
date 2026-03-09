using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chess
{
    public class NumericUpDown : WatermarkTextBox
    {
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(long), typeof(NumericUpDown), new(long.MinValue));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(long), typeof(NumericUpDown), new(long.MaxValue));
        public static readonly DependencyProperty ScrollProperty = DependencyProperty.Register(nameof(Scroll), typeof(bool), typeof(NumericUpDown), new(false));
        public long MinValue
        {
            get => (long)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }
        public long MaxValue
        {
            get => (long)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        public bool Scroll
        {
            get => (bool)GetValue(ScrollProperty);
            set => SetValue(ScrollProperty, value);
        }
        public ICommand UpCommand { get; }
        public ICommand DownCommand { get; }
        private void SetText(bool direction)
        {
            if (long.TryParse(Text, out long value))
            {
                //long Base = Math.Max(1, direction ? value : value - 1);
                //long Step = value <= 9 ? 1 : (long)Math.Pow(10, (long)Math.Log10(Base));
                //long New = value + Step * (direction ? 1 : -1);
                Text = $"{value + (direction ? 1 : -1)}";
            }
            else
            {
                Text = Math.Max(MinValue, 1).ToString();
            }
        }
        public NumericUpDown() : base()
        {
            UpCommand = new RunCommand(_ => SetText(true));
            DownCommand = new RunCommand(_ => SetText(false));
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (Scroll)
            {
                if (IsWatermarkShowing)
                {
                    HideWatermark();
                };
                SetText(e.Delta > 0);
                Dispatcher.InvokeAsync(() => { CaretIndex = Text.Length; SelectionLength = 0; });
                e.Handled = true;
            }
        }
        private static readonly Regex Regex = new("[^0-9]+");
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text))
            {
                ShowWatermark();
            }
            else
            {
                base.OnTextInput(e);
            }
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (long.TryParse(Text, out long value))
            {
                int index = CaretIndex;
                Text = $"{Math.Max(MinValue, Math.Min(MaxValue, value))}";
                Dispatcher.InvokeAsync(() => { CaretIndex = index; SelectionLength = 0; });
            }
        }
    }
}
