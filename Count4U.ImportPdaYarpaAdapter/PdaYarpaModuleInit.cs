using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportPdaYarpaAdapter
{
    public class PdaYarpaModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PdaYarpaModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("PdaYarpaModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportPdaYarpaAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaYarpaAdapter;
            moduleInfo.UserControlType = typeof(ImportPdaYarpaAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
            moduleInfo.Description = "";
            moduleInfo.IsDefault = false;
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaYarpaAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaYarpaAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("PdaYarpaModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
			}

    }
}
