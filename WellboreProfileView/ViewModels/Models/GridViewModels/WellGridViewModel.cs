using System;
using System.Linq;
using WellboreProfileView.Models.DataBaseModels;
using WellboreProfileView.ViewModels.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public class WellGridViewModel : BaseNameBindableEditableObject, IValidateGridObject
    {
        public long Id { get; set; }

        public string Name
        {
            get { return CurrentNameDataObject.Name; }
            set
            {
                CurrentNameDataObject.Name = value;
                RaisePropertyChanged();
            }
        }

        public SmartObservableCollection<WellboreGridViewModel> ChildItems { get; private set; }

        public WellGridViewModel()
        {
            ChildItems = new SmartObservableCollection<WellboreGridViewModel>();
        }

        public WellGridViewModel(Well well) :this()
        {
            Id = well.Id;
            Name = well.Name;

            foreach (Wellbore wellWellbore in well.Wellbores)
                ChildItems.AddExisting(new WellboreGridViewModel(wellWellbore));
        }

        public bool IsValidate(object parentObject, out string message)
        {
            message = String.Empty;
            if (String.IsNullOrEmpty(Name))
            {
                message = "Имя скважины не может быть пустым!";
                return false;
            }

            AreaGridViewModel areaGridViewModel = (AreaGridViewModel)parentObject;
            int count = areaGridViewModel.ChildItems.ToList().FindAll(item => item.Name.Equals(Name)).Count;
            if (count > 1)
            {
                message = String.Format("Имя скважины должно быть уникальным для площади {0}!", areaGridViewModel.Name);
                return false;
            }
            return true;
        }
    }
}