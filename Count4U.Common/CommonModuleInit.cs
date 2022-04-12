using Count4U.Common.Interfaces;
using Count4U.Common.Services.Ini;
using Count4U.Common.Services.Navigation;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.View;
using Count4U.Common.View.Adapter;
using Count4U.Common.View.ExportPda;
using Count4U.Common.View.Filter;
using Count4U.Common.View.SearchView;
using Count4U.Common.ViewModel.ExportPda;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.Common
{
    public class CommonModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public CommonModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        #region Implementation of IModule

        public void Initialize()
        {
            //services
            this._container.RegisterType(typeof(INavigationRepository), typeof(NavigationRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof (IIniFileParser), typeof (IniFileParser), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IIniFileInventor), typeof(IniFileInventor), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(UICommandRepository), typeof(UICommandRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(UICommandRepository<>), typeof(UICommandRepository<>), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IUICommandIconRepository), typeof(UICommandIconRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(UICommandTitleRepository), typeof(UICommandTitleRepository), new ContainerControlledLifetimeManager());            

            //views
            this._container.RegisterType<object, AdaptersMaskView>(Common.ViewNames.AdaptersMaskView);
            this._container.RegisterType<object, ExportPdaSettingsControlView>(Common.ViewNames.ExportPdaSettingsControlView);
			this._container.RegisterType<object, ExportPdaMerkavaAdapterView>(Common.ViewNames.ExportPdaMerkavaAdapterView);
			this._container.RegisterType<object, ExportPdaSettingsControlViewModel>();
			this._container.RegisterType<object, ExportPdaProgramTypeViewModel>();
			this._container.RegisterType<object, ExportPdaMerkavaAdapterViewModel>();
			
            this._container.RegisterType<object, ExportPdaProgramTypeView>(Common.ViewNames.ExportPdaProgramTypeView);
            this._container.RegisterType<object, BottomView>(Common.ViewNames.BottomView);
            this._container.RegisterType<object, FilterView>(Common.ViewNames.FilterView);
            this._container.RegisterType<object, SortView>(Common.ViewNames.SortView);
            this._container.RegisterType<object, FilterTemplateView>(Common.ViewNames.FilterTemplateView);
            this._container.RegisterType<object, FilterTemplateAddEditView>(Common.ViewNames.FilterTemplateAddEditView);
            this._container.RegisterType<object, SearchFilterView>(Common.ViewNames.SearchFilterView);


			
			
            
        }

        #endregion
    }
}