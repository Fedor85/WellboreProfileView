using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Views
{
    /// <summary>
    /// Interaction logic for WellboreTableControl.xaml
    /// </summary>
    public partial class WellboreTableControl : BaseRegionNameControl, IWellboreTableControl
    {
        public WellboreTableControl()
        {
            InitializeComponent();
            DevExpress.Xpf.Grid.GridControl.AllowInfiniteGridSize = true;
        }
    }
}