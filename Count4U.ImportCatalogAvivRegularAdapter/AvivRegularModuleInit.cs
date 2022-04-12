using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportCatalogAvivRegularAdapter
{
    public class AvivRegularModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AvivRegularModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("AvivRegularModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogAvivRegularAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAvivRegularAdapter;
            moduleInfo.UserControlType = typeof(ImportAvivRegularAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportAvivRegularAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogAvivRegularAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("AvivRegularModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}