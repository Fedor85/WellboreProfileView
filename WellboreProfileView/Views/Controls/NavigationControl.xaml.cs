using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Views
{
    /// <summary>
    /// Interaction logic for NavigationControl.xaml
    /// </summary>
    public partial class NavigationControl : BaseRegionNameControl, INavigationControl
    {
        public NavigationControl()
        {
            InitializeComponent();
        }
    }
}