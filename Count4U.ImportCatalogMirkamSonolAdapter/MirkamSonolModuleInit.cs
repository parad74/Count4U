using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.ImportCatalogMirkamSonolAdapter.MikamSonolSAP;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportCatalogMirkamSonolAdapter
{
    public class MirkamSonolModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MirkamSonolModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("MirkamSonolModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogMirkamSonolAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMirkamSonolAdapter;
            moduleInfo.UserControlType = typeof(ImportMirkamSonolAdapterView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.ImportCatalog;
            moduleInfo.Description = "";            
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

            ImportModuleInfo moduleInfoSAP = new ImportModuleInfo();
            moduleInfoSAP.Name = Common.Constants.ImportAdapterName.ImportCatalogMirkamSonolSAPAdapter;
            moduleInfoSAP.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMirkamSonolSAPAdapter;
            moduleInfoSAP.UserControlType = typeof(ImportMirkamSonolSAPAdapterView);
            moduleInfoSAP.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
            moduleInfoSAP.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfoSAP.Name, moduleInfoSAP, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportMirkamSonolAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogMirkamSonolAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportMirkamSonolSAPAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogMirkamSonolSAPAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("MirkamSonolModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}

        }
    }
}
