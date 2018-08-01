using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public class MainPageControlModule : BaseModule
    {
        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IMainPageControl>();
            ((IRegionUserControl)view).RegionName = RegionNames.MainPageRegion;
            RegionManager.Regions[RegionNames.MainPageRegion].Add(view, ControlNames.MainPageControl);
        }
    }
}