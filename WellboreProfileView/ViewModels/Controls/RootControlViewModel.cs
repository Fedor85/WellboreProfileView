using System;
using System.ComponentModel;
using DevZest.Windows;
using Microsoft.Practices.Unity;
using Prism;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Enums;
using WellboreProfileView.Interfaces.Services;

namespace WellboreProfileView.ViewModels
{
    public class RootControlViewModel : BaseControlViewModel, IActiveAware
    {
        [Dependency]
        public ISettingServices SettingServices { get; set; }

        [Dependency]
        public IDialogService DialogService { get; set; }

        [Dependency]
        public IControlViewManager ControlViewManager { get; set; }

        [Dependency]
        public IRegionContextManager RegionContextManager { get; set; }

        private SplitterDistance splitterDistance;

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

        public SplitterDistance SplitterDistance
        {
            get
            {
                return splitterDistance;
            }
            set
            {
                splitterDistance = value;
                RaisePropertyChanged();
            }
        }

        private void Activate()
        {
            RegionContextManager.ActionSubscribeChangeRegionContext(RegionNames.NavigationTreeViewRegion, ChangeRegionContext);
            InitializeSplitterDistance();
        }

        private void DeActivate()
        {
            RegionContextManager.UnsubscribeChangeRegionContext(RegionNames.NavigationTreeViewRegion, ChangeRegionContext);
            ControlViewManager.DeactivateAllActiveViewToRootControl();
            SaveSplitterDistance();
        }

        private void ChangeRegionContext(object regionContext)
        {
            RegionContextManager.SetRegionContext(RegionNames.MainPageRegion, regionContext as INavigationTreeViewRegionContext);
        }

        protected override void ClosingUserControl(CancelEventArgs cansel)
        {
            if (DialogService.Ask("Закрыть программу?") == InternalDialogResult.OK)
            {
                DeActivate();
            }
            else
            {
                cansel.Cancel = true;
            }
        }

        private void InitializeSplitterDistance()
        {
            double? splitterDistance = SettingServices.GetTreeViewWellSplitterDistance();
            SplitterDistance = new SplitterDistance(splitterDistance.HasValue ? splitterDistance.Value : 0, SplitterUnitType.Star);
        }

        private void SaveSplitterDistance()
        {
            SettingServices.SaveTreeViewWellSplitterDistance(SplitterDistance.Value);
        }
    }
}