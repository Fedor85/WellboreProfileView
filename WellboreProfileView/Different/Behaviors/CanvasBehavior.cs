using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using WellboreProfileView.Views;

namespace WellboreProfileView.Behaviors
{
    public class CanvasBehavior : Behavior<BaseCanvasControl>
    {
        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register("DrawingRange", typeof(Canvas), typeof(CanvasBehavior), new PropertyMetadata(null));

        public Canvas DrawingRange
        {
            get
            {
                return (Canvas)GetValue(DependencyProperty);
            }
            set
            {
                SetValue(DependencyProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= Loaded;
        }


        private void Loaded(object sender, RoutedEventArgs e)
        {
            BaseCanvasControl drawControl = sender as BaseCanvasControl;
            DrawingRange = drawControl.DrawingRange;
        }
    }
}