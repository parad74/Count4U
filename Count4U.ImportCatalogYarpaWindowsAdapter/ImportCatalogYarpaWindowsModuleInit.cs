using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogYarpaWindowsAdapter
{
    public class ImportCatalogYarpaWindowsModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportCatalogYarpaWindowsModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportCatalogYarpaWindowsModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogYarpaWindowsAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogYarpaWindowsAdapter;
            moduleInfo.UserControlType = typeof(ImportCatalogYarpaWindowsView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogYarpaWindowsViewModel), Common.Constants.ImportAdapterName.ImportCatalogYarpaWindowsAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportCatalogYarpaWindowsModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
    }
}
