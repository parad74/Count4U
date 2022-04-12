using Count4U.Common.View.ExportPda;
using Count4U.Modules.ContextCBI.Views.Report;
using Count4U.Report.ViewModels.ReportButton;
using Count4U.Report.Views;
using Count4U.Report.Views.ExportPda;
using Count4U.Report.Views.Script;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using System;
using Count4U.Model.Main;
using NLog;
using Count4U.Model.Interface;

namespace Count4U.GenerationReport
{
    public class ReportModuleInit : IModule
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IUnityContainer _container;

		public ReportModuleInit(IUnityContainer container)
        {
            _container = container;

           
        }

        #region Implementation of IModule

        public void Initialize()
        {
			_logger.Info("Initialize ReportModule");
			try
			{

				this._container.RegisterType(typeof(IReportRepository), typeof(ReportEFRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ReportButtonViewModel), typeof(ReportButtonViewModel), new TransientLifetimeManager());
				this._container.RegisterType(typeof(IContextReportRepository), typeof(ContextReportRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IGenerateReportRepository), typeof(GenerateReportRepository), new TransientLifetimeManager());
				this._container.RegisterType(typeof(IScriptReportRepository), typeof(ScriptReportRepository), new TransientLifetimeManager());
				this._container.RegisterType(typeof(IReportIniRepository), typeof(ReportIniRepository), new TransientLifetimeManager());
				this._container.RegisterType(typeof(IReportInfoRepository), typeof(ReportInfoRepository), new TransientLifetimeManager());
				this._container.RegisterType<object, ReportTemplateView>(Common.ViewNames.ReportTemplateView);
				this._container.RegisterType<object, ReportFavoritesView>(Common.ViewNames.ReportFavoritesView);
				this._container.RegisterType<object, ReportAddEditView>(Common.ViewNames.ReportAddEditView);
				this._container.RegisterType<object, ReportScriptView>(Common.ViewNames.ReportScriptView);
				this._container.RegisterType<object, ReportScriptSaveView>(Common.ViewNames.ReportScriptSaveView);
				this._container.RegisterType<object, ExportPdaExtraSettingsView>(Common.ViewNames.ExportPdaExtraSettingsView);
				this._container.RegisterType(typeof(IReportPrintProvider), typeof(PrintReportProvider), new TransientLifetimeManager());
				this._container.RegisterType(typeof(IReportSaveProvider), typeof(SaveReportProvider), new TransientLifetimeManager());

                this._container.RegisterType<ImportPdaPrintQueue, ImportPdaPrintQueue>(new ContainerControlledLifetimeManager());         

				_logger.Info("Initialize ReportModule - OK");
			}
			catch (Exception ex)
			{
				_logger.ErrorException("Initialize ReportModule failed", ex);
			}

        }

        #endregion
    }
}