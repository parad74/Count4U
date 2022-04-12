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
using System.Xml.Linq;
using Microsoft.Practices.Prism.Commands;
using System.IO;
using Count4U.Modules.ContextCBI.Properties;
using Count4U.Modules.ContextCBI.Xml.Config;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab
{
	public class ConfigAdapterSettingViewModel : CBIContextBaseViewModel
	{
		private readonly IUnityContainer _container;
		private readonly IRegionManager _regionManager;
		private readonly ICustomerConfigRepository _customerConfigRepository;
		private readonly IImportAdapterRepository _importAdapterRepository;
		private readonly IEventAggregator _eventAggregator;
		private readonly DBSettings _dbSettings;

		//private IExportErpModuleInfo _selectedExportErp;
		//private ObservableCollection<IExportErpModuleInfo> _itemsExportErp;

		private bool _isEditable;

		//private bool _makat;
		//private bool _makatOriginal;
		//private bool _excludeNotExistingInCatalog;

		//private bool _isMakatRadioVisible;
		//private bool _isExcludeNotExistingInCatalogVisible;

		private Customer _customer;
		//private Branch _branch;
		//private Inventor _inventor;

		//private bool _isEventFromCode;

		//private static string DefaultName = ExportErpAdapterName.ExportErpDefaultAdapter;

		private readonly DelegateCommand _openConfigCommand;
		private readonly DelegateCommand _reloadConfigCommand;
		//private readonly DelegateCommand _saveConfigCommand;
		//private readonly DelegateCommand _importByConfigCommand;

		private string _configXML;
		private XDocument _configXDocument;
		private string _dataInConfigPath = String.Empty;
		private string _dataInConfigPathWithFile = String.Empty;
		private bool _fromCustomer = true;

		public IBaseImportExportModuleInfo _selectedAdapter;
		public IImportModuleInfo _selectedImportAdapter;
		public IExportErpModuleInfo _selectedExportErpAdapter;
		public IExportPdaModuleInfo _selectedExportPdaAdapter;
		public IImportModuleInfo _selectedUpdateAdapter;
		

		public ConfigAdapterSettingViewModel(
		IContextCBIRepository contextCbiRepository,
		IUnityContainer container,
		IRegionManager regionManager,
		ICustomerConfigRepository customerConfigRepository,
		IImportAdapterRepository importAdapterRepository,
		IEventAggregator eventAggregator,
		DBSettings dbSettings)
			: base(contextCbiRepository)
		{
			this._eventAggregator = eventAggregator;
			this._importAdapterRepository = importAdapterRepository;
			this._customerConfigRepository = customerConfigRepository;
			this._regionManager = regionManager;
			this._container = container;
			this._dbSettings = dbSettings;
			this._isEditable = false;
			this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			//this._saveConfigCommand = new DelegateCommand(SaveConfigCommandExecuted, SaveConfigCommandCanExecute);
			//this._importByConfigCommand = new DelegateCommand(ImportByConfigCommandExecuted, ImportByConfigCommandCanExecute);
			this._reloadConfigCommand = new DelegateCommand(ReloadConfigCommandExecuted, ReloadConfigCommandCanExecute);
			

			//_makat = false;
			//_makatOriginal = true;
			//_excludeNotExistingInCatalog = false;
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


		public IBaseImportExportModuleInfo SelectedAdapter
		{
			get { return this._selectedAdapter; }
			set
			{
				this._selectedAdapter = value;
				RaisePropertyChanged(() => SelectedAdapter);
			}
		}

		public IImportModuleInfo SelectedImportAdapter
		{
			get { return this._selectedImportAdapter; }
			set
			{
				this._selectedImportAdapter = value;
				RaisePropertyChanged(() => SelectedImportAdapter);
			}
		}


		public IExportErpModuleInfo SelectedExportErpAdapter
		{
			get { return this._selectedExportErpAdapter; }
			set
			{
				this._selectedExportErpAdapter = value;
				RaisePropertyChanged(() => SelectedExportErpAdapter);
			}
		}


		public IExportPdaModuleInfo SelectedExportPdaAdapter
		{
			get { return this._selectedExportPdaAdapter; }
			set
			{
				this._selectedExportPdaAdapter = value;
				RaisePropertyChanged(() => SelectedExportPdaAdapter);
			}
		}


		public IImportModuleInfo SelectedUpdateAdapter
		{
			get { return this._selectedUpdateAdapter; }
			set
			{
				this._selectedUpdateAdapter = value;
				RaisePropertyChanged(() => SelectedUpdateAdapter);
			}
		}

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);
			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Subscribe(ImportExportAdapterChanged);
		}

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);
			this._eventAggregator.GetEvent<ImportExportAdapterChangedEvent>().Unsubscribe(ImportExportAdapterChanged);

		}


		private void ImportExportAdapterChanged(ImportExportAdapterChangedEventPayload payload)
		{
			if (payload.ImportModule == null && payload.ExportErpModule == null 
				&& payload.ExportPdaModule == null && payload.ResultModule == null
				&& payload.UpdateModule == null)
			{
				this.ConfigXDocument = null;
				this.DataInConfigPath = String.Empty;
				return;
			}
			this.SelectedImportAdapter = payload.ImportModule;
			this.SelectedUpdateAdapter = payload.UpdateModule;
			this.SelectedExportPdaAdapter = payload.ExportPdaModule;
			this.SelectedExportErpAdapter = payload.ExportErpModule;
			string adapterName = "";

			if (payload.ImportModule != null)
			{
				this.SelectedAdapter = payload.ImportModule;
				adapterName = payload.ImportModule.Name;
			}
			else if (payload.ExportPdaModule != null)
			{
				this.SelectedAdapter = payload.ExportPdaModule;
				adapterName = payload.ExportPdaModule.Name;
			}
			else if (payload.ExportErpModule != null)
			{
				this.SelectedAdapter = payload.ExportErpModule;
				adapterName = payload.ExportErpModule.Name;
			}
			else if (payload.UpdateModule != null)
			{
				this.SelectedAdapter = payload.UpdateModule;
				adapterName = payload.UpdateModule.Name;
			}
			else if (payload.ResultModule != null)
			{
				this.SelectedAdapter = payload.ResultModule;
				LoadReportIni(payload.ResultModule);
				return;
			}

			this.LoadXMLAdapterConfigFromCustomer(base.CurrentCustomer, adapterName);

		}


		public string GetConfigFolderPath(object currentDomainObject)
		{
			if (currentDomainObject == null) return String.Empty;
			//if (selected == null) return String.Empty;

			string dataInOutPath = base.ContextCBIRepository.GetConfigFolderPath(currentDomainObject);
			//string dataInOutPath = selected.GetObjectWorkingFolderPath(base.ContextCBIRepository, this._dbSettings, 
			//	currentDomainObject, @"\Config");
			return dataInOutPath;
		}
			
		
		//"ImportWithModulesBaseView"
		//"ExportPdaWithModulesView"
		//"ExportErpWithModulesView"
		//protected string GetImportOrExportFolderPath(IBaseImportExportModuleInfo selected, object currentDomainObject)
		//{
		//	if (currentDomainObject == null) return "";

		//	if (selected is IImportModuleInfo)
		//	{
		//		//IImportModuleInfo importModuleInfo = selectedAdapter as IImportModuleInfo;
		//		return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);
		//	}
		//	else if (selected is IExportErpModuleInfo)
		//	{
		//		//IExportErpModuleInfo exportErpModuleInfo = selectedAdapter as IExportErpModuleInfo;
		//		return GetExportErpFolderPath();
		//	}
		//	else if (selected is IExportPdaModuleInfo)
		//	{
		//		//IExportPdaModuleInfo exportPdaModuleInfo = selectedAdapter as IExportPdaModuleInfo;
		//		return base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);
		//	}
		//	else if (selected is ResultModuleInfo)
		//	{
		//		ResultModuleInfo iniPath = selected as ResultModuleInfo;
		//		return iniPath.ConfigPath;
		//	}
		//	else return String.Empty;

		//}


		//Здась не тестировалось, но тестировалось в ConfigEditAndSaveViewModel
		//"ImportWithModulesBaseView"
		//"ExportPdaWithModulesView"
		//"ExportErpWithModulesView"
		//protected string GetRelativePath(IBaseImportExportModuleInfo selected, object currentDomainObject)
		//{
		//	if (currentDomainObject == null) return "";

		//	if (selected is IImportModuleInfo)
		//	{
		//		//IImportModuleInfo importModuleInfo = selectedAdapter as IImportModuleInfo;
		//		return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);
		//	}
		//	else if (selected is IExportErpModuleInfo)
		//	{
		//		//IExportErpModuleInfo exportErpModuleInfo = selectedAdapter as IExportErpModuleInfo;
		//		return GetExportErpFolderPath();
		//	}
		//	else if (selected is IExportPdaModuleInfo)
		//	{
		//		//IExportPdaModuleInfo exportPdaModuleInfo = selectedAdapter as IExportPdaModuleInfo;
		//		return base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);
		//	}
		//	else if (selected is ResultModuleInfo)
		//	{
		//		//TOTO 
		//		 ResultModuleInfo iniPath =  selected as ResultModuleInfo;
		//		 return iniPath.ConfigPath;
		//	}

		//	else return String.Empty;


		//}
		

		//public string GetExportErpFolderPath()
		//{
		//	//if (string.IsNullOrWhiteSpace(code) == true)
		//	//	return String.Empty;

		//	string currentObjectType = "";
		//	string code = "";
		//	//if (this._isDefaultAdapterFromCustomer == true)
		//	//{
		//		currentObjectType = "Customer";
		//		code = base.CurrentCustomer.Code;
		//	//}
		//	//else if (this._isDefaultAdapterFromBranch == true)
		//	//{
		//	//	currentObjectType = "Branch";
		//	//	code = base.CurrentBranch.Code;
		//	//}
		//	//else if (this._isDefaultAdapterFromInventor == true)
		//	//{
		//	//	currentObjectType = "Inventor";
		//	//	code = base.CurrentInventor.Code;
		//	//}


		//	return UtilsPath.ExportErpFolder(this._dbSettings, currentObjectType, code);
		//}

		public void SetIsEditable(bool isEditable)
		{
			this.IsEditable = isEditable;

	    }

		public void SetCustomer(Customer customer)
		{
			this._customer = customer;

			//if (!String.IsNullOrEmpty(customer.UpdateCatalogAdapterCode) && _itemsCatalog.Any(r => r.Name == customer.UpdateCatalogAdapterCode))
			//	SelectedCatalog = _itemsCatalog.FirstOrDefault(r => r.Name == customer.UpdateCatalogAdapterCode);
			//else
			//	SelectedCatalog = _itemsCatalog.FirstOrDefault(r => r.IsDefault);
		}

		/////////
		public string ConfigXML
		{
			get { return _configXML; }
			set
			{
				_configXML = value;
				RaisePropertyChanged(() => ConfigXML);
			}
		}

		public XDocument ConfigXDocument
		{
			get { return _configXDocument; }
			set
			{
				_configXDocument = value;
				if (_configXDocument != null)
				{
					_configXML = _configXDocument.ToString();
				}
				else
				{
					_configXML = String.Empty;
				}
				RaisePropertyChanged(() => ConfigXDocument);
				RaisePropertyChanged(() => ConfigXML);

			}
		}

		public string DataInConfigPath
		{
			get { return this._dataInConfigPath; }
			set
			{
				this._dataInConfigPath = value;
				this.RaisePropertyChanged(() => this.DataInConfigPath);
				OpenConfigCommand.RaiseCanExecuteChanged();
				//ReloadConfigCommand.RaiseCanExecuteChanged();
			}
		}

		public string DataInConfigPathWithFile
		{
			get { return this._dataInConfigPathWithFile; }
			set
			{
				this._dataInConfigPathWithFile = value;
				this.RaisePropertyChanged(() => this.DataInConfigPathWithFile);
				//OpenConfigCommand.RaiseCanExecuteChanged();
				ReloadConfigCommand.RaiseCanExecuteChanged();
			}
		}
		

		//public DelegateCommand ImportByConfigCommand
		//{
		//	get { return _importByConfigCommand; }
		//}

		//private void ImportByConfigCommandExecuted()
		//{
		//	if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;

		//}

		//private bool ImportByConfigCommandCanExecute()
		//{
		//	if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return false;

		//	if (File.Exists(this.DataInConfigPath + @"\Config.xml") == true) return true;

		//	return false;
		//}

		public DelegateCommand OpenConfigCommand
		{
			get { return _openConfigCommand; }
		}

		public DelegateCommand ReloadConfigCommand
		{
			get { return _reloadConfigCommand; }
		}

		/// <summary>
		/// OpenConfigCommand
		/// </summary>
		private void OpenConfigCommandExecuted()
		{
			if (!Directory.Exists(_dataInConfigPath)) return;

			Utils.OpenFolderInExplorer(_dataInConfigPath);
		}

		private bool OpenConfigCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return false;

			return Directory.Exists(this.DataInConfigPath);
		}


		//public DelegateCommand SaveConfigCommand
		//{
		//	get { return _saveConfigCommand; }
		//}

		//public string SaveConfigContent
		//{
		//	//get { return Resources.View_InventorStatusChange_btnSaveConfigAsDefault + " for " + SelectedAdapter.Title; }
		//	get { return "SaveConfigAsDefault"; }
		//}


		/// <summary>
		/// SaveConfigCommand
		/// </summary>
		//private void SaveConfigCommandExecuted()
		//{

		//	string adapterDefaultParamFolderPath = _dbSettings.AdapterDefaultParamFolderPath().TrimEnd(@"\".ToCharArray());
		//	string adapterName = "";
		//	if (this.SelectedAdapter != null)
		//	{
		//		adapterName = this.SelectedAdapter.Name;
		//	}
		//	string path = adapterDefaultParamFolderPath + @"\" + adapterName;
		//	string fileConfig = path + @"\Config.xml";

		//	if (Directory.Exists(path) == false)
		//	{
		//		Directory.CreateDirectory(path);
		//	}

		//	if (File.Exists(fileConfig) == true)	   //заменим файл config.xml
		//	{
		//		try
		//		{
		//			File.Delete(fileConfig);
		//		}
		//		catch { }
		//	}

		//	this.ConfigXDocument = XDocument.Parse(_configXML);
		//	this.ConfigXDocument = ViewModelConfigRepository.VerifyToAdapter(this.ConfigXDocument, this.SelectedAdapter);

		//	this.ConfigXDocument.Save(fileConfig);

		//}

		//private bool SaveConfigCommandCanExecute()
		//{
		//	return true;
		//}




		private void ReloadConfigCommandExecuted()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;
			if (Directory.Exists(this.DataInConfigPath) == false) return;
			if (File.Exists(this.DataInConfigPathWithFile) == false) return;

			if (this.SelectedAdapter is ResultModuleInfo) 
			{
				LoadReportIni(this.SelectedAdapter as ResultModuleInfo);
			}
			else
			{
				this.ConfigXDocument = XDocument.Load(this.DataInConfigPathWithFile); //, LoadOptions.PreserveWhitespace
			}
		
		}
	

		private bool ReloadConfigCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return false;
			if (Directory.Exists(this.DataInConfigPath) == false) return false;
			if (File.Exists(this.DataInConfigPathWithFile) == false) return false;

			return true;

		}

		public bool FromCustomer	 //сейчас только кастомер
		{
			get { return this._fromCustomer; }
			set
			{
				this._fromCustomer = value;

				this.RaisePropertyChanged(() => this.FromCustomer);

				if (value == true)
				{
					//this.FromBranch = false;
					//this.FromInventor = false;
					//this.FromAdapter = false;
					string adapterName = "";
					if (this.SelectedAdapter != null) adapterName = this.SelectedAdapter.Name;
					this.LoadXMLAdapterConfigFromCustomer(this._customer, adapterName);

				}
			}
		}



		public void LoadXMLAdapterConfigFromCustomer(Customer customer, string adapterName)
		{
			if (customer == null) return;
			if (string.IsNullOrWhiteSpace(adapterName) == true) return;
			string fileConfig = "";

			this.DataInConfigPath = this.GetConfigFolderPath(customer);//, adapterName);
			string adapterConfigFileName = @"\"  + adapterName + ".config";
			this.DataInConfigPathWithFile = this.DataInConfigPath + adapterConfigFileName; //@"\Config.xml";

			this._configXDocument =null;
			if (File.Exists(this.DataInConfigPathWithFile) == true)
			{  //если есть сохраненный файла config.xml
			try
				{
					this.ConfigXDocument = XDocument.Load(this.DataInConfigPathWithFile);
				}
				catch (Exception exp)
				{
					this.ConfigXML = "Error Load Xml form file : " + fileConfig + " :  " + exp.Message;
				}
			}
			else
			{	//config not exist
				this.ConfigXDocument = null;
			}
	
			OpenConfigCommand.RaiseCanExecuteChanged();
		}


		public void LoadReportIni(ResultModuleInfo selected)
		{
			string fileConfig = "";
			object currentDomainObject = null;
			if (this.FromCustomer == true)
			{
				currentDomainObject = base.CurrentCustomer;
			}

			this.DataInConfigPath = selected.ConfigPath;
			this.DataInConfigPathWithFile = selected.ConfigPathWithFile;

			if (File.Exists(selected.ConfigPathWithFile) == true)
			{
				try
				{
					using (StreamReader sr = new StreamReader(selected.ConfigPathWithFile))
					{
						this.ConfigXML = sr.ReadToEnd();
					}
				}
				catch (Exception exp)
				{
					this.ConfigXML = "Error Load Xml form file : " + fileConfig + " :  " + exp.Message;
				}
			}
			else
			{
				this.ConfigXML = "";
			}

			OpenConfigCommand.RaiseCanExecuteChanged();
		}

	}


}