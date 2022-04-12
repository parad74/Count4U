using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportLocationDefaultAdapter
{
    public class LocationDefaultModuleInit: IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public LocationDefaultModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("LocationDefaultModuleInit module initialization...");
			try
			{
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportLocationDefaultAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportLocationDefaultAdapter;
			moduleInfo.UserControlType = typeof(ImportLocationDefaultAdapterView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportLocation;
			moduleInfo.Description = "";
			moduleInfo.IsDefault = true;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo moduleXlsxInfo = new ImportModuleInfo();
			moduleXlsxInfo.Name = Common.Constants.ImportAdapterName.ImportLocationXlsxAdapter;
			moduleXlsxInfo.Title = Common.Constants.ImportAdapterTitle.ImportLocationXlsxAdapter;
			moduleXlsxInfo.UserControlType = typeof(ImportLocationXlsxAdapterView);
			moduleXlsxInfo.ImportDomainEnum = ImportDomainEnum.ImportLocation;
			moduleXlsxInfo.Description = "";
			moduleXlsxInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleXlsxInfo.Name, moduleXlsxInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo moduleUpdateTagXlsxInfo = new ImportModuleInfo();
			moduleUpdateTagXlsxInfo.Name = Common.Constants.ImportAdapterName.ImportLocationUpdateTagAdapter;
			moduleUpdateTagXlsxInfo.Title = Common.Constants.ImportAdapterTitle.ImportLocationUpdateTagAdapter;
			moduleUpdateTagXlsxInfo.UserControlType = typeof(ImportLocationUpdateTagAdapterView);
			moduleUpdateTagXlsxInfo.ImportDomainEnum = ImportDomainEnum.ImportLocation;
			moduleUpdateTagXlsxInfo.Description = "";
			moduleUpdateTagXlsxInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleUpdateTagXlsxInfo.Name, moduleUpdateTagXlsxInfo, new ContainerControlledLifetimeManager());

			
			

			ImportModuleInfo moduleYesXlsxInfo = new ImportModuleInfo();
			moduleYesXlsxInfo.Name = Common.Constants.ImportAdapterName.ImportLocationYesXlsxAdapter;
			moduleYesXlsxInfo.Title = Common.Constants.ImportAdapterTitle.ImportLocationYesXlsxAdapter;
			moduleYesXlsxInfo.UserControlType = typeof(ImportLocationYesXlsxAdapterView);
			moduleYesXlsxInfo.ImportDomainEnum = ImportDomainEnum.ImportLocation;
			moduleYesXlsxInfo.Description = "";
			moduleYesXlsxInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleYesXlsxInfo.Name, moduleYesXlsxInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportLocationDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportLocationDefaultAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportLocationXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportLocationXlsxAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportLocationYesXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportLocationYesXlsxAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("LocationDefaultModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
         
    }

  
}