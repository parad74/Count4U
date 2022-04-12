using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportBranchGazitVerifoneAdapter
{
    public class ImportBranchGazitVerifoneModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportBranchGazitVerifoneModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportBranchGazitVerifoneModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportBranchGazitVerifoneAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportBranchGazitVerifoneAdapter;
            moduleInfo.UserControlType = typeof(ImportBranchGazitView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportBranch;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());


			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportBranchGazitViewModel), Common.Constants.ImportAdapterName.ImportBranchGazitVerifoneAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportBranchGazitVerifoneModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
    }
}
