using System.ComponentModel;
using Prism.Mvvm;

namespace WellboreProfileView.ViewModels
{
    public class BaseNameBindableEditableObject : BindableBase, IEditableObject
    {
        protected NameDataObject CurrentNameDataObject;

        private NameDataObject BackupNameDataObject;

        private bool inEdit;

        public void BeginEdit()
        {
            if (inEdit)
                return;

            inEdit = true;
            BackupNameDataObject = (NameDataObject)CurrentNameDataObject.Clone();
        }

        public void EndEdit()
        {
            if (!inEdit)
                return;

            inEdit = false;
            BackupNameDataObject = null;
        }

        public void CancelEdit()
        {
            if (!inEdit)
                return;

            inEdit = false;
            CurrentNameDataObject = BackupNameDataObject;
        }

        protected BaseNameBindableEditableObject()
        {
            CurrentNameDataObject = new NameDataObject();
        }
    }
}