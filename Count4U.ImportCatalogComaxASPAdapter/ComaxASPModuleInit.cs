using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Modularity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportCatalogComaxASPAdapter
{
    public class ComaxASPModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ComaxASPModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ComaxASPModuleInit module initialization...");
			try
			{
            //default
            ImportModuleInfo defaultModuleInfo = new ImportModuleInfo();
            defaultModuleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogDefaultAdapter;
			defaultModuleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogDefaultAdapter;
            defaultModuleInfo.UserControlType = typeof(ImportComaxASPAdapterView);
            defaultModuleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            defaultModuleInfo.IsDefault = true;
            defaultModuleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), defaultModuleInfo.Name, defaultModuleInfo, new ContainerControlledLifetimeManager());

            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogComaxASPAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogComaxASPAdapter;
            moduleInfo.UserControlType = typeof(ImportComaxASPAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportComaxASPAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogDefaultAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportComaxASPAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogComaxASPAdapter);

			}
			catch (Exception exc)
			{
				_logger.Error("ComaxASPModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			} 
        }
    }
}