using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public class NavigationButtonsPanelControlModule : BaseModule
    {
        public override void Initialize()
        {
            object view = UnityContainer.Resolve<INavigationButtonsPanelControl>();
            ((IRegionUserControl)view).RegionName = RegionNames.NavigationButtonsPanelRegion;
            RegionManager.Regions[RegionNames.NavigationButtonsPanelRegion].Add(view, ControlNames.NavigationButtonsPanelControl);
        }
    }
}