using System.Windows.Controls;
using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public abstract class BaseDrawPlanControlModule : BaseModule
    {
        protected virtual string RegionName { get; }

        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IDrawRangeControl>();
            ((IRegionUserControl)view).RegionName = RegionName;
            ((UserControl)view).DataContext = UnityContainer.Resolve<IDrawPlanControlViewModel>();
            RegionManager.Regions[RegionName].Add(view, ControlNames.DrawingPlanControl);
        }
    }
}