using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Xml.Linq;
using Count4U.Common.Enums;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.Import;
using Count4U.Modules.ContextCBI.Xml.Config;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc
{
	public class ConfigEditAndSaveViewModel : CBIContextBaseViewModel, IChildWindowViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationRepository _navigationRepository;
		private readonly DBSettings _dbSettings;
		

        private readonly DelegateCommand _closeCommand;
        private readonly DelegateCommand _openCommand;
		 private readonly DelegateCommand _openConfigCommand;
		private readonly DelegateCommand _saveConfigCommand;
		private readonly DelegateCommand _reloadConfigCommand;
										   
		

		//private bool FromInDataPath
		//private string InDataPath
		//private readonly DelegateCommand	OpenInDataCommand

		//private bool 	FromAbsolutePath
		//private string AbsolutePath
		//private readonly DelegateCommand	OpenAbsoluteCommand

		//private bool 	FromFtpPath
		//private string FtpPath
		//private readonly DelegateCommand	OpenFtpCommand

	
		private bool _isDefaultAdapterFromCustomer;
		private bool _isDefaultAdapterFromBranch;
		private bool _isDefaultAdapterFromInventor;

		private bool _isFromInternalEnabled;

		private bool _fromInDataPath;
		private string _inDataPath = String.Empty;
		private string _inDataRelativePath = String.Empty;
		
 		private readonly DelegateCommand _openInDataCommand;
	
		private bool _fromAbsolutePath;
		private string _absolutePath = @"C:\temp";
		private string _absoluteRelativePath = String.Empty;
		private readonly DelegateCommand _openAbsoluteCommand;
		private readonly DelegateCommand _applyAbsoluteCommand;
		
	
		private bool _fromFtpPath;
		private string _ftpPath = String.Empty;
		private string _ftpRelativePath = String.Empty;
		private readonly DelegateCommand _openFtpCommand;

	  	private string _dataInConfigPath = String.Empty;
	
        private string _log;
		private string _configXML;
        private string _path;

		private XDocument _configXDocument;

		public ConfigEditAndSaveViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            INavigationRepository navigationRepository,
			DBSettings dbSettings
            )
            : base(contextCbiRepository)
        {
            _navigationRepository = navigationRepository;
			_dbSettings = dbSettings;
            _eventAggregator = eventAggregator;
            _closeCommand = new DelegateCommand(CloseCommandExecuted);
            _openCommand = new DelegateCommand(OpenCommandExecuted, OpenCommandCanExecute);
			_openInDataCommand = new DelegateCommand(OpenInDataCommandExecuted, OpenInDataCommandCanExecute);
			_openAbsoluteCommand = new DelegateCommand(OpenAbsoluteCommandExecuted, OpenAbsoluteCommandCanExecute);
			_applyAbsoluteCommand = new DelegateCommand(ApplyAbsoluteCommandExecuted, ApplyAbsoluteCommandCanExecute);
			_openFtpCommand = new DelegateCommand(OpenFtpCommandExecuted, OpenFtpCommandCanExecute);
			_openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			_saveConfigCommand = new DelegateCommand(SaveConfigCommandExecuted, SaveConfigCommandCanExecute);
			_reloadConfigCommand = new DelegateCommand(ReloadConfigCommandExecuted, ReloadConfigCommandCanExecute);

			_fromInDataPath = true;
			_fromAbsolutePath = false;
			_fromFtpPath = false;
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
				if (value != null)
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

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
            }
        }

		public string InDataPath
		{
			get { return _inDataPath; }
			set
			{
				_inDataPath = value;
				if (string.IsNullOrWhiteSpace(_inDataPath) == false)
				{
					if (Directory.Exists(_inDataPath) == false)
					{
						Directory.CreateDirectory(_inDataPath);
					}
				}
				RaisePropertyChanged(() => InDataPath);
				this._openInDataCommand.RaiseCanExecuteChanged();
			}
		}

		public string InDataRelativePath
		{
			get { return _inDataRelativePath; }
			set
			{
				_inDataPath = value;
				RaisePropertyChanged(() => InDataRelativePath);
			}
		}

		public string AdapterType { get; set; }
		public string AdapterName { get; set; }
		
	
		private void OpenInDataCommandExecuted()
		{
			if (!Directory.Exists(_inDataPath)) return;

			Utils.OpenFolderInExplorer(_inDataPath);
		}

		private bool OpenInDataCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(_inDataPath) == true) return false;
			return Directory.Exists(_inDataPath);
		}


		//protected IObservable<long> observCountingChecked;
		//protected IDisposable disposeObservCountingChecked;
		//public void CheckAbsolutePath(long x)
		//{
		//	if (_fromAbsolutePath == true)
		//	{
		//		RaisePropertyChanged(() => AbsolutePath);
		//	}
		//}

		public string AbsolutePath
		{
			get { return _absolutePath; }
			set
			{
				_absolutePath = value;
				//if (string.IsNullOrWhiteSpace(_absolutePath) == false)
				//{
				//	if (Directory.Exists(_absolutePath) == false)
				//	{
				//		Directory.CreateDirectory(_absolutePath);
				//	}
				//}
				this.ConfigXDocument = UpdatePath(this.ConfigXDocument);
				RaisePropertyChanged(() => AbsolutePath);
				//ApplyAbsoluteCommand
			}
		}

		public string AbsoluteRelativePath
		{
			get { return _absoluteRelativePath; }
			set
			{
				_absoluteRelativePath = value;
				RaisePropertyChanged(() => AbsoluteRelativePath);
			}
		}

		

		private void OpenAbsoluteCommandExecuted()
		{
			if (!Directory.Exists(_absolutePath)) return;

			Utils.OpenFolderInExplorer(_absolutePath);
		}

		private bool OpenAbsoluteCommandCanExecute()
		{
			return Directory.Exists(AbsolutePath);
		}

		private void ApplyAbsoluteCommandExecuted()
		{
			if (_fromAbsolutePath == false) return;
			if (string.IsNullOrWhiteSpace(AbsolutePath) == false)
			{
				if (Directory.Exists(AbsolutePath) == false)
				{
					Directory.CreateDirectory(AbsolutePath);
				}
			}

			if (!Directory.Exists(AbsolutePath)) return;

			this.ConfigXDocument = UpdatePath(this.ConfigXDocument);
		}

		private bool ApplyAbsoluteCommandCanExecute()
		{
			if (_fromAbsolutePath == false) return false;
			if (string.IsNullOrWhiteSpace(AbsolutePath) == true)   return false;
			return true;//IsPathValid(AbsolutePath);
		}

		private bool IsPathValid(string path)
		{
			if (String.IsNullOrEmpty(path))
				return false;

			FileInfo fi = new FileInfo(path);
			if (fi.DirectoryName != null)
				if (!Directory.Exists(fi.DirectoryName))
					return false;

			return true;
		}

		public string FtpPath
		{
			get { return _ftpPath; }
			set
			{
				_ftpPath = value;
				if (string.IsNullOrWhiteSpace(_ftpPath) == false)
				{
					if (Directory.Exists(_ftpPath) == false)
					{
						Directory.CreateDirectory(_ftpPath);
					}
				}
				RaisePropertyChanged(() => FtpPath);
			}
		}


		public string FtpRelativePath
		{
			get { return _ftpRelativePath; }
			set
			{
				_ftpRelativePath = value;
				RaisePropertyChanged(() => FtpRelativePath);
			}
		}

		public bool FromInDataPath
		{
			get { return this._fromInDataPath; }
			set
			{
				this._fromInDataPath = value;
				this.RaisePropertyChanged(() => this.FromInDataPath);
				if (value == true)
				{
					this._fromAbsolutePath = false;
					this._fromFtpPath = false;
					this.InDataPath = GetImportFolderPath(this.AdapterType);
					this.InDataRelativePath = GetRelativePath( this.AdapterType);
					//this.RaisePropertyChanged(() => this.InDataPath);
					//this.RaisePropertyChanged(() => this.InDataRelativePath);
					this.ConfigXDocument = UpdatePath(this.ConfigXDocument);

					this.Path = this.InDataPath;
					this.RaisePropertyChanged(() => this.Path);
					//this._setAsDefaultCommand.RaiseCanExecuteChanged();
				}
		
			}
		}


		public string DataInConfigPath
		{
			get { return this._dataInConfigPath; }
			set
			{
				this._dataInConfigPath = value;
				this.RaisePropertyChanged(() => this.DataInConfigPath);
			}
		}

		public bool FromFtpPath
		{
			get { return this._fromFtpPath; }
			set
			{
				this._fromFtpPath = value;
				this.RaisePropertyChanged(() => this.FromFtpPath);

				if (value == true)
				{
					this._fromInDataPath = false;
					this._fromAbsolutePath = false;
					this.InDataPath = GetImportFolderPath(this.AdapterType);
					this.InDataRelativePath = GetRelativePath(this.AdapterType);

					//this.RaisePropertyChanged(() => this.InDataPath);
					//this.RaisePropertyChanged(() => this.InDataRelativePath);
					this.ConfigXDocument = UpdatePath(this.ConfigXDocument);

					this.Path = this.FtpPath;
					this.RaisePropertyChanged(() => this.Path);


				}
			}
		}

		public bool FromAbsolutePath
		{
			get { return this._fromAbsolutePath; }
			set
			{
				this._fromAbsolutePath = value;
				this.RaisePropertyChanged(() => this.FromAbsolutePath);

				if (value == true)
				{
					this._fromInDataPath = false;
					this._fromFtpPath = false;
					this.InDataPath = GetImportFolderPath(this.AdapterType);
					this.InDataRelativePath = GetRelativePath( this.AdapterType);

					//this.RaisePropertyChanged(() => this.InDataPath);
					//this.RaisePropertyChanged(() => this.InDataRelativePath);
					this.ConfigXDocument = UpdatePath(this.ConfigXDocument);

					this.Path = this.AbsolutePath;
					this.RaisePropertyChanged(() => this.Path);
				}
				//this._setAsDefaultCommand.RaiseCanExecuteChanged();
			}
		}

		private void OpenFtpCommandExecuted()
		{
			if (!Directory.Exists(_ftpPath)) return;

			Utils.OpenFolderInExplorer(_ftpPath);
		}

		private bool OpenFtpCommandCanExecute()
		{
			return Directory.Exists(_ftpPath);
		}

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public DelegateCommand OpenCommand
        {
            get { return _openCommand; }
        }

		
		public DelegateCommand SaveConfigCommand
        {
            get { return _saveConfigCommand; }
        }

	
		public DelegateCommand ReloadConfigCommand
        {
			get { return _reloadConfigCommand; }
        }

		public DelegateCommand OpenConfigCommand
        {
			get { return _openConfigCommand; }
        }


		public DelegateCommand OpenInDataCommand
		{
			get { return _openInDataCommand; }
		}
		public DelegateCommand OpenAbsoluteCommand
		{
			get { return _openAbsoluteCommand; }
		}


		public DelegateCommand ApplyAbsoluteCommand
		{
			get { return _applyAbsoluteCommand; }
		}

		public DelegateCommand OpenFtpCommand
		{
			get { return _openFtpCommand; }
		}

		public bool IsDefaultAdapterFromInventor
		{
			get { return this._isDefaultAdapterFromInventor; }
			set
			{
				this._isDefaultAdapterFromInventor = value;
				this.RaisePropertyChanged(() => this.IsDefaultAdapterFromInventor);
				if (value == true)
				{
					this._isDefaultAdapterFromBranch = false;
					this._isDefaultAdapterFromCustomer = false;
					this.InDataPath = GetImportFolderPath(this.AdapterType);
					this.InDataRelativePath = GetRelativePath(this.AdapterType);
					
					//this.RaisePropertyChanged(() => this.InDataPath);
					//this.RaisePropertyChanged(() => this.InDataRelativePath);
					this.ConfigXDocument = UpdatePath(this.ConfigXDocument);
				}
				//if (this._isDefaultAdapterFromInventor)
				//{
				//	this.SetDefaultAdapterByCBI(Common.NavigationSettings.CBIDbContextInventor);
				//}
			}
		}

		

		public bool IsDefaultAdapterFromCustomer
		{
			get { return this._isDefaultAdapterFromCustomer; }
			set
			{
				this._isDefaultAdapterFromCustomer = value;
				this.RaisePropertyChanged(() => this.IsDefaultAdapterFromCustomer);
				if (value == true)
				{
					this._isDefaultAdapterFromInventor = false;
					this._isDefaultAdapterFromBranch = false;
					this.InDataPath = GetImportFolderPath(this.AdapterType);
					this.InDataRelativePath = GetRelativePath(this.AdapterType);

					//this.RaisePropertyChanged(() => this.InDataPath);
					//this.RaisePropertyChanged(() => this.InDataRelativePath);
					this.ConfigXDocument = UpdatePath(this.ConfigXDocument);
				}
				//if (this._isDefaultAdapterFromCustomer)
				//{
				//	this.SetDefaultAdapterByCBI(Common.NavigationSettings.CBIDbContextCustomer);
				//}
			}
		}

		public bool IsDefaultAdapterFromBranch
		{
			get { return this._isDefaultAdapterFromBranch; }
			set
			{
				this._isDefaultAdapterFromBranch = value;
				this.RaisePropertyChanged(() => this.IsDefaultAdapterFromBranch);
				if (value == true)
				{
					this._isDefaultAdapterFromInventor = false;
					this._isDefaultAdapterFromCustomer = false;
					this.InDataPath = GetImportFolderPath(this.AdapterType);
					this.InDataRelativePath = GetRelativePath(this.AdapterType);

					//this.RaisePropertyChanged(() => this.InDataPath);
					//this.RaisePropertyChanged(() => this.InDataRelativePath);
					this.ConfigXDocument = UpdatePath(this.ConfigXDocument);
				}
				//if (this._isDefaultAdapterFromBranch)
				//{
				//	this.SetDefaultAdapterByCBI(Common.NavigationSettings.CBIDbContextBranch);
				//}
			}
		}

	

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
					
            ExportLogViewData data = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, isRemove: true) as ExportLogViewData;
            if (data == null) return;

			//this._path = data.Path;
			this.InDataPath = data.InDataPath;
			this.DataInConfigPath = data.DataInConfigPath;
			this.AdapterType = data.AdapterType;

			this._absolutePath = @"";
		

			if (data.AdapterType == "ImportWithModulesBaseViewModel")
			{
					this._fromInDataPath = false;
					this._fromFtpPath = false;
					this._fromAbsolutePath = true;
					this._absolutePath = @"c:\temp";
					if (base.CurrentCustomer != null)
					{
						this._absolutePath = base.CurrentCustomer.ImportCatalogPath;
					}
			}
			this.AdapterName = data.AdapterName;

			// пока решено 
			//все Config из customer, все данные из Inventor
			this._isDefaultAdapterFromInventor = true;
			//this._isDefaultAdapterFromCustomer = base.CBIDbContext == Common.NavigationSettings.CBIDbContextCustomer;
			//this._isDefaultAdapterFromBranch = base.CBIDbContext == Common.NavigationSettings.CBIDbContextBranch;
			//this._isDefaultAdapterFromInventor = base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor;
			//string currentDomainObject = "";
			//if (this._isDefaultAdapterFromCustomer == true) currentDomainObject = base.CurrentCustomer.Code;
			//else if (this._isDefaultAdapterFromBranch == true) currentDomainObject = base.CurrentBranch.Code;
			//else if (this._isDefaultAdapterFromInventor == true) currentDomainObject = base.CurrentInventor.Code;

			//if (string.IsNullOrWhiteSpace(data.InDataPath) == false)
			//{
			////	string configPath = data.InDataPath + @"\" + currentDomainObject + @"\Config";
			//	string configPath = data.InDataPath + @"\Config";
			//	if (Directory.Exists(configPath) == false)		 //нет - создаем
			//	{
			//		Directory.CreateDirectory(configPath);
			//	}
				
				//this.DataInConfigPath = configPath +  @"\" + this.AdapterName;
				//if (Directory.Exists(this.DataInConfigPath) == false)		 //нет - создаем
				//{
				//	Directory.CreateDirectory(this.DataInConfigPath);
				//}

			//}

			string adapterConfigFileName = @"\" + this.AdapterName + ".config";

			string fileConfig = this.DataInConfigPath + adapterConfigFileName;

			if (File.Exists(fileConfig) == false)	   //если нет сохраненного файла config.xml
			{
			this.ConfigXDocument = XDocument.Parse(data.ConfigXML);
			this.ConfigXDocument = UpdatePath(this.ConfigXDocument);
			this.ConfigXDocument.Save(fileConfig);
			}
			else		    //если нет сохраненных файл config.xml
			{
				this.ConfigXDocument = XDocument.Load(fileConfig);
				//configXMLstring = ConfigXDocument.ToString();
			}
			//configXMLstring = ConfigXDocument.ToString();

			//if (data.ConfigXML.Length > 500000)
			//	_configXML = configXMLstring.Substring(0, 500000);
			//else
			//	_configXML = configXMLstring;

			//observCountingChecked = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)).Select(x => x);
			//disposeObservCountingChecked = observCountingChecked.Subscribe(this.CheckAbsolutePath);
        }

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);
				//if (disposeObservCountingChecked != null) disposeObservCountingChecked.Dispose();
		}


		//"ImportWithModulesBaseView"
		//"ExportPdaWithModulesView"
		//"ExportErpWithModulesView"
		protected string GetImportFolderPath(string adapterType) // было GetImportPath()
		{
			object currentDomainObject = null;

			if (String.IsNullOrEmpty(this.CBIDbContext) == false)
			{
				//object currentDomainObject = base.GetCurrentDomainObject();

				if (this._isDefaultAdapterFromCustomer == true)
				{
					currentDomainObject = base.CurrentCustomer;
				}
				else if (this._isDefaultAdapterFromBranch == true)
				{
					currentDomainObject = base.CurrentBranch;
				}
				else if (this._isDefaultAdapterFromInventor == true)
				{
					currentDomainObject = base.CurrentInventor;
				}
			
				if (currentDomainObject != null)
				{
					if (adapterType == "ImportWithModulesBaseViewModel")
					{
						return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);
					}
					else if (adapterType == "ExportPdaWithModulesViewModel")
					{
						return base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);
					}
					else if (adapterType == "ExportErpWithModulesViewModel")
					{
						return GetExportErpFolderPath();
					}
					else return String.Empty;
				}
			}
			return String.Empty;
		}

		
			//"ImportWithModulesBaseView"
		//"ExportPdaWithModulesView"
		//"ExportErpWithModulesView"
		protected string GetRelativePath(string adapterType) // было GetImportPath()
		{
			object currentDomainObject = null;

			if (String.IsNullOrEmpty(this.CBIDbContext) == false)
			{
				//object currentDomainObject = base.GetCurrentDomainObject();

				if (this._isDefaultAdapterFromCustomer == true)
				{
					currentDomainObject = base.CurrentCustomer;
				}
				else if (this._isDefaultAdapterFromBranch == true)
				{
					currentDomainObject = base.CurrentBranch;
				}
				else if (this._isDefaultAdapterFromInventor == true)
				{
					currentDomainObject = base.CurrentInventor;
				}
			
				if (currentDomainObject != null)
				{
					if (adapterType == "ImportWithModulesBaseViewModel")
					{
						return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);
					}
					else if (adapterType == "ExportPdaWithModulesViewModel")
					{
						return base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);
					}
					else if (adapterType == "ExportErpWithModulesViewModel")
					{
						return GetExportErpFolderPath();
					}
					else return String.Empty;
				}
			}
			return String.Empty;
		}

		protected XDocument UpdatePath(XDocument doc)
		{
			if (doc == null) return new XDocument();
			doc = ViewModelConfigRepository.RemoveAllElement(doc, "FROMPATH");

			DomainObjectType domainObjectType = DomainObjectType.inventor;
			if (this._isDefaultAdapterFromCustomer == true)
			{
				domainObjectType = DomainObjectType.customer;
			}
			else if (this._isDefaultAdapterFromBranch == true)
			{
				domainObjectType = DomainObjectType.branch;
			}
			else if (this._isDefaultAdapterFromInventor == true)
			{
				domainObjectType = DomainObjectType.inventor;
			}
			HowUse howUse = HowUse.relative;

			if (FromInDataPath == true)
			{
				howUse =  HowUse.relative;
			}
			else if (FromAbsolutePath == true)
			{
				howUse =  HowUse.asis;
			}
			else if (FromFtpPath == true)
			{
				howUse = HowUse.ftp;
			}

			string absolutePath  = this.AbsolutePath;
			//XDocument doc, string adapterType, string fromDomainObjectType, HowUse pathHowUse,		string absolutePath = @"c:\temp", 
			doc = ViewModelConfigRepository.UpdatePath(doc, this.AdapterType, domainObjectType, howUse, absolutePath, true);

			return doc;
		}


		//public enum DomainObject
		//{
		//	customer,
		//	branch,
		//	inventor
		//}

		//public enum AdapterType
		//{
		//	import,
		//	exportpda,
		//	exporterp
		//}

		//public enum From
		//{
		//	indata,
		//	absolute , 
		//	ftp
		//}

		//public enum HowUse
		//{
		//	relative,
		//	copydb,
		//	asis

		//}
		//https://habr.com/post/24673/
		//protected XDocument UpdateASISPath(XDocument doc, string adapterType, bool _isDefault = false)
		//{

		//	XElement rootPath = new XElement("FROMPATH");

		//	if (this._isDefaultAdapterFromCustomer == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.customer.ToString()));
		//	}
		//	else if (this._isDefaultAdapterFromBranch == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.branch.ToString()));
		//	}
		//	else if (this._isDefaultAdapterFromInventor == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.inventor.ToString()));
		//	}

		//	if (adapterType == "ImportWithModulesBaseView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.import.ToString()));
		//	}
		//	else if (adapterType == "ExportPdaWithModulesView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.exportpda.ToString()));
		//	}
		//	else if (adapterType == "ExportErpWithModulesView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.exporterp.ToString()));
		//	}

		//	rootPath.Add(new XAttribute("howuse", HowUse.asis.ToString()));


		//	if (FromInDataPath == true)
		//	{
		//		rootPath.Add(new XAttribute("from", From.indata.ToString()));
		//		rootPath.Add(new XAttribute("value", InDataPath));
		//	}
		//	else if (FromAbsolutePath == true)
		//	{
		//		rootPath.Add(new XAttribute("from", From.absolute.ToString()));
		//		rootPath.Add(new XAttribute("value", AbsolutePath));
		//	}
		//	//else if (FromFtpPath == true)
		//	//{
		//	//	rootPath.Add(new XAttribute("from", From.ftp.ToString()));
		//	//	rootPath.Add(new XAttribute("value", FtpPath));
		//	//}

		//	string isdefault = "0";
		//	if (_isDefault == true) isdefault = "1";
		//	rootPath.Add(new XAttribute("isdefault", isdefault));

		//	doc.Root.AddFirst(rootPath);
		//	return doc;
		//}


		//XDocument doc, string adapterType, string fromDomainObjectType, HowUse pathHowUse,		string absolutePath = @"c:\temp", 
		//protected XDocument UpdateRelativePath(XDocument doc, string adapterType, bool _isDefault = false)
		//{
		//	XElement rootPath = new XElement("FROMPATH");
		//	if (this._isDefaultAdapterFromCustomer == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.customer.ToString()));
		//	}
		//	else if (this._isDefaultAdapterFromBranch == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.branch.ToString()));
		//	}
		//	else if (this._isDefaultAdapterFromInventor == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.inventor.ToString()));
		//	}


		//	if (adapterType == "ImportWithModulesBaseView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.import.ToString()));
		//	}
		//	else if (adapterType == "ExportPdaWithModulesView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.exportpda.ToString()));
		//	}
		//	else if (adapterType == "ExportErpWithModulesView")
		//	{
		//		rootPath.Add(new XAttribute("adapteruse", AdapterUse.exporterp.ToString()));
		//	}

			

		//	if (FromInDataPath == true)
		//	{
		//		rootPath.Add(new XAttribute("howuse", HowUse.relative.ToString()));
		//		rootPath.Add(new XAttribute("from", From.indata.ToString()));
		//		rootPath.Add(new XAttribute("value", ""));
		//	}
		//	else if (FromAbsolutePath == true)
		//	{
		//		rootPath.Add(new XAttribute("howuse", HowUse.asis.ToString()));
		//		rootPath.Add(new XAttribute("from", From.absolute.ToString()));
		//		rootPath.Add(new XAttribute("value", this.AbsolutePath));
		//	}
		//	else if (FromFtpPath == true)
		//	{
		//		rootPath.Add(new XAttribute("from", From.ftp.ToString()));
		//	}

			

		//	string isdefault = "0";
		//	if (_isDefault == true) isdefault = "1";
		//	rootPath.Add(new XAttribute("isdefault", isdefault));

		//	doc.Root.AddFirst(rootPath);
		//	return doc;
		//}

		//// db
		//protected XDocument UpdateDBPath(XDocument doc, bool _isDefault = false)
		//{
		//	XElement rootPath = new XElement("FROMPATH");
		//	if (this._isDefaultAdapterFromCustomer == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.customer.ToString()));
		//	}
		//	else if (this._isDefaultAdapterFromBranch == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.branch.ToString()));
		//	}
		//	else if (this._isDefaultAdapterFromInventor == true)
		//	{
		//		rootPath.Add(new XAttribute("domainobject", DomainObject.inventor.ToString()));
		//	}

		//	rootPath.Add(new XAttribute("adapteruse", AdapterUse.import.ToString()));
		//	rootPath.Add(new XAttribute("howuse", HowUse.copydb.ToString()));
		//	rootPath.Add(new XAttribute("from", "db"));

		//	rootPath.Add(new XAttribute("value", ""));

		//	string isdefault = "0";
		//	if (_isDefault == true) isdefault = "1";
		//	rootPath.Add(new XAttribute("isdefault", isdefault));

		//	doc.Root.AddFirst(rootPath);
		//	return doc;
		//}

		//https://habr.com/post/24673/
		//protected XDocument RemoveAllElement(XDocument doc, string tag)
		//{
		//	IEnumerable<XElement> tracks = doc.Descendants(tag);
		//	//IEnumerable<XElement> tracks = doc.Root.Descendants(tag).Where(
		//	//	t => t.Element("artist").Value == "DMX").ToList();
		//	tracks.Remove();
		//	return doc;
		//}

		public string GetExportErpFolderPath() 
		{
			//if (string.IsNullOrWhiteSpace(code) == true)
			//	return String.Empty;

			string currentObjectType = "";
			string code = "";
			if (this._isDefaultAdapterFromCustomer == true)
			{
				currentObjectType = "Customer";
				code = base.CurrentCustomer.Code;
			}
			else if (this._isDefaultAdapterFromBranch == true)
			{
				currentObjectType = "Branch";
				code = base.CurrentBranch.Code;
			}
			else if (this._isDefaultAdapterFromInventor == true)
			{
				currentObjectType = "Inventor";
				code = base.CurrentInventor.Code;
			}


			return UtilsPath.ExportErpFolder(this._dbSettings, currentObjectType, code);
		}

        private void CloseCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

		//private void OkCommandExecuted()
		//{
		//	MaskSelectItemViewModel selected = this._items.FirstOrDefault(r => r.IsChecked);
		//	if (selected != null)
		//	{
		//		ResultData = new MaskSelectedData() { Value = selected.IsEdit ? selected.MaskEditTemplate : selected.MaskTemplate };
		//	}

		//	this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
		//}
		//, IChildWindowViewModel
		public object ResultData { get; set; }


        private void OpenCommandExecuted()
        {
            if (!Directory.Exists(_path)) return;

            Utils.OpenFolderInExplorer(_path);
        }

        private bool OpenCommandCanExecute()
        {
            return Directory.Exists(_path);
        }
	
		/// <summary>
		///ReloadConfigCommand 
		/// </summary>
		private void ReloadConfigCommandExecuted()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;
			if (Directory.Exists(this.DataInConfigPath) == false) return;

			string adapterConfigFileName = @"\" + this.AdapterName + ".config";
			string fileConfig = this.DataInConfigPath + adapterConfigFileName;
			if (File.Exists(fileConfig) == false) return;


			this.ConfigXDocument = XDocument.Load(fileConfig); //, LoadOptions.PreserveWhitespace
			//string configXMLstring = ConfigXDocument.ToString();

			//if (configXMLstring.Length > 500000)
			//{
			//	this.ConfigXML = configXMLstring.Substring(0, 500000);
			//}
			//else
			//{
			//	this.ConfigXML = configXMLstring;
			//}
			
		}

		private bool ReloadConfigCommandCanExecute()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return false;
			if (Directory.Exists(this.DataInConfigPath) == false) return false;
			string adapterConfigFileName = @"\" + this.AdapterName + ".config";
			string fileConfig = this.DataInConfigPath + adapterConfigFileName;
			if (File.Exists(fileConfig) == false) return false;

			return true;
	
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

	
		/// <summary>
		/// SaveConfigCommand
		/// </summary>
		private void SaveConfigCommandExecuted()
		{
			if (string.IsNullOrWhiteSpace(this.DataInConfigPath) == true) return;

			if (Directory.Exists(this.DataInConfigPath) == false)
			{
				Directory.CreateDirectory(this.DataInConfigPath);
			}


			string adapterConfigFileName = @"\" + this.AdapterName + ".config";
			string fileConfig = this.DataInConfigPath + adapterConfigFileName;

			if (File.Exists(fileConfig) == true)	   //заменим файл config.xml
			{
				try
				{
					File.Delete(fileConfig);
				}
				catch { }
			}
			this.ConfigXDocument = XDocument.Parse(_configXML);
			this.ConfigXDocument.Save(fileConfig);

		}

		private bool SaveConfigCommandCanExecute()
		{
			return Directory.Exists(_dataInConfigPath);
		}
    }
}