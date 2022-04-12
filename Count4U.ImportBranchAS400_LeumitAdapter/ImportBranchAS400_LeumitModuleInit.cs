using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportBranchAS400_LeumitAdapter
{
    public class ImportBranchAS400_LeumitModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportBranchAS400_LeumitModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportBranchAS400_LeumitModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportBranchAS400_LeumitAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportBranchAS400_LeumitAdapter;
            moduleInfo.UserControlType = typeof(ImportBranchAS400_LeumitAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportBranch;
            moduleInfo.IsDefault = true;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportBranchAS400_LeumitAdapterViewModel), Common.Constants.ImportAdapterName.ImportBranchAS400_LeumitAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportBranchAS400_LeumitModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
    }
}
