using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportCatalogAvivMultiBarcodesAdapter
{
    public class AvivMultiBarcodesModuleInit: IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AvivMultiBarcodesModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("AvivMultiBarcodesModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogAvivMultiBarcodesAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAvivMultiBarcodesAdapter;
            moduleInfo.UserControlType = typeof(ImportAvivMultiBarcodesView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportAvivMultiBarcodesViewModel), Common.Constants.ImportAdapterName.ImportCatalogAvivMultiBarcodesAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("AvivMultiBarcodesModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}