using System;
using System.Linq;
using WellboreProfileView.Models.DataBaseModels;
using WellboreProfileView.ViewModels.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public class WellboreGridViewModel : BaseNameBindableEditableObject, IValidateGridObject
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

        public long WellId { get; set; }

        public SmartObservableCollection<ProfilePathGridViewModel> ChildItems { get; private set; }

        public WellboreGridViewModel()
        {
            ChildItems = new SmartObservableCollection<ProfilePathGridViewModel>();
        }

        public WellboreGridViewModel(Wellbore wellbore) :this()
        {
            Id = wellbore.Id;
            Name = wellbore.Name;
            WellId = wellbore.WellId;

            foreach (ProfilePath profilePath in wellbore.ProfilePaths)
                ChildItems.AddExisting(new ProfilePathGridViewModel(profilePath));
        }

        public bool IsValidate(object parentObject, out string message)
        {
            message = String.Empty;
            if (String.IsNullOrEmpty(Name))
            {
                message = "Имя свола не может быть пустым!";
                return false;
            }
            WellboresRootGridViewModel wellboresRootGridViewModel = (WellboresRootGridViewModel)parentObject;
            int count = wellboresRootGridViewModel.MainItems.ToList().FindAll(item => item.Name.Equals(Name)).Count;
            if (count > 1)
            {
                message = String.Format("Имя ствола должно быть уникальным для скважины {0}!", wellboresRootGridViewModel.Root.Name);
                return false;
            }
            return true;
        }
    }
}