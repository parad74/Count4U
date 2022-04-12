using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportCatalogUnizagAdapter
{
    public class UnizagModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UnizagModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("UnizagModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogUnizagAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogUnizagAdapter;
            moduleInfo.UserControlType = typeof(ImportUnizagAdapterView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";            
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportUnizagAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogUnizagAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("UnizagModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
			}
    }
}