using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Shaballa
{
    public class DragDropBehavior
    {
        public static readonly DependencyProperty CanDragProperty = DependencyProperty.RegisterAttached("CanDrag", typeof(bool), typeof(DragDropBehavior), new(false, OnCanDragChanged));
        public static readonly DependencyProperty CanDropProperty = DependencyProperty.RegisterAttached("CanDrop", typeof(bool), typeof(DragDropBehavior), new(false, OnCanDropChanged)); 
        public static void SetCanDrag(FrameworkElement element, bool value) => element.SetValue(CanDragProperty, value);
        public static bool GetCanDrag(FrameworkElement element) => (bool)element.GetValue(CanDropProperty);
        public static void SetCanDrop(FrameworkElement element, bool value) => element.SetValue(CanDragProperty, value);
        public static bool GetCanDrop(FrameworkElement element) => (bool)element.GetValue(CanDropProperty);
        private static void OnCanDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.MouseLeftButtonDown += OnMouseLeftButtonDown;
                    element.MouseRightButtonDown += OnMouseRightButtonDown;
                }
                else
                {
                    element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                    element.MouseRightButtonDown -= OnMouseRightButtonDown;
                }
            }
        }
        private static void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                DragDrop.DoDragDrop(element, new DataObject(), DragDropEffects.None);
                element.Cursor = (Cursor)Application.Current.Resources["Grab"];
            }
        }
        private static void OnCanDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.AllowDrop = true;
                    element.Drop += OnDrop;
                    element.GiveFeedback += OnGiveFeedback;
                }
                else
                {
                    element.AllowDrop = false;
                    element.Drop -= OnDrop;
                    element.GiveFeedback += OnGiveFeedback;
                }
            }
        }
        private static void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            e.Handled = true;
        }
        private static void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            //if (sender is FrameworkElement element && element.DataContext is IDragDrop source)
            //{
            //    source.Drag();
            //    element.Cursor = (Cursor)Application.Current.Resources["Grabbing"];
            //    DragDropEffects result = DragDrop.DoDragDrop(element, new DataObject("dragdrop", source), DragDropEffects.Move);
            //    element.Cursor = (Cursor)Application.Current.Resources["Grab"];
            //    if (result == DragDropEffects.None)
            //    {
            //        source.Visibility = Visibility.Visible;
            //    }
            //}
        }
        private static void OnDrop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent("dragdrop") && e.Data.GetData("dragdrop") is IDragDrop source && sender is FrameworkElement fe && fe.DataContext is IDragDrop target && !ReferenceEquals(source, target))
            //{
            //    target.Drop(source);
            //}
        }
    }
}
