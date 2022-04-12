using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.UpdateCatalogERPQuantityRetalixPOS_HOAdapter
{
    public class UpdateCatalogERPQuantityRetalixPOS_HOModuleInit : IModule
    {
         private readonly IUnityContainer _container;
		 private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

         public UpdateCatalogERPQuantityRetalixPOS_HOModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("UpdateCatalogERPQuantityRetalixPOS_HOModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityRetalixPOS_HOAdapter;
            moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityRetalixPOS_HOAdapter;
            moduleInfo.UserControlType = typeof(UpdateCatalogERPQuantityRetalixPOS_HOView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.UpdateCatalog;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityRetalixPOS_HOViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityRetalixPOS_HOAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("UpdateCatalogERPQuantityRetalixPOS_HOModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
    }
}
