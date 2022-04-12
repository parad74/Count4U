using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Enums;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common.ViewModel.Misc;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Xml.Config;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.Audit.ViewModels.Import
{
    public abstract class ImportWithModulesBaseViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        protected readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;
        private readonly INavigationRepository _navigationRepository;
        protected readonly UICommandRepository _commandRepository;
		protected readonly DBSettings _dbSettings;

        private readonly DelegateCommand _logCommand;
		private readonly DelegateCommand _configCommand;
        private DelegateCommand _importCommand;
        private DelegateCommand _clearCommand;
		private DelegateCommand _navigateToGridCommand;

        private ObservableCollection<IImportModuleInfo> _adapters;
        private IImportModuleInfo _selectedAdapter;
        private ImportModuleBaseViewModel _dynViewModel;

        private string _logText;
        private bool _isWriteLogToFile;

		public ImportDomainEnum _mode;

        protected bool _isBusy;
        private string _progress;
        protected string _progressText;
        private string _progressStep;
        private string _progressTime;
        private readonly DelegateCommand _busyCancelCommand;
        private CancellationTokenSource _cts;
        private readonly DispatcherTimer _timer;
        private DateTime _timerStart;
        protected bool _isCancelOk;

        protected ImportWithModulesBaseViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IUnityContainer container,
            INavigationRepository navigationRepository,
            UICommandRepository commandRepository ,
			DBSettings dbSettings
            )
            : base(contextCBIRepository)
        {

			this._dbSettings = dbSettings;
            this._commandRepository = commandRepository;
            this._navigationRepository = navigationRepository;
            this._container = container;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

            this._clearCommand = _commandRepository.Build(enUICommand.Clear, this.ClearCommandExecuted, this.ClearCommandCanExecute);
			this._navigateToGridCommand = _commandRepository.Build(enUICommand.FromImportToGrid, NavigateToGridCommandExecuted, this.NavigateToGridCommandCanExecute);//??
            this._busyCancelCommand = new DelegateCommand(BusyCancelCommandExecuted, BusyCancelCommandCanExecute);

            this._logCommand = _commandRepository.Build(enUICommand.Log, LogCommandExecuted, LogCommandCanExecute);
			this._configCommand = _commandRepository.Build(enUICommand.Config, ConfigCommandExecuted, ConfigCommandCanExecute);

            this._timer = new DispatcherTimer();
            this._timer.Tick += ProgressTimer_Tick;

            this._isWriteLogToFile = true;
            this._isCancelOk = true;
        }

		public ImportDomainEnum Mode
		{
			get { return _mode; }
		}
	
        public ObservableCollection<IImportModuleInfo> Adapters
        {
            get { return this._adapters; }
            set { _adapters = value; }
        }

        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {
                if (_isBusy != value)
                {

                    this._isBusy = value;
                    this.RaisePropertyChanged(() => this.IsBusy);

                    this.ImportCommand.RaiseCanExecuteChanged();
                    this.ClearCommand.RaiseCanExecuteChanged();
					this.NavigateToGridCommand.RaiseCanExecuteChanged();

                    this.EventAggregator.GetEvent<ApplicationBusyEvent>().Publish(this._isBusy);

                    if (_isBusy)
                    {
                        _timer.Start();
                        _timerStart = DateTime.Now;
                    }
                    else
                    {
                        _timer.Stop();
                    }
                }
            }
        }

        public IImportModuleInfo SelectedAdapter
        {
            get { return this._selectedAdapter; }
            set
            {
                this._selectedAdapter = value;
                RaisePropertyChanged(() => SelectedAdapter);

                using (new CursorWait())
                {
                    if (this._dynViewModel != null)
                        this._dynViewModel.ClearRegions();

                    if (this._selectedAdapter != null)
                    {
                        UriQuery query = Utils.UriQueryFromNavigationContext(base.NavigationContext);

                        query.Add(Common.NavigationSettings.AdapterName, this._selectedAdapter.Name);

						query = Utils.AddFromContextToQuery(query, FromContext.SelectAdapterWithoutAction);

                        this.Container.RegisterType(typeof(object), this._selectedAdapter.UserControlType, Common.ViewNames.ImportByModuleView);
						this._regionManager.RequestNavigate(Common.RegionNames.ImportByModule, new Uri(Common.ViewNames.ImportByModuleView + query, UriKind.Relative));
			   
						IRegion region = this._regionManager.Regions[Common.RegionNames.ImportByModule];
						UserControl userControl = region.ActiveViews.FirstOrDefault() as UserControl;
                        if (userControl != null)
                        {
                            this._dynViewModel = userControl.DataContext as ImportModuleBaseViewModel;
                            if (this._dynViewModel != null)
                            {
                                this._dynViewModel.RaiseCanImport = () => this.ImportCommand.RaiseCanExecuteChanged();
                                this._dynViewModel.UpdateLog = r => Utils.RunOnUI(() =>
                                    {
                                        this._logText = r;
                                        this._logCommand.RaiseCanExecuteChanged();
										this._configCommand.RaiseCanExecuteChanged();
                                    });
                                this._dynViewModel.SetIsBusy = r => Utils.RunOnUI(() =>
                                    {
                                        this.IsBusy = r;
                                    });
                                this._dynViewModel.UpdateProgress = r => Utils.RunOnUI(() =>
                                    {
                                        this.Progress = r.ToString();
                                        this.UpdateProgressStep(this._dynViewModel.StepCurrent, this._dynViewModel.StepTotal, this._dynViewModel.Session);
                                    });
                                this._dynViewModel.UpdateStep = () => Utils.RunOnUI(() => this.UpdateProgressStep(this._dynViewModel.StepCurrent, this._dynViewModel.StepTotal, this._dynViewModel.Session));
                            }

                            this._importCommand.RaiseCanExecuteChanged();
                            OnSelectedAdapterChanged();
                        }
                    }
                }
				CompareSelectedAdapterInventorAndCustomer();
            }
        }

		public virtual void CompareSelectedAdapterInventorAndCustomer()	 {	}


		

		public void SetSelectedAdapterWithoutGUI(string comment/*IImportModuleInfo info, ImportModuleBaseViewModel viewModel*/)
		{
			//viewModel.SetIsBusy = r => Utils.RunOnUI(() =>
			//	{
			//		this.IsBusy = r;
			//	});
			//	viewModel.UpdateProgress = r => Utils.RunOnUI(() =>
			//	{
			//		this.Progress = r.ToString();
			//		this.UpdateProgressStep(viewModel.StepCurrent, viewModel.StepTotal, viewModel.Session);
			//	});
			//	viewModel.UpdateStep = () => Utils.RunOnUI(() => this.UpdateProgressStep(viewModel.StepCurrent, viewModel.StepTotal, viewModel.Session));

			OnSelectedAdapterChanged();
		}


        public DelegateCommand LogCommand
        {
            get { return _logCommand; }
        }

		public DelegateCommand ConfigCommand
		{
			get { return _configCommand; }
		}

        public DelegateCommand BusyCancelCommand
        {
            get { return _busyCancelCommand; }
        }

        public string Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged(() => Progress);
            }
        }

        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                _progressText = value;
                RaisePropertyChanged(() => ProgressText);
            }
        }

        public string ProgressTime
        {
            get { return _progressTime; }
            set
            {
                _progressTime = value;
                RaisePropertyChanged(() => ProgressTime);
            }
        }

        public string ProgressStep
        {
            get { return _progressStep; }
            set
            {
                _progressStep = value;
                RaisePropertyChanged(() => ProgressStep);
            }
        }

        protected CancellationTokenSource Cts
        {
            get { return _cts; }
            set { _cts = value; }
        }

        protected string LogText
        {
            get { return _logText; }
            set { _logText = value; }
        }

        public DelegateCommand ImportCommand
        {
            get { return _importCommand; }
            set { _importCommand = value; }
        }

        public DelegateCommand ClearCommand
        {
            get { return _clearCommand; }
            set { _clearCommand = value; }
        }

		public DelegateCommand NavigateToGridCommand
        {
			get { return _navigateToGridCommand; }
			set { _navigateToGridCommand = value; }
        }

		

        protected IUnityContainer Container
        {
            get { return _container; }
        }

        protected ImportModuleBaseViewModel DynViewModel
        {
            get { return _dynViewModel; }
            set { _dynViewModel = value; }
        }

        protected IEventAggregator EventAggregator
        {
            get { return _eventAggregator; }
        }

        public bool IsWriteLogToFile
        {
            get { return _isWriteLogToFile; }
            set { _isWriteLogToFile = value; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            if (this.DynViewModel != null)
            {
                this.DynViewModel.ClearRegions();
                this.DynViewModel.OnNavigatedFrom(navigationContext);
            }
        }

        protected virtual void OnSelectedAdapterChanged()
        {

        }

        private bool ClearCommandCanExecute()
        {
			if (SelectedAdapter == null) return false;
            return !this._isBusy;
        }

		public bool NavigateToGridCommandCanExecute()
		{
			return !this._isBusy;
		}

	

        private void ClearCommandExecuted()
        {
            if (this.DynViewModel != null)
            {               
                ImportClearCommandInfo info = new ImportClearCommandInfo();
                info.Callback = () =>
                                    {
                                        Utils.SetCursor(false);
                                        ShowLog();
                                    };

                MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Msg_Clear_Import, 
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    this._container.Resolve<IUserSettingsManager>());

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Utils.SetCursor(true);
                    this.DynViewModel.RunClear(info);
                }              
            }
        }


        private bool LogCommandCanExecute()
        {
            return !String.IsNullOrEmpty(_logText);
        }


		private bool ConfigCommandCanExecute()
        {
			if (this.CurrentInventor == null) return false;
			return true;// !String.IsNullOrEmpty(_logText);
        }
		

        private void LogCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
			payload.ViewName = Common.ViewNames.LogView;
            payload.WindowTitle = WindowTitles.ViewImportLog;
			
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            UtilsConvert.AddObjectToDictionary(payload.Settings, _navigationRepository, _logText);
            OnModalWindowRequest(payload);
        }


		public void SaveConfigByDefaultForCustomer(Customer customer,
				ImportModuleInfo selectedAdapter,
				ImportModuleBaseViewModel importModuleBaseViewModel,
				DomainObjectType fromDomainObjectType, HowUse pathHowUse,
				CBIState state, bool resave = false)
		{
			if (customer == null) return;
			if (selectedAdapter == null) return;
			XDocument doc = new XDocument();
			string xmlString = "";

			string dataContextViewModelName = importModuleBaseViewModel.GetType().Name;
			ImportModuleInfo info = new ImportModuleInfo();
			importModuleBaseViewModel.InitDefault(state);

			XElement root = ViewModelConfigRepository.GetXElementImportAdapterProperty(
				importModuleBaseViewModel,
					dataContextViewModelName, 
					selectedAdapter as ImportModuleInfo);
			doc.Add(root);
			xmlString = doc.ToString();

		
			// ============ Save file config ========================
			string getConfigFolderPath = this._contextCBIRepository.GetConfigFolderPath(customer);
			string adapterName = "";
			if (selectedAdapter != null)
			{
				adapterName = selectedAdapter.Name;
			}
			if (String.IsNullOrWhiteSpace(adapterName) == true) return;
			string customerConfigPath = this._contextCBIRepository.GetConfigFolderPath(customer);
			string adapterConfigFileName = @"\" + adapterName + ".config";
			string fileConfig = customerConfigPath + adapterConfigFileName;
			if (Directory.Exists(customerConfigPath) == false)	 {Directory.CreateDirectory(customerConfigPath);	}

			if (resave == true)
			{
				if (File.Exists(fileConfig) == true)	   //заменим файл config.xml
				{
					try	  {	File.Delete(fileConfig);	}	catch { }
				}
			}


			string absolutePath =  @"C:\temp" ;
			if (state != null)
			{
				if (state.CurrentCustomer != null)
				{
					if (string.IsNullOrWhiteSpace(state.CurrentCustomer.ImportCatalogPath) == false)
					{
						try
						{
							if (Path.IsPathRooted(state.CurrentCustomer.ImportCatalogPath) == true)
							{
								absolutePath = state.CurrentCustomer.ImportCatalogPath;
							}
						}
						catch { }
					}
				}
			}

			if (File.Exists(fileConfig) == false)	   //файла нет  config.xml
			{
				XDocument configXDocument = XDocument.Parse(xmlString);
				configXDocument = ViewModelConfigRepository.VerifyToAdapter(configXDocument, selectedAdapter);
				//		public static XDocument UpdatePath(XDocument doc, string adapterType, string fromDomainObjectType, HowUse pathHowUse,
				//string absolutePath = @"c:\temp", bool _isDefault = true)
				
				configXDocument = ViewModelConfigRepository.UpdatePath(configXDocument,
					"ImportWithModulesBaseViewModel", fromDomainObjectType, pathHowUse,/*DomainObjectType.inventor, HowUse.relative,*/ absolutePath);
				configXDocument.Save(fileConfig);
			}
			else //file there is
			{
				XDocument configXDocument = XDocument.Load(fileConfig);
				if (configXDocument != null)
				{
					configXDocument = ViewModelConfigRepository.UpdatePath(configXDocument,
					"ImportWithModulesBaseViewModel", fromDomainObjectType, pathHowUse,/*DomainObjectType.inventor, HowUse.relative,*/ absolutePath);
					configXDocument.Save(fileConfig);
				}
			}
		}

		private void ConfigCommandExecuted()
        {
			ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
			payload.Settings = new Dictionary<string, string>();
			payload.ViewName = ViewNames.ConfigEditAndSaveView;
			payload.WindowTitle = WindowTitles.ViewConfig;
			payload.PathInData	= this._dbSettings.ImportFolderPath();
			Utils.AddContextToDictionary(payload.Settings, base.Context);
			Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);


			XDocument doc = new XDocument();		// this.DynViewModel,
			string xmlString = "";
			if (SelectedAdapter != null)
			{
				string dataContextViewModelName = this.DynViewModel.GetType().Name;
				XElement root = ViewModelConfigRepository.GetXElementImportAdapterProperty(this.DynViewModel,
					dataContextViewModelName, SelectedAdapter as ImportModuleInfo);
				doc.Add(root);
				xmlString =doc.ToString();
			}

			string _configXML = "";
			_configXML = _configXML + Environment.NewLine + xmlString;

			ExportLogViewData data = new ExportLogViewData();
			data.ConfigXML = _configXML;
			//if (this.DynViewModel != null)
			//data.Path = @"C:\MIS";//this.DynViewModel.BuildPathToExportErpDataFolder();
			data.InDataPath = GetImportFolderPath();
			data.DataInConfigPath = this._contextCBIRepository.GetConfigFolderPath(base.State.CurrentCustomer);
			//data.DataInConfigPath = GetConfigFolderPath(base.State.CurrentCustomer);

			data.AdapterType = "ImportWithModulesBaseViewModel";
			if (this is ImportFromPdaViewModel) 
			{
				data.AdapterType = "ImportFromPdaViewModel";
			}
		
			data.AdapterName = "";
			if (SelectedAdapter != null)
			{
				data.AdapterName = SelectedAdapter.Name;
			}
		
			UtilsConvert.AddObjectToDictionary(payload.Settings, _navigationRepository, data); //_configXML
			OnModalWindowRequest(payload);
        }

		//public string GetConfigFolderPath(object currentDomainObject)
		//{
		//	if (currentDomainObject != null)
		//	{

		//		string dataInPath = base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);

		//		string dataInConfigPath = dataInPath + @"\Config";

		//		if (string.IsNullOrWhiteSpace(dataInPath) == false)
		//		{
		//			if (Directory.Exists(dataInConfigPath) == false)
		//			{
		//				Directory.CreateDirectory(dataInConfigPath);
		//			}
		//		}
		//		return dataInConfigPath;
		//	}
		//	return String.Empty;
		//}

		protected string GetImportFolderPath() // было GetImportPath()
		{
			if (!String.IsNullOrEmpty(this.CBIDbContext))
			{
				object currentDomainObject = base.GetCurrentDomainObject();

				if (currentDomainObject != null)
				{
					return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);

				}
			}
			return String.Empty;
		}
		//private XElement GetXElementAdapterProperty(object viewModel)
		//{
		//	IImportModuleInfo selectedAdapter = SelectedAdapter;
		//	Type vm = selectedAdapter.UserControlType;
		//	//_configXML = _configXML + "<TEST>" + selectedAdapter.UserControlType.Name + "</TEST>" + Environment.NewLine;
		
		//	XElement root = new XElement("ROOT");

		//	XElement adapter = new XElement("INFO");
		//	adapter.Add(new XAttribute("Name", selectedAdapter.Name));
		//	adapter.Add(new XAttribute("Title", selectedAdapter.Title));
		//	adapter.Add(new XAttribute("UserControlType", selectedAdapter.UserControlType.Name));
		//	adapter.Add(new XAttribute("ImportDomainEnum", selectedAdapter.ImportDomainEnum.ToString()));
		//	adapter.Add(new XAttribute("Description", selectedAdapter.Description));
		//	root.Add(adapter);


		//	if (viewModel != null)
		//	{
		//		PropertyInfo[] props = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

		//		for (int i = 0; i < props.Length; i++) //responseData
		//		{
		//			if (props[i] == null) continue;
		//			//if (props[i].Name.ToLower() 
		//			//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
		//			try
		//			{
		//				string propTypeName = props[i].PropertyType.Name;
		//				string propName = props[i].Name;

		//				if (propTypeName.ToLower() == "boolean"
		//				|| propTypeName.ToLower() == "string"
		//				|| propTypeName.ToLower() == "int16"
		//				|| propTypeName.ToLower() == "int32"
		//				 || propTypeName.ToLower() == "int64")
		//				{
		//					if (propName.Contains("Execute") == true) continue;
		//					if (propName.Contains("Error") == true) continue;
		//					if (propName.Contains("Command") == true) continue;
		//					var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
		//					//bool notBulk = props[i].Attributes.OfType<NotInludeAttribute>().Any();
		//					if (attr != null) continue;

		//					var propValue = props[i].GetValue(this.DynViewModel, null);
		//					if (propValue != null)
		//					{
		//						XElement propAdapter = new XElement("PROPERTY");
		//						propAdapter.Add(new XAttribute("returntype", propTypeName));
		//						propAdapter.Add(new XAttribute("name", props[i].Name));
		//						propAdapter.Add(new XAttribute("value", propValue.ToString()));
		//						adapter.Add(propAdapter);

		//						//	string ret = propTypeName + " " + props[i].Name + " = " + propValue.ToString();
		//						//_configXML = _configXML + ret + Environment.NewLine;
		//					}

		//				}
		//			}
		//			catch (Exception ex)
		//			{
		//				//_configXML = _configXML + " Error : " + ex.Message + Environment.NewLine;
		//			}
		//		}
		//	}
		//	return root;
		//}


		private static void GetPropertyValues(Object obj)
		{
			Type t = obj.GetType();
			Console.WriteLine("Type is: {0}", t.Name);
			PropertyInfo[] props = t.GetProperties();
			Console.WriteLine("Properties (N = {0}):",
							  props.Length);
			foreach (var prop in props)
				if (prop.GetIndexParameters().Length == 0)
					Console.WriteLine("   {0} ({1}): {2}", prop.Name,
									  prop.PropertyType.Name,
									  prop.GetValue(obj,null));
				else
					Console.WriteLine("   {0} ({1}): <Indexed>", prop.Name,
									  prop.PropertyType.Name);

		}

		private void NavigateToGridCommandExecuted()
		{
			UriQuery query = new UriQuery();
			Utils.AddContextToQuery(query, base.Context);
			Utils.AddDbContextToQuery(query, base.CBIDbContext);
			Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());


			UtilsNavigate.InventProductListOpen(this._regionManager, query);
		}

        protected void ShowLog()
        {
            if (_logCommand.CanExecute())
                _logCommand.Execute();
        }

		protected void ShowConfig()
		{
			if (_configCommand.CanExecute())
				_configCommand.Execute();
		}

        private bool BusyCancelCommandCanExecute()
        {
            return _isCancelOk;
        }

        private void BusyCancelCommandExecuted()
        {
            if (Cts != null)
                Cts.Cancel();
            Progress = String.Empty;
            ProgressStep = String.Empty;
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - this._timerStart;
            ProgressTime = String.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
        }

        protected void UpdateProgressStep(int cur, int total, int session = 0)
        {
            string r = String.Empty;
            if (session == 0)
            {
                r = String.Format(Localization.Resources.ViewModel_ImportWithModulesBase_Step, cur, total);
            }
            else
            {
                r = String.Format(Localization.Resources.ViewModel_ImportWithModulesBase_StepSession, cur, total, session);
            }
            //System.Diagnostics.Debug.Print(r);
            this.ProgressStep = r;
        }
    }
}