using System;
using System.ComponentModel;
using System.Text;
using Prism.Mvvm;
using WellboreProfileView.Models.DataBaseModels;
using WellboreProfileView.ViewModels.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public class ProfilePathGridViewModel : BindableBase, IValidateGridObject, IEditableObject
    {
        private ProfilePathDataObject CurrentProfilePathDataObject;

        private ProfilePathDataObject BackupProfilePathDataObject;

        private bool inEdit;

        public long Id { get; set; }

        public long WellboreId { get; set; }

        public double VerticalDepth
        {
            get
            {
                return CurrentProfilePathDataObject.VerticalDepth;
            }
            set
            {
                CurrentProfilePathDataObject.VerticalDepth = value;
                RaisePropertyChanged();
            }
            
        }

        public double InclinationAngle
        {
            get
            {
                return CurrentProfilePathDataObject.InclinationAngle;
            }
            set
            {
                CurrentProfilePathDataObject.InclinationAngle = value;
                RaisePropertyChanged();
            }

        }

        public double AzimuthAngle
        {
            get
            {
                return CurrentProfilePathDataObject.AzimuthAngle;
            }
            set
            {
                CurrentProfilePathDataObject.AzimuthAngle = value;
                RaisePropertyChanged();
            }

        }

        public double Extension
        {
            get
            {
                return CurrentProfilePathDataObject.Extension;
            }
            set
            {
                CurrentProfilePathDataObject.Extension = value;
                RaisePropertyChanged();
            }

        }

        public ProfilePathGridViewModel()
        {
            CurrentProfilePathDataObject = new ProfilePathDataObject();
        }

        public ProfilePathGridViewModel(ProfilePath profilePath):this()
        { 
            Id = profilePath.Id;
            VerticalDepth = profilePath.VerticalDepth;
            InclinationAngle = profilePath.InclinationAngle;
            AzimuthAngle = profilePath.AzimuthAngle;
            Extension = profilePath.Extension;
        }

        public void BeginEdit()
        {
            if (inEdit)
                return;

            inEdit = true;
            BackupProfilePathDataObject = (ProfilePathDataObject)CurrentProfilePathDataObject.Clone();
        }

        public void EndEdit()
        {
            if (!inEdit)
                return;

            inEdit = false;
            BackupProfilePathDataObject = null;
        }

        public void CancelEdit()
        {
            if (!inEdit)
                return;

            inEdit = false;
            CurrentProfilePathDataObject = BackupProfilePathDataObject;
        }

        public bool IsValidate(object parentObject, out string message)
        {
            StringBuilder errors = new StringBuilder();
            if (VerticalDepth < 0)
                errors.AppendLine("Глубина, м не может быть < 0");

            if (InclinationAngle < 0)
                errors.AppendLine("Зенит, град не может быть < 0");

            if (AzimuthAngle < 0)
                errors.AppendLine("Азимут, град не может быть < 0");

            if (Extension < 0)
                errors.AppendLine("Удлинение, м не может быть < 0");

            message = errors.ToString();
            return String.IsNullOrEmpty(message);
        }
    }
}