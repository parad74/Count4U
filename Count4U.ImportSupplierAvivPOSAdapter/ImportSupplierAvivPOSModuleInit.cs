using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportSupplierAvivPOSAdapter
{
    public class ImportSupplierAvivPOSModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportSupplierAvivPOSModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportSupplierAvivPOSModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportSupplierAvivPOSAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportSupplierAvivPOSAdapter;
            moduleInfo.UserControlType = typeof(ImportSupplierAvivPOSAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportSupplier;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSupplierAvivPOSAdapterViewModel), Common.Constants.ImportAdapterName.ImportSupplierAvivPOSAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportSupplierAvivPOSModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
			}
    }
}
