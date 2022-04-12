using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Count4U.Common.Constants;
using Count4U.Common.Constants.AdapterNames;
using Count4U.Common.Enums;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.ExportPda;
using Count4U.Common.ViewModel.Misc;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.Misc;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{	 //используется при создании\редактировании кастомера
    public class ExportPdaSettingsViewModel : CBIContextBaseViewModel
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly ICustomerConfigRepository _customerConfigRepository;
        private readonly IImportAdapterRepository _importAdapterRepository;
        private readonly IEventAggregator _eventAggregator;
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly ICustomerRepository _customerRepository;

        private IExportPdaModuleInfo _selectedExportPda;
        private ObservableCollection<IExportPdaModuleInfo> _itemsExportPda;
		private string _adapterName;
		private bool _exportPdaVisibility = false;
		private bool _exportSettingsVisibility = true;
		private bool _programTypeVisibility = true;

        private Customer _customer;
		private Inventor _inventor;

        private ExportPdaSettingsControlViewModel _exportPdaSettingsControlViewModel;
        private ExportPdaProgramTypeViewModel _exportPdaProgramTypeViewModel;
		private ExportPdaMerkavaAdapterViewModel _exportPdaMerkavaAdapterControlViewModel;
	
        private bool _isEditable;
        private bool _isNew;

		private static string DefaultName = ExportPdaAdapterName.ExportHT630Adapter;

		private bool _isEventFromCode;

		private readonly UICommandRepository<IExportPdaModuleInfo> _commandRepositoryObject;
		private readonly DelegateCommand<IExportPdaModuleInfo> _showConfigCommand;

		//public IExportPdaModuleInfo _selectedAdapter;
		private bool _isShowConfig;

        public ExportPdaSettingsViewModel(
            IContextCBIRepository contextCbiRepository,
            IUnityContainer container,
            IRegionManager regionManager,
            ICustomerConfigRepository customerConfigRepository,
            IImportAdapterRepository importAdapterRepository,
			ICustomerRepository customerRepository,
            IEventAggregator eventAggregator,
			UICommandRepository<IExportPdaModuleInfo> commandRepositoryObject,
			IUserSettingsManager userSettingsManager)
            : base(contextCbiRepository)
        {
			this._eventAggregator = eventAggregator;
			this._importAdapterRepository = importAdapterRepository;
			this._customerConfigRepository = customerConfigRepository;
			this._customerRepository = customerRepository; 
			this._userSettingsManager = userSettingsManager;
			this._regionManager = regionManager;
			this._container = container;
			this._commandRepositoryObject = commandRepositoryObject;
			this.IsEditable = true;

			this._showConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowConfigCommandCanExecuted);
			this._isShowConfig = true;
        }

		//public IExportPdaModuleInfo SelectedAdapter
		//{
		//	get { return this._selectedAdapter; }
		//	set
		//	{
		//		this._selectedAdapter = value;
		//		RaisePropertyChanged(() => SelectedAdapter);
		//	}
		//}

		public DelegateCommand<IExportPdaModuleInfo> ShowConfigCommand
		{
			get { return this._showConfigCommand; }
		}

		

		private void ShowConfigCommandExecuted(IExportPdaModuleInfo exportPdaModuleInfo)
		{
 			//this.SelectedExportPda = exportPdaModuleInfo;
			//if (exportPdaModuleInfo != null)
			//{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ExportPdaModule = exportPdaModuleInfo });
			//}
		}

		public bool ShowConfigCommandCanExecuted(IExportPdaModuleInfo exportPdaModuleInfo)
		{
			if (this.SelectedExportPda == null) return false;
			bool configFileExists = base.IsConfigFileImportExists(this.SelectedExportPda.Name);
			return configFileExists;
		}


        public event EventHandler CheckValidation = delegate { };
        private void OnCheckValidation(EventArgs e)
        {
            EventHandler handler = CheckValidation;
            if (handler != null) handler(this, e);
        }

		public bool IsExportSettingsVisibility
		{
			get { return _exportSettingsVisibility; }
			set
			{
				_exportSettingsVisibility = value;

				RaisePropertyChanged(() => IsExportSettingsVisibility);
				if (_exportSettingsVisibility == true)
				{

				}
			}
		}

		public ExportPdaMerkavaAdapterViewModel ExportPdaMerkavaAdapterControl
		{
			get { return _exportPdaMerkavaAdapterControlViewModel; }
			//set { _exportPdaAdapterControlViewModel = value; }
		}

		public bool IsProgramTypeVisibility
		{
			get { return _programTypeVisibility; }
			set
			{
				_programTypeVisibility = value;

				RaisePropertyChanged(() => IsProgramTypeVisibility);
				if (_programTypeVisibility == true)
				{

				}
			}
		}

		public bool IsExportPdaVisibility
		{
			get { return _exportPdaVisibility; }
			set
			{
				_exportPdaVisibility = value;

				RaisePropertyChanged(() => IsExportPdaVisibility);
				if (_exportPdaVisibility == true)
				{

				}
			}
		}

		public string AdapterName
		{
			get { return _adapterName; }
		}

		public void GotNewFofusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ExportPdaModule = this._selectedExportPda });
			}
		}

		public void LostFocusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ExportPdaModule = null });
			}
		}

        public IExportPdaModuleInfo SelectedExportPda
        {
            get { return _selectedExportPda; }
            set
            {
                _selectedExportPda = value;
                RaisePropertyChanged(() => SelectedExportPda);
				this._showConfigCommand.RaiseCanExecuteChanged();

				if (_selectedExportPda != null)
					this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ExportPdaModule = this._selectedExportPda });


				this.FillSelectExportPdaAdapter();
            }
        }

		public bool IsShowConfig
		{
			get { return this._isShowConfig; }
			set
			{
				this._isShowConfig = value;
				RaisePropertyChanged(() => IsShowConfig);
			}
		}

		public void SetIsShowConfig(bool isShowConfig)
		{
			this.IsShowConfig = isShowConfig;
		}

		private void FillSelectExportPdaAdapter()
		{
			this._adapterName = this._selectedExportPda.Name;
			
			ExportCommandInfo infoExportDefault = UtilsExport.GetExportPdaCommandInfoDefaultData(this._adapterName, this._userSettingsManager);
			Customer customer = base.CurrentCustomer;
			if (customer != null)
			{
				ExportCommandInfo infoCustomer =
					UtilsExport.GetExportPdaCustomerData(this._customerConfigRepository, customer, infoExportDefault, this._adapterName);
				infoExportDefault = infoCustomer;
			}

			//================ ExportHT630Adapter =============================
			//================ ExportPdaMISAdapter =============================
				if (this._adapterName == ExportPdaAdapterName.ExportHT630Adapter
					|| this._adapterName == ExportPdaAdapterName.ExportPdaMISAdapter)
				{
					this.IsExportSettingsVisibility = true;
					this.IsProgramTypeVisibility = true;
					this.IsExportPdaVisibility = false;
					
					if (_exportPdaSettingsControlViewModel != null)
					{
						//	UtilsExport.GetExportPdaMISDefaultData(this._userSettingsManager);
						//контрол. заполнение данными по умолчанию. заполнение всех выпадающих списков. 
						this._exportPdaSettingsControlViewModel.FillGUIAdapterData(this.SelectedExportPda, infoExportDefault);

						//заполняет из кастомера (? И по умолчанию)
						//UtilsExport.FillExportPdaSettingsControl(this._exportPdaSettingsControlViewModel, this._customer, this._customerConfigRepository, SelectedExportPda);
						//this._exportPdaSettingsControlViewModel.FillAdapterCustomerData(base.CurrentCustomer, this.SelectedExportPda.Name);
					}

					if (this._exportPdaProgramTypeViewModel != null)
					{
						//контрол. заполнение данными по умолчанию. заполнение всех выпадающих списков. 
						this._exportPdaProgramTypeViewModel.FillGUIAdapterData(this.SelectedExportPda, infoExportDefault);
						//заполняет из кастомера (? И по умолчанию)		 //здесь оставили пока как было
						this._exportPdaProgramTypeViewModel.FillAdapterCustomerData(base.CurrentCustomer);
					}
				
					//================ ExportHT630Adapter =============================
					if (this._adapterName == ExportPdaAdapterName.ExportHT630Adapter)
					{
						this.IsExportSettingsVisibility = true;
						this.IsProgramTypeVisibility = true;
						this.IsExportPdaVisibility = false;

						if (this._exportPdaProgramTypeViewModel != null)
						{
							this._exportPdaProgramTypeViewModel.IsEditable = true;
							this._exportPdaProgramTypeViewModel.IsMISVisible = false;
							this._exportPdaProgramTypeViewModel.IsHT360Visible = true;
						}
						if (this._exportPdaProgramTypeViewModel != null)
						{
							this._exportPdaSettingsControlViewModel.IsMISVisible = false;
							this._exportPdaSettingsControlViewModel.IsHT360Visible = true;
						}

					}
					//================ ExportPdaMISAdapter =============================
					else if (this._adapterName == ExportPdaAdapterName.ExportPdaMISAdapter)
					{
						this.IsExportSettingsVisibility = true;
						this.IsProgramTypeVisibility = true;
						this.IsExportPdaVisibility = false;

						if (this._exportPdaProgramTypeViewModel != null)
						{
							this._exportPdaProgramTypeViewModel.IsEditable = true;

							this._exportPdaProgramTypeViewModel.IsMISVisible = true;
							this._exportPdaProgramTypeViewModel.IsHT360Visible = false;
						}
						if (this._exportPdaProgramTypeViewModel != null)
						{

							this._exportPdaSettingsControlViewModel.IsMISVisible = true;
							this._exportPdaSettingsControlViewModel.IsHT360Visible = false;
						}

					}
				}
				else if (this._adapterName == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter
					|| this._adapterName == ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter
					|| this._adapterName == ExportPdaAdapterName.ExportPdaNativSqliteAdapter
					|| this._adapterName == ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter
					|| this._adapterName == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter)
				{
					this.IsExportSettingsVisibility = false;
					this.IsProgramTypeVisibility = false;
					this.IsExportPdaVisibility = true;
					this._exportPdaProgramTypeViewModel.IsMISVisible = false;
					this._exportPdaProgramTypeViewModel.IsHT360Visible = false;
					this._exportPdaSettingsControlViewModel.IsMISVisible = false;
					this._exportPdaSettingsControlViewModel.IsHT360Visible = false;

					this._exportPdaMerkavaAdapterControlViewModel = Utils.GetViewModelFromRegion<ExportPdaMerkavaAdapterViewModel>(
						Common.RegionNames.ExportPdaAdapter, this._regionManager);

					if (this._exportPdaMerkavaAdapterControlViewModel != null)
					{
						//контрол. заполнение данными по умолчанию. заполнение всех выпадающих списков. 
						this._exportPdaMerkavaAdapterControlViewModel.FillGUIAdapterData(this.SelectedExportPda, infoExportDefault);

						//заполняет из кастомера (? и по умолчанию)
						//this._exportPdaMerkavaAdapterControlViewModel.FillAdapterCustomerData(base.CurrentCustomer);
						//this._exportPdaAdapterControlViewModel.PropertyChanged += ExportControlViewModel_PropertyChanged;
					}
				}
		}

        public ObservableCollection<IExportPdaModuleInfo> ItemsExportPda
        {
            get { return _itemsExportPda; }
        }

        public bool IsEditable				  //!!todo
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(() => IsEditable);

                if (_exportPdaSettingsControlViewModel != null)
                    _exportPdaSettingsControlViewModel.IsEditable = this._isEditable;

                if (_exportPdaProgramTypeViewModel != null)
                {
                    _exportPdaProgramTypeViewModel.IsEditable = this._isEditable;
                }
            }
        }

        public void SetIsNew(bool isNew)
        {
            this._isNew = isNew;
        }

        public void SetIsEditable(bool isEditable)
        {
            IsEditable = isEditable;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Subscribe(ImportExportAdapterChanged);

            string currentCustomerCode = base.CurrentCustomer == null ? String.Empty : base.CurrentCustomer.Code;
            string currentBranchCode = base.CurrentBranch == null ? String.Empty : base.CurrentBranch.Code;
            string currentInventorCode = base.CurrentInventor == null ? String.Empty : base.CurrentInventor.Code;

            var exportPdaAdapters = Utils.GetExportPdaAdapters(this._container, this._importAdapterRepository, currentCustomerCode, currentBranchCode, currentInventorCode);

            _itemsExportPda = new ObservableCollection<IExportPdaModuleInfo>(exportPdaAdapters);

            this._exportPdaSettingsControlViewModel = Utils.GetViewModelFromRegion<ExportPdaSettingsControlViewModel>(Common.RegionNames.ExportPdaSettingsInner, _regionManager);
            if (this._exportPdaSettingsControlViewModel != null)
            {
                this._exportPdaSettingsControlViewModel.PropertyChanged += ControlViewModel_PropertyChanged;
            }

            this._exportPdaProgramTypeViewModel = Utils.GetViewModelFromRegion<ExportPdaProgramTypeViewModel>(Common.RegionNames.ExportPdaProgramType, _regionManager);
			this._exportPdaMerkavaAdapterControlViewModel = Utils.GetViewModelFromRegion<ExportPdaMerkavaAdapterViewModel>(Common.RegionNames.ExportPdaAdapter, _regionManager);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Unsubscribe(ImportExportAdapterChanged);

            if (this._exportPdaSettingsControlViewModel != null)
            {
                this._exportPdaSettingsControlViewModel.PropertyChanged -= ControlViewModel_PropertyChanged;
            }
        }

		private void ImportExportAdapterChanged(ImportExportAdapterChangedEventPayload payload)
		{
			if (payload == null) return;
			string exportPdaAdapterName = "";
			if (payload.ExportPdaModule != null)
			{
				exportPdaAdapterName = payload.ExportPdaModule.Name;
			}

			if (string.IsNullOrWhiteSpace(exportPdaAdapterName) == true)
			{
				if (payload.ImportModule != null)
				{
					exportPdaAdapterName = AdapterConnections.GetRelated(payload.ImportModule.Name, ImportDomainEnum.ExportCatalogPDA);
				}
			}

			//string exportErpAdapterName = Common.Constants.ExportErpAdapterName.GetExportErpByImportAdapterName(payload.ImportModule.Name);
			if (_selectedExportPda == null)
			{
				if (String.IsNullOrEmpty(exportPdaAdapterName) || _itemsExportPda.Any(r => r.Name == exportPdaAdapterName) == false)
				{
					exportPdaAdapterName = Common.Constants.ExportPdaAdapterName.ExportHT630Adapter;
				}

				_selectedExportPda = _itemsExportPda.FirstOrDefault(r => r.Name == exportPdaAdapterName);
				RaisePropertyChanged(() => SelectedExportPda);
				this._showConfigCommand.RaiseCanExecuteChanged();
			}
			
		}

        void ControlViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaxCharacters" || e.PropertyName == "FileTypeSelected")
                this.OnCheckValidation(null);
        }

        public void SetCustomer(Customer customer, bool updateExportPdaSettings = true)
        {
			_isEventFromCode = true;
            this._customer = customer;

			if (!String.IsNullOrEmpty(customer.ExportCatalogAdapterCode) && _itemsExportPda.Any(r => r.Name == customer.ExportCatalogAdapterCode))
			{
				SelectedExportPda = _itemsExportPda.FirstOrDefault(r => r.Name == customer.ExportCatalogAdapterCode);
			}
			else
			{
				SelectedExportPda = _itemsExportPda.FirstOrDefault(r => r.Name == DefaultName);
			}
			this._showConfigCommand.RaiseCanExecuteChanged();

			  //!!TO DO перенести в отдельный VM
			//здесь заполняются данные из customer ??? правильно только при создании ивентора первый раз по наследованию от кастомера
			//заполняет из кастомера (? и по умолчанию)
			//??? перенесла в selectAdapter
			//if (updateExportPdaSettings)
			//{
			//	UtilsExport.FillExportPdaSettingsControl(_exportPdaSettingsControlViewModel, customer, _customerConfigRepository, SelectedExportPda);
			//}

			//if (!String.IsNullOrWhiteSpace(this._customer.ImportIturAdapterParms))
			//{
			//	string maxCharacters = this._customer.ImportIturAdapterParms;
			//	int v;
			//	if (Int32.TryParse(maxCharacters, out v))
			//	{
			//		this._exportPdaSettingsControlViewModel.MaxCharactersSet(v);
			//	}
			//}

			//if (_exportPdaProgramTypeViewModel != null)
			//{
			//	this._exportPdaProgramTypeViewModel.SelectCurrentAccordingForCustomer(customer);
			//}

			this._isEventFromCode = false;
        }

		//
		//
		//
		public void SetInventor(Inventor inventor, enInventorAdapterInherit mode)
		{
			this._inventor = inventor;

			_isEventFromCode = true;

			SetSelectedAdapterStateForInventor(mode);

			_isEventFromCode = false;
		}

		//public void SetSelectedAdapterStateForBranch(enBranchAdapterInherit mode)
		//{
		//	_isEventFromCode = true;

		//	// IExportErpModuleInfo info = ItemsExportErp.FirstOrDefault(r => r.IsDefault);
		//	IExportErpModuleInfo info = ItemsExportErp.FirstOrDefault(r => r.Name == DefaultName);

		//	Customer customer = this.CurrentCustomer;

		//	if (mode == enBranchAdapterInherit.InheritFromCustomer && customer != null)
		//	{
		//		string erpCode = customer.ExportERPAdapterCode;
		//		if (!String.IsNullOrEmpty(erpCode) && ItemsExportErp.Any(r => r.Name == erpCode))
		//			info = ItemsExportErp.FirstOrDefault(r => r.Name == erpCode);

		//		if (customer.ImportLocationAdapterParms == Common.Constants.Misc.MakatValue)
		//			Makat = true;
		//		else if (customer.ImportLocationAdapterParms == Common.Constants.Misc.MakatOriginalValue)
		//			MakatOriginal = true;
		//		else
		//			MakatOriginal = true;
		//	}
		//	else
		//	{
		//		string erpCode = _branch.ExportERPAdapterCode;
		//		if (!String.IsNullOrEmpty(erpCode) && ItemsExportErp.Any(r => r.Name == erpCode))
		//			info = ItemsExportErp.FirstOrDefault(r => r.Name == erpCode);

		//		if (_branch.ImportLocationAdapterParms == Common.Constants.Misc.MakatValue)
		//			Makat = true;
		//		else if (_branch.ImportLocationAdapterParms == Common.Constants.Misc.MakatOriginalValue)
		//			MakatOriginal = true;
		//		else
		//			MakatOriginal = true;
		//	}

		//	SelectedExportErp = info;

		//	_isEventFromCode = false;
		//}


		public void SetSelectedAdapterStateForInventor(enInventorAdapterInherit mode)
		{
			_isEventFromCode = true;

			//  IExportErpModuleInfo info = ItemsExportErp.FirstOrDefault(r => r.IsDefault);
			IExportPdaModuleInfo info = ItemsExportPda.FirstOrDefault(r => r.Name == DefaultName);

			Customer customer = this.CurrentCustomer;
			Branch branch = this.CurrentBranch;
			//Inventor inventor = this.CurrentInventor;

			if (mode == enInventorAdapterInherit.InheritFromCustomer && customer != null)
			{
				string pdaCode = customer.ExportCatalogAdapterCode;
				if (!String.IsNullOrEmpty(pdaCode) && ItemsExportPda.Any(r => r.Name == pdaCode))
					info = ItemsExportPda.FirstOrDefault(r => r.Name == pdaCode);
   			}
			else if (mode == enInventorAdapterInherit.InheritFromBranch && branch != null)
			{
				string pdaCode = branch.ExportCatalogAdapterCode;
				if (!String.IsNullOrEmpty(pdaCode) && ItemsExportPda.Any(r => r.Name == pdaCode))
					info = ItemsExportPda.FirstOrDefault(r => r.Name == pdaCode);
	  		}
			else
			{
				string pdaCode = _inventor.ExportCatalogAdapterCode;
				if (!String.IsNullOrEmpty(pdaCode) && ItemsExportPda.Any(r => r.Name == pdaCode))
					info = ItemsExportPda.FirstOrDefault(r => r.Name == pdaCode);
			}

			SelectedExportPda = info;

			_isEventFromCode = false;
		}

		/// ////////////////
		//public void ApplyInventorChanges()
		//{
		//	if (this._inventor != null)
		//	{
		//		this._inventor.PDAType = this._selectedExportPda == null ? String.Empty : this.SelectedExportPda.Name;
		//	}
		//}

		public void ApplyCustomerChangesFromInventor()
		{
			this._customer = base.CurrentCustomer;
			ApplyChanges();
			this._customer = null;
		}
		// сохранинение Сustomer
		public void ApplyChanges()
		{
			if (this._customer != null)
			{
				this._customer.ExportCatalogAdapterCode = this._selectedExportPda == null ? String.Empty : this.SelectedExportPda.Name;
				string key = String.Empty;
				CustomerConfig cc = null;
				string keyCode = _customer.Code + "|" + this._customer.ExportCatalogAdapterCode;
				CustomerConfigs configs = _customerConfigRepository.GetCustomerConfigsByCode(keyCode);
				if (configs != null)
				{
					if (this._customer.ExportCatalogAdapterCode == ExportPdaAdapterName.ExportHT630Adapter
						|| this._customer.ExportCatalogAdapterCode == ExportPdaAdapterName.ExportPdaMISAdapter)
					{

						//============== exportPdaProgramTypeViewModel =======================
						if (_exportPdaProgramTypeViewModel != null)
						{
							_exportPdaProgramTypeViewModel.Save(_customer);
						}
						// ======================  exportPdaSettingsControlViewModel    =====================
						if (this._exportPdaSettingsControlViewModel != null)
						{
							this._customer.MaxCharacters = this._exportPdaSettingsControlViewModel.MaxCharactersValidated.ToString();

							key = CustomerConfigIniEnum.ClientID.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.ClientId.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.ClientId.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.FileType.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.FileType.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.FileType.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.Hash.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.Hash.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.Hash.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.NewItem.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.NewItem.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.NewItem.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.NewItemBool.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.NewItemBool.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.NewItemBool.ToString();
								_customerConfigRepository.Update(cc);
							}



							key = CustomerConfigIniEnum.ChangeQuantityType.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.ChangeQuantityType.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.ChangeQuantityType.ToString();
								_customerConfigRepository.Update(cc);
							}


							key = CustomerConfigIniEnum.QType.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.QType.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.QType.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.Password.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.Password.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.Password.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.Host1.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.Host1.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.Host1.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.Host2.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.Host2.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.Host2.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.Timeout.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.Timeout.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.Timeout.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.Retry.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.Retry.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.Retry.ToString();
								_customerConfigRepository.Update(cc);
							}

							//SameBarcodeInLocation
							key = CustomerConfigIniEnum.SameBarcodeInLocation.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.SameBarcodeInLocation.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.SameBarcodeInLocation.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.DefaultHost.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.DefaultHost.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.DefaultHost.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.HTcalculateLookUp.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.HTcalculateLookUp.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.HTcalculateLookUp.ToString();
								_customerConfigRepository.Update(cc);
							}

							//key = CustomerConfigIniEnum.LastSync.ToString();
							//cc = configs.FirstOrDefault(r => r.Name == key);
							//if (cc == null)
							//{
							//	cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
							//	cc.Value = this._controlViewModel.LastSync.ToString();
							//	_customerConfigRepository.Insert(cc);
							//}
							//else
							//{
							//	cc.Value = this._controlViewModel.LastSync.ToString();
							//	_customerConfigRepository.Update(cc);
							//}

							//=============				   	AddNewLocation,
							key = CustomerConfigIniEnum.AddNewLocation.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AddNewLocation.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AddNewLocation.ToString();
								_customerConfigRepository.Update(cc);
							}
							//===========================================================================	
							//=============				   	AddExtraInputValueSelectFromBatchListForm,
							key = CustomerConfigIniEnum.AddExtraInputValueSelectFromBatchListForm.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AddExtraInputValueSelectFromBatchListForm.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AddExtraInputValueSelectFromBatchListForm.ToString();
								_customerConfigRepository.Update(cc);
							}

							//=============				   	AllowNewValueFromBatchListForm,
							key = CustomerConfigIniEnum.AllowNewValueFromBatchListForm.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AllowNewValueFromBatchListForm.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AllowNewValueFromBatchListForm.ToString();
								_customerConfigRepository.Update(cc);
							}

							//=============				   	SearchIfExistInBatchList,
							key = CustomerConfigIniEnum.SearchIfExistInBatchList.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.SearchIfExistInBatchList.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.SearchIfExistInBatchList.ToString();
								_customerConfigRepository.Update(cc);
							}


							//=============				   	AllowMinusQuantity,
							key = CustomerConfigIniEnum.AllowMinusQuantity.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AllowMinusQuantity.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AllowMinusQuantity.ToString();
								_customerConfigRepository.Update(cc);
							}


							//=============				   	FractionCalculate,
							key = CustomerConfigIniEnum.FractionCalculate.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.FractionCalculate.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.FractionCalculate.ToString();
								_customerConfigRepository.Update(cc);
							}


							//=============				   	PartialQuantity,
							key = CustomerConfigIniEnum.PartialQuantity.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.PartialQuantity.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.PartialQuantity.ToString();
								_customerConfigRepository.Update(cc);
							}

						   //=============				   	ConfirmNewLocation,
							key = CustomerConfigIniEnum.ConfirmNewLocation.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.ConfirmNewLocation.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.ConfirmNewLocation.ToString();
								_customerConfigRepository.Update(cc);
							}

							//=============				   ConfirmNewItem
							key = CustomerConfigIniEnum.ConfirmNewItem.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.ConfirmNewItem.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.ConfirmNewItem.ToString();
								_customerConfigRepository.Update(cc);
							}
							//=============				   AutoSendData
							key = CustomerConfigIniEnum.AutoSendData.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AutoSendData.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AutoSendData.ToString();
								_customerConfigRepository.Update(cc);
							}

							//=============				   AllowQuantityFraction
							key = CustomerConfigIniEnum.AllowQuantityFraction.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AllowQuantityFraction.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AllowQuantityFraction.ToString();
								_customerConfigRepository.Update(cc);
							}
							//=============				   AddExtraInputValue
							key = CustomerConfigIniEnum.AddExtraInputValue.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AddExtraInputValue.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AddExtraInputValue.ToString();
								_customerConfigRepository.Update(cc);
							}
							//=============				   AddExtraInputValueHeaderName
							key = CustomerConfigIniEnum.AddExtraInputValueHeaderName.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AddExtraInputValueHeaderName.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AddExtraInputValueHeaderName.ToString();
								_customerConfigRepository.Update(cc);
							}
							//=============				   AllowZeroQuantity
							key = CustomerConfigIniEnum.AllowZeroQuantity.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.AllowZeroQuantity.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.AllowZeroQuantity.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.MaxQuantity.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.MaxQuantity.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.MaxQuantity.ToString();
								_customerConfigRepository.Update(cc);
							}

							//key = CustomerConfigIniEnum.LookUpEXE.ToString();
							//cc = configs.FirstOrDefault(r => r.Name == key);
							//if (cc == null)
							//{
							//	cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
							//	cc.Value = this._controlViewModel.LookUpEXE.ToString();
							//	_customerConfigRepository.Insert(cc);
							//}
							//else
							//{
							//	cc.Value = this._controlViewModel.LookUpEXE.ToString();
							//	_customerConfigRepository.Update(cc);
							//}


							key = CustomerConfigIniEnum.UseAlphaKey.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.UseAlphaKey.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.UseAlphaKey.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.IturNamePrefix.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IturNamePrefix.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IturNamePrefix.ToString();
								_customerConfigRepository.Update(cc);
							}

							// = MaxLen		??
							key = CustomerConfigIniEnum.MaxLen.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.MaxCharacters.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.MaxCharacters.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.SearchDef.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.SearchDef.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.SearchDef.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.IturInvertPrefix.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertPrefix ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertPrefix ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}


							key = CustomerConfigIniEnum.IsAddBinarySearch.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IsAddBinarySearch ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IsAddBinarySearch ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}

							//info.IsInvertWords 
							key = CustomerConfigIniEnum.IsInvertWords.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertWords ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertWords ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}

							//info.IsInvertLetters 
							key = CustomerConfigIniEnum.IsInvertLetters.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertLetters ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertLetters ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}


							//info.IsCutAfterInvert 
							key = CustomerConfigIniEnum.IsCutAfterInvert.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IsCutAfterInvert ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IsCutAfterInvert ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}


							//info.IsInvertWordsConfig
							key = CustomerConfigIniEnum.IsInvertWordsConfig.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertWordsConfig ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertWordsConfig ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}

							//info.IsInvertLettersConfig 
							key = CustomerConfigIniEnum.IsInvertLettersConfig.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertLettersConfig ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IsInvertLettersConfig ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}

							// EncodingCodePage
							key = CustomerConfigIniEnum.EncodingCodePage.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.Encoding.CodePage.ToString();
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.Encoding.CodePage.ToString();
								_customerConfigRepository.Update(cc);
							}

							key = CustomerConfigIniEnum.IturNameType.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaSettingsControlViewModel.IturTypeByName ? "0" : "1";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaSettingsControlViewModel.IturTypeByName ? "0" : "1";
								_customerConfigRepository.Update(cc);
							}
						}
					}

					// ======================  exportPdaMerkavaAdapterControlViewModel    =====================
					if (this._customer.ExportCatalogAdapterCode == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter
						|| this._customer.ExportCatalogAdapterCode == ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter
						|| this._customer.ExportCatalogAdapterCode == ExportPdaAdapterName.ExportPdaNativSqliteAdapter
						|| this._customer.ExportCatalogAdapterCode == ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter
						|| this._customer.ExportCatalogAdapterCode == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter)
					{
						//IncludeCurrentInventor
						if (_exportPdaMerkavaAdapterControlViewModel != null)
						{
							key = CustomerConfigIniEnum.IncludeCurrentInventor.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaMerkavaAdapterControlViewModel.IncludeCurrentInventor ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaMerkavaAdapterControlViewModel.IncludeCurrentInventor ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}

							//IncludePreviousInventor
							key = CustomerConfigIniEnum.IncludePreviousInventor.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaMerkavaAdapterControlViewModel.IncludePreviousInventor ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaMerkavaAdapterControlViewModel.IncludePreviousInventor ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}

							//IncludeProfile
							key = CustomerConfigIniEnum.IncludeProfile.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaMerkavaAdapterControlViewModel.IncludeProfile ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaMerkavaAdapterControlViewModel.IncludeProfile ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}


							//CreateZipFile
							key = CustomerConfigIniEnum.CreateZipFile.ToString();
							cc = configs.FirstOrDefault(r => r.Name == key);
							if (cc == null)
							{
								cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
								cc.Value = this._exportPdaMerkavaAdapterControlViewModel.CreateZipFile ? "1" : "0";
								_customerConfigRepository.Insert(cc);
							}
							else
							{
								cc.Value = this._exportPdaMerkavaAdapterControlViewModel.CreateZipFile ? "1" : "0";
								_customerConfigRepository.Update(cc);
							}
						}
					}

				}
				this._customerRepository.Update(this._customer);
			}
		}

        public bool IsFormValid()
        {
            if (this._exportPdaSettingsControlViewModel != null)
                return this._exportPdaSettingsControlViewModel.IsFormValid();

            return true;
        }
    }
}