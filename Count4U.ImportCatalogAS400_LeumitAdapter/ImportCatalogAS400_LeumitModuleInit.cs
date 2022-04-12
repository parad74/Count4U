using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogAS400_LeumitAdapter
{
    public class ImportCatalogAS400_LeumitModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportCatalogAS400_LeumitModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportCatalogAS400_LeumitModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400_LeumitAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400_LeumitAdapter;
            moduleInfo.UserControlType = typeof(ImportCatalogAS400_LeumitView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400_LeumitViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400_LeumitAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportCatalogAS400_LeumitModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
