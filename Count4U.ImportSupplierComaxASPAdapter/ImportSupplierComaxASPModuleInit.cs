using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportSupplierComaxASPAdapter
{
    public class ImportSupplierComaxASPModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportSupplierComaxASPModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportSupplierComaxASPModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportSupplierComaxASPAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportSupplierComaxASPAdapter;
            moduleInfo.UserControlType = typeof(ImportSupplierComaxASPAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportSupplier;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSupplierComaxASPAdapterViewModel), Common.Constants.ImportAdapterName.ImportSupplierComaxASPAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportSupplierComaxASPModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
