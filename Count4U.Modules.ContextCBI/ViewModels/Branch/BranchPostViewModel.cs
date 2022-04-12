using System;
using System.Text;
using System.Windows.Input;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BranchPostViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly INavigationRepository _navigationRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICustomerRepository _customerRepository;
        private readonly UICommandRepository _uiCommandRepository;

        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _addInventorCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _branchDashboardCommand;
        private readonly DelegateCommand _homeDashboardCommand;
        private readonly DelegateCommand _closeCommand;

        private string _title;
        private string _log;
        private Branch _branch;

        public BranchPostViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            UICommandRepository uiCommandRepository)
            : base(contextCbiRepository)
        {
            _uiCommandRepository = uiCommandRepository;
            _customerRepository = customerRepository;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _branchRepository = branchRepository;
            _navigationRepository = navigationRepository;

            _addCommand = _uiCommandRepository.Build(enUICommand.BranchPostAddBranch, AddCommandExecuted);
            _editCommand = _uiCommandRepository.Build(enUICommand.BranchPostEditBranch, EditCommandExecuted);
            _addInventorCommand = _uiCommandRepository.Build(enUICommand.BranchPostAddInventor, AddInventorCommandExecuted);
            _branchDashboardCommand = _uiCommandRepository.Build(enUICommand.BranchPostBranchDashboard, BranchDashboardCommandExecuted);
            _homeDashboardCommand = _uiCommandRepository.Build(enUICommand.BranchPostHomeDashboard, HomeDashboardCommandExecuted, HomeDashboardCommandCanExecute);

            _closeCommand = new DelegateCommand(CloseCommandExecuted);
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string Log
        {
            get { return _log; }
            set
            {
                _log = value;
                RaisePropertyChanged(() => Log);
            }
        }

        public DelegateCommand AddCommand
        {
            get { return _addCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public DelegateCommand AddInventorCommand
        {
            get { return _addInventorCommand; }
        }

        public DelegateCommand BranchDashboardCommand
        {
            get { return _branchDashboardCommand; }
        }

        public DelegateCommand HomeDashboardCommand
        {
            get { return _homeDashboardCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BranchPostData postData = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.BranchPost, true) as BranchPostData;
            if (postData != null)
            {
                if (postData.IsNew)
                {
                    _branch = _branchRepository.GetBranchByCode(postData.BranchCode);
                    Title = string.Format(Localization.Resources.ViewModel_BranchPost_tbCreated, _branch.Name);

                    StringBuilder log = new StringBuilder();
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_Code, _branch.Code, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_CodeLocal, _branch.BranchCodeLocal, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_CodeERP, _branch.BranchCodeERP, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_Name, _branch.Name, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_Address, _branch.Address.Trim(), Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_ContactPerson, _branch.ContactPerson, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_Description, _branch.Description, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_Fax, _branch.Fax, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_Mail, _branch.Mail, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_Phone, _branch.Phone, Environment.NewLine));

                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_ImportCatalog, _branch.ImportCatalogProviderCode, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_ImportItur, _branch.ImportIturProviderCode, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_ImportLocation, _branch.ImportLocationProviderCode, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_BranchPost_PDA, _branch.ImportPDAProviderCode, Environment.NewLine));

                    Log = log.ToString();


                    Mouse.OverrideCursor = null;
                }
                else //is edit
                {

                }
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void AddCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);

            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.BranchAddView + query, UriKind.Relative));
        }

        private void EditCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowChangeTitleEvent>().Publish(new ModalWindowChangeTitleEventPayload() { Title = Localization.Resources.Window_Title_EditBranch, ViewModel = this });

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);

            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.BranchEditView + query, UriKind.Relative));
        }

        private void CloseCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void AddInventorCommandExecuted()
        {
            using (new CursorWait())
            {
                Customer customer = _customerRepository.GetCustomerByCode(_branch.CustomerCode);
                Branch branch = _branchRepository.GetBranchByCode(_branch.Code);

                AuditConfig auditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
                if (auditConfig != null)
                {
                    base.ContextCBIRepository.SetCurrentCustomer(customer, auditConfig);
                    base.ContextCBIRepository.SetCurrentBranch(branch, auditConfig);
                }

                UriQuery query = new UriQuery();
                query.Add(NavigationSettings.IsCustomerComboVisible, String.Empty);
                query.Add(NavigationSettings.IsBranchComboVisible, String.Empty);
                Utils.AddContextToQuery(query, CBIContext.CreateInventor);

                _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.InventorAddView + query, UriKind.Relative));
            }
        }

        private bool HomeDashboardCommandCanExecute()
        {
            return true;
        }

        private void HomeDashboardCommandExecuted()
        {
            try
            {
                using (new CursorWait())
                {
                    _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

                    UriQuery query = new UriQuery();
                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                    UtilsNavigate.HomeDashboardOpen(CBIContext.Main, this._regionManager, query);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("HomeDashboardCommandExecuted", exc);
            }
        }

        private void BranchDashboardCommandExecuted()
        {
            try
            {
                using (new CursorWait())
                {
                    _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

                    UriQuery query = new UriQuery();
                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                    UtilsNavigate.BranchDashboardOpen(this.Context, this._regionManager, query);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BranchDashboardCommandExecuted", exc);
            }
        }
    }
}