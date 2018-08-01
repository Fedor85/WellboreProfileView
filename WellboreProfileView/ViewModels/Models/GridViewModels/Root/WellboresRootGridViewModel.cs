using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.ViewModels
{
    public class WellboresRootGridViewModel : BaseRootGridViewModel<WellboreGridViewModel>
    {
        public WellGridViewModel Root { get; private set; }
        
        public WellboresRootGridViewModel(Well well)
        {
            Root = new WellGridViewModel(well);
            MainItems = Root.ChildItems;
        }

        public void FixParentRootId()
        {
            foreach (WellboreGridViewModel wellboreGridViewModel in MainItems.AddItems)
                wellboreGridViewModel.WellId = Root.Id;
        }
    }
} 