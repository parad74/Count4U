using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogPriorityKedsAdapter
{
    public class ImportCatalogPriorityKedsModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportCatalogPriorityKedsModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportCatalogPriorityKedsModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogPriorityKedsAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogPriorityKedsAdapter;
            moduleInfo.UserControlType = typeof(ImportCatalogPriorityKedsView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogPriorityKedsViewModel), Common.Constants.ImportAdapterName.ImportCatalogPriorityKedsAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportCatalogPriorityKedsModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
