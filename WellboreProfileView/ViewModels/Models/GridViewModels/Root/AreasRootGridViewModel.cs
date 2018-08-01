using System.Collections.Generic;
using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.ViewModels
{
    public class AreasRootGridViewModel : BaseRootGridViewModel<AreaGridViewModel>
    {
        public AreasRootGridViewModel(List<Area> areas) 
        {
            MainItems = new SmartObservableCollection<AreaGridViewModel>();
            foreach (Area area in areas)
                MainItems.AddExisting(new AreaGridViewModel(area));

        }
    }
}