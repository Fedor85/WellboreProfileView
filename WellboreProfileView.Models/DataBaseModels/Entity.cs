using WellboreProfileView.Models.Interfaces;

namespace WellboreProfileView.Models.DataBaseModels
{
    public class Entity : IIdEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
