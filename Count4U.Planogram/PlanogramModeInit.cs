using System;
using Count4U.Planogram.View;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.Planogram
{
    public class PlanogramModeInit : IModule
    {
        private readonly IUnityContainer _unityContainer;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PlanogramModeInit(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
			_logger.Info("PlanogramModeInit module initialization...");
			try
			{
            this._unityContainer.RegisterType<object, PlanBasementView>(Common.ViewNames.PlanBasementView);
            this._unityContainer.RegisterType<object, PlanCanvasView>(Common.ViewNames.PlanCanvasView);
            this._unityContainer.RegisterType<object, PlanSizeChangeView>(Common.ViewNames.PlanSizeChangeView);
            this._unityContainer.RegisterType<object, PlanIturAddView>(Common.ViewNames.PlanIturAddView);
            this._unityContainer.RegisterType<object, PlanInfoView>(Common.ViewNames.PlanInfoView);
            this._unityContainer.RegisterType<object, PlanAddEditDeleteView>(Common.ViewNames.PlanAddEditDeleteView);
            this._unityContainer.RegisterType<object, PlanTreeView>(Common.ViewNames.PlanTreeView);
            this._unityContainer.RegisterType<object, PlanPropertiesView>(Common.ViewNames.PlanPropertiesView);
            this._unityContainer.RegisterType<object, PlanLocationAssignView>(Common.ViewNames.PlanLocationAssignView);
            this._unityContainer.RegisterType<object, PlanTextAssignView>(Common.ViewNames.PlanTextAssignView);
            this._unityContainer.RegisterType<object, PlanPictureAssignView>(Common.ViewNames.PlanPictureAssignView);
			}
			catch (Exception exc)
			{
				_logger.Error("PlanogramModeInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
			}
    }
}