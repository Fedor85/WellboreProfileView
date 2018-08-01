namespace WellboreProfileView.Interfaces
{
    public interface IPageRegionContext : IEntityRegionContext
    {
        long DisplayPageRegionTypeId { get; set; }
    }
}