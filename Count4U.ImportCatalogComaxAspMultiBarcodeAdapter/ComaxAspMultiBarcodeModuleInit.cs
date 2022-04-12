using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportCatalogComaxAspMultiBarcodeAdapter
{
    public class ComaxAspMultiBarcodeModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ComaxAspMultiBarcodeModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        #region Implementation of IModule

        public void Initialize()
        {
			_logger.Info("ComaxAspMultiBarcodeModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogComaxAspMultiBarcodeAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogComaxAspMultiBarcodeAdapter;
            moduleInfo.UserControlType = typeof(ImportComaxAspMultiBarcodeAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportComaxAspMultiBarcodeAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogComaxAspMultiBarcodeAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ComaxAspMultiBarcodeModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }

        #endregion
    }
}