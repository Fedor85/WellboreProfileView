using System.Windows;
using System.Windows.Interactivity;
using WellboreProfileView.Views;

namespace WellboreProfileView.Behaviors
{
    public class Viewport3DBehavior : Behavior<BaseViewport3DControl>
    {
        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register("DrawingRange3D", typeof(FeedbackViewport3D), typeof(Viewport3DBehavior), new PropertyMetadata(null));

        public FeedbackViewport3D DrawingRange3D
        {
            get
            {
                return (FeedbackViewport3D)GetValue(DependencyProperty);
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
            BaseViewport3DControl drawControl = sender as BaseViewport3DControl;
            DrawingRange3D = drawControl.DrawingRange3D;
        }
    }
}