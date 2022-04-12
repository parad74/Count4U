using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Audit;
using Count4U.Model;
using System.ComponentModel;
using NLog;
using Count4U.Common.Web;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class CustomerAddViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly INavigationRepository _navigationRepository;

        private readonly CustomerFormViewModel _customerFormViewModel;
        private ImportFoldersViewModel _importFoldersViewModel;
        private ExportPdaSettingsViewModel _exportPdaSettingsViewModel;
        private ExportErpSettingsViewModel _exportErpSettingsViewModel;
        private UpdateAdaptersViewModel _updateViewModel;
        private DynamicColumnSettingsViewModel _dynamicColumnsViewModel;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private Customer _customer;

        public CustomerAddViewModel(IRegionManager regionManager,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            CustomerFormViewModel customerFormViewModel,
            IUserSettingsManager userSettingsManager,
            INavigationRepository navigationRepository
            )
            : base(contextCBIRepository)
        {
            _navigationRepository = navigationRepository;
            this._userSettingsManager = userSettingsManager;
            this._customerFormViewModel = customerFormViewModel;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this._okCommand = new DelegateCommand(this.OkCommandExecuted, this.OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(this.CancelCommandExecuted);

            customerFormViewModel.PropertyChanged += customerFormViewModel_PropertyChanged;
        }

        void customerFormViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this._okCommand.RaiseCanExecuteChanged();
        }

        #region public properties

        public string Title
        {
            get { return Localization.Resources.ViewModel_CustomerAdd_CreateNewCustomer; }
        }

        public string OkButtonText
        {
            get { return Localization.Resources.Command_Create; }
        }

        public string Image
        {
            get { return "/Count4U.Media;component/Background/customer_add.png"; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public CustomerFormViewModel CustomerFormVM
        {
            get { return this._customerFormViewModel; }
        }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _customer = new Customer();
            _customer.ID = 0;
            _customer.Code = Utils.CodeNewGenerate(4);            

            _customerFormViewModel.CustomerSet(_customer, false, true);

            _importFoldersViewModel = Utils.GetViewModelFromRegion<ImportFoldersViewModel>(Common.RegionNames.ImportFolderCustomerAdd, this._regionManager);
            if (_importFoldersViewModel != null)
            {
				this._importFoldersViewModel.SetIsEditable(true);
				this._importFoldersViewModel.SetIsShowConfig(false);
				this._importFoldersViewModel.SetCustomer(_customer);
            }

            _exportPdaSettingsViewModel = Utils.GetViewModelFromRegion<ExportPdaSettingsViewModel>(Common.RegionNames.ExportPdaSettingsCustomerAdd, this._regionManager);
            if (_exportPdaSettingsViewModel != null)
            {
                _exportPdaSettingsViewModel.SetIsEditable(true);
                _exportPdaSettingsViewModel.SetIsNew(true);
				_exportPdaSettingsViewModel.SetIsShowConfig(false);
                _exportPdaSettingsViewModel.SetCustomer(_customer, true);                
                _exportPdaSettingsViewModel.CheckValidation += ExportPdaSettingsViewModel_CheckValidation;
            }

            _exportErpSettingsViewModel = Utils.GetViewModelFromRegion<ExportErpSettingsViewModel>(Common.RegionNames.ExportErpSettingsCustomerAdd, this._regionManager);
            if (_exportErpSettingsViewModel != null)
            {
				this._exportErpSettingsViewModel.SetIsEditable(true);
				this._exportErpSettingsViewModel.SetIsShowConfig(false);
				this._exportErpSettingsViewModel.SetCustomer(_customer);                
            }

            _updateViewModel = Utils.GetViewModelFromRegion<UpdateAdaptersViewModel>(Common.RegionNames.UpdateFolderCustomerAdd, this._regionManager);
            if (_updateViewModel != null)
            {
				this._updateViewModel.SetIsEditable(true);
				this._updateViewModel.SetIsShowConfig(false);
				this._updateViewModel.SetCustomer(_customer);
            }

            _dynamicColumnsViewModel = Utils.GetViewModelFromRegion<DynamicColumnSettingsViewModel>(Common.RegionNames.DynamicColumnSettingsCustomerAdd, this._regionManager);
			if (_dynamicColumnsViewModel != null)
            {
				//this._dynamicColumnsViewModel.SetIsShowConfig(false);
				this._dynamicColumnsViewModel.SetIsEditable(true);
				this._dynamicColumnsViewModel.SetCustomer(_customer);
				this._dynamicColumnsViewModel.SetIsCustomer(true);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
			if (_exportPdaSettingsViewModel != null)
			{
				_exportPdaSettingsViewModel.CheckValidation -= ExportPdaSettingsViewModel_CheckValidation;
				_exportPdaSettingsViewModel.OnNavigatedFrom(navigationContext);
			}

			if (_importFoldersViewModel != null)
			{
				_importFoldersViewModel.OnNavigatedFrom(navigationContext);
			}

			if (_exportErpSettingsViewModel != null)
			{
				_exportErpSettingsViewModel.OnNavigatedFrom(navigationContext);
			}

			if (_updateViewModel != null)
			{
				_updateViewModel.OnNavigatedFrom(navigationContext);
			}

			if (_dynamicColumnsViewModel != null)
			{
				_dynamicColumnsViewModel.OnNavigatedFrom(navigationContext);
			}
        }      

        private bool OkCommandCanExecute()
        {
            bool isExportFormOk = true;
            if (this._exportPdaSettingsViewModel != null)
                isExportFormOk = this._exportPdaSettingsViewModel.IsFormValid();

            return this._customerFormViewModel.IsFormValid() && isExportFormOk;
        }

        private void OkCommandExecuted()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            FtpCommandResult ftpCommandResult = this.SaveCustomer();

            this._eventAggregator.GetEvent<CustomerAddedEvent>().Publish(this._customer);

            CustomerPostData data = new CustomerPostData() { CustomerCode = _customer.Code, IsNew = true , ftpCommandResult = ftpCommandResult };
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, this.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            UtilsConvert.AddObjectToQuery(query, _navigationRepository, data, Common.NavigationObjects.CustomerPost);

            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.CustomerPostView + query, UriKind.Relative));
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private FtpCommandResult SaveCustomer()
        {
            FtpCommandResult ftpCommandResult = new FtpCommandResult();
            ftpCommandResult.Successful = SuccessfulEnum.Successful;
            try
            {
				if (_customerFormViewModel != null)
				{
					_customerFormViewModel.ApplyChanges();
                    ftpCommandResult = _customerFormViewModel.CheckFtpAfterApplyChanges();
                    if (ftpCommandResult.Successful != SuccessfulEnum.Successful)
                    {
                      return ftpCommandResult;
                    }
                }

				if (_importFoldersViewModel != null)
				{
					_importFoldersViewModel.ApplyChanges();
				}

				if (_exportPdaSettingsViewModel != null)
				{
					_exportPdaSettingsViewModel.ApplyChanges();
				}

				if (_exportErpSettingsViewModel != null)
				{
					_exportErpSettingsViewModel.ApplyChanges();
				}

				if (_updateViewModel != null)
				{
					_updateViewModel.ApplyChanges();
				}

				if (_dynamicColumnsViewModel != null)
				{
					_dynamicColumnsViewModel.ApplyChangesNonDynColumns();
				}

                switch (this.Context)
                {
                    case CBIContext.CreateInventor:
                        base.ContextCBIRepository.CreateContextCustomer(this._customer,
                                                                        this.GetCreateAuditConfig(), true);
                        break;
                    case CBIContext.History:
                        throw new InvalidOperationException();
                    case CBIContext.Main:
                        base.ContextCBIRepository.CreateContextCustomer(this._customer,
                                                                        this.GetMainAuditConfig(), true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

				if (_dynamicColumnsViewModel != null)
				{
					_dynamicColumnsViewModel.ApplyChanges();
				}
            }
            catch (Exception exc)
            {
                ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
                ftpCommandResult.Error = "SaveCustomer : with error" + exc.Message;
                _logger.ErrorException("SaveCustomer", exc);
            }
            return ftpCommandResult;
        }

        void ExportPdaSettingsViewModel_CheckValidation(object sender, EventArgs e)
        {
            this._okCommand.RaiseCanExecuteChanged();
        }
    }
}