using System;
using System.Text;
using System.Windows.Input;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
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
    public class CustomerPostViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly INavigationRepository _navigationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly UICommandRepository _uiCommandRepository;

        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _closeCommand;
        private readonly DelegateCommand _addBranchCommand;
        private readonly DelegateCommand _customerDashboardCommand;
        private readonly DelegateCommand _homeDashboardCommand;

        private string _title;
        private string _log;
        private Customer _customer;

        public CustomerPostViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            ICustomerRepository customerRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            UICommandRepository uiCommandRepository)
            : base(contextCbiRepository)
        {
            _uiCommandRepository = uiCommandRepository;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _customerRepository = customerRepository;
            _navigationRepository = navigationRepository;
            _addCommand = _uiCommandRepository.Build(enUICommand.CustomerPostAddCustomer, AddCommandExecuted, AddCommandCanExecute);
            _addBranchCommand = _uiCommandRepository.Build(enUICommand.CustomerPostAddBranch, AddBranchCommandExecuted, AddCommandCanExecute);
            _editCommand = _uiCommandRepository.Build(enUICommand.CustomerPostEditCustomer, EditCommandExecuted, AddCommandCanExecute);
            _customerDashboardCommand = _uiCommandRepository.Build(enUICommand.CustomerPostCustomerDashboard, CustomerDashboardCommandExecuted, AddCommandCanExecute);
            _homeDashboardCommand = _uiCommandRepository.Build(enUICommand.CustomerPostHomeDashboard, HomeDashboardCommandExecuted, HomeDashboardCommandCanExecute);

            _closeCommand = new DelegateCommand(CloseCommandExecuted);
        }

        private bool AddCommandCanExecute()
        {
            return _customer != null;
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

        public DelegateCommand AddBranchCommand
        {
            get { return _addBranchCommand; }
        }

        public DelegateCommand CustomerDashboardCommand
        {
            get { return _customerDashboardCommand; }
        }

        public DelegateCommand HomeDashboardCommand
        {
            get { return _homeDashboardCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            CustomerPostData postData = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.CustomerPost, true) as CustomerPostData;
            if (postData != null)
            {
                if (postData.IsNew)
                {
                    _customer = _customerRepository.GetCustomerByCode(postData.CustomerCode);
                    if (_customer == null)
					{
                        Mouse.OverrideCursor = null;
                        StringBuilder log1 = new StringBuilder();
                        log1.Append(Localization.Resources.ViewModel_CustomerDontCreate);
                        log1.Append(Environment.NewLine);
                        if (postData.ftpCommandResult != null)
                        {
                            log1.Append(postData.ftpCommandResult.ToString());
                        }
                        Log = log1.ToString();
                        return;
                    }
                    Title = string.Format(Localization.Resources.ViewModel_CustomerPost_tbCreated, _customer.Name);

                    StringBuilder log = new StringBuilder();
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_Code, _customer.Code, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_Name, _customer.Name, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_Address, _customer.Address.Trim(), Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_ContactPerson, _customer.ContactPerson, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_Description, _customer.Description, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_Fax, _customer.Fax, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_Mail, _customer.Mail, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_Phone, _customer.Phone, Environment.NewLine));

                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_ImportCatalog, _customer.ImportCatalogProviderCode, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_ImportItur, _customer.ImportIturProviderCode, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_ImportLocation, _customer.ImportLocationProviderCode, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_CustomerPost_PDA, _customer.ImportPDAProviderCode, Environment.NewLine));

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

            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.CustomerAddView + query, UriKind.Relative));
        }

        private void EditCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowChangeTitleEvent>().Publish(new ModalWindowChangeTitleEventPayload() { Title = Localization.Resources.Window_Title_EditCustomer, ViewModel = this });

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);

            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.CustomerEditView + query, UriKind.Relative));
        }

        private void CloseCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void AddBranchCommandExecuted()
        {
            UriQuery query = new UriQuery();
            query.Add(NavigationSettings.IsCustomerComboVisible, String.Empty);
            Utils.AddContextToQuery(query, base.Context);

            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.BranchAddView + query, UriKind.Relative));
        }

        private bool HomeDashboardCommandCanExecute()
        {
            return _customer != null;
           // return true;
        }

        private void HomeDashboardCommandExecuted()
        {
            try
            {
                _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

                UriQuery query = new UriQuery();
                Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                UtilsNavigate.HomeDashboardOpen(CBIContext.Main, this._regionManager, query);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("HomeDashboardCommandExecuted", exc);
            }
        }

        private void CustomerDashboardCommandExecuted()
        {
            try
            {
                _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

                UriQuery query = new UriQuery();
                Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                UtilsNavigate.CustomerDashboardOpen(this.Context, this._regionManager, query);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("CustomerDashboardCommandExecuted", exc);
            }
        }
    }
}