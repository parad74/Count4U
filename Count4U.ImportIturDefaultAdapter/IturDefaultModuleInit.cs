using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.ImportIturDefaultAdapter.Facing;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportIturDefaultAdapter
{
    public class IturDefaultModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IturDefaultModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("IturDefaultModuleInit module initialization...");
			try
			{
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportIturDefaultAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportIturDefaultAdapter;
			moduleInfo.UserControlType = typeof(ImportIturDefaultAdapterView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportItur;
			moduleInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo facing = new ImportModuleInfo();
			facing.Name = Common.Constants.ImportAdapterName.ImportIturFacingAdapter;
			facing.Title = Common.Constants.ImportAdapterTitle.ImportIturFacingAdapter;
			facing.UserControlType = typeof(ImportIturFacingAdapterView);
			facing.ImportDomainEnum = ImportDomainEnum.ImportItur;
			this._container.RegisterInstance(typeof(IImportModuleInfo), facing.Name, facing, new ContainerControlledLifetimeManager());

			ImportModuleInfo YesXlsxAdapter = new ImportModuleInfo();
			YesXlsxAdapter.Name = Common.Constants.ImportAdapterName.ImportIturYesXlsxAdapter;
			YesXlsxAdapter.Title = Common.Constants.ImportAdapterTitle.ImportIturYesXlsxAdapter;
			YesXlsxAdapter.UserControlType = typeof(ImportIturYesXlsxAdapterView);
			YesXlsxAdapter.ImportDomainEnum = ImportDomainEnum.ImportItur;
			this._container.RegisterInstance(typeof(IImportModuleInfo), YesXlsxAdapter.Name, YesXlsxAdapter, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportIturDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportIturDefaultAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportIturFacingAdapterViewModel), Common.Constants.ImportAdapterName.ImportIturFacingAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportIturYesXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportIturYesXlsxAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("IturDefaultModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }

         
    }
}