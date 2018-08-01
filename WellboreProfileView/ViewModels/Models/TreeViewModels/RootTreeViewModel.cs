using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.ViewModels
{
    public class RootTreeView
    {
        private BaseTreeViewModel currentTreeViewItem;

        public BaseTreeViewModel CurrentTreeViewItem
        {
            get
            {
                return currentTreeViewItem;
            }
            set
            {
                currentTreeViewItem = value;
                if (SelectedItemChanged != null)
                    SelectedItemChanged(value);
            }
        }

        public event Action<BaseTreeViewModel> SelectedItemChanged;

        public ObservableCollection<BaseTreeViewModel> Areas { get; private set; }

        public RootTreeView(List<Area> areas)
        {
            Areas = new ObservableCollection<BaseTreeViewModel>();
            foreach (Area area in areas)
                Areas.Add(new AreaTreeViewModel(area, this));
        }

        public void SetSelectItem(BaseTreeViewModel treeViewModel)
        {
            if (treeViewModel == null)
                return;

            SetSelectItem(treeViewModel.GetEntityTypeId(), treeViewModel.Id);
        }

        public void SetSelectItem(long entityTypeId, long id)
        {
            BaseTreeViewModel treeViewModel = GetBaseTreeViewModel(Areas, entityTypeId, id);
            if (treeViewModel != null)
                treeViewModel.IsSelected = true;
        }

        public BaseTreeViewModel FindSelecetItem()
        {
            foreach (BaseTreeViewModel area in Areas)
            {
                BaseTreeViewModel selectItem = area.FindSelecetItem();
                if (selectItem != null)
                    return selectItem;
            }
            return null;
        }

        private BaseTreeViewModel GetBaseTreeViewModel(ObservableCollection<BaseTreeViewModel> items, long entityTypeId, long id)
        {
            foreach (BaseTreeViewModel treeViewModel in items)
            {
                if (treeViewModel.Equals(entityTypeId, id))
                    return treeViewModel;

                BaseTreeViewModel childTreeViewModel = GetBaseTreeViewModel(treeViewModel.Childs, entityTypeId, id);
                if (childTreeViewModel != null)
                    return childTreeViewModel;
            }

            return null;
        }
    }
}