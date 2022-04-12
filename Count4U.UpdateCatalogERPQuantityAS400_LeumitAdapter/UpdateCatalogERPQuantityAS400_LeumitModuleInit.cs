using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.UpdateCatalogERPQuantityAS400_LeumitAdapter
{
    public class UpdateCatalogERPQuantityAS400_LeumitModuleInit : IModule
    {
         private readonly IUnityContainer _container;
		 private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

         public UpdateCatalogERPQuantityAS400_LeumitModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("UpdateCatalogERPQuantityAS400_LeumitModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityAS400_LeumitAdapter;
            moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityAS400_LeumitAdapter;
            moduleInfo.UserControlType = typeof(UpdateCatalogERPQuantityAS400_LeumitView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.UpdateCatalog;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityAS400_LeumitViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityAS400_LeumitAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("UpdateCatalogERPQuantityAS400_LeumitModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
			}
    }
}
