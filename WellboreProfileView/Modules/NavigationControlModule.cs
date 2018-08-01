using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public class NavigationControlModule : BaseModule
    {
        public override void Initialize()
        {
            object view = UnityContainer.Resolve<INavigationControl>();
            ((IRegionUserControl)view).RegionName = RegionNames.NavigationTreeViewRegion;
            RegionManager.Regions[RegionNames.NavigationTreeViewRegion].Add(view, ControlNames.NavigationControl);
        }
    }
}