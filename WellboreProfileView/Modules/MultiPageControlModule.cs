using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public class MultiPageControlModule : BaseModule
    {
        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IMultiPageControl>();
            ((IRegionUserControl)view).RegionName = RegionNames.PageRegion;
            RegionManager.Regions[RegionNames.PageRegion].Add(view, ControlNames.MultiPageControl);
        }
    }
}