using System.Windows;
using System.Windows.Controls.Primitives;
namespace Chess
{
    public class CenterPopup : Popup
    {
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(nameof(Direction), typeof(int), typeof(CenterPopup), new(1), ValidateDirectionValue);
        public int Direction
        {
            get => (int)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }
        private static bool ValidateDirectionValue(object value)
        {
            return value is int i && (i == 1 || i == -1);
        }
        protected override void OnInitialized(EventArgs e)
        {
            AllowsTransparency = true;
            Placement = PlacementMode.Custom;
            CustomPopupPlacementCallback = new(CenterHorizontally);
        }
        private CustomPopupPlacement[] CenterHorizontally(Size popupSize, Size targetSize, Point offset)
        {
            return [ new(new((targetSize.Width - popupSize.Width) / 2, targetSize.Height * Direction), PopupPrimaryAxis.Horizontal) ];
        }
    }
}
