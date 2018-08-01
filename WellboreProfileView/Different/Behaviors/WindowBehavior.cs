using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WellboreProfileView.Behaviors
{
    public class WindowBehavior : Behavior<UserControl>
    {
        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register("ParentWindow", typeof(Window), typeof(WindowBehavior), new PropertyMetadata(null));

        public Window ParentWindow
        {
            get
            {
                return (Window)GetValue(DependencyProperty);
            }
            set
            {
                SetValue(DependencyProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += UserControlLoadedHandler;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= UserControlLoadedHandler;
        }

        private void UserControlLoadedHandler(object sender, RoutedEventArgs e)
        {
            ParentWindow = Window.GetWindow(AssociatedObject);
        }
    }
}