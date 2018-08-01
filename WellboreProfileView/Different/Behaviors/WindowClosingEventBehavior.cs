using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WellboreProfileView.Behaviors
{
    public class WindowClosingEventBehavior : Behavior<UserControl>
    {
        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register("WindowClosing", typeof(Action<CancelEventArgs>), typeof(WindowClosingEventBehavior), new PropertyMetadata(null));

        public Action<CancelEventArgs> WindowClosing
        {
            get
            {
                return (Action<CancelEventArgs>)GetValue(DependencyProperty);
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
            Window window = Window.GetWindow(AssociatedObject);
            if (window != null)
                window.Closing -= WindowClosingHandler;
        }

        /// <summary>
        /// Registers to the containing windows Closing event when the UserControl is loaded.
        /// </summary>
        private void UserControlLoadedHandler(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(AssociatedObject);
            if (window == null)
                throw new Exception(String.Format("UserControl {0} не активирован.\nОбратитесь к разработчикам.", AssociatedObject.GetType().Name));

            window.Closing += WindowClosingHandler;
        }

        /// <summary>
        /// The containing window is closing, raise the UserControlClosing event.
        /// </summary>
        private void WindowClosingHandler(object sender, CancelEventArgs e)
        {
            WindowClosing(e);
        }
    }
}