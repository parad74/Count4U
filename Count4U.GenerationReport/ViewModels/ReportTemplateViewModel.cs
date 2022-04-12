using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.GenerationReport;
using Microsoft.Practices.ServiceLocation;
using NLog;
using Count4U.Common.Constants;

namespace Count4U.Report.ViewModels
{
    public class ReportTemplateViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly INavigationRepository _navigationRepository;
        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IReportRepository _reportRepository;
        private readonly UICommandRepository _commandRepository;

        private bool _isUseCurrentContext;
        private bool _isUseCurrentContextEnabled;
        private bool _isCurrentUser;
        private bool _isContext;
        private bool _isMain;
        private bool _isAll;

        protected readonly ObservableCollection<ReportItemViewModel> _items;
        protected ReportItemViewModel _selectedItem;

        private readonly DelegateCommand _generateCommand;

        protected AllowedReportTemplate _allowedReportTemplateType;
        private SelectParams _selectParams;
        private Itur _navigationItur;
        private Location _navigationLocation;
        private DocumentHeader _navigationDocumentHeader;

        private ViewDomainContextEnum _domainContextType;


        public ReportTemplateViewModel(
           IContextCBIRepository contextCbiRepository,
           INavigationRepository navigationRepository,
           IGenerateReportRepository generateReportRepository,
            IReportRepository reportRepository,
            UICommandRepository commandRepository)
            : base(contextCbiRepository)
        {
            _commandRepository = commandRepository;
            this._reportRepository = reportRepository;
            this._generateReportRepository = generateReportRepository;
            this._navigationRepository = navigationRepository;
            this._generateCommand = _commandRepository.Build(enUICommand.Generate, GenerateCommandExecuted, GenerateCommandCanExecute);
            this._items = new ObservableCollection<ReportItemViewModel>();
        }

        public DelegateCommand GenerateCommand
        {
            get { return _generateCommand; }
        }

        public bool IsUseCurrentContext
        {
            get { return _isUseCurrentContext; }
            set
            {
                _isUseCurrentContext = value;
                RaisePropertyChanged(() => IsUseCurrentContext);
            }
        }

        public bool IsUseCurrentContextEnabled
        {
            get { return _isUseCurrentContextEnabled; }
            set
            {
                _isUseCurrentContextEnabled = value;
                RaisePropertyChanged(() => IsUseCurrentContextEnabled);
            }
        }

        public bool IsCurrentUser
        {
            get { return _isCurrentUser; }
            set
            {
                _isCurrentUser = value;

                RaisePropertyChanged(() => IsCurrentUser);

                BuildItems();
            }
        }

        public bool IsContext
        {
            get { return _isContext; }
            set
            {
                _isContext = value;

                RaisePropertyChanged(() => IsContext);

                BuildItems();
            }
        }

        public bool IsMain
        {
            get { return _isMain; }
            set
            {
                _isMain = value;

                RaisePropertyChanged(() => IsMain);

                BuildItems();
            }
        }

        public bool IsAll
        {
            get { return _isAll; }
            set
            {
                _isAll = value;

                RaisePropertyChanged(() => IsAll);

                BuildItems();
            }
        }

        public ObservableCollection<ReportItemViewModel> Items
        {
            get { return _items; }
        }

        public ReportItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
                _isUseCurrentContext = false;
                RaisePropertyChanged(() => IsUseCurrentContext);
                if (SelectedItem != null)
                {
                    _isUseCurrentContextEnabled = SelectedItem.AllowedContextSelectParm;	   //TODO Checked 
                }
                else
                {
                    _isUseCurrentContextEnabled = false;
                }
                RaisePropertyChanged(() => IsUseCurrentContextEnabled);
                _generateCommand.RaiseCanExecuteChanged();

            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._domainContextType = UtilsConvert.ConvertToEnum<ViewDomainContextEnum>(navigationContext);
            this._allowedReportTemplateType = UtilsConvert.ConvertToEnum<AllowedReportTemplate>(navigationContext);

            switch (_allowedReportTemplateType)
            {
                case AllowedReportTemplate.Main:
                    IsMain = true;
                    break;
                case AllowedReportTemplate.Audit:
                    IsCurrentUser = true;
                    break;
                case AllowedReportTemplate.All:
                    IsAll = true;
                    break;
                case AllowedReportTemplate.Context:
                    IsContext = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _selectParams = Utils.GetSelectParamsFromNavigationContext(navigationContext);

            _isUseCurrentContextEnabled = _selectParams != null;

            this._navigationItur = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Itur) as Itur;
            this._navigationLocation = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.Location) as Location;
            this._navigationDocumentHeader = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.DocumentHeader) as DocumentHeader;
        }

        private void BuildItems()
        {
            try
            {
                _items.Clear();

                List<AllowedReportTemplate> allowedReportTemplate = new List<AllowedReportTemplate>();
                if (_isMain)
                    allowedReportTemplate.Add(AllowedReportTemplate.Main);
                if (_isAll)
                    allowedReportTemplate.Add(AllowedReportTemplate.All);
                if (_isCurrentUser)
                    allowedReportTemplate.Add(AllowedReportTemplate.Audit);
                if (_isContext)
                    allowedReportTemplate.Add(AllowedReportTemplate.Context);

                foreach (GenerationReport.Report report in FillReportList(this._domainContextType, allowedReportTemplate))
                {
                    ReportItemViewModel item = new ReportItemViewModel(report);
                    _items.Add(item);
                }

                SelectedItem = _items.FirstOrDefault();
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildItems", exc);
            }
        }

        protected Reports FillReportList(ViewDomainContextEnum viewDomainContextEnum,
              List<AllowedReportTemplate> allowedReportTemplate)
        {
            string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
            string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
            string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

            Reports reports = _reportRepository.GetAllowedReportTemplate(
                currentCustomerCode, currentBranchCode, currentInventorCode,
                viewDomainContextEnum, allowedReportTemplate);

            return reports;
        }

        private bool GenerateCommandCanExecute()
        {
            return _selectedItem != null;
        }

        private void GenerateCommandExecuted()
        {
            GenerateReportArgs args = new GenerateReportArgs();
            args.Report = _selectedItem.Report;
            args.ViewDomainContextType = this._domainContextType;
            args.Itur = this._navigationItur;
            args.Location = this._navigationLocation;
            args.Doc = this._navigationDocumentHeader;
			args.Device = null;
            args.SelectParams = _isUseCurrentContext ? _selectParams : null;
            args.Customer = base.CurrentCustomer;
            args.Branch = base.CurrentBranch;
            args.Inventor = base.CurrentInventor;
            args.DbPath = base.GetDbPath;

            this._generateReportRepository.GenerateReport(args);

            GC.Collect();
        }
    }
}