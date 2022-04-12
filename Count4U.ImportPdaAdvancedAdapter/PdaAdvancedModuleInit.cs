using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportPdaAdvancedAdapter
{
    public class PdaAdvancedModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PdaAdvancedModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("PdaAdvancedModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportPdaAdvancedAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaAdvancedAdapter;
            moduleInfo.UserControlType = typeof(ImportPdaAdvancedAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
            moduleInfo.Description = "";
            moduleInfo.IsDefault = false;
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

            ImportModuleInfo moduleInfo1 = new ImportModuleInfo();
            moduleInfo1.Name = Common.Constants.ImportAdapterName.ImportPdaAdvanced1Adapter;
            moduleInfo1.Title = Common.Constants.ImportAdapterTitle.ImportPdaAdvanced1Adapter;
            moduleInfo1.UserControlType = typeof(ImportPdaAdvancedAdapter1View);
            moduleInfo1.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
            moduleInfo1.Description = "";
            moduleInfo1.IsDefault = false;
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo1.Name, moduleInfo1, new ContainerControlledLifetimeManager());

            ImportModuleInfo moduleInfo2 = new ImportModuleInfo();
            moduleInfo2.Name = Common.Constants.ImportAdapterName.ImportPdaAdvanced2Adapter;
            moduleInfo2.Title = Common.Constants.ImportAdapterTitle.ImportPdaAdvanced2Adapter;
            moduleInfo2.UserControlType = typeof(ImportPdaAdvancedAdapter2View);
            moduleInfo2.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
            moduleInfo2.Description = "";
            moduleInfo2.IsDefault = false;
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo2.Name, moduleInfo2, new ContainerControlledLifetimeManager());

            ImportModuleInfo moduleInfo3 = new ImportModuleInfo();
            moduleInfo3.Name = Common.Constants.ImportAdapterName.ImportPdaAdvanced3Adapter;
            moduleInfo3.Title = Common.Constants.ImportAdapterTitle.ImportPdaAdvanced3Adapter;
            moduleInfo3.UserControlType = typeof(ImportPdaAdvancedAdapter3View);
            moduleInfo3.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
            moduleInfo3.Description = "";
            moduleInfo3.IsDefault = false;
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo3.Name, moduleInfo3, new ContainerControlledLifetimeManager());

			ImportModuleInfo moduleInfo4 = new ImportModuleInfo();
			moduleInfo4.Name = Common.Constants.ImportAdapterName.ImportPdaAdvanced4Adapter;
			moduleInfo4.Title = Common.Constants.ImportAdapterTitle.ImportPdaAdvanced4Adapter;
			moduleInfo4.UserControlType = typeof(ImportPdaAdvancedAdapter4View);
			moduleInfo4.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			moduleInfo4.Description = "";
			moduleInfo4.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo4.Name, moduleInfo4, new ContainerControlledLifetimeManager());

			ImportModuleInfo moduleInfo5 = new ImportModuleInfo();
			moduleInfo5.Name = Common.Constants.ImportAdapterName.ImportPdaAdvanced5Adapter;
			moduleInfo5.Title = Common.Constants.ImportAdapterTitle.ImportPdaAdvanced5Adapter;
			moduleInfo5.UserControlType = typeof(ImportPdaAdvancedAdapter5View);
			moduleInfo5.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			moduleInfo5.Description = "";
			moduleInfo5.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo5.Name, moduleInfo5, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaAdvancedAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaAdvancedAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaAdvancedAdapter1ViewModel), Common.Constants.ImportAdapterName.ImportPdaAdvanced1Adapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaAdvancedAdapter2ViewModel), Common.Constants.ImportAdapterName.ImportPdaAdvanced2Adapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaAdvancedAdapter3ViewModel), Common.Constants.ImportAdapterName.ImportPdaAdvanced3Adapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaAdvancedAdapter4ViewModel), Common.Constants.ImportAdapterName.ImportPdaAdvanced4Adapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaAdvancedAdapter5ViewModel), Common.Constants.ImportAdapterName.ImportPdaAdvanced5Adapter);
			}
			catch (Exception exc)
			{
				_logger.Error("PdaAdvancedModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }

    }
}
