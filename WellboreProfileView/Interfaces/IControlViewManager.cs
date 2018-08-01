using Prism.Regions;

namespace WellboreProfileView.Interfaces
{
    public interface IControlViewManager
    {
        void AddRegionManager(string regionManagerName, IRegionManager regionManager);

        void ActivateMainPageControlViews(long entityTypeId, long displayPageRegionTypeId);

        void DeactivateAllActiveViewToRootControl();

        void DeactivateAllActiveViewToMainPageControl();

        void DeactivateAllActiveViewToMultiPageControl();

        void DeactivateAllViewToMultiDrawRangeControl();

        void DeactivateMainPageControlViews(long EntityTpeId);

        void ActivateMultiPageControlViews(long displayPageRegionTypeId, long multiTablePositionTypeId);

        void ActivateAllViewToRegionManager(string regionManagerNames);
    }
}