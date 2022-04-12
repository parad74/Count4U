using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Count4U.UpdateCatalogERPQuantityXtechMeuhedetXlsxAdapter;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.UpdateCatalogERPQuantityXtechMeuhedetAdapter
{
    public class UpdateCatalogERPQuantityXtechMeuhedetModuleInit : IModule
    {
         private readonly IUnityContainer _container;
		 private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

         public UpdateCatalogERPQuantityXtechMeuhedetModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("UpdateCatalogERPQuantityXtechMeuhedetModuleInit module initialization...");
			try
			{
			//ImportModuleInfo moduleInfo = new ImportModuleInfo();
			//moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityXtechMeuhedetAdapter;
			//moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityXtechMeuhedetAdapter;
			//moduleInfo.UserControlType = typeof(UpdateCatalogERPuantityXTechMeuhedetView);
			//moduleInfo.ImportDomainEnum=ImportDomainEnum.UpdateCatalog;
			//moduleInfo.IsDefault = false;
			//moduleInfo.Description = "";
			//this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			

			ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityXtechMeuhedetXlsxAdapter;
			moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityXtechMeuhedetXlsxAdapter;
			moduleInfo.UserControlType = typeof(UpdateCatalogERPQuantityXtechMeuhedetXlsxView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.UpdateCatalog;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityXtechMeuhedetXlsxViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityXtechMeuhedetXlsxAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("UpdateCatalogERPQuantityXtechMeuhedetModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
