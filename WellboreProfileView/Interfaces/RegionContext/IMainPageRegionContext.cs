namespace WellboreProfileView.Interfaces
{
    public interface IMainPageRegionContext
    {
        INavigationTreeViewRegionContext NavigationTreeViewRegionContext { get; set; }

        long DisplayPageRegionTypeId { get; set; }
    }
}