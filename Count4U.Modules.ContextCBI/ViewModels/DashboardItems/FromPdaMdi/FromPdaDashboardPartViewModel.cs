using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Extensions;


namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.FromPda
{
    public class FromPdaDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IInventProductRepository _inventProductRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IRegionManager _regionManager;
        private readonly UICommandRepository _commandRepository;

        private readonly DelegateCommand _getFromPdaCommand;
        private readonly DelegateCommand _reportCommand;
        private readonly DelegateCommand _moreCommand;
        private readonly DelegateCommand _exportERPCommand;

        private readonly ObservableCollection<FromPdaDashboardItem> _items;

        private string _totalRecordsText;
        private string _totalDocumentsText;

        public FromPdaDashboardPartViewModel(
            IContextCBIRepository contextCBIRepository,
            IInventProductRepository inventProductRepository,
            ISessionRepository sessionRepository,
            IRegionManager regionManager,
            UICommandRepository commandRepository
            )
            : base(contextCBIRepository)
        {
            _commandRepository = commandRepository;
            _sessionRepository = sessionRepository;
            this._regionManager = regionManager;
            this._inventProductRepository = inventProductRepository;

            this._getFromPdaCommand = _commandRepository.Build(enUICommand.GetFromPda, GetFromPdaCommandExecuted);
            this._reportCommand = _commandRepository.Build(enUICommand.Report, ReportCommandExecuted);
            this._moreCommand = _commandRepository.Build(enUICommand.More, MoreCommandExecuted);
			this._exportERPCommand = _commandRepository.Build(enUICommand.ExportERP, ExportErpCommandExecuted);

            this._items = new ObservableCollection<FromPdaDashboardItem>();
        }

        public ObservableCollection<FromPdaDashboardItem> Items
        {
            get { return _items; }
        }

        public DelegateCommand GetFromPdaCommand
        {
            get { return this._getFromPdaCommand; }
        }

        public DelegateCommand ReportCommand
        {
            get { return this._reportCommand; }
        }

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public string TotalRecordsText
        {
            get { return _totalRecordsText; }
            set
            {
                _totalRecordsText = value;
                RaisePropertyChanged(() => TotalRecordsText);
            }
        }

        public string TotalDocumentsText
        {
            get { return _totalDocumentsText; }
            set
            {
                _totalDocumentsText = value;
                RaisePropertyChanged(() => TotalDocumentsText);
            }
        }

        public DelegateCommand ExportERPCommand
        {
            get { return _exportERPCommand; }
        }

        private void GetFromPdaCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportFromPdaOpen(this._regionManager, uriQuery);
        }

        public void Refresh()
        {
			Task.Factory.StartNew(BuildItems).LogTaskFactoryExceptions("Refresh");
        }

        public void Clear()
        {

        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.PDA);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			Task.Factory.StartNew(BuildItems).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        private void BuildItems()
        {
            Sessions sessions = _sessionRepository.GetSessions(base.GetDbPath);

            Utils.RunOnUI(() =>
                              {
                                  _items.Clear();
                                  foreach (Session session in sessions)
                                  {
                                      FromPdaDashboardItem viewModel = new FromPdaDashboardItem();
                                      DateTime d = session.CreateDate;
                                      viewModel.CreateDate = d.ToString(@"hh:mm:ss dd-MM-yyyy");
                                      viewModel.CountDocument = session.CountDocument.ToString();
                                      viewModel.CountItem = session.CountItem.ToString();
                                      _items.Add(viewModel);
                                  }

                                  TotalRecordsText = String.Format(Localization.Resources.ViewModel_FromPdaDahsboardPart_tbTotalItems, sessions.Select(r => r.CountItem).Sum());
                                  TotalDocumentsText = String.Format(Localization.Resources.ViewModel_FromPdaDahsboardPart_tbTotalDocuments, sessions.Select(r => r.CountDocument).Sum());
                              });
        }

        private void MoreCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            UtilsNavigate.InventProductListOpen(this._regionManager, query);
        }

        private void ExportErpCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, base.ContextCBIRepository.GetCurrentCBIConfig(base.Context));
            UtilsNavigate.ExportErpWithModulesOpen(this._regionManager, uriQuery);
        }
    }
}