using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportIturERPAdapter
{
    public class IturERPModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IturERPModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("IturERPModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportIturERPAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportIturERPAdapter;
            moduleInfo.UserControlType = typeof(ImportIturERPAdapterView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.ImportItur;
            moduleInfo.IsDefault = true;
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportIturERPAdapterViewModel), Common.Constants.ImportAdapterName.ImportIturERPAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("IturERPModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
         
    }
}
