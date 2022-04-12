using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Count4U.Common.ViewModel;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using Count4U.GenerationReport;
//using Microsoft.Reporting.WinForms;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.Views.Report;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Count4U.Report.ViewModels
{
    public abstract class BaseReportTemplateViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;
        protected readonly IUnityContainer _container;
        protected readonly IReportRepository _reportRepository;
        protected readonly IServiceLocator _serviceLocator;
        private readonly IContextReportRepository _contextReportRepository;

        protected Microsoft.Reporting.WinForms.ReportDataSource _lastRreportDS;
        protected string _lastDomainContextDataSet;        

        protected BaseReportTemplateViewModel(
             IUnityContainer container,
             IServiceLocator serviceLocator,
             IDBSettings dbSettings,
             IContextCBIRepository contextCbiRepository,
             IReportRepository reportRepository,
            IContextReportRepository contextReportRepository)
            : base(contextCbiRepository)
        {
            this._contextReportRepository = contextReportRepository;
            this._reportRepository = reportRepository;
            this._dbSettings = dbSettings;
            this._container = container;
            this._serviceLocator = serviceLocator;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._lastRreportDS = null;
            this._lastDomainContextDataSet = "";
        }
    }
}