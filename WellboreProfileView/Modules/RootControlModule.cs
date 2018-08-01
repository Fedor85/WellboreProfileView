using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public class RootControlModule : BaseModule
    {
        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IRootControl>();
            ((IRegionUserControl)view).RegionName = RegionNames.RootControlRegion;
            RegionManager.Regions[RegionNames.RootControlRegion].Add(view, ControlNames.RootControl);
        }
    }
}