using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportSupplierSapb1ZometsfarimAdapter;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportSupplierDefaultAdapter
{
	public class ImportSupplierDefaultModuleInit : IModule
	{
		private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportSupplierDefaultModuleInit(IUnityContainer container)
		{
			this._container = container;
		}

		public void Initialize()
		{
			_logger.Info("ImportSupplierDefaultModuleInit module initialization...");
			try
			{
				ImportModuleInfo moduleInfo = new ImportModuleInfo();
				moduleInfo.Name = Common.Constants.ImportAdapterName.ImportSupplierDefaultAdapter;
				moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportSupplierDefaultAdapter;
				moduleInfo.UserControlType = typeof(ImportSupplierDefaultAdapterView);
				moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportSupplier;
				moduleInfo.IsDefault = true;
				moduleInfo.Description = "";
				this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

				ImportModuleInfo sapb1Zometsfarim = new ImportModuleInfo();
				sapb1Zometsfarim.Name = Common.Constants.ImportAdapterName.ImportSupplierSapb1ZometsfarimAdapter;
				sapb1Zometsfarim.Title = Common.Constants.ImportAdapterTitle.ImportSupplierSapb1ZometsfarimAdapter;
				sapb1Zometsfarim.UserControlType = typeof(ImportSupplierSapb1ZometsfarimAdapterView);
				sapb1Zometsfarim.ImportDomainEnum = ImportDomainEnum.ImportSupplier;
				sapb1Zometsfarim.IsDefault = true;
				sapb1Zometsfarim.Description = "";
				this._container.RegisterInstance(typeof(IImportModuleInfo), sapb1Zometsfarim.Name, sapb1Zometsfarim, new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSupplierDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportSupplierDefaultAdapter);
				this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSupplierSapb1ZometsfarimAdapterViewModel), Common.Constants.ImportAdapterName.ImportSupplierSapb1ZometsfarimAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportSupplierDefaultModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
	}
}
