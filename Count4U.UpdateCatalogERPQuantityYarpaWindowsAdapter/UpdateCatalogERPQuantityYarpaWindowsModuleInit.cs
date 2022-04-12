using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.UpdateCatalogERPQuantityYarpaWindowsAdapter
{
    public class UpdateCatalogERPQuantityYarpaWindowsModuleInit : IModule
    {
         private readonly IUnityContainer _container;
		 private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

         public UpdateCatalogERPQuantityYarpaWindowsModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("UpdateCatalogERPQuantityYarpaWindowsModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityYarpaWindowsAdapter;
            moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityYarpaWindowsAdapter;
            moduleInfo.UserControlType = typeof(UpdateCatalogERPQuantityYarpaWindowsView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.UpdateCatalog;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityYarpaWindowsViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityYarpaWindowsAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("UpdateCatalogERPQuantityYarpaWindowsModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
