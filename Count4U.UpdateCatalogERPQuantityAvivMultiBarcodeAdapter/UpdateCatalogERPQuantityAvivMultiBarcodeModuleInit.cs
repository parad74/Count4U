using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.UpdateCatalogERPQuantityAvivMultiBarcodeAdapter
{
    public class UpdateCatalogERPQuantityAvivMultiBarcodeModuleInit : IModule
    {
         private readonly IUnityContainer _container;
		 private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

         public UpdateCatalogERPQuantityAvivMultiBarcodeModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("UpdateCatalogERPQuantityAvivMultiBarcodeModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityAvivMultiBarcodeAdapter;
            moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityAvivMultiBarcodeAdapter;
            moduleInfo.UserControlType = typeof(UpdateCatalogERPQuantityAvivMultiBarcodeView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.UpdateCatalog;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityAvivMultiBarcodeViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityAvivMultiBarcodeAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("UpdateCatalogERPQuantityAvivMultiBarcodeModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
			}
    }
}
