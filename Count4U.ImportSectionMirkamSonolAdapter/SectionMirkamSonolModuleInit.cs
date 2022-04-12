using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportSectionMirkamSonolAdapter
{
    public class SectionMirkamSonolModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SectionMirkamSonolModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("SectionMirkamSonolModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportSectionMirkamSonolAdapter;
            moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportSectionMirkamSonolAdapter;
            moduleInfo.UserControlType = typeof(ImportSectionMirkamSonolAdapterView);
            moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportSection;
            moduleInfo.Description = "Description";
            moduleInfo.IsDefault = false;
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSectionMirkamSonolAdapterViewModel), Common.Constants.ImportAdapterName.ImportSectionMirkamSonolAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("SectionMirkamSonolModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }

    }
}
