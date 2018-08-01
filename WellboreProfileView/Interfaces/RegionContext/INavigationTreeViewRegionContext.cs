namespace WellboreProfileView.Interfaces
{
    public interface INavigationTreeViewRegionContext : IEntityRegionContext
    {
        long EntityTypeId { get; set; }

        string FullName { get; set; }
    }
}