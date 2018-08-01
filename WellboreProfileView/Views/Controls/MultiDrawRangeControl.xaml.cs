using System.Windows.Controls;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Views
{
    /// <summary>
    /// Interaction logic for MultiDrawRangeControl.xaml
    /// </summary>
    public partial class MultiDrawRangeControl : BaseRegionNameControl, IMultiDrawRangeControl
    {
        public MultiDrawRangeControl()
        {
            InitializeComponent();
        }
    }
}
