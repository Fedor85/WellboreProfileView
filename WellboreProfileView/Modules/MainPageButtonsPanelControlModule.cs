using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public class MainPageButtonsPanelControlModule : BaseModule
    {
        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IMainPageButtonsPanelControl>();
            ((IRegionUserControl)view).RegionName = RegionNames.MainPageButtonsPanelRegion;
            RegionManager.Regions[RegionNames.MainPageButtonsPanelRegion].Add(view, ControlNames.MainPageButtonsPanelControl);
        }
    }
}