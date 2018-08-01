using WellboreProfileView.Interfaces;

namespace WellboreProfileView.RegionContext
{
    public class NavigationTreeViewRegionContext : EntityRegionContext, INavigationTreeViewRegionContext
    {
        public long EntityTypeId { get; set; }

        public string FullName { get; set; }
    }
}