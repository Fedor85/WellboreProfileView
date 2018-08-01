using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public abstract class BaseDrawRange3DControlModule : BaseModule
    {
        protected virtual string RegionName { get; }

        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IDraw3DControl>();
            ((IRegionUserControl)view).RegionName = RegionName;
            RegionManager.Regions[RegionName].Add(view, ControlNames.DrawRange3DControl);
        }
    }
}