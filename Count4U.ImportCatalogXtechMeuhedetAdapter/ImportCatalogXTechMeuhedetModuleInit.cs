using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportCatalogXtechMeuhedetXlsxAdapter;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogXtechMeuhedetAdapter
{
    public class ImportCatalogXTechMeuhedetModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportCatalogXTechMeuhedetModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportCatalogXTechMeuhedetModuleInit module initialization...");
			try
			{
			//ImportModuleInfo moduleInfo = new ImportModuleInfo();
			//moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogXtechMeuhedetAdapter;
			//moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogXtechMeuhedetAdapter;
			//moduleInfo.UserControlType = typeof(ImportCatalogXTechMeuhedetView);
			//moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			//moduleInfo.Description = "";
			//this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogXtechMeuhedetXlsxAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogXtechMeuhedetXlsxAdapter;
            moduleInfo.UserControlType = typeof(ImportCatalogXtechMeuhedetXlsxView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogXtechMeuhedetXlsxViewModel), Common.Constants.ImportAdapterName.ImportCatalogXtechMeuhedetXlsxAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportCatalogXTechMeuhedetModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
