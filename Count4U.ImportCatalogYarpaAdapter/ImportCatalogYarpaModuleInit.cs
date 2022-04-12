using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogYarpaAdapter
{
    public class ImportCatalogYarpaModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportCatalogYarpaModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportCatalogYarpaModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogYarpaAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogYarpaAdapter;
            moduleInfo.UserControlType = typeof(ImportCatalogYarpaView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogYarpaViewModel), Common.Constants.ImportAdapterName.ImportCatalogYarpaAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportCatalogYarpaModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
