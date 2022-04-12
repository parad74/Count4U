using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using NLog;
using System;

namespace Count4U.ImportUnitPlanDefaultAdapter
{
    public class UnitPlanDefaultModuleInit: IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UnitPlanDefaultModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("UnitPlanDefaultModuleInit module initialization...");
			try
			{
            ImportModuleInfo moduleInfo = new ImportModuleInfo();
            moduleInfo.Name = Common.Constants.ImportAdapterName.ImportUnitPlanDefaultAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportUnitPlanDefaultAdapter;
            moduleInfo.UserControlType = typeof(ImportUnitPlanDefaultAdapterView);
            moduleInfo.ImportDomainEnum=ImportDomainEnum.ImportUnitPlan;
            moduleInfo.Description = "Description";
            moduleInfo.IsDefault = true;
            this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			}
			catch (Exception exc)
			{
				_logger.Error("UnitPlanDefaultModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
         
    }
}
