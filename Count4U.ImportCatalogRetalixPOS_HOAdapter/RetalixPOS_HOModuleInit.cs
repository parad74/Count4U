using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogRetalixPOS_HOAdapter
{
    public class RetalixPOS_HOModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public RetalixPOS_HOModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("RetalixPOS_HOModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogRetalixPOS_HOAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogRetalixPOS_HOAdapter;
            moduleInfo.UserControlType = typeof(ImportCatalogRetalixPOS_HOAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogRetalixPOS_HOAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogRetalixPOS_HOAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("RetalixPOS_HOModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
