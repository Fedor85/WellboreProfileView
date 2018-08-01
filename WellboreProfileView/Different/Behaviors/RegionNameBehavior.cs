using System.Windows;
using System.Windows.Interactivity;
using WellboreProfileView.Views;

namespace WellboreProfileView.Behaviors
{
    public class RegionNameBehavior : Behavior<BaseRegionNameControl>
    {
        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register("RegionName", typeof(string), typeof(RegionNameBehavior), new PropertyMetadata(null));

        public string RegionName
        {
            get
            {
                return (string)GetValue(DependencyProperty);
            }
            set
            {
                SetValue(DependencyProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ChangeRegionName += ChangeRegionName;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ChangeRegionName -= ChangeRegionName;
        }


        private void ChangeRegionName()
        {
            RegionName = AssociatedObject.RegionName;
        }

    }
}