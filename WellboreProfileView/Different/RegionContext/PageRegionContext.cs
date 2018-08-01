using WellboreProfileView.Interfaces;

namespace WellboreProfileView.RegionContext
{
    public class PageRegionContext : EntityRegionContext, IPageRegionContext
    {
        public long DisplayPageRegionTypeId { get; set; }
    }
}