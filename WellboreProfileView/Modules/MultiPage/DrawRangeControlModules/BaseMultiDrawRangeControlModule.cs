using System.Windows.Controls;
using Microsoft.Practices.Unity;
using Prism.Regions;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public abstract class BaseMultiDrawRangeControlModule : BaseModule
    {
        protected virtual string RegionName { get; }

        protected virtual string RegionManagerName { get; }

        public override void Initialize()
        {
            object mainView = UnityContainer.Resolve<IMultiDrawRangeControl>();
            ((IRegionUserControl)mainView).RegionName = RegionName;
            IRegionManager multiRegionManager = RegionManager.Regions[RegionName].Add(mainView, ControlNames.MultiDrawRangeControl, true);
            ControlViewManager.AddRegionManager(RegionManagerName, multiRegionManager);

            object profileView = UnityContainer.Resolve<IDrawRangeControl>();
            ((IRegionUserControl)profileView).RegionName = RegionNames.LeftMultiDrawRangeRegion;
            ((UserControl)profileView).DataContext = UnityContainer.Resolve<IDrawProfileControlViewModel>();
            multiRegionManager.Regions[RegionNames.LeftMultiDrawRangeRegion].Add(profileView, ControlNames.DrawingProfileControl);


            object planView = UnityContainer.Resolve<IDrawRangeControl>();
            ((IRegionUserControl)profileView).RegionName = RegionNames.RightMultiDrawRangeRegion;
            ((UserControl)planView).DataContext = UnityContainer.Resolve<IDrawPlanControlViewModel>();
            multiRegionManager.Regions[RegionNames.RightMultiDrawRangeRegion].Add(planView, ControlNames.DrawingPlanControl);
        }
    }
}