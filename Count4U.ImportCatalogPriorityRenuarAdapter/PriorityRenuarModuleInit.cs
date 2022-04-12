using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportCatalogPriorityRenuarAdapter
{
    public class PriorityRenuarModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PriorityRenuarModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("PriorityRenuarModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogPriorityRenuarAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogPriorityRenuarAdapter;
            moduleInfo.UserControlType = typeof(ImportCatalogPriorityRenuarAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogPriorityRenuarAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogPriorityRenuarAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("PriorityRenuarModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
