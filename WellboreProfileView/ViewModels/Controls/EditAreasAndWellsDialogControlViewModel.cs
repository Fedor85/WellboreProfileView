using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Prism.Commands;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Enums;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Mappers;
using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.ViewModels
{
    public class EditAreasAndWellsDialogControlViewModel : BaseControlViewModel, IDialogViewModel
    {
        private IDataGatewayService dataGatewayService;

        private IDialogService dialogService;

        private AreasRootGridViewModel root;

        public Window ParentWindow { get; set; }

        public DelegateCommand RefreshGridDataContextDCommand { get; }

        public DelegateCommand MakeСhangeGridDataContextDCommand { get; }

        public DelegateCommand SaveСhangeGridDataContextDCommand { get; }

        public DelegateCommand CanselСhangeGridDataContextDCommand { get; }

        public string Title => "Редактор списка площадей и скважин";

        public object Content { set { Root = (AreasRootGridViewModel)value; } }

        public InternalDialogResult DialogResult { get; set; }

        public AreasRootGridViewModel Root
        {
            get { return root; }
            set
            {
                root = value;
                RaisePropertyChanged();
            }
        }

        public EditAreasAndWellsDialogControlViewModel(IDataGatewayService dataGatewayService, IDialogService dialogService)
        {
            this.dataGatewayService = dataGatewayService;
            this.dialogService = dialogService;
            DialogResult = InternalDialogResult.None;
            RefreshGridDataContextDCommand = new DelegateCommand(RefreshGridDataContext);
            MakeСhangeGridDataContextDCommand = new DelegateCommand(MakeСhangeGridDataContext, IsHaveChanged);
            SaveСhangeGridDataContextDCommand = new DelegateCommand(SaveСhangeGridDataContext, IsHaveChanged);
            CanselСhangeGridDataContextDCommand = new DelegateCommand(CanselСhangeGridDataContext);
            InitializeDataContext();
        }

        private void RefreshGridDataContext()
        {
            if (!Root.RunCommitEditing())
                return;

            if (Root.MainItems.IsDirty)
            {
                if (dialogService.Ask("Имеются изменения!\nОбновить?") == InternalDialogResult.OK)
                {
                    InitializeDataContext();
                    RaiseCanExecuteCommand();
                    DialogResult = InternalDialogResult.Refresh;
                }
            }
        }

        private void MakeСhangeGridDataContext()
        {
            if (!Root.RunCommitEditing())
                return;

            if (Root.MainItems.IsDirty)
            {
                if (dialogService.Ask("Имеются изменения!\nПрименить?") == InternalDialogResult.OK)
                {
                    SaveDataContext();
                    InitializeDataContext();
                    RaiseCanExecuteCommand();
                    DialogResult = InternalDialogResult.Refresh;
                }
            }
        }

        private void SaveСhangeGridDataContext()
        {
            if (!Root.RunCommitEditing())
                return;

            if (Root.MainItems.IsDirty)
            {
                if (dialogService.Ask("Имеются изменения!\nСохранить?") == InternalDialogResult.OK)
                {
                    SaveDataContext();
                    Root.MainItems.AcceptChanges();
                    DialogResult = InternalDialogResult.Refresh;
                }
            }
            ParentWindow.Close();
        }

        private void CanselСhangeGridDataContext()
        {
            if (!Root.RunCommitEditing())
                return;

            if (Root.MainItems.IsDirty)
            {
                if (dialogService.Ask("Имеются изменения!\nВыйти?") == InternalDialogResult.OK)
                {
                    Root.MainItems.AcceptChanges();
                    DialogResult = InternalDialogResult.Refresh;
                    ParentWindow.Close();
                }
            }
            else
            {
                ParentWindow.Close();
            }
        }

        protected override void ClosingUserControl(CancelEventArgs cansel)
        {
            if (!Root.RunCommitEditing())
            {
                cansel.Cancel = true;
                return;
            }

            if (Root.MainItems.IsDirty)
            {
                if (dialogService.Ask("Имеются изменения!\nПродолжить?") != InternalDialogResult.OK)
                    cansel.Cancel = true;
            }
        }

        private void InitializeDataContext()
        {
            Root = MapperViewModel.GetAreaRootGridViewModel(dataGatewayService.GetAllAreaAndWellOrderByName());
            Root.MainItems.AnyCollectionChanged += RaiseCanExecuteCommand;
        }

        private void SaveDataContext()
        {
            IEnumerable<Well> removeWells = MapperViewModel.GetRemoveWells(Root);
            IEnumerable<Area> removeAreas = MapperViewModel.GetRemoveAreas(Root);
            IEnumerable<Area> updateAreas = MapperViewModel.GetUpdateAreas(Root);
            dataGatewayService.RemoveWells(removeWells);
            dataGatewayService.RemoveAndUpdateAreas(removeAreas, updateAreas);
        }

        private void RaiseCanExecuteCommand()
        {
            MakeСhangeGridDataContextDCommand.RaiseCanExecuteChanged();
            SaveСhangeGridDataContextDCommand.RaiseCanExecuteChanged();
        }

        private bool IsHaveChanged()
        {
            return Root.MainItems.IsDirty;
        }
    }
}