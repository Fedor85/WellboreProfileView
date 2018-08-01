using WellboreProfileView.Enums;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.RegionContext
{
    public class MainPageRegionContext : IMainPageRegionContext
    {
        public INavigationTreeViewRegionContext NavigationTreeViewRegionContext { get; set; }

        public long DisplayPageRegionTypeId { get; set; }

        public MainPageRegionContext()
        {
            DisplayPageRegionTypeId = (long)DisplayPageRegionType.NoN;
        }
    }
}