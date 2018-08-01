using System;
using System.Windows;
using DevExpress.Xpf.Core;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using WellboreProfileView.Aspose;
using WellboreProfileView.DataProvider;
using WellboreProfileView.Domain.Services;
using WellboreProfileView.Infrastructure.Service;
using WellboreProfileView.Infrastructure.Services;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Controls;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Mappers;
using WellboreProfileView.Modules;
using WellboreProfileView.Services;
using WellboreProfileView.ViewModels;
using WellboreProfileView.Views;

namespace WellboreProfileView
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Initialize();
            Window window = Shell as Window;
            if (window != null && Application.Current != null)
            {
                Application.Current.MainWindow = window;
                Application.Current.MainWindow.Show();
            }
        }

        private void Initialize()
        {
            ApplicationThemeHelper.ApplicationThemeName = Theme.Office2016WhiteName;
            AutoMapperInitializer.Initialize();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            RegisterModules((ModuleCatalog)ModuleCatalog);
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterServices();
            RegisterTypes();
        }

        private void RegisterModules(ModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(typeof(RootControlModule));
            moduleCatalog.AddModule(typeof(NavigationButtonsPanelControlModule));
            moduleCatalog.AddModule(typeof(NavigationControlModule));
            moduleCatalog.AddModule(typeof(MainPageControlModule));

            moduleCatalog.AddModule(typeof(MainPageButtonsPanelControlModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(MainPageCaptionControlModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(MultiPageControlModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(WellboreTableControlToPageRegionModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(WellboreTableControlToMultiPageBottomRegionModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(WellboreTableControlToMultiPageUpRegionModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(InfoProfileCoordinatesControlToMultiPageUpModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(InfoProfileCoordinatesControlToMultiPageBottomModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(DrawProfileControlToMultiPageUpModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(DrawProfileControlToMultiPageBottomModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(DrawPlanControlToMultiPageUpModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(DrawPlanControlToMultiPageBottomModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(MultiDrawRangeControlToMultiPageUpModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(MultiDrawRangeControlToMultiPageBottomModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(DrawRange3DControlToMultiPageBottomModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(DrawRange3DControlToMultiPageUpModule), InitializationMode.OnDemand);
        }
        
        private void RegisterServices()
        {
            IConfigurationService configuration = new ConfigurationService();
            Container.RegisterInstance(configuration);

            IDataGatewayService dataGatewayService = new DataGatewayService(configuration.ConnectionString);
            string errorConnection;
            if (!dataGatewayService.ConnectionExists(out errorConnection))
                throw new ApplicationException(errorConnection);

            Container.RegisterInstance(dataGatewayService);
            Container.RegisterType<ISettingServices, RegistryServices>();
            Container.RegisterType<IDialogService, Services.DialogService>();

            Container.RegisterInstance<IControlViewManager>(new ControlViewManager(Container.Resolve<IRegionManager>(), Container.Resolve<IModuleManager>()));
            Container.RegisterType<IRegionContextManager, RegionContextManager>();
            //обязательно регистрируем как экземпляр, так как сервис работает со свой коллекцией комманд
            Container.RegisterInstance<IButtonsEventCommandService>(new ButtonsEventCommandService());
            Container.RegisterType<IImportService, ExcelImportService>();
            Container.RegisterType<ICalculationTrajectoryService, CalculationTrajectoryService>();
        }
        
        private void RegisterTypes()
        {
            Container.RegisterType<IRootControl, RootControl>();

            Container.RegisterType<INavigationControl, NavigationControl>();
            Container.RegisterType<INavigationButtonsPanelControl, NavigationButtonsPanelControl>();

            Container.RegisterType<IMainPageControl, MainPageControl>();
            Container.RegisterType<IMainPageButtonsPanelControl, MainPageButtonsPanelControl>();
            Container.RegisterType<IMainPageCaptionControl, MainPageCaptionControl>();

            Container.RegisterType<IMultiPageControl, MultiPageControl>();
            Container.RegisterType<IWellboreTableControl, WellboreTableControl>();
            Container.RegisterType<IInfoProfileCoordinatesControl, InfoProfileCoordinatesControl>();
            Container.RegisterType<IDrawRangeControl, DrawRangeControl>();
            Container.RegisterType<IMultiDrawRangeControl, MultiDrawRangeControl>();
            Container.RegisterType<IDraw3DControl, DrawRange3DControl>();

            Container.RegisterType<IDialogControl, EditAreasAndWellsDialogControl>(ControlNames.EditAreasAndWellsControl);
            Container.RegisterType<IDialogControl, AskDialogControl>(ControlNames.AskDialogControl);
            Container.RegisterType<IDialogControl, MessageDialogControl>(ControlNames.MessageDialogControlViewModel);

            Container.RegisterType<IDrawProfileControlViewModel, DrawProfileControlViewModel>();
            Container.RegisterType<IDrawPlanControlViewModel, DrawPlanControlViewModel>();
        }
    }
}