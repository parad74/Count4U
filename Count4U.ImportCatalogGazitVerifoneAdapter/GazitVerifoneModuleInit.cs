using System.Reflection;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportCatalogGazitVerifonePriceAdapter;
using NLog;
using System;

namespace Count4U.ImportCatalogGazitVerifoneAdapter
{
    public class GazitVerifoneModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GazitVerifoneModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("GazitVerifoneModuleInit module initialization...");
			try
			{
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportCatalogGazitVerifoneAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogGazitVerifoneAdapter;
			moduleInfo.UserControlType = typeof(ImportGazitVerifoneAdapterView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			moduleInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportGazitVerifoneAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogGazitVerifoneAdapter);


			ImportModuleInfo moduleInfo1 = new ImportModuleInfo();
			moduleInfo1.Name = Common.Constants.ImportAdapterName.ImportCatalogGazitVerifonePriceAdapter;
			moduleInfo1.Title = Common.Constants.ImportAdapterTitle.ImportCatalogGazitVerifonePriceAdapter;
			moduleInfo1.UserControlType = typeof(ImportGazitVerifonePriceAdapterView);
			moduleInfo1.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			moduleInfo1.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo1.Name, moduleInfo1, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportGazitVerifonePriceAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogGazitVerifonePriceAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("GazitVerifoneModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}