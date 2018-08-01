using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public class MainPageCaptionControlModule : BaseModule
    {
        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IMainPageCaptionControl>();
            ((IRegionUserControl)view).RegionName = RegionNames.MainPageCaptionRegion;
            RegionManager.Regions[RegionNames.MainPageCaptionRegion].Add(view, ControlNames.MainPageCaptionControl);
        }
    }
}