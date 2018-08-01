using System;
using System.Linq;
using WellboreProfileView.Models.DataBaseModels;
using WellboreProfileView.ViewModels.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public class AreaGridViewModel : BaseNameBindableEditableObject, IValidateGridObject
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

        public SmartObservableCollection<WellGridViewModel> ChildItems { get; private set; }

        public AreaGridViewModel()
        {
            ChildItems = new SmartObservableCollection<WellGridViewModel>();
        }

        public AreaGridViewModel(Area area) :this()
        {
            Id = area.Id;
            Name = area.Name;

            foreach (Well well in area.Wells)
                ChildItems.AddExisting(new WellGridViewModel(well));
        }

        public bool IsValidate(object parentObject, out string message)
        {
            message = String.Empty;
            if (String.IsNullOrEmpty(Name))
            {
                message = "Имя площади не может быть пустым!";
                return false;
            }

            int count = ((AreasRootGridViewModel)parentObject).MainItems.ToList().FindAll(item => item.Name.Equals(Name)).Count;
            if (count > 1)
            {
                message = "Имя площади должно быть уникальным!";
                return false;
            }
            return true;
        }
    }
}