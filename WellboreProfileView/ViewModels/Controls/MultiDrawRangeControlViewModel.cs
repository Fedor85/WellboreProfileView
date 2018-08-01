using System;
using Microsoft.Practices.Unity;
using Prism;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public class MultiDrawRangeControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IControlViewManager ControlViewManager { get; set; }

        private bool active;

        public event EventHandler IsActiveChanged;

        public bool IsActive
        {
            get
            {
                return active;
            }
            set
            {
                if (active != value)
                {
                    active = value;
                    if (active)
                        Activate();
                    else
                        DeActivate();
                }
            }
        }

        private void Activate()
        {
            if (String.IsNullOrEmpty(RegionName))
                return;

            if (RegionName.Equals(RegionNames.MultiPageUpRegion))
            {
                ControlViewManager.ActivateAllViewToRegionManager(RegionManagerNames.UpMultiTableRegionManager);
            }
            else if (RegionName.Equals(RegionNames.MultiPageBottomRegion))
            {
                ControlViewManager.ActivateAllViewToRegionManager(RegionManagerNames.BottomMultiTableRegionManager);
            }
        }
        
        private void DeActivate()
        {
            ControlViewManager.DeactivateAllViewToMultiDrawRangeControl();
        }
    }
}