using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportBranchXtechMeuhedetXlsxAdapter;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportBranchXtechMeuhedetAdapter
{
    public class ImportBranchXtechMeuhedetModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportBranchXtechMeuhedetModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportBranchXtechMeuhedetModuleInit module initialization...");
			try
			{
			//OLD
			//ImportModuleInfo moduleInfo = new ImportModuleInfo();
			//moduleInfo.Name = Common.Constants.ImportAdapterName.ImportBranchXTechMeuhedetAdapter;
			//moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportBranchXTechMeuhedetAdapter;
			//moduleInfo.UserControlType = typeof(ImportBranchXTechMeuhedetView);
			//moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportBranch;
			//moduleInfo.IsDefault = false;
			//moduleInfo.Description = "";
			//this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());


			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportBranchXtechMeuhedetXlsxAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportBranchXtechMeuhedetXlsxAdapter;
			moduleInfo.UserControlType = typeof(ImportBranchXtechMeuhedetXlsxAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportBranch;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportBranchXtechMeuhedetXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportBranchXtechMeuhedetXlsxAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportBranchXtechMeuhedetModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}	
        }
    }
}