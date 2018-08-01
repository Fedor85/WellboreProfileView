using System;
using Microsoft.Practices.Unity;
using Prism;
using Utils;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Mappers;

namespace WellboreProfileView.ViewModels
{
    public class NavigationControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IDataGatewayService DataGatewayService { get; set; }

        [Dependency]
        public ISettingServices SettingServices { get; set; }

        [Dependency]
        public IRegionContextManager RegionContextManager { get; set; }

        [Dependency]
        public IButtonsEventCommandService ButtonsEventCommandService { get; set; }


        private RootTreeView root;

        private bool active;

        public event EventHandler IsActiveChanged;

        public bool IsActive
        {
            get
            {
                return active;
            }
            set
            {
                if (active != value)
                {
                    active = value;
                    if (active)
                        Activate();
                    else
                        DeActivate();
                }
            }
        }

        public RootTreeView Root
        {
            get { return root; }
            set
            {
                root = value;
                RaisePropertyChanged();
            }
        }

        public BaseTreeViewModel CurrenTreeViewItem { get; set; }

        private void Activate()
        {
            ButtonsEventCommandService.Subscribe(CommandNames.RefreshTreeViewDCommand, RefreshTreeView, true);
            InitializeRootTreeView();
            InitializeCurrenTreeViewModel();
        }

        private void DeActivate()
        {
            ButtonsEventCommandService.UnSubscribe(CommandNames.RefreshTreeViewDCommand, RefreshTreeView);
            SaveCurrenTreeViewModel();
            Root = null;
        }

        private void InitializeRootTreeView()
        {
            Root = MapperViewModel.GetAreaRootTreeView(DataGatewayService.GetAllAreaAndWellOrderByName());
            Root.SelectedItemChanged += RootSelectedItemChanged;
        }

        private void InitializeCurrenTreeViewModel()
        {
            string stringTreeViewItem = SettingServices.GetLastOpenedNavigationTreeViewItem();
            if (!String.IsNullOrEmpty(stringTreeViewItem))
            {
                long id = StringHelper.GetTreeViewItemId(stringTreeViewItem);
                long entityTypeId = StringHelper.GetTreeViewItemEntityTypeId(stringTreeViewItem);
                Root.SetSelectItem(entityTypeId, id);
            }
            SetDefaultCurrenTreeViewItemIfNecessary();
        }

        private void SetDefaultCurrenTreeViewItemIfNecessary()
        {
            BaseTreeViewModel selecetItem = Root.FindSelecetItem();
            if (selecetItem == null && Root.Areas.Count > 0)
                Root.Areas[0].IsSelected = true;
        }

        private void SaveCurrenTreeViewModel()
        {
            if (CurrenTreeViewItem != null)
                SettingServices.SaveLastOpenedNavigationTreeViewItem(CurrenTreeViewItem.GetEntityTypeId(), CurrenTreeViewItem.Id);
        }

        private void RootSelectedItemChanged(BaseTreeViewModel baseTreeViewModel)
        {
            CurrenTreeViewItem = baseTreeViewModel;
            RegionContextManager.SetRegionContext(RegionName, CurrenTreeViewItem);
        }

        private void RefreshTreeView()
        {
            InitializeRootTreeView();
            Root.SetSelectItem(CurrenTreeViewItem);
            SetDefaultCurrenTreeViewItemIfNecessary();
        }
    }
}