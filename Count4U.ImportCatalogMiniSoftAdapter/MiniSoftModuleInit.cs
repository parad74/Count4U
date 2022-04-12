using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogMiniSoftAdapter
{
    public class MiniSoftModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MiniSoftModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("MiniSoftModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogMiniSoftAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMiniSoftAdapter;
            moduleInfo.UserControlType = typeof(ImportCatalogMiniSoftAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogMiniSoftAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogMiniSoftAdapter);

			}
			catch (Exception exc)
			{
				_logger.Error("MiniSoftModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
