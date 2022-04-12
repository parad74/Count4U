using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportCatalogGeneralXLSXAdapter;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogGeneralCSVAdapter
{
    public class ImportCatalogGeneralCSVModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportCatalogGeneralCSVModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportCatalogGeneralCSVModuleInit module initialization...");
			try
			{
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogGeneralCSVAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogGeneralCSVAdapter;
			moduleInfo.UserControlType = typeof(ImportCatalogGeneralCSVView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			moduleInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo moduleXlsxInfo = new ImportModuleInfo();
			moduleXlsxInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogGeneralXLSXAdapter;
			moduleXlsxInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogGeneralXLSXAdapter;
			moduleXlsxInfo.UserControlType = typeof(ImportCatalogGeneralXLSXView);
			moduleXlsxInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			moduleXlsxInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleXlsxInfo.Name, moduleXlsxInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogGeneralCSVViewModel), Common.Constants.ImportAdapterName.ImportCatalogGeneralCSVAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogGeneralXLSXViewModel), Common.Constants.ImportAdapterName.ImportCatalogGeneralXLSXAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportCatalogGeneralCSVModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
