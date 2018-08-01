using System.Windows.Controls;
using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public abstract class BaseDrawProfileControlModule : BaseModule
    {
        protected virtual string RegionName { get; }

        public override void Initialize()
        {
            object view = UnityContainer.Resolve<IDrawRangeControl>();
            ((IRegionUserControl)view).RegionName = RegionName;
            ((UserControl)view).DataContext = UnityContainer.Resolve<IDrawProfileControlViewModel>();
            RegionManager.Regions[RegionName].Add(view, ControlNames.DrawingProfileControl);
        }
    }
}