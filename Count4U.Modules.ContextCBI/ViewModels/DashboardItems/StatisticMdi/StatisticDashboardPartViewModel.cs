using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.Interfaces;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.StatisticMdi
{
    public class StatisticDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {        
        private readonly UICommandRepository _commandRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly IRegionManager _regionManager;

        private readonly DelegateCommand _workerStatisticCommand;
        private readonly DelegateCommand _sessionStatisticCommand;
        private readonly DelegateCommand _documentHeaderStatisticCommand;
		private readonly DelegateCommand _devicePDAFormOpenCommand;
        private readonly DelegateCommand _deviceWorkerPDAFormOpenCommand;
        

        private ReportButtonViewModel _workerReports;
        private ContextMenu _workerReportsContextMenu;

        private ReportButtonViewModel _sessionReports;
        private ContextMenu _sessionReportsContextMenu;

        private ReportButtonViewModel _documentHeaderReports;
        private ContextMenu _documentHeaderReportsContextMenu;

        public StatisticDashboardPartViewModel(
            IContextCBIRepository contextCbiRepository,
            UICommandRepository commandRepository,
            IUnityContainer unityContainer,
            IRegionManager regionManager)
            : base(contextCbiRepository)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
            _commandRepository = commandRepository;

            _workerStatisticCommand = _commandRepository.Build(enUICommand.WorkerStatistic, WorkerStatisticCommandExecuted);
            _sessionStatisticCommand = _commandRepository.Build(enUICommand.SessionStatistic, SessionStatisticCommandExecuted);
			this._devicePDAFormOpenCommand = _commandRepository.Build(enUICommand.DevicePDAStatistic, DevicePDAFormOpenCommandExecuted);
            this._deviceWorkerPDAFormOpenCommand = _commandRepository.Build(enUICommand.DeviceWorkerPDAStatistic, DeviceWorkerPDAFormOpenCommandExecuted);
            _documentHeaderStatisticCommand = _commandRepository.Build(enUICommand.DocumentHeaderStatistic, DocumentHeaderStatisticCommandExecuted);
        }


		public DelegateCommand DevicePDAFormOpenCommand
		{
			get { return this._devicePDAFormOpenCommand; }
		}
        public DelegateCommand DeviceWorkerPDAFormOpenCommand
        {
            get { return this._deviceWorkerPDAFormOpenCommand; }
        }

        
        private void DevicePDAFormOpenCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
			UtilsNavigate.DevicePDAOpen(this._regionManager, uriQuery);
        }

        private void DeviceWorkerPDAFormOpenCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.DeviceWorkerPDAOpen(this._regionManager, uriQuery);
        }

        public DelegateCommand WorkerStatisticCommand
        {
            get { return _workerStatisticCommand; }
        }

        public DelegateCommand SessionStatisticCommand
        {
            get { return _sessionStatisticCommand; }
        }

        public DelegateCommand DocumentHeaderStatisticCommand
        {
            get { return _documentHeaderStatisticCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            //WORKER
            _workerReports = _unityContainer.Resolve<ReportButtonViewModel>();

            _workerReports.OnNavigatedTo(navigationContext);
            _workerReports.Initialize(RunReportFormWorker, () =>
            {
                SelectParams sp = new SelectParams();
                sp.IsEnablePaging = false;
                return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.WorkerStatistic);

            _workerReportsContextMenu = new ContextMenu();
            _workerReports.BuildMenu(_workerReportsContextMenu);

            //SESSION
            _sessionReports = _unityContainer.Resolve<ReportButtonViewModel>();

            _sessionReports.OnNavigatedTo(navigationContext);
            _sessionReports.Initialize(RunReportFormSession, () =>
            {
                SelectParams sp = new SelectParams();
                sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.SessionStatistic);

            _sessionReportsContextMenu = new ContextMenu();
            _sessionReports.BuildMenu(_sessionReportsContextMenu);

            //DOCUMENT HEADER
            _documentHeaderReports = _unityContainer.Resolve<ReportButtonViewModel>();

            _documentHeaderReports.OnNavigatedTo(navigationContext);
            _documentHeaderReports.Initialize(RunReportFormDocumentHeader, () =>
            {
                SelectParams sp = new SelectParams();
                sp.IsEnablePaging = false;
				return new Tuple<SelectParams, Itur, Location, DocumentHeader,Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.Doc);

            _documentHeaderReportsContextMenu = new ContextMenu();
            _documentHeaderReports.BuildMenu(_documentHeaderReportsContextMenu);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Clear();
        }

        private void RunReportFormWorker()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.WorkerStatistic);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            SelectParams sp = new SelectParams();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private void RunReportFormSession()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.SessionStatistic);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            SelectParams sp = new SelectParams();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private void RunReportFormDocumentHeader()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Doc);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            SelectParams sp = new SelectParams();
            sp.IsEnablePaging = false;
            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        public void Refresh()
        {

        }

        public void Clear()
        {
            _workerReports.OnNavigatedFrom(null);
        }

        private void WorkerStatisticCommandExecuted()
        {
            _workerReportsContextMenu.Placement = PlacementMode.Mouse;
            _workerReportsContextMenu.IsOpen = true;
        }

        private void SessionStatisticCommandExecuted()
        {
            _sessionReportsContextMenu.Placement = PlacementMode.Mouse;
            _sessionReportsContextMenu.IsOpen = true;
        }

        private void DocumentHeaderStatisticCommandExecuted()
        {
            _documentHeaderReportsContextMenu.Placement = PlacementMode.Mouse;
            _documentHeaderReportsContextMenu.IsOpen = true;
        }
    }
}