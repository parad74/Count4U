using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Count4U.Common.Constants;

namespace Count4U.Report.ViewModels
{
    public class ReportAddEditViewModel : CBIContextBaseViewModel, IChildWindowViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ObservableCollection<Count4U.GenerationReport.Report> _reports;
        private Count4U.GenerationReport.Report _selectedReport;
        private readonly IReportRepository _reportRepository;

        private Count4U.GenerationReport.Report _report;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private readonly IEventAggregator _eventAggregator;

        private AllowedReportTemplate _allowedReportTemplate;

        private string _name;
        private string _domainContext;
        private string _description;
        private string _path;
        private bool _menu;
        private string _menuCaption;
        private string _tag;
        private string _domainType;
        private string _menuLocalizationCode;
        private string _reportCode;

        private bool _isNew;

        public ReportAddEditViewModel(IContextCBIRepository contextCbiRepository,
            IReportRepository reportRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            this._eventAggregator = eventAggregator;
            this._reportRepository = reportRepository;
            this._reports = new ObservableCollection<GenerationReport.Report>();

            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public GenerationReport.Report SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                this._selectedReport = value;
                RaisePropertyChanged(() => SelectedReport);

                this.Path = _selectedReport == null ? String.Empty : _selectedReport.Path;

                string domainType = String.Empty;
                var dic = this._reportRepository.GetViewDomainContextEnumDictionary();
                if (dic.ContainsKey(this._path))
                    domainType = dic[this._path].ToString();
                this.DomainType = domainType;

                this.Name = _selectedReport == null ? String.Empty : _selectedReport.FileName;

                this.ReportCode = _selectedReport == null ? String.Empty : _selectedReport.CodeReport;

                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<GenerationReport.Report> Reports
        {
            get { return this._reports; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public bool IsNew
        {
            get { return this._isNew; }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string DomainContext
        {
            get { return _domainContext; }
            set
            {
                this._domainContext = value;
                RaisePropertyChanged(() => DomainContext);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                this._description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public bool Menu
        {
            get { return _menu; }
            set
            {
                this._menu = value;
                RaisePropertyChanged(() => Menu);
            }
        }

        public string MenuCaption
        {
            get { return _menuCaption; }
            set
            {
                this._menuCaption = value;
                RaisePropertyChanged(() => MenuCaption);
            }
        }

        public string Tag
        {
            get { return this._tag; }
            set
            {
                this._tag = value;
                RaisePropertyChanged(() => Tag);
            }
        }

        public string DomainType
        {
            get { return _domainType; }
            set
            {
                _domainType = value;
                RaisePropertyChanged(() => DomainType);
            }
        }

        public string MenuLocalizationCode
        {
            get { return _menuLocalizationCode; }
            set
            {
                _menuLocalizationCode = value;
                RaisePropertyChanged(() => MenuLocalizationCode);
            }
        }

        public string ReportCode
        {
            get { return _reportCode; }
            set
            {
                _reportCode = value;
                RaisePropertyChanged(() => ReportCode);

                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public object ResultData { get; set; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._allowedReportTemplate = UtilsConvert.ConvertToEnum<AllowedReportTemplate>(navigationContext);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.ReportUniqueCode))
            {
                this._isNew = false;

                string raw = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.ReportUniqueCode).Value;
                var split = raw.Split(new[] { '^' });
                string code = split[0];
                string fileName = split[1];
                string path = split[2];

                this._report = _reportRepository.GetReports(code, fileName, path, _allowedReportTemplate.ToString()).FirstOrDefault();

                if (_report != null)
                {
                    this._domainContext = _report.DomainContext;
                    this._description = _report.Description;
                    this._path = _report.Path;
                    this._menu = _report.Menu;
                    this._menuCaption = _report.MenuCaption;
                    this._tag = _report.Tag;
                    this._domainType = _report.DomainType;
                    this._menuLocalizationCode = _report.MenuCaptionLocalizationCode;
                    this._reportCode = _report.CodeReport;

                    _reports.Add(_report); //to see report file name in disabled reports ListBox

                    this.SelectedReport = _reports.FirstOrDefault();
                }
            }
            else
            {
                this._isNew = true;

                List<string> excluded = new List<string>();

                if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.ReportFileNames))
                {
                    string raw = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.ReportFileNames).Value;
                    excluded = raw.Split(new[] { '^' }).ToList();
                }

                string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
                string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
                string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

                List<AllowedReportTemplate> allowedReportTemplates = new List<AllowedReportTemplate>();
                allowedReportTemplates.Add(AllowedReportTemplate.All);

                Reports reports = this._reportRepository.GetAllowedReportTemplate(currentCustomerCode, currentBranchCode, currentInventorCode,
                                                           ViewDomainContextEnum.All, allowedReportTemplates);
                foreach (var report in reports)
                {
                    if (!excluded.Contains(String.Format("{0}+{1}", report.Path, report.FileName)))
                        _reports.Add(report);
                }
            }
        }

        private bool OkCommandCanExecute()
        {
            if (String.IsNullOrWhiteSpace(_reportCode))
                return false;

            if (IsReportCodeUnique() == false)
                return false;

            if (this._isNew)
                return _selectedReport != null;

            return true;
        }

        private void OkCommandExecuted()
        {
            try
            {
                if (this._isNew)
                {
                    GenerationReport.Report reportNew = new GenerationReport.Report();

                    switch (CBIDbContext)
                    {
                        case NavigationSettings.CBIDbContextCustomer:
                            reportNew.Code = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
                            break;
                        case NavigationSettings.CBIDbContextBranch:
                            reportNew.Code = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
                            break;
                        case NavigationSettings.CBIDbContextInventor:
                            reportNew.Code = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;
                            break;
                        default:
                            reportNew.Code = ReportTemplateDomainEnum.Any.ToString();
                            break;
                    }

                    reportNew.CodeReport = _reportCode;
                    reportNew.Path = _selectedReport.Path;
                    reportNew.FileName = _selectedReport.FileName;
                    reportNew.DomainType = _reportRepository.GetViewDomainContextEnumDictionary()[_selectedReport.Path].ToString();
                    reportNew.DomainContext = _domainContext;
                    reportNew.Description = _description;
                    reportNew.Menu = this._menu;
                    reportNew.MenuCaption = this._menuCaption;
                    reportNew.DomainType = this._domainType;
                    reportNew.Tag = this._tag;
                    reportNew.MenuCaptionLocalizationCode = this._menuLocalizationCode;

                    this._reportRepository.Insert(reportNew, _allowedReportTemplate);

                    this.ResultData = new ReportAddedEditedData() { IsOk = true, Report = reportNew };
                }
                else
                {
                    this._report.CodeReport = _reportCode;
                    this._report.Description = Description;
                    this._report.DomainContext = DomainContext;
                    this._report.DomainType = _reportRepository.GetViewDomainContextEnumDictionary()[this._path].ToString();

                    this._report.Menu = this._menu;
                    this._report.MenuCaption = this._menuCaption;
                    this._report.Tag = this._tag;
                    this._report.MenuCaptionLocalizationCode = this._menuLocalizationCode;

                    this._reportRepository.Update(_report, _allowedReportTemplate);

                    this.ResultData = new ReportAddedEditedData() { IsOk = true, Report = _report };
                }
            }
            catch (Exception e)
            {
                _logger.ErrorException("OkCommandExecuted", e);
            }


            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void CancelCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool IsReportCodeUnique()
        {
            if (_isNew)
            {
                return _reportRepository.GetReportByCodeReport(_reportCode, _allowedReportTemplate).Count == 0;
            }
            else
            {
                int count = _reportRepository.GetReportByCodeReport(_reportCode, _allowedReportTemplate).Count;
                return count == 0 || count == 1;
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ReportCode":
                        if (IsReportCodeUnique() == false)
                            return Localization.Resources.ViewModel_ReportAddEdit_CodeUnique;
                        break;
                }

                return String.Empty;
            }
        }

        public string Error { get; private set; }
    }
}