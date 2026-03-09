using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess
{
    public enum EvaluateType : byte
    {
        None,
        Brilliant,
        Great,
        Best,
        Excellent,
        Good,
        Book,
        Inaccuracy,
        Mistake,
        Miss,
        Blunder,
        Move,
    }
    public enum HighLightType : byte
    {
        None,
        Red,
        Orange,
        Blue,
        Green,
    }
    public class HighLight(Position position, bool isclick = false, EvaluateType evaluateType = EvaluateType.None, HighLightType highLightType = HighLightType.None) : Cell(position)
    {
        public bool IsWarning = false;
        private bool _IsCheck = false;
        public bool IsCheck
        {
            get => _IsCheck;
            set
            {
                _IsCheck = value;
                OnPropertyChanged(nameof(SolidColorBrush));
            }
        }
        public bool IsClick
        {
            get => EvaluateType == EvaluateType.None && isclick;
            set
            {
                isclick = value;
                OnPropertyChanged(nameof(SolidColorBrush));
            }
        }
        public EvaluateType EvaluateType
        {
            get => evaluateType;
            set
            {
                evaluateType = value;
                OnPropertyChanged(nameof(SolidColorBrush));
            }
        }
        public HighLightType HighLightType
        {
            get => highLightType;
            set
            {
                highLightType = value;
                OnPropertyChanged(nameof(SolidColorBrush));
            }
        }
        public SolidColorBrush SolidColorBrush => GetColor();
        private SolidColorBrush GetColor()
        {
            return IsCheck ? new(Color.FromArgb(125, 255, 0, 0)) : IsClick ? new(Color.FromArgb(128, 255, 255, 51)) : HighLightType switch
            {
                HighLightType.Red => new(Color.FromArgb(204, 235, 97, 80)),
                HighLightType.Orange => new(Color.FromArgb(204, 255, 170, 0)),
                HighLightType.Blue => new(Color.FromArgb(204, 82, 176, 220)),
                HighLightType.Green => new(Color.FromArgb(204, 172, 206, 89)),
                HighLightType.None => EvaluateType switch
                {
                    EvaluateType.Brilliant => new(Color.FromArgb(128, 38, 194, 163)),
                    EvaluateType.Great => new(Color.FromArgb(128, 116, 155, 191)),
                    EvaluateType.Best or EvaluateType.Excellent => new(Color.FromArgb(128, 129, 182, 76)),
                    EvaluateType.Good => new(Color.FromArgb(128, 149, 183, 118)),
                    EvaluateType.Book => new(Color.FromArgb(128, 213, 164, 125)),
                    EvaluateType.Inaccuracy => new(Color.FromArgb(128, 247, 198, 49)),
                    EvaluateType.Mistake => new(Color.FromArgb(128, 255, 164, 89)),
                    EvaluateType.Miss => new(Color.FromArgb(128, 255, 119, 105)),
                    EvaluateType.Blunder => new(Color.FromArgb(128, 250, 65, 45)),
                    //EvaluateType.Correct => new(Color.FromArgb(128, 172, 206, 89)),
                    //EvaluateType.Incorrect => new(Color.FromArgb(128, 201, 52, 48)),
                    EvaluateType.Move => new(Color.FromArgb(128, 255, 255, 51)),
                    EvaluateType.None => IsClick ? new(Color.FromArgb(128, 255, 255, 51)) : new(Color.FromArgb(0, 0, 0, 0)),
                    _ => new(),
                },
                _ => new(),
            };
        }
        public void HighLightColorOverlay()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                HighLightType = HighLightType != HighLightType.Orange ? HighLightType.Orange : HighLightType.None;
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                HighLightType = HighLightType != HighLightType.Blue ? HighLightType.Blue : HighLightType.None;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                HighLightType = HighLightType != HighLightType.Green ? HighLightType.Green : HighLightType.None;
            }
            else
            {
                HighLightType = HighLightType != HighLightType.Red ? HighLightType.Red : HighLightType.None;
            }
        }
        public async Task Warning()
        {
            IsWarning = true;
            for (int i = 0; i < 6; i++)
            {
                IsCheck = !IsCheck;
                await Task.Delay(250);
            }
            IsWarning = false;
            HighLightType = HighLightType.None;
            OnPropertyChanged(nameof(SolidColorBrush));
        }
    }
    public class HighlightToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HighLight highlight)
            {
                return highlight.IsCheck ? new SolidColorBrush(Color.FromArgb(125, 255, 0, 0)) : highlight.IsClick ? new(Color.FromArgb(128, 255, 255, 51)) : highlight.HighLightType switch
                {
                    HighLightType.Red => new(Color.FromArgb(204, 235, 97, 80)),
                    HighLightType.Orange => new(Color.FromArgb(204, 255, 170, 0)),
                    HighLightType.Blue => new(Color.FromArgb(204, 82, 176, 220)),
                    HighLightType.Green => new(Color.FromArgb(204, 172, 206, 89)),
                    HighLightType.None => highlight.EvaluateType switch
                    {
                        EvaluateType.Brilliant => new(Color.FromArgb(128, 38, 194, 163)),
                        EvaluateType.Great => new(Color.FromArgb(128, 116, 155, 191)),
                        EvaluateType.Best or EvaluateType.Excellent => new(Color.FromArgb(128, 129, 182, 76)),
                        EvaluateType.Good => new(Color.FromArgb(128, 149, 183, 118)),
                        EvaluateType.Book => new(Color.FromArgb(128, 213, 164, 125)),
                        EvaluateType.Inaccuracy => new(Color.FromArgb(128, 247, 198, 49)),
                        EvaluateType.Mistake => new(Color.FromArgb(128, 255, 164, 89)),
                        EvaluateType.Miss => new(Color.FromArgb(128, 255, 119, 105)),
                        EvaluateType.Blunder => new(Color.FromArgb(128, 250, 65, 45)),
                        //EvaluateType.Correct => new(Color.FromArgb(128, 172, 206, 89)),
                        //EvaluateType.Incorrect => new(Color.FromArgb(128, 201, 52, 48)),
                        EvaluateType.Move => new(Color.FromArgb(128, 255, 255, 51)),
                        EvaluateType.None => highlight.IsClick ? new(Color.FromArgb(128, 255, 255, 51)) : new(Color.FromArgb(0, 0, 0, 0)),
                        _ => new(),
                    },
                    _ => new(),
                };
            }
            return new();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
