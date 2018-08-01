using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Views
{
    /// <summary>
    /// Interaction logic for MainPageControl.xaml
    /// </summary>
    public partial class MainPageControl : BaseRegionNameControl, IMainPageControl
    {
        public MainPageControl()
        {
            InitializeComponent();
        }
    }
}