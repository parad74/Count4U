using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportBranchAS400HonigmanAdapter;
using Count4U.ImportBranchDefaultXlsxAdapter;
using Count4U.ImportBranchPriorityCastroAdapter;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportBranchDefaultAdapter
{
    public class ImportBranchDefaultModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportBranchDefaultModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Error("Initialize start ImportBranchDefaultModuleInit ");
			try{
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportBranchDefaultAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportBranchDefaultAdapter;
			moduleInfo.UserControlType = typeof(ImportBranchDefaultAdapterView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportBranch;
			moduleInfo.IsDefault = true;
			moduleInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo moduleXlsxInfo = new ImportModuleInfo();
			moduleXlsxInfo.Name = Common.Constants.ImportAdapterName.ImportBranchDefaultXlsxAdapter;
			moduleXlsxInfo.Title = Common.Constants.ImportAdapterTitle.ImportBranchDefaultXlsxAdapter;
			moduleXlsxInfo.UserControlType = typeof(ImportBranchDefaultXlsxAdapterView);
			moduleXlsxInfo.ImportDomainEnum = ImportDomainEnum.ImportBranch;
			moduleXlsxInfo.IsDefault = true;
			moduleXlsxInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleXlsxInfo.Name, moduleXlsxInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo as400HonigmanModuleInfo = new ImportModuleInfo();
			as400HonigmanModuleInfo.Name = Common.Constants.ImportAdapterName.ImportBranchAS400HonigmanAdapter;
			as400HonigmanModuleInfo.Title = Common.Constants.ImportAdapterTitle.ImportBranchAS400HonigmanAdapter;
			as400HonigmanModuleInfo.UserControlType = typeof(ImportBranchAS400HonigmanView);
			as400HonigmanModuleInfo.ImportDomainEnum = ImportDomainEnum.ImportBranch;
			as400HonigmanModuleInfo.IsDefault = true;
			as400HonigmanModuleInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), as400HonigmanModuleInfo.Name, as400HonigmanModuleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo priorityCastro = new ImportModuleInfo();
			priorityCastro.Name = Common.Constants.ImportAdapterName.ImportBranchPriorityCastroAdapter;
			priorityCastro.Title = Common.Constants.ImportAdapterTitle.ImportBranchPriorityCastroAdapter;
			priorityCastro.UserControlType = typeof(ImportBranchPriorityCastroView);
			priorityCastro.ImportDomainEnum = ImportDomainEnum.ImportBranch;
			priorityCastro.IsDefault = true;
			priorityCastro.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), priorityCastro.Name, priorityCastro, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportBranchDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportBranchDefaultAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportBranchDefaultXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportBranchDefaultXlsxAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportBranchAS400HonigmanViewModel), Common.Constants.ImportAdapterName.ImportBranchAS400HonigmanAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportBranchPriorityCastroViewModel), Common.Constants.ImportAdapterName.ImportBranchPriorityCastroAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("PrepareModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}

        }
    }
}
