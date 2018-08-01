using WellboreProfileView.Enums;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.RegionContext
{
    public class MainPageButtonsPanelControlRegionContext : IMainPageButtonsPanelControlRegionContext
    {
        public long DisplayPageRegionTypeId { get; set; }

        public MainPageButtonsPanelControlRegionContext()
        {
            DisplayPageRegionTypeId = (long)DisplayPageRegionType.NoN;
        }
    }
}