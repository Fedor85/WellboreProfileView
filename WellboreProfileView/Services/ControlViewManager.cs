using System.Collections.Generic;
using Prism.Modularity;
using Prism.Regions;
using WellboreProfileView.Enums;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Services
{
    public class ControlViewManager : IControlViewManager
    {
        private IRegionManager regionManager { get; set; }

        private IModuleManager moduleManager { get; set; }

        public Dictionary<string, IRegionManager> RegionManagers;

        public ControlViewManager(IRegionManager regionManager, IModuleManager moduleManager)
        {
            this.regionManager = regionManager;
            this.moduleManager = moduleManager;
            RegionManagers = new Dictionary<string, IRegionManager>();
        }

        public void AddRegionManager(string regionManagerName, IRegionManager regionManager)
        {
            if (RegionManagers.ContainsKey(regionManagerName))
            {
                IRegionManager currentRegionManager = RegionManagers[regionManagerName];
                DeactivateAllActiveViewToRegionManager(currentRegionManager);
                RegionManagers[regionManagerName] = regionManager;
            }
            else
            {
                RegionManagers.Add(regionManagerName, regionManager);
            }
        }

        #region Activate

        #region MainPageControl

        public void ActivateMainPageControlViews(long entityTypeId, long displayPageRegionTypeId)
        {
            switch (entityTypeId)
            {
                case (long)EntityType.Area:
                    ActivateViewsForArea();
                    break;
                case (long)EntityType.Well:
                    ActivateViewsForWell(displayPageRegionTypeId);
                    break;
            }
        }

        private void ActivateViewsForArea()
        {
            ActivateView(ModuleNames.MainPageCaptionControlModule, RegionNames.MainPageCaptionRegion, ControlNames.MainPageCaptionControl);
        }

        private void ActivateViewsForWell(long displayPageRegionTypeId)
        {
            ActivateView(ModuleNames.MainPageButtonsPanelControlModule, RegionNames.MainPageButtonsPanelRegion, ControlNames.MainPageButtonsPanelControl);
            ActivateView(ModuleNames.MainPageCaptionControlModule, RegionNames.MainPageCaptionRegion, ControlNames.MainPageCaptionControl);
            if (displayPageRegionTypeId == (long)DisplayPageRegionType.OneTable)
            {
                DeactivateAllActiveViewToRegion(RegionNames.PageRegion);
                ActivateView(ModuleNames.WellboreTableControlToPageRegionModule, RegionNames.PageRegion, ControlNames.WellboreTableControl);

            }
            else if (EnumsHelper.IsMultiTable(displayPageRegionTypeId))
            {
                DeactivateAllActiveViewToRegion(RegionNames.PageRegion);
                ActivateView(ModuleNames.MultiPageControlModule, RegionNames.PageRegion, ControlNames.MultiPageControl);
            }
        }

        #endregion

        public void ActivateMultiPageControlViews(long displayPageRegionTypeId, long multiTablePositionTypeId)
        {
            if (multiTablePositionTypeId == (long)MultiTablePositionType.Up)
            {
                ActivateView(ModuleNames.WellboreTableControlToMultiPageUpRegionModule, RegionNames.MultiPageUpRegion, ControlNames.WellboreTableControl);
                switch (displayPageRegionTypeId)
                {
                    case (long)DisplayPageRegionType.MultiTableText:
                        ActivateView(ModuleNames.InfoProfileCoordinatesControlToMultiPageBottomModule, RegionNames.MultiPageBottomRegion, ControlNames.InfoProfileCoordinatesControl);
                        break;
                    case (long)DisplayPageRegionType.MultiTableProfile:
                        ActivateView(ModuleNames.DrawProfileControlToMultiPageBottomModule, RegionNames.MultiPageBottomRegion, ControlNames.DrawingProfileControl);
                        break;
                    case (long)DisplayPageRegionType.MultiTablePlan:
                        ActivateView(ModuleNames.DrawPlanControlToMultiPageBottomModule, RegionNames.MultiPageBottomRegion, ControlNames.DrawingPlanControl);
                        break;
                    case (long)DisplayPageRegionType.MultiTableMultiDraw:
                        ActivateView(ModuleNames.MultiDrawRangeControlToMultiPageBottomModule, RegionNames.MultiPageBottomRegion, ControlNames.MultiDrawRangeControl);
                        break;
                    case (long)DisplayPageRegionType.MultiTable3D:
                        ActivateView(ModuleNames.DrawRange3DControlToMultiPageBottomModule, RegionNames.MultiPageBottomRegion, ControlNames.DrawRange3DControl);
                        break;
                }
                
            }
            else if (multiTablePositionTypeId == (long)MultiTablePositionType.Down)
            {
                switch (displayPageRegionTypeId)
                {
                    case (long)DisplayPageRegionType.MultiTableText:
                        ActivateView(ModuleNames.InfoProfileCoordinatesControlToMultiPageUpModule, RegionNames.MultiPageUpRegion, ControlNames.InfoProfileCoordinatesControl);
                        break;
                    case (long)DisplayPageRegionType.MultiTableProfile:
                        ActivateView(ModuleNames.DrawProfileControlToMultiPageUpModule, RegionNames.MultiPageUpRegion, ControlNames.DrawingProfileControl);
                        break;
                    case (long)DisplayPageRegionType.MultiTablePlan:
                        ActivateView(ModuleNames.DrawPlanControlToMultiPageUpModule, RegionNames.MultiPageUpRegion, ControlNames.DrawingPlanControl);
                        break;
                    case (long)DisplayPageRegionType.MultiTableMultiDraw:
                        ActivateView(ModuleNames.MultiDrawRangeControlToMultiPageUpModule, RegionNames.MultiPageUpRegion, ControlNames.MultiDrawRangeControl);
                        break;
                    case (long)DisplayPageRegionType.MultiTable3D:
                        ActivateView(ModuleNames.DrawRange3DControlToMultiPageUpModule, RegionNames.MultiPageUpRegion, ControlNames.DrawRange3DControl);
                        break;
                }
                ActivateView(ModuleNames.WellboreTableControlToMultiPageBottomRegionModule, RegionNames.MultiPageBottomRegion, ControlNames.WellboreTableControl);
            }
        }

        public void ActivateAllViewToRegionManager(string regionManagerNames)
        {
            ActivateAllActiveViewToRegionManager(regionManagerNames);
        }

        #endregion

        #region Deactivate

        public void DeactivateAllActiveViewToRootControl()
        {
            DeactivateAllActiveViewToRegion(RegionNames.MainPageRegion);
            DeactivateAllActiveViewToRegion(RegionNames.NavigationTreeViewRegion);
            DeactivateAllActiveViewToRegion(RegionNames.NavigationButtonsPanelRegion);
        }

        public void DeactivateAllActiveViewToMainPageControl()
        {
            DeactivateAllActiveViewToRegion(RegionNames.MainPageButtonsPanelRegion);
            DeactivateAllActiveViewToRegion(RegionNames.MainPageCaptionRegion);
            DeactivateAllActiveViewToRegion(RegionNames.PageRegion);
        }

        public void DeactivateAllActiveViewToMultiPageControl()
        {
            DeactivateAllActiveViewToRegion(RegionNames.MultiPageUpRegion);
            DeactivateAllActiveViewToRegion(RegionNames.MultiPageBottomRegion);
        }

        public void DeactivateAllViewToMultiDrawRangeControl()
        {
            DeactivateAllActiveViewToRegionManager(RegionManagerNames.UpMultiTableRegionManager);
            DeactivateAllActiveViewToRegionManager(RegionManagerNames.BottomMultiTableRegionManager);
        }

        #region MainPageControl

        public void DeactivateMainPageControlViews(long EntityTpeId)
        {
            switch (EntityTpeId)
            {
                case (long)EntityType.Area:
                    DeactivateViewsForArea();
                    break;
                case (long)EntityType.Well:
                    DeactivateViewsForWell();
                    break;
                case (long)EntityType.NoN:
                    DeactivateAllActiveViewToMainPageControl();
                    break;
            }
        }

        private void DeactivateViewsForArea()
        {
            DeactivateAllActiveViewToRegion(RegionNames.MainPageButtonsPanelRegion);
            DeactivateAllActiveViewToRegion(RegionNames.PageRegion);
        }
        private void DeactivateViewsForWell()
        {
            DeactivateAllActiveViewToRegion(RegionNames.PageRegion);
        }

        #endregion

        #endregion

        private void ActivateView(string moduleName, string regionName, string viewName)
        {
            moduleManager.LoadModule(moduleName);
            IRegion region = regionManager.Regions[regionName];
            object view = region.GetView(viewName);
            foreach (object activeView in region.ActiveViews)
            {
                if (activeView.Equals(view))
                    return;
            }
            region.Activate(view);
        }

        private void ActivateAllActiveViewToRegionManager(string regionManagerName)
        {
            if (RegionManagers.ContainsKey(regionManagerName))
                ActivateAllActiveViewToRegionManager(RegionManagers[regionManagerName]);
        }

        private void ActivateAllActiveViewToRegionManager(IRegionManager regionManager)
        {
            foreach (IRegion region in regionManager.Regions)
                ActivateAllActiveViewToRegion(region);
        }

        private void ActivateAllActiveViewToRegion(IRegion region)
        {
            foreach (object view in region.Views)
                region.Activate(view);
        }

        private void DeactivateAllActiveViewToRegionManager(string regionManagerName)
        {
            if (RegionManagers.ContainsKey(regionManagerName))
                DeactivateAllActiveViewToRegionManager(RegionManagers[regionManagerName]);
        }

        private void DeactivateAllActiveViewToRegionManager(IRegionManager regionManager)
        {
            foreach (IRegion region in regionManager.Regions)
                DeactivateAllActiveViewToRegion(region);
        }

        private void DeactivateAllActiveViewToRegion(string regionName)
        {
            IRegion region = regionManager.Regions[regionName];
            DeactivateAllActiveViewToRegion(region);
        }

        private void DeactivateAllActiveViewToRegion(IRegion region)
        {
            foreach (object view in region.ActiveViews)
                region.Deactivate(view);
        }
    }
}