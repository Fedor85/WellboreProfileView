namespace WellboreProfileView.Modules
{
    public class MultiDrawRangeControlToMultiPageUpModule : BaseMultiDrawRangeControlModule
    {
        protected override string RegionName { get { return RegionNames.MultiPageUpRegion; } }

        protected override string RegionManagerName { get { return RegionManagerNames.UpMultiTableRegionManager; } }
    }
}