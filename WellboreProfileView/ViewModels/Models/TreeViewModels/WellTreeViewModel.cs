using WellboreProfileView.Enums;
using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.ViewModels
{
    public class WellTreeViewModel : BaseTreeViewModel
    {
        public WellTreeViewModel(Well well, BaseTreeViewModel parent) : base(well.Id, well.Name, parent)
        {
        }

        public override long GetEntityTypeId()
        {
            return (long)EntityType.Well;
        }
    }
}