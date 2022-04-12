using System;
using Count4U.Modules.Prepare.Update;
using Count4U.Modules.Prepare.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.Modules.Prepare
{
    public class PrepareModuleInit : IModule
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public PrepareModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            this._regionManager = regionManager;
            this._container = container;
        }

        #region Implementation of IModule

		public void Initialize()
		{
			_logger.Info("PrepareModuleInit module initialization...");
			try
			{
				this._container.RegisterType<object, UpdateView>(Common.ViewNames.UpdateView);
				_logger.Info("module initialization UpdateView");
				this._container.RegisterType<object, SqlScriptSettingsView>(Common.ViewNames.SqlScriptSettingsView);
				_logger.Info("module initialization SqlScriptSettingsView");
				//#if DEBUG
				//            this._container.Resolve<GUIRun>().Run();
				//#else
				//            this._regionManager.RequestNavigate(Common.RegionNames.ApplicationWindow, new Uri(Common.ViewNames.UpdateView, UriKind.Relative));
				//#endif
				this._regionManager.RequestNavigate(Common.RegionNames.ApplicationWindow, new Uri(Common.ViewNames.UpdateView, UriKind.Relative));
				_logger.Info("module initialization ApplicationWindow");
			}
			catch (Exception exc)
			{
				_logger.Error("PrepareModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}

		}

        #endregion
    }
}