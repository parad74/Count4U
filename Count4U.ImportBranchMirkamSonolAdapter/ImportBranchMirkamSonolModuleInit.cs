using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportBranchMirkamSonolAdapter
{
    public class ImportBranchMirkamSonolModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportBranchMirkamSonolModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportBranchMirkamSonolModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportBranchMirkamSonolAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportBranchMirkamSonolAdapter;
            moduleInfo.UserControlType = typeof(ImportBranchMirkamSonolView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportBranch;
            moduleInfo.IsDefault = false;
            moduleInfo.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());


			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportBranchMirkamSonolViewModel), Common.Constants.ImportAdapterName.ImportBranchMirkamSonolAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportBranchMirkamSonolModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
    }
}
