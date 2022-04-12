using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogPosSuperPharmAdapter
{
    public class PosSuperPharmModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PosSuperPharmModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("PosSuperPharmModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogPosSuperPharmAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogPosSuperPharmAdapter;
            moduleInfo.UserControlType = typeof(ImportPosSuperPharmAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPosSuperPharmAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogPosSuperPharmAdapter);

			}
			catch (Exception exc)
			{
				_logger.Error("PosSuperPharmModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}