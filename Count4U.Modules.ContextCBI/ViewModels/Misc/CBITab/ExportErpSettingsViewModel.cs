using System;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Constants.AdapterNames;
using Count4U.Common.Enums;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events.Misc;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Count4U.Model.Interface.Audit;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Count4U.Common.Services.UICommandService;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{
    public class ExportErpSettingsViewModel : CBIContextBaseViewModel
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly ICustomerConfigRepository _customerConfigRepository;
        private readonly IImportAdapterRepository _importAdapterRepository;
        private readonly IEventAggregator _eventAggregator;

        private IExportErpModuleInfo _selectedExportErp;
        private ObservableCollection<IExportErpModuleInfo> _itemsExportErp;

        private bool _isEditable;

        private bool _makat;
        private bool _makatOriginal;
		private bool _excludeNotExistingInCatalog;

        private bool _isMakatRadioVisible;
		private bool _isExcludeNotExistingInCatalogVisible;

        private Customer _customer;
        private Branch _branch;
        private Inventor _inventor;

        private bool _isEventFromCode;

        private static string DefaultName = ExportErpAdapterName.ExportErpDefaultAdapter;

		private readonly UICommandRepository<IExportErpModuleInfo> _commandRepositoryObject;
		private readonly DelegateCommand<IExportErpModuleInfo> _showConfigCommand;

		private bool _isShowConfig;

        public ExportErpSettingsViewModel(
        IContextCBIRepository contextCbiRepository,
        IUnityContainer container,
        IRegionManager regionManager,
        ICustomerConfigRepository customerConfigRepository,
        IImportAdapterRepository importAdapterRepository,
		UICommandRepository<IExportErpModuleInfo> commandRepositoryObject,
        IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
			this._eventAggregator = eventAggregator;
			this._importAdapterRepository = importAdapterRepository;
			this._customerConfigRepository = customerConfigRepository;
			this._commandRepositoryObject = commandRepositoryObject;
			this._regionManager = regionManager;
			this._container = container;

			this._makat = false;
			this._makatOriginal = true;
			this._excludeNotExistingInCatalog = false;

			this._showConfigCommand = _commandRepositoryObject.Build(enUICommand.ShowConfig, ShowConfigCommandExecuted, ShowConfigCommandCanExecuted);
			this._isShowConfig = true;

        }

		public DelegateCommand<IExportErpModuleInfo> ShowConfigCommand
		{
			get { return this._showConfigCommand; }
		}

		private void ShowConfigCommandExecuted(IExportErpModuleInfo exportErpModuleInfo)
		{
			//this.SelectedExportPda = exportPdaModuleInfo;
			//if (exportPdaModuleInfo != null)
			//{
			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ExportErpModule = exportErpModuleInfo });
			//}
		}

		public bool ShowConfigCommandCanExecuted(IExportErpModuleInfo exportErpModuleInfo)
		{
			if (this.SelectedExportErp == null) return false;
			bool configFileExists = base.IsConfigFileImportExists(this.SelectedExportErp.Name);
			return configFileExists;
		}

		public void GotNewFofusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ExportErpModule = this._selectedExportErp });
			}
		}

		public void LostFocusConfig()
		{
			if (IsShowConfig == true)
			{
				this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ExportErpModule = null });
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



        public IExportErpModuleInfo SelectedExportErp
        {
            get { return _selectedExportErp; }
            set
            {
                _selectedExportErp = value;
                RaisePropertyChanged(() => SelectedExportErp);
				this._showConfigCommand.RaiseCanExecuteChanged();

                if (_selectedExportErp != null)	 {
                    this.IsMakatRadioVisible = _selectedExportErp.Name != ExportErpAdapterName.ExportErpGazitAdapter;
					//this.IsExcludeNotExistingInCatalogVisible = _selectedExportErp.Name == ExportErpAdapterName.ExportErpGeneralCSVAdapter;
					if (this._customer != null) this.IsExcludeNotExistingInCatalogVisible = true;
					if (this._branch != null) this.IsExcludeNotExistingInCatalogVisible = false ;
					if (this._inventor != null) this.IsExcludeNotExistingInCatalogVisible = false;				
				}

                if (!_isEventFromCode)
                {
                    if (_selectedExportErp != null)
                    {
                        this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Publish(new ImportExportAdapterChangedEventPayload() { ExportErpModule = this._selectedExportErp });
                    }
                }
            }
        }

        public ObservableCollection<IExportErpModuleInfo> ItemsExportErp
        {
            get { return _itemsExportErp; }
        }

        public bool Makat
        {
            get { return _makat; }
            set
            {
                _makat = value;
                RaisePropertyChanged(() => Makat);

                this._makatOriginal = false;
                RaisePropertyChanged(() => MakatOriginal);
            }
        }

        public bool MakatOriginal
        {
            get { return _makatOriginal; }
            set
            {
                _makatOriginal = value;
                RaisePropertyChanged(() => MakatOriginal);

                this._makat = false;
                RaisePropertyChanged(() => Makat);
            }
        }

        public bool IsMakatRadioVisible
        {
            get { return _isMakatRadioVisible; }
            set
            {
                _isMakatRadioVisible = value;
                RaisePropertyChanged(() => IsMakatRadioVisible);
            }
        }



		public bool IsExcludeNotExistingInCatalogVisible
        {
			get { return _isExcludeNotExistingInCatalogVisible; }
            set
            {
				_isExcludeNotExistingInCatalogVisible = value;
				RaisePropertyChanged(() => IsExcludeNotExistingInCatalogVisible);
            }
        }

		public bool ExcludeNotExistingInCatalog
        {
			get { return _excludeNotExistingInCatalog; }
            set
            {
				_excludeNotExistingInCatalog = value;
				RaisePropertyChanged(() => ExcludeNotExistingInCatalog);
	         }
        }
		

        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(() => IsEditable);
            }
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

            var exportErpAdapters = Utils.GetExportErpAdapters(this._container, this._importAdapterRepository, currentCustomerCode, currentBranchCode, currentInventorCode);

            _itemsExportErp = new ObservableCollection<IExportErpModuleInfo>(exportErpAdapters);

            this._isEditable = !navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.IsReadOnly);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Unsubscribe(ImportExportAdapterChanged);
        }

        private void ImportExportAdapterChanged(ImportExportAdapterChangedEventPayload payload)
        {
			if (payload == null) return;
			string exportErpAdapterName = "";

			if (payload.ExportErpModule != null)
			{
				exportErpAdapterName = payload.ExportErpModule.Name;
			}

			if (string.IsNullOrWhiteSpace(exportErpAdapterName) == true)
			{
				if (payload.ImportModule != null)
				{
					//string updateAdapterName = Common.Constants.UpdateCatalogAdapterName.GetUpdateByImportAdapterName(payload.ImportModule.Name);
					exportErpAdapterName = AdapterConnections.GetRelated(payload.ImportModule.Name, ImportDomainEnum.ExportCatalogERP);
				}
			}

            //string exportErpAdapterName = Common.Constants.ExportErpAdapterName.GetExportErpByImportAdapterName(payload.ImportModule.Name);
			if (_selectedExportErp == null)
			{
				if (String.IsNullOrEmpty(exportErpAdapterName) || _itemsExportErp.Any(r => r.Name == exportErpAdapterName) == false)
				{
					exportErpAdapterName = Common.Constants.ExportErpAdapterName.ExportErpDefaultAdapter;
				}
				_selectedExportErp = _itemsExportErp.FirstOrDefault(r => r.Name == exportErpAdapterName);
				RaisePropertyChanged(() => SelectedExportErp);
				this._showConfigCommand.RaiseCanExecuteChanged();
			}
            
           
        }

        public void SetCustomer(Customer customer)
        {
            _isEventFromCode = true;

            this._customer = customer;

            string erpCode = customer.ExportERPAdapterCode;
            if (!String.IsNullOrEmpty(erpCode) && _itemsExportErp.Any(r => r.Name == erpCode))
            {
                SelectedExportErp = _itemsExportErp.FirstOrDefault(r => r.Name == erpCode);
            }
            else
            {
             //   SelectedExportErp = _itemsExportErp.FirstOrDefault(r => r.IsDefault);
				SelectedExportErp = _itemsExportErp.FirstOrDefault(r => r.Name == DefaultName);
            }

			this._showConfigCommand.RaiseCanExecuteChanged();

			if (_customer.MakatOrMakatOriginal == Common.Constants.Misc.MakatValue)
                Makat = true;
			else if (_customer.MakatOrMakatOriginal == Common.Constants.Misc.MakatOriginalValue)
                MakatOriginal = true;
            else
                MakatOriginal = true;


			this.ExcludeNotExistingInCatalog = false;
			string keyCode = customer.Code + "|" + SelectedExportErp.Name;
			Dictionary<string, CustomerConfig> configDictionary = this._customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
			if (configDictionary != null)	  
			{
				this.ExcludeNotExistingInCatalog = configDictionary.GetBoolValue(this.ExcludeNotExistingInCatalog, CustomerConfigIniEnum.ExcludeNotExistingInCatalog);
			}


            _isEventFromCode = false;
        }

        public void SetBranch(Branch branch, enBranchAdapterInherit mode)
        {
            this._branch = branch;

            _isEventFromCode = true;

            SetSelectedAdapterStateForBranch(mode);

            _isEventFromCode = false;
        }

        public void SetInventor(Inventor inventor, enInventorAdapterInherit mode)
        {
            this._inventor = inventor;

            _isEventFromCode = true;

            SetSelectedAdapterStateForInventor(mode);

            _isEventFromCode = false;
        }

        public void SetSelectedAdapterStateForBranch(enBranchAdapterInherit mode)
        {
            _isEventFromCode = true;

           // IExportErpModuleInfo info = ItemsExportErp.FirstOrDefault(r => r.IsDefault);
            IExportErpModuleInfo info = ItemsExportErp.FirstOrDefault(r => r.Name == DefaultName);

            Customer customer = this.CurrentCustomer;

            if (mode == enBranchAdapterInherit.InheritFromCustomer && customer != null)
            {
                string erpCode = customer.ExportERPAdapterCode;
                if (!String.IsNullOrEmpty(erpCode) && ItemsExportErp.Any(r => r.Name == erpCode))
                    info = ItemsExportErp.FirstOrDefault(r => r.Name == erpCode);

				if (customer.MakatOrMakatOriginal == Common.Constants.Misc.MakatValue)
                    Makat = true;
				else if (customer.MakatOrMakatOriginal == Common.Constants.Misc.MakatOriginalValue)
                    MakatOriginal = true;
                else
                    MakatOriginal = true;
            }
            else
            {
                string erpCode = _branch.ExportERPAdapterCode;
                if (!String.IsNullOrEmpty(erpCode) && ItemsExportErp.Any(r => r.Name == erpCode))
                    info = ItemsExportErp.FirstOrDefault(r => r.Name == erpCode);

				if (_branch.MakatOrMakatOriginal == Common.Constants.Misc.MakatValue)
                    Makat = true;
				else if (_branch.MakatOrMakatOriginal == Common.Constants.Misc.MakatOriginalValue)
                    MakatOriginal = true;
                else
                    MakatOriginal = true;
            }

            SelectedExportErp = info;

            _isEventFromCode = false;
        }


        public void SetSelectedAdapterStateForInventor(enInventorAdapterInherit mode)
        {
            _isEventFromCode = true;

          //  IExportErpModuleInfo info = ItemsExportErp.FirstOrDefault(r => r.IsDefault);
            IExportErpModuleInfo info = ItemsExportErp.FirstOrDefault(r => r.Name == DefaultName);

            Customer customer = this.CurrentCustomer;
            Branch branch = this.CurrentBranch;

            if (mode == enInventorAdapterInherit.InheritFromCustomer && customer != null)
            {
                string erpCode = customer.ExportERPAdapterCode;
                if (!String.IsNullOrEmpty(erpCode) && ItemsExportErp.Any(r => r.Name == erpCode))
                    info = ItemsExportErp.FirstOrDefault(r => r.Name == erpCode);

				if (customer.MakatOrMakatOriginal == Common.Constants.Misc.MakatValue)
                    Makat = true;
				else if (customer.MakatOrMakatOriginal == Common.Constants.Misc.MakatOriginalValue)
                    MakatOriginal = true;
                else
                    MakatOriginal = true;
            }
            else if (mode == enInventorAdapterInherit.InheritFromBranch && branch != null)
            {
                string erpCode = branch.ExportERPAdapterCode;
                if (!String.IsNullOrEmpty(erpCode) && ItemsExportErp.Any(r => r.Name == erpCode))
                    info = ItemsExportErp.FirstOrDefault(r => r.Name == erpCode);

				if (branch.MakatOrMakatOriginal == Common.Constants.Misc.MakatValue)
                    Makat = true;
				else if (branch.MakatOrMakatOriginal == Common.Constants.Misc.MakatOriginalValue)
                    MakatOriginal = true;
                else
                    MakatOriginal = true;
            }
            else
            {
                string erpCode = _inventor.ExportERPAdapterCode;
                if (!String.IsNullOrEmpty(erpCode) && ItemsExportErp.Any(r => r.Name == erpCode))
                    info = ItemsExportErp.FirstOrDefault(r => r.Name == erpCode);

                if (_inventor.ImportLocationParms == Common.Constants.Misc.MakatValue)
                    Makat = true;
                else if (_inventor.ImportLocationParms == Common.Constants.Misc.MakatOriginalValue)
                    MakatOriginal = true;
                else
                    MakatOriginal = true;
            }

            SelectedExportErp = info;

            _isEventFromCode = false;
        }

        public void ApplyChanges()
        {
            if (this._customer != null)
            {
                this._customer.ExportERPAdapterCode = this._selectedExportErp == null ? String.Empty : this.SelectedExportErp.Name;

                if (_makat == true)
                {
					this._customer.MakatOrMakatOriginal = Common.Constants.Misc.MakatValue;
                }
                else if (_makatOriginal == true)
                {
					this._customer.MakatOrMakatOriginal = Common.Constants.Misc.MakatOriginalValue;
                }

			//		if ( customer == null || this._customerConfigRepository == null)
			//	return;
			////перед этим запонить данными по умолчанию для каждого авдптера отдельно.
			////if (selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMISAdapter)
			//string keyCode = customer.Code + "|" + adapterName;
			//Dictionary<string, CustomerConfig> configDictionary = this._customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
			//if (configDictionary != null)
			//this.IsExcludeNotExistingInCatalog = configDictionary.GetBoolValue(this.IsExcludeNotExistingInCatalog, CustomerConfigIniEnum.ExcludeNotExistingInCatalog);
				string key = String.Empty;
				CustomerConfig cc = null;
				string keyCode = _customer.Code + "|" + this._customer.ExportERPAdapterCode;
				CustomerConfigs configs = _customerConfigRepository.GetCustomerConfigsByCode(keyCode);
				if (configs != null)
				{

					key = CustomerConfigIniEnum.ExcludeNotExistingInCatalog.ToString();
					cc = configs.FirstOrDefault(r => r.Name == key);
					if (cc == null)
					{
						cc = new CustomerConfig() { CustomerCode = keyCode, Name = key };
						cc.Value = this._excludeNotExistingInCatalog ? "1" : "0"; 
						_customerConfigRepository.Insert(cc);
					}
					else
					{
						cc.Value = this._excludeNotExistingInCatalog ? "1" : "0";
						_customerConfigRepository.Update(cc);
					}
				}
            }

            if (this._branch != null)
            {
                this._branch.ExportERPAdapterCode = this._selectedExportErp == null ? String.Empty : this.SelectedExportErp.Name;

                if (_makat)
                {
					this._branch.MakatOrMakatOriginal = Common.Constants.Misc.MakatValue;
                }
                else if (_makatOriginal)
                {
					this._branch.MakatOrMakatOriginal = Common.Constants.Misc.MakatOriginalValue;
                }
            }

            if (this._inventor != null)
            {
                this._inventor.ExportERPAdapterCode = this._selectedExportErp == null ? String.Empty : this.SelectedExportErp.Name;

                if (_makat)
                {
                    this._inventor.ImportLocationParms = Common.Constants.Misc.MakatValue;
                }
                else if (_makatOriginal)
                {
                    this._inventor.ImportLocationParms = Common.Constants.Misc.MakatOriginalValue;
                }
            }

        }
    }

	public static class ExportErpParamParse
	{
		public static int GetIntValue(this Dictionary<string, CustomerConfig> config, int parm,
			CustomerConfigIniEnum adapterParm)
		{
			int? parseValue = ParseValue(config, adapterParm);
			if (parseValue != null)
			{
				return parseValue.Value;
			}
			else
			{
				return parm;

			}
		}

		public static string GetStringValue(this Dictionary<string, CustomerConfig> config, string parm,
		CustomerConfigIniEnum adapterParm)
		{
			string parseValue = ParseStringValue(config, adapterParm);
			if (string.IsNullOrWhiteSpace(parseValue) == false)
			{
				return parseValue;
			}
			else
			{
				return parm;
			}
		}


		public static bool GetBoolValue(this Dictionary<string, CustomerConfig> config, bool parm,
		CustomerConfigIniEnum adapterParm)
		{
			string parseValue = ParseStringValue(config, adapterParm);
			if (string.IsNullOrWhiteSpace(parseValue) == false)
			{
				bool ret = parseValue == "1";
				return ret;
			}
			else
			{
				return parm;
			}
		}

		private static int? ParseValue(Dictionary<string, CustomerConfig> config, CustomerConfigIniEnum en)
		{
			string value;
			int n;
			if (config.Any(r => r.Value != null && r.Value.Name == en.ToString()))
			{
				value = config.First(r => r.Value.Name == en.ToString()).Value.Value;
				if (Int32.TryParse(value, out n))
					return n;
			}

			return null;
		}

		private static string ParseStringValue(Dictionary<string, CustomerConfig> config, CustomerConfigIniEnum en)
		{
			string value = "";
			if (config.Any(r => r.Value != null && r.Value.Name == en.ToString()))
			{
				try
				{
					value = config.First(r => r.Value.Name == en.ToString()).Value.Value;
					return value;
				}
				catch { return value; }
			}

			return value;
		}
	}
}