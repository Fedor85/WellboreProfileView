using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Events;
using WellboreProfileView.Aspose;
using WellboreProfileView.Events;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Enums;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Mappers;
using WellboreProfileView.Models;
using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.ViewModels
{
    public class WellboreTableControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IRegionContextManager RegionContextManager { get; set; }

        [Dependency]
        public IDataGatewayService DataGatewayService { get; set; }

        [Dependency]
        public IDialogService DialogService { get; set; }

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        [Dependency]
        public IButtonsEventCommandService ButtonsEventCommandService { get; set; }

        [Dependency]
        public IImportService ImportService { get; set; }

        private long? EntityId;

        private WellboresRootGridViewModel root;

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

        public WellboresRootGridViewModel Root
        {
            get { return root; }set
            {
                root = value;
                RaisePropertyChanged();
            }
        }

        private void Activate()
        {
            EventAggregator.GetEvent<SaveDataChangeEvent>().Subscribe(SavePageData);
            EventAggregator.GetEvent<QueryExistenceWellboresRootGridViewModel>().Subscribe(RootPublish);
            RegionContextManager.ActionSubscribeChangeRegionContext(RegionName, ChangeRegionContext);
            ButtonsEventCommandService.Subscribe(CommandNames.RefreshPageData, RefreshPageData, true);
            ButtonsEventCommandService.Subscribe(CommandNames.SavePageData, RefreshPageData, false);
            ButtonsEventCommandService.Subscribe(CommandNames.ImportPageData, ImportPageData, false);
        }

        private void DeActivate()
        {
            SavePageData();
            EventAggregator.GetEvent<SaveDataChangeEvent>().Unsubscribe(SavePageData);
            EventAggregator.GetEvent<QueryExistenceWellboresRootGridViewModel>().Unsubscribe(RootPublish);
            ButtonsEventCommandService.UnSubscribe(CommandNames.RefreshPageData, RefreshPageData);
            ButtonsEventCommandService.UnSubscribe(CommandNames.SavePageData, RefreshPageData);
            ButtonsEventCommandService.UnSubscribe(CommandNames.ImportPageData, ImportPageData);
            RegionContextManager.UnsubscribeChangeRegionContext(RegionName, ChangeRegionContext);
            Root = null;
        }

        private void ChangeRegionContext(object regionContext)
        {
            IEntityRegionContext entityRegionContext = regionContext as IEntityRegionContext;
            EntityId = entityRegionContext == null ? (long?)null : entityRegionContext.EntityId;
            RefreshPageData();
        }

        private void RefreshPageData()
        {
            if (SaveChangeIfNeeded())
                Save();

            if (EntityId.HasValue)
            {
                Root = MapperViewModel.GetWellboreRootGridViewModel(DataGatewayService.GetWellAndWellboreProfilePaths(EntityId.Value));
                Root.MainItems.AnyCollectionChanged += MainItemsAnyCollectionChanged;
                Root.ChangeCurrentItem += RootChangeCurrentItem;
                ButtonsEventCommandService.DeActivate(CommandNames.SavePageData);
                ButtonsEventCommandService.DeActivate(CommandNames.ImportPageData);
                Root.SetDefaultCurrentItem();
            }
            else
            {
                Root = null;
            }
            RootPublish();
        }
        private void RootPublish()
        {
            EventAggregator.GetEvent<ChageWellboresRootGridViewModelEvent>().Publish(Root);
        }

        private void MainItemsAnyCollectionChanged()
        {
            ButtonsEventCommandService.Activate(CommandNames.SavePageData);
        }

        private void RootChangeCurrentItem(object currentItem)
        {
            ButtonsEventCommandService.SetExecute(CommandNames.ImportPageData, currentItem != null && currentItem is WellboreGridViewModel);
        }

        private void SavePageData()
        {
            if (SaveChangeIfNeeded())
            {
                Save();
                Root.MainItems.AcceptChanges();
            }
        }

        private bool SaveChangeIfNeeded()
        {
            if (Root != null && Root.MainItems.IsDirty)
            {
                if (DialogService.Ask("Имеются изменения!\nСохранить?") == InternalDialogResult.OK)
                    return true;
            }
            return false;
        }

        private void Save()
        {
            Root.FixParentRootId();
            IEnumerable<ProfilePath> removeProfilePath = MapperViewModel.GetRemoveProfilePaths(Root);
            IEnumerable<Wellbore> removeWellbores = MapperViewModel.GetRemoveWellbores(Root);
            IEnumerable<Wellbore> updateWellbores = MapperViewModel.GetUpdateWellbores(Root);
            DataGatewayService.RemoveProfilePaths(removeProfilePath);
            DataGatewayService.RemoveAndUpdateWellbores(removeWellbores, updateWellbores);
        }

        private void ImportPageData()
        {
            if (Root != null && !(Root.CurrentItem is WellboreGridViewModel))
            {
                DialogService.Message("Импорт профиля невозможен! Необходимо выбрать ствол!");
                return;
            }

            WellboreGridViewModel currentwelWellboreGridViewModel = Root.CurrentItem as WellboreGridViewModel;
            if (currentwelWellboreGridViewModel.ChildItems.Count > 0)
            {
                if (DialogService.Ask(String.Format("Данные профиля по стволу \"{0}\"\nбудут удалены.\nПродолжить?", currentwelWellboreGridViewModel.Name)) != InternalDialogResult.OK)
                    return;
            }

            string filePath = DialogService.OpenFileDialog("Excel Files(*.xls;*.xlsx)|*.xls;*.xlsx");
            if (String.IsNullOrEmpty(filePath))
                return;

            try
            {
                List<ProfilePathPoint> profilePaths = ImportService.GetProfilePaths(filePath);
                FixFirstRow(profilePaths);
                currentwelWellboreGridViewModel.ChildItems.Clear();
                List<ProfilePathGridViewModel> profilePathGridViewModels = Mapper.Map(profilePaths, new List<ProfilePathGridViewModel>());
                currentwelWellboreGridViewModel.ChildItems.AddRange(profilePathGridViewModels);

            }
            catch (RangeNotFoundException e)
            {
                DialogService.Message(e.Message);
            }
        }

        private void FixFirstRow(List<ProfilePathPoint> profilePaths)
        {
            if (profilePaths.Count == 0)
                return;

            if (profilePaths[0].VerticalDepth != 0)
                profilePaths.Insert(0,new ProfilePathPoint(0, 0, 0, 0));
        }
    }
}