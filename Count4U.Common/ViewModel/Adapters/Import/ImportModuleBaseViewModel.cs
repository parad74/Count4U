using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using NLog;
using Count4U.Model.Count4U.Validate;
using Count4U.Common.Extensions;
using Count4U.Model.Interface.Count4U;
using System.Threading;
using Count4U.Common.Helpers.Ftp;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4Mobile;
using System.Xml.Linq;
using Count4U.Model.Extensions;
using Count4U.Localization;

namespace Count4U.Common.ViewModel.Adapters.Import
{
    public abstract class ImportModuleBaseViewModel : ModuleBaseViewModel
    {
        #region private fields

        private const string FolderNameFolLogFile = "Log";

        protected readonly IEventAggregator _eventAggregator;
		protected readonly IRegionManager _regionManager;
        private readonly IIniFileParser _iniFileParser;
		private readonly ITemporaryInventoryRepository _temporaryInventoryRepository; 

        protected readonly IUserSettingsManager _userSettingsManager;

        private readonly List<string> _maskRegions;
		private string _adapterName;

		
		private Dictionary<string, string> _parmsDictionary;
		private string _importFolder;

		
        private int _stepTotal;
        private int _stepCurrent;
        private int _session;

        private Action _raiseCanImport;
        private Action _updateStep;

        private Encoding _encoding;
        private bool _isInvertLetters;
        private bool _isInvertWords;
        private bool _isTryFast;

        private Action<bool> _inputFileFolderChanged;

        #endregion

        protected ImportModuleBaseViewModel(IServiceLocator serviceLocator,
                                            IContextCBIRepository contextCBIRepository,
                                            IEventAggregator eventAggregator,
                                            IRegionManager regionManager,
                                            ILog logImport,
                                            IIniFileParser iniFileParser,
											ITemporaryInventoryRepository temporaryInventoryRepository ,
                                            IUserSettingsManager userSettingsManager)
            : base(contextCBIRepository, logImport, serviceLocator)
        {
            this._userSettingsManager = userSettingsManager;
            this._iniFileParser = iniFileParser;
			this._temporaryInventoryRepository = temporaryInventoryRepository;

            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

            this._maskRegions = new List<string>();
			this._parmsDictionary = new Dictionary<string, string>();
        }

        #region properties

		protected ITemporaryInventoryRepository TemporaryInventoryRepository
		{
			get { return _temporaryInventoryRepository; }
		}

		[NotInludeAttribute]
        public int StepTotal
        {
            get { return _stepTotal; }
            set
            {
                _stepTotal = value;
                if (_updateStep != null)
                    _updateStep();
            }
        }

		[NotInludeAttribute]
		public string AdapterName
		{
			get { return _adapterName; }
		}

		public Dictionary<string, string> ParmsDictionary
		{
			get { return _parmsDictionary; }
			set { _parmsDictionary = value; }
		}

		public void AddParamsInDictionary(string parameters)
		{
			if (string.IsNullOrWhiteSpace(parameters) == true) return;
			string[] parmArray = parameters.Split(';');   //ImportPath=C\:temp;ExportPath=C\:temp;
			foreach (var parm in parmArray)
			{
				string[] keyVal = parm.Split('=');
				if (keyVal.Length > 1)
				{
					this.ParmsDictionary[keyVal[0]] = keyVal[1];
				}
			}
		}



		[NotInludeAttribute]
        public int StepCurrent
        {
            get { return _stepCurrent; }
            set
            {
                _stepCurrent = value;
                if (_updateStep != null)
                    _updateStep();
            }
        }

		[NotInludeAttribute]
        public int Session
        {
            get { return _session; }
            set
            {
                _session = value;
                if (_updateStep != null)
                    _updateStep();
            }
        }

        public Action RaiseCanImport
        {
            set { this._raiseCanImport = value; }
            protected get { return this._raiseCanImport; }
        }

        public Action UpdateStep
        {
            set { _updateStep = value; }
        }

        public Action<bool> InputFileFolderChanged
        {
            protected get { return _inputFileFolderChanged; }
            set { _inputFileFolderChanged = value; }
        }

        #endregion

        #region Implementation of IAdapter

		public abstract void InitDefault(CBIState state = null);
        public abstract void InitFromIni();
		public abstract void Clear();
        public abstract void Import();
	//	public abstract void InitXDocumentConfig(XDocument configXDocument);
	

        public override Encoding Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                _encoding = value;

                RaisePropertyChanged(() => Encoding);

                this.EncondingUpdated();
            }
        }

		
        public bool IsInvertLetters
        {
            get { return _isInvertLetters; }
            set
            {
                this._isInvertLetters = value;

               // EncondingUpdated();
            }
        }

		
        public bool IsInvertWords
        {
            get { return _isInvertWords; }
            set
            {
                this._isInvertWords = value;

             //   EncondingUpdated();
            }
        }

		[NotInludeAttribute]
        public bool IsTryFast
        {
            get { return _isTryFast; }
        }

        #endregion

        public abstract bool CanImport();

		protected abstract void InitFromConfig(ImportCommandInfo info, CBIState state);  
		protected abstract void RunImport(ImportCommandInfo info);   //ImportCommandInfo info
        protected abstract void RunClear();

        protected abstract void EncondingUpdated();

		public void RunImportBase(ImportCommandInfo info)
        {
            base.IsSaveFileLog = info.IsWriteLogToFile;
            base.CancellationToken = info.CancellationToken;
            this._isInvertLetters = info.IsInvertLetters;
            this._isInvertWords = info.IsInvertWords;
            this._isTryFast = info.TryFast;

			IIturAnalyzesRepository iturAnalyzesRepository = base.ServiceLocator.GetInstance <IIturAnalyzesRepository>();
			iturAnalyzesRepository.ClearProductDictionary();

            this.ProcessImportInfo(info);

            bool preImportCheckResult = this.PreImportCheck();
            if (!preImportCheckResult)
                return;

            base.SetIsBusy(true);

            Task.Factory.StartNew(() =>
                                      {
                                          try
                                          {
                                              base.LogImport.Clear();
                                              DateTime timeStart = DateTime.Now;

											  RunImport(info);

                                              var fullTime = (DateTime.Now - timeStart);
                                              string textFullTime = String.Format("{0:00}:{1:00}:{2:00}", fullTime.Hours, fullTime.Minutes, fullTime.Seconds);
                                              base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format(Localization.Resources.ViewModel_ImportModuleBaseTotalTime, textFullTime));

                                              this.UpdateLogFromILog();
                                          }
                                          catch (Exception exc)
                                          {
                                              _logger.ErrorException("RunImport", exc);
											  base.LogImport.Add(MessageTypeEnum.Error, exc.Message ); 
                                          }

                                          Utils.RunOnUIAsync(() => base.SetIsBusy(false));

                                          if (info.Callback != null)
                                              Utils.RunOnUIAsync(info.Callback);
									  }).LogTaskFactoryExceptions("RunImport");
        }


		public void RunImportWithoutGUIBase(ImportCommandInfo info, CBIState state)
		{
			base.IsSaveFileLog = info.IsWriteLogToFile;
			base.CancellationToken = info.CancellationToken;
			this._isInvertLetters = info.IsInvertLetters;
			this._isInvertWords = info.IsInvertWords;
			this._adapterName = info.AdapterName;
			this._isTryFast = info.TryFast;

			IIturAnalyzesRepository iturAnalyzesRepository = base.ServiceLocator.GetInstance<IIturAnalyzesRepository>();
			iturAnalyzesRepository.ClearProductDictionary();

			this.ProcessImportInfo(info);

			//bool preImportCheckResult = this.PreImportCheck();
			//if (!preImportCheckResult)
			//	return;

			base.SetIsBusy(true);

			//Task.Factory.StartNew(() =>
			//{
				try
				{
					base.LogImport.Clear();
					DateTime timeStart = DateTime.Now;

					if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
					{
						Init1();
						info.FromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData; //пока фиксируем
						try { InitFromConfig(info, state); }
						catch (Exception exc) { _logger.ErrorException("InitFromConfig", exc); }

						//Init2();
					}

					//bool preImportCheckResult = this.PreImportCheck();
					//if (!preImportCheckResult)
					//	return;
					//if (CanImport() == false)
					//{
					//		base.LogImport.Add(MessageTypeEnum.EndTrace, Resources.Msg_DynViewModelCanNotImport);
					//		this.UpdateLogFromILog();
					//		Utils.RunOnUIAsync(() => base.SetIsBusy(false));

					//		if (info.Callback != null)
					//			Utils.RunOnUIAsync(info.Callback);
					//	return;
					//}

					RunImport(info);

					var fullTime = (DateTime.Now - timeStart);
					string textFullTime = String.Format("{0:00}:{1:00}:{2:00}", fullTime.Hours, fullTime.Minutes, fullTime.Seconds);
					base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format(Localization.Resources.ViewModel_ImportModuleBaseTotalTime, textFullTime));

					this.UpdateLogFromILog();
				}
				catch (Exception exc)
				{
					_logger.ErrorException("RunImport", exc);
					base.LogImport.Add(MessageTypeEnum.Error, exc.Message);
					this.UpdateLogFromILog();
				}

				//Utils.RunOnUI(() => base.SetIsBusy(false));
				base.SetIsBusy(false);

				if (info.Callback != null) 
					Utils.RunOnUI(info.Callback);
			//}).LogTaskFactoryExceptions("RunImport");
		}


		
		public void RunImportClearWithoutGUIBase(ImportCommandInfo info, CBIState state)	  
		{
			this._adapterName = info.AdapterName;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
				{
					info.FromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData; //пока фиксируем
					try { InitFromConfig(info, state); }
					catch (Exception exc) { _logger.ErrorException("InitFromConfig", exc); }

				}
			IIturAnalyzesRepository iturAnalyzesRepository = base.ServiceLocator.GetInstance<IIturAnalyzesRepository>();
			iturAnalyzesRepository.ClearProductDictionary();

            RunClear();

            if (info.Callback != null)
                Utils.RunOnUI(info.Callback);	
		}

        public void RunClear(ImportClearCommandInfo info)
        {
			IIturAnalyzesRepository iturAnalyzesRepository = base.ServiceLocator.GetInstance<IIturAnalyzesRepository>();
			iturAnalyzesRepository.ClearProductDictionary();

            RunClear();
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

            if (info.Callback != null)
                Utils.RunOnUI(info.Callback);
        }

        protected virtual bool PreImportCheck()
        {
            return true;
        }

        protected virtual void ProcessImportInfo(ImportCommandInfo info)
        {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AdapterName))
            {
                this._adapterName = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AdapterName).Value;
            }

			InitRegular();
        }

		private void InitRegular()
		{
			Init1();
			Init2();
		}

		private void Init1()
		{
			StepTotal = 1;
			StepCurrent = 1;
			Session = 0;

			try
			{
				this.InitDefault();
			}
			catch (Exception exc)
			{
				_logger.ErrorException("InitDefault", exc);
			}
		}

		private void Init2()
		{
			try
			{
				this.BaseInitFromIniFile();
				this.InitFromIni();
			}
			catch (Exception exc)
			{
				_logger.ErrorException("InitFromIni", exc);
			}
		}

	

        private void BaseInitFromIniFile()
        {
            try
            {
                Dictionary<ImportProviderParmEnum, string> iniData = this.GetIniData(String.Empty);

                base.InitEncodingFromIniData(iniData);

                if (iniData.ContainsKey(ImportProviderParmEnum.InvertLetters))
                {
                    if (iniData[ImportProviderParmEnum.InvertLetters] == "1") this._isInvertLetters = true;
                    if (iniData[ImportProviderParmEnum.InvertLetters] == "0") this._isInvertLetters = false;
                }


                if (iniData.ContainsKey(ImportProviderParmEnum.InvertWords))
                {
                    if (iniData[ImportProviderParmEnum.InvertWords] == "1") this._isInvertWords = true;
                    if (iniData[ImportProviderParmEnum.InvertWords] == "0") this._isInvertWords = false;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("InitFromIniFile", exc);
            }
        }

        public void UpdateLogFromILog()
        {
            //this.UpdateLog(base.LogImport.PrintLog(encoding: this._encoding)); //test
			this.UpdateLog(base.LogImport.PrintLog());
        }

        protected IImportProvider GetProviderInstance(string providerName)
        {
            IImportProvider provider = base.ServiceLocator.GetInstance<IImportProvider>(providerName);
            return provider;
        }

        protected IImportProvider GetProviderInstance(ImportProviderEnum providerType)
        {
            IImportProvider provider = base.ServiceLocator.GetInstance<IImportProvider>(providerType.ToString());
            return provider;
        }


		protected IImportModuleInfo GetImportModuleInfoInstance(string moduleInfoName)
        {
			IImportModuleInfo moduleInfo = base.ServiceLocator.GetInstance<IImportModuleInfo>(moduleInfoName);
			return moduleInfo;
        }

		protected TemporaryInventory GetTemporaryInventoryWithImportModuleInfo(string moduleInfoName, string domain, string fromFile, DateTime updateDateTime)
		{
			IImportModuleInfo moduleInfo = base.ServiceLocator.GetInstance<IImportModuleInfo>(moduleInfoName);
			TemporaryInventory temporaryInventory = new TemporaryInventory();
			string operation = moduleInfo.ImportDomainEnum.ToString().ToUpper();
			temporaryInventory.Operation = CutLength(operation, 49);
			string dateModified = updateDateTime.ToShortDateString() + " " + updateDateTime.ToShortTimeString();
			temporaryInventory.DateModified = CutLength(dateModified, 49);
			//temporaryInventory.Domain = "IMPORT ADAPTER";
			temporaryInventory.Domain = domain; ;
			temporaryInventory.NewKey = CutLength(moduleInfo.Name, 49);
			temporaryInventory.OldKey = CutLength(moduleInfo.Title, 49);
			temporaryInventory.DbFileName = CutLength(fromFile, 249);
			return temporaryInventory;
		}

		protected string CutLength(string stringValue, int MaxLength)
		{
			stringValue = stringValue.Trim();
			if (string.IsNullOrWhiteSpace(stringValue) == true) return "";
			if (stringValue.Length <= MaxLength) return stringValue;
			else return stringValue.Substring(0, MaxLength);
		}
        #region save log file

        protected void SaveFileLog(FileLogInfo info)
        {
			//FileLogInfo fileLogInfo2 = new FileLogInfo();
			//fileLogInfo2.File = this.Path1 + ".full.temp";
			//base.SaveFileLog(fileLogInfo2);

			// ======= 
            if (!IsSaveFileLog)
                return;

            string logText = LogImport.FileLog(this._encoding);

            try
            {
                if (!String.IsNullOrEmpty(info.Directory))
                {
					SaveDirectoryLog(info.Directory, logText, ".full.log.txt");
                }
                else
                {
                    if (!String.IsNullOrEmpty(info.File))
                    {
						string filename = info.File + ".full.temp";
                        SaveFileLog(filename, logText);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("SaveFileLog", exc);
            }

			//=============

			if (!String.IsNullOrEmpty(info.Directory))
			{
				string filenameDirectory = info.Directory;
				FileLogInfo fileLogInfo = new FileLogInfo();
				fileLogInfo.Directory = info.Directory;
				this.SaveFileOnlyPrintLog(fileLogInfo);
			}
			else
			{
				if (!String.IsNullOrEmpty(info.File))
				{
					ImportCatalogBulkRepository repository = this.ServiceLocator.GetInstance<ImportCatalogBulkRepository>();
					long countInCatalog = repository.CountRow("Product", base.GetDbPath);
					base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format("In Catalog imported {0} Products {1}", countInCatalog, Environment.NewLine));
					FileLogInfo fileLogInfo = new FileLogInfo();
					fileLogInfo.File = info.File;
					this.SaveFileOnlyPrintLog(fileLogInfo);
				}
			}

        }

		protected void SaveFileOnlyPrintLog(FileLogInfo info)
		{
			if (!IsSaveFileLog)
				return;

			string logText = LogImport.PrintLog();

			try
			{
				if (!String.IsNullOrEmpty(info.Directory))
				{
					SaveDirectoryLog(info.Directory, logText);
				}
				else
				{
					if (!String.IsNullOrEmpty(info.File))
					{
						SaveFileLog(info.File, logText);
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("SaveFileLog", exc);
			}
		}

        private void SaveDirectoryLog(string directory, string log, string extension = ".log.txt")
        {
            if (!Directory.Exists(directory))
            {
                _logger.Error("SaveDirectoryLog - directory is missing in file system: " + directory);
                return;
            }

            DirectoryInfo di = new DirectoryInfo(directory);

            string finalDirectory = Path.Combine(directory, FolderNameFolLogFile, AdapterName);
            if (!Directory.Exists(finalDirectory))
            {
                Directory.CreateDirectory(finalDirectory);
            }

			//string dt = DateTime.Now.ToString("_yyyyMMdd_hhmmss_");
			string fileName = String.Format("{0}{1}", di.Name, extension);

            string finalFilePath = Path.Combine(finalDirectory, fileName);

            File.WriteAllText(finalFilePath, log);
        }

		private void SaveFileLog(string file, string log)
		{
			//if (!File.Exists(file))
			//{
			//	_logger.Error("SaveFileLog - file is missing in file system: " + file);
			//	return;
			//}
			try
			{
				FileInfo fi = new FileInfo(file);

				string withoutExtension = fi.Name.Replace(fi.Extension, String.Empty);
				//string dt = DateTime.Now.ToString("_yyyyMMdd_hhmmss_");
				string finalFileName = String.Format("{0}.log.txt", withoutExtension);

				if (fi.Directory == null)
					return;

				string parentDir = fi.Directory.FullName;
				string finalDir = Path.Combine(parentDir, FolderNameFolLogFile, AdapterName);

				if (!Directory.Exists(finalDir))
					Directory.CreateDirectory(finalDir);

				string finalFilePath = Path.Combine(finalDir, finalFileName);


				try
				{
					File.WriteAllText(finalFilePath, log);
				}
				catch (Exception exc)
				{
					_logger.ErrorException("SaveFileLog - File.WriteAllText", exc);
				}
			}
			catch { return; }
		}

        #endregion

        protected AdapterMaskViewModel BuildMaskControl(string FileName, string regionName)
        {
            if (!this._maskRegions.Contains(regionName))
                this._maskRegions.Add(regionName);

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            query.Add(Common.NavigationSettings.MaskFileName, FileName);
            query.Add(Common.NavigationSettings.AdapterName, this._adapterName);

            this._regionManager.RequestNavigate(regionName, new Uri(Common.ViewNames.AdaptersMaskView + query, UriKind.Relative));

            IRegion region = this._regionManager.Regions[regionName];
            UserControl userControl = region.ActiveViews.FirstOrDefault() as UserControl;
            if (userControl != null)
            {
                return userControl.DataContext as AdapterMaskViewModel;
            }
            return null;
        }

        public void ClearRegions()
        {
            foreach (string maskRegion in this._maskRegions.ToList())
            {
                this._regionManager.Regions.Remove(maskRegion);
                this._maskRegions.Remove(maskRegion);
            }
        }

        protected string BuildTooltip(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
				IEnumerable<string> lines;
				Encoding encoding = this.Encoding ?? Encoding.GetEncoding(1255);
				string extension = Path.GetExtension(filePath);
				if (extension == ".xlsx")
				{
					IFileParser fileParser = ServiceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString());
					lines = fileParser.GetRecords(filePath, encoding, 0);
				}
				else
				{
					lines = File.ReadLines(filePath, encoding).ToList();
				}
				if (lines == null )
					return String.Empty;
                if (!lines.Any())
                    return String.Empty;
				int count = lines.Count();
				if (count == 0 ) 
					return String.Empty;
				if (count == 1 && lines.First() == null)
					return String.Empty;

                if (encoding == Encoding.GetEncoding(1255) || encoding == Encoding.GetEncoding(862))
                {	 //! TODO: привязать к парсеру, чтобы сначала строка разбиралась на части,
                    //? а потом к экспорт провайдеру чтобы собиралась
                    return lines.Take(10).Select(r => r.ReverseDosHebrew(false, false)).//(this.IsInvertLetters, this.IsInvertWords)).
                        Aggregate((total, current) => String.Format("{0}{1}{2}",
                        total, Environment.NewLine, current));
                }
                else
                {
                    return lines.Take(10).
                        Aggregate((total, current) => String.Format("{0}{1}{2}",
                        total, Environment.NewLine, current));
                }
            }
            return String.Empty;
        }

		[NotInludeAttribute]
		public string ImportFolder
		{
			get { return this.GetImportPath(); }
			//set { _importFolder = value; }
		}

		protected string GetImportPath()
		{
			string folder = GetImportFolderPath().Trim('\\');
			if (this.ParmsDictionary.ContainsKey("ImportPath") == true)
			{
				string importPath = this.ParmsDictionary["ImportPath"];
				if (string.IsNullOrWhiteSpace(importPath) == false)
				{
					folder = importPath.Trim('\\');
				}
			}
			return folder;
		}


	
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

		public string GetImportFolderPathFromConfig(string domainObjectType) // было GetImportPath()
		{
			object currentDomainObject = "";
			if (domainObjectType.ToLower() == "customer")
				{
					currentDomainObject = base.CurrentCustomer;
				}
			else if (domainObjectType.ToLower() == "branch")
			{
				currentDomainObject = base.CurrentBranch;
			}
			else if (domainObjectType.ToLower() == "inventor")
			{
				currentDomainObject = base.CurrentInventor;
			}

			if (currentDomainObject != null)
			{
				return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);

			}

			return String.Empty;
		}


		// <ObjectCode>\InData\Config\AdapterName.config
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


		public virtual string GetXDocumentConfigPath(ref ImportCommandInfo info)
		{
			string adapterName = info.AdapterName;
			string adapterConfigFileName = @"\" + adapterName + ".config";
			string fileConfigPath = String.Empty;
			
			switch (info.FromConfigXDoc)
			{
				case ConfigXDocFromEnum.InitWithoutConfig:
					info.ConfigXDocumentPath = String.Empty;
					fileConfigPath = String.Empty;
					break;
				case ConfigXDocFromEnum.FromConfigXDocument:
				case ConfigXDocFromEnum.FromDefaultAdapter:
				case ConfigXDocFromEnum.FromRootPath:
				case ConfigXDocFromEnum.FromRootFolderAndCustomer:
				case ConfigXDocFromEnum.FromRootFolderAndBranch:
				case ConfigXDocFromEnum.FromRootFolderAndInventor:
				case ConfigXDocFromEnum.FromFtpCustomer:
				case ConfigXDocFromEnum.FromFtpBranch:
				case ConfigXDocFromEnum.FromFtpInventor:
					info.ConfigXDocumentPath = String.Empty;
					fileConfigPath = String.Empty;
					break;

				case ConfigXDocFromEnum.FromCustomerInData:
					fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath(base.CurrentCustomer) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
					info.ConfigXDocumentPath = fileConfigPath;
					break;
				case ConfigXDocFromEnum.FromBranchInData:
					fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath(base.CurrentBranch) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
					info.ConfigXDocumentPath = fileConfigPath;
					break;
				case ConfigXDocFromEnum.FromInventorInData:
					fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath(base.CurrentInventor) + adapterConfigFileName;							 	// <ObjectCode>\InData\Config\<AdapterName>.Config
					info.ConfigXDocumentPath = fileConfigPath;
					break;
				//case ConfigXDocFromEnum.FromDomainObjectInData:
				//	object currentDomainObject = base.GetCurrentDomainObject();
				//	fileConfigPath = this.GetConfigFolderPath( currentDomainObject)  +  adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
				//	info.ConfigXDocumentPath = fileConfigPath;
				//	break;
				case ConfigXDocFromEnum.FromFullPath:
					fileConfigPath = info.ConfigXDocumentPath;
					break;
			}
			return fileConfigPath;
		}
		//OLD - может быть в будущем будем такое использовать
		//public virtual string GetXDocumentConfigPath(ref ImportCommandInfo info)
		//{
		//	string adapterName = info.AdapterName;
		//	string adapterConfigFileName = @"\" + adapterName + ".config";

		//	IDBSettings dbSettings = ServiceLocator.GetInstance<IDBSettings>();
		//	string adapterDefaultParamFolderPath = dbSettings.AdapterDefaultConfigFolderPath().TrimEnd(@"\".ToCharArray());
		//	string fileConfigPath = String.Empty;
		//	if (string.IsNullOrWhiteSpace(adapterName) == false)		   //by default будет браться с адаптера
		//	{
		//		fileConfigPath = adapterDefaultParamFolderPath + adapterConfigFileName; // @"\" + adapterName + @"\Config.xml";
		//	}

		//	switch (info.FromConfigXDoc)
		//	{
		//		case ConfigXDocFromEnum.InitWithoutConfig:
		//			info.ConfigXDocumentPath = String.Empty;
		//			fileConfigPath = String.Empty;
		//			break;
		//		case ConfigXDocFromEnum.FromConfigXDocument:
		//		case ConfigXDocFromEnum.FromDefaultAdapter:
		//		case ConfigXDocFromEnum.FromRootPath:
		//		case ConfigXDocFromEnum.FromRootFolderAndCustomer:
		//		case ConfigXDocFromEnum.FromRootFolderAndBranch:
		//		case ConfigXDocFromEnum.FromRootFolderAndInventor:
		//		case ConfigXDocFromEnum.FromFtpCustomer:
		//		case ConfigXDocFromEnum.FromFtpBranch:
		//		case ConfigXDocFromEnum.FromFtpInventor:
		//			break;

		//		case ConfigXDocFromEnum.FromCustomerInData:
		//			fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath(base.CurrentCustomer) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
		//			info.ConfigXDocumentPath = fileConfigPath;
		//			break;
		//		case ConfigXDocFromEnum.FromBranchInData:
		//			fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath(base.CurrentBranch) +  adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
		//			info.ConfigXDocumentPath = fileConfigPath;
		//			break;
		//		case ConfigXDocFromEnum.FromInventorInData:
		//			fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath( base.CurrentInventor)  +  adapterConfigFileName;							 	// <ObjectCode>\InData\Config\<AdapterName>.Config
		//			info.ConfigXDocumentPath = fileConfigPath;
		//			break;
		//		//case ConfigXDocFromEnum.FromDomainObjectInData:
		//		//	object currentDomainObject = base.GetCurrentDomainObject();
		//		//	fileConfigPath = this.GetConfigFolderPath( currentDomainObject)  +  adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
		//		//	info.ConfigXDocumentPath = fileConfigPath;
		//		//	break;
		//		case ConfigXDocFromEnum.FromFullPath:
		//			fileConfigPath = info.ConfigXDocumentPath;
		//			break;


		//	}

		//	//XDocument configXDoc = new XDocument();
		//	//if (File.Exists(fileConfigPath) == true)	   //если нет сохраненного файла config.xml
		//	//{
		//	//	try
		//	//	{
		//	//		configXDoc = XDocument.Load(fileConfigPath);
		//	//	}
		//	//	catch (Exception exp)
		//	//	{
		//	//	}
		//	//}
		//	return fileConfigPath;
		//}


		//protected string GetImportPath()
		//{
		//	if (!String.IsNullOrEmpty(this.CBIDbContext))
		//	{
		//		object currentDomainObject = base.GetCurrentDomainObject();

		//		if (currentDomainObject != null)
		//		{
		//			return base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);

		//		}
		//	}
		//	return String.Empty;
		//}

        protected override string GetModulesFolderPath()
        {
            return FileSystem.ImportModulesFolderPath();
        }

        protected Dictionary<ImportProviderParmEnum, string> GetIniData(string sectionName = "", String iniFileName = "")
        {
			if (String.IsNullOrEmpty(iniFileName) == true) return new Dictionary<ImportProviderParmEnum, string> ();
            Dictionary<ImportProviderParmEnum, string> result = base.ParseAdapterIniFile(sectionName, iniFileName);

            return result;
        }

        public string BuildMaskRegionName(string seq = null)
        {
            return String.Format("{0}{1}", this.GetType().FullName + seq, "MaskRegion");
        }

        protected bool IsPathOkForOpenAsFolder(string path)
        {
			try
			{
				if (String.IsNullOrEmpty(path))
					return false;

				if (Directory.Exists(path) == true) return true; //add

				string directory = Path.GetDirectoryName(path);

				if (String.IsNullOrWhiteSpace(directory))
					return false;

				return Directory.Exists(directory);
			}
			catch 
			{
				return false;
			}
        }

		protected void OpenPathAsFolder(string path)
		{
			try
			{
				if (String.IsNullOrEmpty(path))
					return;

				string directory = Path.GetDirectoryName(path);

				if (String.IsNullOrWhiteSpace(directory))
					return;

				if (!Directory.Exists(directory)) return;

				Utils.OpenFolderInExplorer(directory);
			}
			catch
			{
				return;
			}
		}


        protected bool ContinueAfterBranchERPWarning(string path, int start, int length)
        {
            string pathWarning = String.Empty;

            if (base.CurrentBranch == null || String.IsNullOrWhiteSpace(base.CurrentBranch.BranchCodeERP))
                return true;

            string branchERPTrimmed = base.CurrentBranch.BranchCodeERP.Trim().ToLower();

            if (!String.IsNullOrEmpty(path) && File.Exists(path))
            {
                string firstLine = File.ReadLines(path).FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(firstLine))
                {
                    if (firstLine.Length >= start + length)
                        firstLine = firstLine.Substring(start, length);
                    string fileERP = firstLine.Trim().ToLower();

                    if (branchERPTrimmed != fileERP)
                    {
                        pathWarning = String.Format(Localization.Resources.ViewModel_XTechMeuhedet_ERP_Code_Different,
                            String.IsNullOrWhiteSpace(fileERP) ? "empty" : fileERP,
                            branchERPTrimmed);
                    }
                }
            }

            return ERPWarning(pathWarning);
        }

        protected bool ContinueAfterBranchERPWarning(string path, string regEx)
        {
            string pathWarning = String.Empty;

            if (base.CurrentBranch == null || String.IsNullOrWhiteSpace(base.CurrentBranch.BranchCodeERP))
                return true;

            if (String.IsNullOrWhiteSpace(path))
                return true;

            string branchERPTrimmed = base.CurrentBranch.BranchCodeERP.Trim().ToLower();

            FileInfo fi = new FileInfo(path);
            if (String.IsNullOrWhiteSpace(fi.Name))
                return true;

            string fileERP = "empty";

            try
            {
                Match match = Regex.Match(fi.Name, regEx, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    if (match.Groups.Count > 1)
                    {
                        if (match.Groups[0].Captures.Count > 0)
                        {
                            fileERP = match.Groups[1].Captures[0].Value;
                        }
                    }

                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("ContinueAfterBranchERPWarning", exc);
            }

            if (branchERPTrimmed != fileERP)
            {
                pathWarning = String.Format(Localization.Resources.ViewModel_XTechMeuhedet_ERP_Code_Different,
                          fileERP,
                          branchERPTrimmed);
            }

            return ERPWarning(pathWarning);
        }


		protected bool ContinueAfterBranchERPWarning(string path, string regEx, string includeERPCode)
		{
			string pathWarning = String.Empty;

			if (String.IsNullOrWhiteSpace(includeERPCode))
				return true;

			string branchERPTrimmed = includeERPCode.Trim().ToLower();

			FileInfo fi = new FileInfo(path);
			if (String.IsNullOrWhiteSpace(fi.Name))
				return true;

			string fileERP = "empty";

			try
			{
				Match match = Regex.Match(fi.Name, regEx, RegexOptions.IgnoreCase);

				if (match.Success)
				{
					if (match.Groups.Count > 1)
					{
						if (match.Groups[0].Captures.Count > 0)
						{
							fileERP = match.Groups[1].Captures[0].Value;
						}
					}

				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("ContinueAfterBranchERPWarning", exc);
			}

			if (branchERPTrimmed != fileERP)
			{
				pathWarning = String.Format(Localization.Resources.ViewModel_XTechMeuhedet_ERP_Code_Different,
						  fileERP,
						  branchERPTrimmed);
			}

			return ERPWarning(pathWarning);
		}
	

        protected bool ERPWarning(string pathWarning)
        {
            bool result = true;
            if (!String.IsNullOrWhiteSpace(pathWarning))
            {
                Utils.RunOnUI(() =>
                {
                    MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(pathWarning,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning,
                        this._userSettingsManager,
                        Application.Current.MainWindow);

                    result = messageBoxResult == MessageBoxResult.Yes;
                });
            }

            return result;
        }


        protected void BackupSourceFilesAfterImport(string sourceDir, List<string> files = null, bool appendDateTimeToBackupedFile = false)
        {
//#if DEBUG
//#else
            try
            {
				//if (Directory.Exists(sourceDir))
				//{
                    if (Directory.Exists(sourceDir))
                    {
                        string targetDir = System.IO.Path.Combine(sourceDir, "Backup");
                        if (!Directory.Exists(targetDir))
                            Directory.CreateDirectory(targetDir);

                        if (files == null)
                        {
                            files = Directory.GetFiles(sourceDir).ToList();
                        }

                        foreach (string file in files)
                        {
                            FileInfo fi = new FileInfo(file);
                            string sourceFile = System.IO.Path.Combine(sourceDir, fi.Name);

                            string targetName = fi.Name;

                            if (appendDateTimeToBackupedFile)
                            {
                                string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                                targetName = String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(sourceFile), date, fi.Extension);
                            }
                            string targetFile = System.IO.Path.Combine(targetDir, targetName);

							try
                            {
							  if (File.Exists(targetFile) == true)
							      File.Delete(targetFile);
		                    }
                            catch (Exception exc)
                            {
                                string message = String.Format("BackupSourceFilesAfterImport (File.Delete) {0}", targetFile);
                                WriteErrorExceptionToAppLog(message, exc);
                            }

                            try
                            {
                                _logger.Info("Moving {0} ...", file);
								if (File.Exists(sourceFile) == true) 
								{
									Thread.Sleep(1000);
									try
									{
										File.Move(sourceFile, targetFile);
									}
									catch { }
									while (File.Exists(sourceFile) == true)
									{
										GC.Collect();
										GC.WaitForPendingFinalizers();
										GC.Collect();
										GC.Collect();
										Thread.Sleep(1000);
										try
										{
											File.Move(sourceFile, targetFile);
										}
										catch { }
									}
								}
                            }
                            catch (Exception exc)
                            {
                                string message = String.Format("BackupSourceFilesAfterImport (File.Move) {0}", file);
                                WriteErrorExceptionToAppLog(message, exc);
                            }
                        }

					//Thread.Sleep(500);
                    }
				//}
            }
            catch (Exception exc)
            {
                WriteErrorExceptionToAppLog("BackupSourceFilesAfterImport", exc);
                throw;
            }
//#endif
		}

		//Смотри SendToFtpCommandExecuted
		private void SendToFtpCommandExecuted(string currentCodeFolder, List<string> files = null)
		{
			//string currentCustomerCode = "";
			//if (base.State.CurrentCustomer != null) currentCustomerCode = base.State.CurrentCustomer.Code;
			//if (currentCustomerCode == "") return;

			//var itemsCheck = this._items.Where(k => k.IsChecked == true).Select(k => k).ToList();

			//string host = _userSettingsManager.HostGet().Trim('\\');
			bool enableSsl;
			string host = _userSettingsManager.HostFtpGet(out enableSsl);
			string user = _userSettingsManager.UserGet();
			string password = _userSettingsManager.PasswordGet();
			string rootFolder = @"Count4U\ImportData\Inventor";
			try
			{
				using (new CursorWait())
				{
					FtpClient client = new FtpClient(host, user, password, enableSsl);
					//--------------- find or create folder on ftp @"Count4U\ImportData\Inventor\currentCodeFolder"----------------
					client.uri = host;
					string result3 = client.ChangeWorkingDirectory(rootFolder);			// @"Count4U\ImportData\Inventor";

					//string currentCodeFolder = currentCustomerCode;													// customerCode
					string[] listDirectory = client.ListDirectory();
					bool newDir = true;
					foreach (string dir in listDirectory)
					{																					 // проверяем есть ли такая папка
						if (dir.ToLower() == currentCodeFolder.ToLower())		//  Count4U\ImportData\Inventor/customerCode
						{
							newDir = false;
						}
					}

					if (newDir == true)
					{
						string result1 = client.MakeDirectory(currentCodeFolder);
						_logger.Info("Create folder on ftpServer[" + currentCodeFolder + "]");
						_logger.Info("with Result [" + result1 + "]");
					}
					//-------------	send to ftp
					//очистить ftp folder
					//this.ClearFolderOnFtp(rootFolder + @"\" + currentCodeFolder);

					client.uri = host;
					//string objectCodeFolder = this.GetExportToPDAFolderPath(true);
					string result7 = client.ChangeWorkingDirectory(rootFolder + @"\" + currentCodeFolder);		//  mINV\\ToApp

					foreach (var file in files)
					{
						string fileName = file;
						string filePath = currentCodeFolder + @"\" + fileName;
						string toFTPPath = fileName; //currentCustomerCode + @"\" + fileName;		  //  	 customerCode\fileName

						string result = client.SaveFileToFtp(filePath, toFTPPath);
						_logger.Info("Save file [" + filePath + "]" + " to ftp [" + toFTPPath + "]");
						_logger.Info("with Result [" + result + "]");
					}

				}
			}
			catch (Exception exc)
			{
				string message = String.Format("SendToFtpCommandExecuted ");
				_logger.ErrorException(message, exc);
			}

	  	}

		//Смотри SendToFtpCommandExecuted
		//смотри private void SendToFtpCommandExecuted
		protected void ForwardSourceFilesAfterImport(string sourceDir, List<string> files = null, bool appendDateTimeToBackupedFile = false)
		{
            try
            {
				//if (Directory.Exists(sourceDir))
				//{
                    if (Directory.Exists(sourceDir))
                    {
                        string targetDir = System.IO.Path.Combine(sourceDir, "Backup");
                        if (!Directory.Exists(targetDir))
                            Directory.CreateDirectory(targetDir);

                        if (files == null)
                        {
                            files = Directory.GetFiles(sourceDir).ToList();
                        }

                        foreach (string file in files)
                        {
                            FileInfo fi = new FileInfo(file);
                            string sourceFile = System.IO.Path.Combine(sourceDir, fi.Name);

                            string targetName = fi.Name;

                            if (appendDateTimeToBackupedFile)
                            {
                                string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                                targetName = String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(sourceFile), date, fi.Extension);
                            }
                            string targetFile = System.IO.Path.Combine(targetDir, targetName);

							try
                            {
							  if (File.Exists(targetFile) == true)
							      File.Delete(targetFile);
		                    }
                            catch (Exception exc)
                            {
                                string message = String.Format("BackupSourceFilesAfterImport (File.Delete) {0}", targetFile);
                                WriteErrorExceptionToAppLog(message, exc);
                            }

                            try
                            {
                                _logger.Info("Moving {0} ...", file);
								if (File.Exists(sourceFile) == true) 
								{
									Thread.Sleep(1000);
									try
									{
										File.Move(sourceFile, targetFile);
									}
									catch { }
									while (File.Exists(sourceFile) == true)
									{
										GC.Collect();
										GC.WaitForPendingFinalizers();
										GC.Collect();
										GC.Collect();
										Thread.Sleep(1000);
										try
										{
											File.Move(sourceFile, targetFile);
										}
										catch { }
									}
								}
                            }
                            catch (Exception exc)
                            {
                                string message = String.Format("BackupSourceFilesAfterImport (File.Move) {0}", file);
                                WriteErrorExceptionToAppLog(message, exc);
                            }
                        }

					//Thread.Sleep(500);
                    }
				//}
            }
            catch (Exception exc)
            {
                WriteErrorExceptionToAppLog("BackupSourceFilesAfterImport", exc);
                throw;
            }
		}

		protected void UnsureSourceFilesAfterImport(string sourceDir, List<string> unsureFiles = null, bool appendDateTimeToBackupedFile = false)
		{
#if DEBUG
#else
            try
            {
				//if (Directory.Exists(sourceDir))
				//{
                    if (Directory.Exists(sourceDir))
                    {
                        string targetDir = System.IO.Path.Combine(sourceDir, "Unsure");
                        if (!Directory.Exists(targetDir))
                            Directory.CreateDirectory(targetDir);

                        if (unsureFiles == null)
                        {
                            unsureFiles = Directory.GetFiles(sourceDir).ToList();
                        }

                        foreach (string file in unsureFiles)
                        {
                            FileInfo fi = new FileInfo(file);
                            string sourceFile = System.IO.Path.Combine(sourceDir, fi.Name);

                            string targetName = fi.Name;

                            if (appendDateTimeToBackupedFile)
                            {
                                string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                                targetName = String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(sourceFile), date, fi.Extension);
                            }
                            string targetFile = System.IO.Path.Combine(targetDir, targetName);

							try
                            {
							  if (File.Exists(targetFile) == true)
							      File.Delete(targetFile);
		                    }
                            catch (Exception exc)
                            {
                                string message = String.Format("UnsureSourceFilesAfterImport (File.Delete) {0}", targetFile);
                                WriteErrorExceptionToAppLog(message, exc);
                            }

                            try
                            {
								_logger.Info("Moving to unsure {0} ...", file);
							
								if (File.Exists(sourceFile) == true)
								{
									Thread.Sleep(1000);
									try
									{
										File.Move(sourceFile, targetFile);
									}
									catch { }
									while (File.Exists(sourceFile) == true)
									{
										GC.Collect();
										GC.WaitForPendingFinalizers();
										GC.Collect();
										GC.Collect();
										Thread.Sleep(1000);
										try
										{
											File.Move(sourceFile, targetFile);
										}
										catch { }
									}
								}
                            }
                            catch (Exception exc)
                            {
                                string message = String.Format("UnsureSourceFilesAfterImport (File.Move) {0}", file);
                                WriteErrorExceptionToAppLog(message, exc);
                            }
                        }

					//Thread.Sleep(500);
                    }
				//}
            }
            catch (Exception exc)
            {
                WriteErrorExceptionToAppLog("BackupSourceFilesAfterImport", exc);
                throw;
            }
#endif
		}

        protected string GetPathToIniFile(string iniFileName)
        {
            return Path.Combine(this.GetModulesFolderPath(), iniFileName);
        }
    }
}