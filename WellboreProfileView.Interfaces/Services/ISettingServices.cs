namespace WellboreProfileView.Interfaces.Services
{
    public interface ISettingServices
    {
        string GetLastOpenedNavigationTreeViewItem();

        void SaveLastOpenedNavigationTreeViewItem(long nodeTypeId, long nodeId);

        long? GetDisplayPageRegionType();

        void SaveDisplayPageRegionType(long displayPageRegionType);

        double? GetTreeViewWellSplitterDistance();

        void SaveTreeViewWellSplitterDistance(double width);

        double? GetMultiTablePageControlSplitterDistance();

        void SaveMultiTablePageControlSplitterDistance(double height);

        long? GetMultiTablePositionTypeId();

        void SaveMultiTablePositionType(long multiTablePositionTypeId);
    }
}