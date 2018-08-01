using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public abstract class BaseWellboreTableControlModule : BaseModule
    {
        protected virtual string RegionName { get; }

        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IWellboreTableControl>();
            ((IRegionUserControl)view).RegionName = RegionName;
            RegionManager.Regions[RegionName].Add(view, ControlNames.WellboreTableControl);
        }
    }
}