using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace WellboreProfileView.ViewModels
{
    public abstract class BaseTreeViewModel : BindableBase
    {
        private bool isSelected;

        private bool isExpanded;

        public long Id { get; set; }

        public string Name { get; set; }

        public RootTreeView RootTreeView { get; set; }

        public BaseTreeViewModel Parent { get; set; }

        public ObservableCollection<BaseTreeViewModel> Childs { get; set; }

        private BaseTreeViewModel()
        {
            Childs = new ObservableCollection<BaseTreeViewModel>();
        }

        protected BaseTreeViewModel(long id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        protected BaseTreeViewModel(long id, string name, BaseTreeViewModel parent) : this(id, name)
        {
            Parent = parent;
            RootTreeView = parent.RootTreeView;
        }

        public abstract long GetEntityTypeId();

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    RaisePropertyChanged();

                    if (isSelected)
                    {
                        RootTreeView.CurrentTreeViewItem = this;
                        IsExpanded = true;
                    }
                }
            }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (value != isExpanded)
                {
                    isExpanded = value;
                    RaisePropertyChanged();
                }

                if (isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        public bool Equals(long entityTypeId, long id)
        {
            return GetEntityTypeId() == entityTypeId && Id == id;
        }

        public BaseTreeViewModel FindSelecetItem()
        {
            if (IsSelected)
                return this;

            foreach (BaseTreeViewModel child in Childs)
            {
                BaseTreeViewModel childSelect = child.FindSelecetItem();
                if (childSelect != null)
                   return childSelect;
            }
            return null;
        }

        public string FullName()
        {
            if (Parent == null)
                return Name;

            return String.Format("{0} > {1}", Parent.FullName(), Name);
        }
    }
}