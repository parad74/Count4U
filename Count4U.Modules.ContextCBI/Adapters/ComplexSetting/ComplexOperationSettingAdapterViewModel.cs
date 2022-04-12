using System;
using System.Windows.Input;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Modules.ContextCBI.Adapters.ComplexSetting
{
    public class ComplexOperationSettingAdapterViewModel : ImportModuleBaseViewModel
    {
		private readonly INavigationRepository _navigationRepository;

		//private readonly CustomerFormViewModel _customerFormViewModel;
	
		private readonly DelegateCommand _okCommand;
		private readonly DelegateCommand _cancelCommand;

		public ComplexOperationSettingAdapterViewModel(
            IServiceLocator serviceLocator, 
            IContextCBIRepository contextCBIRepository, 
            IEventAggregator eventAggregator,
            IRegionManager regionManager, 
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager,
			INavigationRepository navigationRepository) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
			this._navigationRepository = navigationRepository;
			this._okCommand = new DelegateCommand(this.OkCommandExecuted, this.OkCommandCanExecute);
			this._cancelCommand = new DelegateCommand(this.CancelCommandExecuted);
			//customerFormViewModel.PropertyChanged += customerFormViewModel_PropertyChanged
        }

		//void customerFormViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		//{
		//	this._okCommand.RaiseCanExecuteChanged();
		//}

		public string Title
		{
			//get { return Localization.Resources.ViewModel_CustomerAdd_CreateNewCustomer; }
			get { return ""; }
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

		//public CustomerFormViewModel CustomerFormVM
		//{
		//	get { return this._customerFormViewModel; }
		//}


		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			//_customer = new Customer();
			//_customer.ID = 0;
			//_customer.Code = Utils.CodeNewGenerate(4);

			//_customerFormViewModel.CustomerSet(_customer, false, true);

		
		}

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);
		
		}

		private bool OkCommandCanExecute()
		{
			bool isExportFormOk = true;
			//if (this._exportPdaSettingsViewModel != null)
			//	isExportFormOk = this._exportPdaSettingsViewModel.IsFormValid();

			//return this._customerFormViewModel.IsFormValid() && isExportFormOk;
			return  isExportFormOk;
		}

		private void OkCommandExecuted()
		{
			Mouse.OverrideCursor = Cursors.Wait;

			this.SaveCustomer();

			//this._eventAggregator.GetEvent<CustomerAddedEvent>().Publish(this._customer);

			//CustomerPostData data = new CustomerPostData() { CustomerCode = _customer.Code, IsNew = true };
			UriQuery query = new UriQuery();
			Utils.AddContextToQuery(query, this.Context);
			Utils.AddDbContextToQuery(query, base.CBIDbContext);
			//UtilsConvert.AddObjectToQuery(query, _navigationRepository, data, Common.NavigationObjects.CustomerPost);

			_regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.CustomerPostView + query, UriKind.Relative));
		}

		private void CancelCommandExecuted()
		{
			this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
		}

		private void SaveCustomer()
		{
			//try
			//{
				//if (_customerFormViewModel != null)
				//{
				//	_customerFormViewModel.ApplyChanges();
				//}

				//if (_importFoldersViewModel != null)
				//{
				//	_importFoldersViewModel.ApplyChanges();
				//}

				//if (_exportPdaSettingsViewModel != null)
				//{
				//	_exportPdaSettingsViewModel.ApplyChanges();
				//}

				//if (_exportErpSettingsViewModel != null)
				//{
				//	_exportErpSettingsViewModel.ApplyChanges();
				//}

				//if (_updateViewModel != null)
				//{
				//	_updateViewModel.ApplyChanges();
				//}

				//if (_dynamicColumnsViewModel != null)
				//{
				//	_dynamicColumnsViewModel.ApplyChangesNonDynColumns();
				//}

				//switch (this.Context)
				//{
					//case CBIContext.CreateInventor:
					//	base.ContextCBIRepository.CreateContextCustomer(this._customer,
					//													this.GetCreateAuditConfig(), true);
					//	break;
					//case CBIContext.History:
					//	throw new InvalidOperationException();
					//case CBIContext.Main:
					//	base.ContextCBIRepository.CreateContextCustomer(this._customer,
					//													this.GetMainAuditConfig(), true);
					//	break;
					//default:
					//	throw new ArgumentOutOfRangeException();
			//	}

			//	if (_dynamicColumnsViewModel != null)
			//	{
			//		_dynamicColumnsViewModel.ApplyChanges();
			//	}
			//}
			//catch (Exception exc)
			//{
			//	//_logger.ErrorException("SaveCustomer", exc);
			//}
		}

		void ExportPdaSettingsViewModel_CheckValidation(object sender, EventArgs e)
		{
			this._okCommand.RaiseCanExecuteChanged();
		}

        public override void InitDefault()
        {
            
        }

        public override void InitConfig()
        {
	    }

        public override void Clear()
        {
            
        }

        public override void Import()
        {
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

        }

        public override bool CanImport()
        {
            return false;
        }

        protected override void RunImport()
        {
            
        }

        protected override void RunClear()
        {

        }

        protected override void EncondingUpdated()
        {

        }
    }
}