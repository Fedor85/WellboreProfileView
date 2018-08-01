using System;
using Microsoft.Practices.Unity;
using Prism;
using WellboreProfileView.Enums;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public class MainPageControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IRegionContextManager RegionContextManager { get; set; }

        [Dependency]
        public IControlViewManager ControlViewManager { get; set; }

        private IMainPageRegionContext RegionContext;

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
            RegionContextManager.ActionSubscribeChangeRegionContext(RegionName, ChangeRegionContext);
            RegionContextManager.ActionSubscribeChangeRegionContext(RegionNames.MainPageButtonsPanelRegion, ChangeMainPageButtonsPanelRegioContext);
        }

        private void DeActivate()
        {
            ControlViewManager.DeactivateAllActiveViewToMainPageControl();
            RegionContextManager.UnsubscribeChangeRegionContext(RegionNames.MainPageButtonsPanelRegion, ChangeMainPageButtonsPanelRegioContext);
            RegionContextManager.UnsubscribeChangeRegionContext(RegionName, ChangeRegionContext);
        }

        private void ChangeMainPageButtonsPanelRegioContext(object mainPageButtonsPanelRegioContext)
        {
            if (RegionContext != null && mainPageButtonsPanelRegioContext != null)
            {
                long displayPageRegionTypeId = ((IMainPageButtonsPanelControlRegionContext)mainPageButtonsPanelRegioContext).DisplayPageRegionTypeId;
                RegionContext.DisplayPageRegionTypeId = displayPageRegionTypeId;
                Update();
            }
        }

        private void ChangeRegionContext(object regionContext)
        {
            RegionContext = regionContext as IMainPageRegionContext;
            Update();
        }

        private void Update()
        {
            if (RegionContext != null)
            {
                long entityTypeId = GetEntityTypeId();
                long displayPageRegionTypeId = GetDisplayPageRegionTypeId();
                ControlViewManager.DeactivateMainPageControlViews(entityTypeId);
                SetChaildsDataContext(entityTypeId, displayPageRegionTypeId);
                ControlViewManager.ActivateMainPageControlViews(entityTypeId, displayPageRegionTypeId);
            }
        }

        private long GetEntityTypeId()
        {
            return RegionContext.NavigationTreeViewRegionContext != null ? RegionContext.NavigationTreeViewRegionContext.EntityTypeId : (long)EntityType.NoN;
        }

        private long GetDisplayPageRegionTypeId()
        {
            return RegionContext != null ? RegionContext.DisplayPageRegionTypeId : (long)DisplayPageRegionType.NoN;
        }

        private void SetChaildsDataContext(long entityTypeId, long displayPageRegionTypeId)
        {
            if (entityTypeId == (long)EntityType.NoN)
            {
                RegionContextManager.SetRegionContext(RegionNames.MainPageCaptionRegion, String.Empty);
            }
            else
            {
                RegionContextManager.SetRegionContext(RegionNames.MainPageCaptionRegion, RegionContext.NavigationTreeViewRegionContext.FullName);
                if (displayPageRegionTypeId != (long)DisplayPageRegionType.NoN)
                    RegionContextManager.SetRegionContext(RegionNames.PageRegion, RegionContextManager.GetPrameters(RegionContext.NavigationTreeViewRegionContext.EntityId,
                                                            RegionContext.DisplayPageRegionTypeId));
            }
        }
    }
}