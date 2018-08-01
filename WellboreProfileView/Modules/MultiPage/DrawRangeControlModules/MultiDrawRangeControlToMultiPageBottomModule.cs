namespace WellboreProfileView.Modules
{
    public class MultiDrawRangeControlToMultiPageBottomModule : BaseMultiDrawRangeControlModule
    {
        protected override string RegionName { get { return RegionNames.MultiPageBottomRegion; } }

        protected override string RegionManagerName { get { return RegionManagerNames.BottomMultiTableRegionManager; } }
    }
}