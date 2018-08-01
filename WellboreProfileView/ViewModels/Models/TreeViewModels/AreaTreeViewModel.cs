using WellboreProfileView.Enums;
using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.ViewModels
{
    public class AreaTreeViewModel : BaseTreeViewModel
    {
        public AreaTreeViewModel(Area area, RootTreeView rootTreeView) : base (area.Id, area.Name)
        {
            RootTreeView = rootTreeView;
            foreach (Well well in area.Wells)
                Childs.Add(new WellTreeViewModel(well, this));
        }

        public override long GetEntityTypeId()
        {
            return (long)EntityType.Area;
        }
    }
}