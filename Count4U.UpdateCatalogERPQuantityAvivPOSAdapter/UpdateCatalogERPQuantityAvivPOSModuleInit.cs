using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.UpdateCatalogERPQuantityAvivPOSAdapter
{
    public class UpdateCatalogERPQuantityAvivPOSModuleInit : IModule
    {
         private readonly IUnityContainer _container;
		 private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

         public UpdateCatalogERPQuantityAvivPOSModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("UpdateCatalogERPQuantityAvivPOSModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityAvivPOSAdapter;
            moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityAvivPOSAdapter;
            moduleInfo.UserControlType = typeof(UpdateCatalogERPQuantityAvivPOSView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.UpdateCatalog;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityAvivPOSViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityAvivPOSAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("UpdateCatalogERPQuantityAvivPOSModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
			}
    }
}
