using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Count4U.Common.Events;
using Count4U.Common.Constants;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common;
using System.Xml.Linq;

namespace Count4U.ImportPdaMISAndDefaultAdapter
{
    public class ImportPdaMISAndDefaultAdapterViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
    {
        private readonly IIturRepository _iturRepository;
		public string _fileName {get; set;}
        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        //private  List<string> _newDocumentCodeList;
		private readonly List<string> _newSessionCodeList;
		 //private readonly	IDBSettings _dbSettings;
		//private bool _listeningSourceFolder;

        private bool _isAutoPrint;
        private Count4U.GenerationReport.Report _report;
        private bool _isContinueGrabFiles;
		//private bool _isCopyFromSource;

		//private string _misCommunicatorPath = ""; //не используется
		private string _misiDnextDataPath = "";

		public ImportPdaMISAndDefaultAdapterViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
            IUserSettingsManager userSettingsManager,
            IGenerateReportRepository generateReportRepository,
            IUnityContainer unityContainer,
			ITemporaryInventoryRepository temporaryInventoryRepository,
			IDBSettings dbSettings) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
            this._unityContainer = unityContainer;
            this._generateReportRepository = generateReportRepository;
            this._iturRepository = iturRepository;
			//this._dbSettings = dbSettings;
           // this._newDocumentCodeList = new List<string>();
			this._newSessionCodeList = new List<string>();

            this._isDirectory = true;
            this._isSingleFile = false;
			//base.ListeningSourceFolder = true;
			//base.IsCopyFromSourceFolder = _userSettingsManager.CopyFromSourceGet();  //checkbox on GUI for MIS не используется
			
			base.ParmsDictionary.Clear();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
			//this._misiDnextDataPath = this._userSettingsManager.ImportPDAPathGet().Trim('\\') + @"\IDnextData";
			//this._misCommunicatorPath = this._userSettingsManager.ImportPDAPathGet().Trim('\\') + @"\MISCommunicator";
        }

		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;

			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				//info.CurrentCustomer = base.CurrentCustomer;
				//info.CurrentBranch = base.CurrentBranch;
				//info.CurrentInventor = base.CurrentInventor;
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
						this.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						//if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
		}

        protected override void RunImport(ImportCommandInfo info)
        {
            this.Import();
        }

        protected override void RunClear()
        {
            this.Clear();
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return _yesNoRequest; }
        }

        public List<string> NewDocumentCodeList
        {
            get
			{
				ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
				List<string> newDocumentCodeList = sessionRepository.GetDocumentHeaderCodeList(this._newSessionCodeList, base.GetDbPath);

				return newDocumentCodeList; 
			}
        }

		//public bool ListeningSourceFolder  //checkbox слушать папку
		//{
		//	get { return this._listeningSourceFolder; }
		//	set
		//	{
		//		this._listeningSourceFolder = value;
		//		RaisePropertyChanged(() => ListeningSourceFolder);
		//	}
		//}

	
		public List<string> NewSessionCodeList
		{
			get { return this._newSessionCodeList; }
		}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
          //  this._newDocumentCodeList.Clear();
			this._newSessionCodeList.Clear();
			//this.IsCopyFromSourceFolder = _userSettingsManager.CopyFromSourceGet(); //checkbox on GUI for MIS не используется
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        protected override void ProcessImportInfo(ImportCommandInfo info)
        {
            ImportFromPdaCommandInfo pdaInfo = info as ImportFromPdaCommandInfo;
            if (pdaInfo != null)
            {
                this._report = pdaInfo.Report as Count4U.GenerationReport.Report;
                this._isAutoPrint = pdaInfo.IsAutoPrint;
                this._isContinueGrabFiles = pdaInfo.IsContinueGrabFiles;
            }
        }

        #region Implementation of IImportAdapter

        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
			//string importPDAPath = this._userSettingsManager.ImportPDAPathGet().Trim('\\');
			//this._misiDnextDataPath = importPDAPath + @"\IDnextData";
			//this._misCommunicatorPath = importPDAPath + @"\MISCommunicator";
			//init GUI
			//base.SourcePath = this._misiDnextDataPath.Trim('\\') + @"\fromHT"; //@"C:\Count4U\iDnextData\fromHT";

			//if (Directory.Exists(SourcePath) == false)
			//{
			//	try { Directory.CreateDirectory(SourcePath); }
			//	catch { }
			//}

            this._fileName = FileSystem.inData;
            this.PathFilter = "*.txt|*.txt|All files (*.*)|*.*";
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
			Customer customer = base.CurrentCustomer;
			//base.ProcessLisner = 0;

            base.IsInvertLetters = false;
            base.IsInvertWords = false;
            StepTotal = 1;
            Session = 0;
        }

        public override void InitFromIni()
        {
//[ImportPdaMISAdapter]
//MISiDnextDataPath = C:\MIS\IDnextData
//MISCommunicatorPath = C:\MIS\MISCommunicator
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData("ImportPdaMISAdapter");
            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			//this.SourcePath = iniData.SetValue(ImportProviderParmEnum.SourcePath, this.SourcePath);
			//base.SourcePath = this._misiDnextDataPath.Trim('\\') + @"\fromHT";
			//this._misCommunicatorPath = iniData.SetValue(ImportProviderParmEnum.MISCommunicatorPath, this._misCommunicatorPath);

			//string importPDAPath = this._userSettingsManager.ImportPDAPathGet();
			//importPDAPath = iniData.SetValue(ImportProviderParmEnum.MISiDnextDataPath, importPDAPath);
			//this._misiDnextDataPath = importPDAPath.Trim('\\') +@"\IDnextData";

			try
            {
                if (base._isDirectory)
                {
                    if (!Directory.Exists(this.Path))
                        Directory.CreateDirectory(this.Path);
                }
            }
            catch (Exception exc)
            {
                WriteErrorExceptionToAppLog("Create inData directory", exc);
            }
        }

		//public bool IsCopyFromSource
		//{
		//	get { return _isCopyFromSource; }
		//	set
		//	{
		//		_isCopyFromSource = value;
		//		RaisePropertyChanged(() => IsCopyFromSource);

		//		_userSettingsManager.CopyFromSourceSet(_isCopyFromSource);
		//	}
		//}

		public override void Import()
		{
			bool saveCountingFromSource = this._userSettingsManager.CountingFromSourceGet();
			bool saveCopyFromSource = this._userSettingsManager.CopyFromSourceGet();
			this._userSettingsManager.CountingFromSourceSet(false);//остановили подсчета на время импорта
			this._userSettingsManager.CopyFromSourceSet(false);//остановили копирование на время импорта
			try
			{
				//bool tempCopyFromSourceFolder = base.IsCopyFromSourceFolder; //надо ли закончить копирование
				//if (tempCopyFromSourceFolder == true)
				//{
				if (this.IsCopyFromMainForm == true) //если это с белой кнопки запуск
				{
					Thread.Sleep(5000);
				}

				//}

				//base.IsCopyFromSourceFolder = false; //остановили копирование на время импорта
				//RaisePropertyChanged(() => IsCopyFromSourceFolder);

				IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportInventProductMisAndDefaultADOProvider);
				provider.ToPathDB = base.GetDbPath;
				string inventorCode = base.CurrentInventor.Code;
				provider.ProviderEncoding = base.Encoding;
				string newSessionCode = Guid.NewGuid().ToString();
				this._newSessionCodeList.Add(newSessionCode);
				//List<string> newDocumentCodeList = new List<string>();
				//TODO Localization
				base.LogImport.Add(MessageTypeEnum.TraceProvider, String.Format("NewSessionCode : {0}", newSessionCode));
				base.LogImport.Add(MessageTypeEnum.TraceProvider, String.Format("Path : {0}", this.Path));

				IMakatRepository makatRepository = base.ServiceLocator.GetInstance<IMakatRepository>();
				makatRepository.ProductMakatDictionaryRefill(base.GetDbPath, true);

				if (base.IsSingleFile == true)
				{
					//string newDocumentCode = Guid.NewGuid().ToString();
					//newDocumentCodeList.Add(newDocumentCode);
					//this._newDocumentCodeList.Add(newDocumentCode);
					provider.Parms.Clear();
					provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					//provider.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;
					provider.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
					provider.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider.FromPathFile = this.Path;
					StepCurrent = 1;
					provider.Import();
					//multi doc RunPrintReport(newDocumentCode);
					// old-old Nikita printQueue.Enqueue(new PrintQueueItem() { DocumentCode = newDocumentCode, ViewModel = this });              
				}
				else
				{
					bool firstRun = true;
					base.Session = 0;
					while (true)
					{
						if (base.CancellationToken.IsCancellationRequested)
							break;

						List<string> getFiles = Directory.GetFiles(this.Path).ToList();
						List<string> files = new List<string>();
						List<string> unsureFiles = new List<string>();

						foreach (string filePath in getFiles)
						{
							FileInfo fi = new FileInfo(filePath);
							//if (fi.Name.Contains(inventorCode) == true)				//!!!
							//{
							files.Add(filePath);
							//}
							//else
							//{
							//	unsureFiles.Add(filePath);
							//}
						}

						if (!files.Any())
							break;

						if (firstRun == false)
						{
#if DEBUG
							break;
#else
                        if (_isContinueGrabFiles == false)
                            break;
#endif
						}

						base.Session++;
						base.StepTotal = files.Count;
						base.StepCurrent = 0;
						foreach (string filePath in files)
						{
							base.StepCurrent++;
							//string newDocumentCode = Guid.NewGuid().ToString();
							//newDocumentCodeList.Add(newDocumentCode);
							//this._newDocumentCodeList.Add(newDocumentCode);
							provider.Parms.Clear();
							provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
							//provider.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;
							provider.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
							provider.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
							string finalPath = System.IO.Path.Combine(this.Path, filePath);
							provider.FromPathFile = finalPath;
							provider.Import();
							// multi doc RunPrintReport(newDocumentCode);
							// old-old Nikita printQueue.Enqueue(new PrintQueueItem() { DocumentCode = newDocumentCode, ViewModel = this });

							if (base.CancellationToken.IsCancellationRequested)
								break;
						}

						if (base.CancellationToken.IsCancellationRequested)
							break;


						base.UnsureSourceFilesAfterImport(this.Path, unsureFiles, true);
						base.BackupSourceFilesAfterImport(this.Path, files, true);

						firstRun = false;
					}
				}
				//TODO Localization

				// один импорт => одна сессия
				List<string> currentSessionCodeList = new List<string>();
				currentSessionCodeList.Add(newSessionCode);
				ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
				List<string> currentDocumentCodeList = sessionRepository.Insert(currentSessionCodeList, base.GetDbPath);

				foreach (var documentCode in currentDocumentCodeList)
				{
					RunPrintReport(documentCode);
					base.LogImport.Add(MessageTypeEnum.TraceProvider, String.Format("Document {0} was print", documentCode));
				}

				base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format("Total Document Import : {0}", currentDocumentCodeList.Count));
				IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
				long countDocumentWithoutError = documentHeaderRepository.GetCountDocumentWithoutError(currentSessionCodeList, base.GetDbPath);
				base.LogImport.Add(MessageTypeEnum.TraceProvider, String.Format("Total Document Import correct : {0}", countDocumentWithoutError));
				long countDocumentWithError = documentHeaderRepository.GetCountDocumentWithError(currentSessionCodeList, base.GetDbPath);
				base.LogImport.Add(MessageTypeEnum.TraceProvider, String.Format("Total Document Import with error:  {0}", countDocumentWithError));

				Session lastSession = sessionRepository.GetSessionWithMaxDateCreated(base.GetDbPath);
                int lastSessionCountItem = 0;
                double lastSessionSumQuantityEdit = 0;
                if (lastSession != null)
                {
                    lastSessionCountItem = lastSession.CountItem;
                    lastSessionSumQuantityEdit = lastSession.SumQuantityEdit;
                }
				base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.View_IturListDetails_tbLastSessionCountItem, lastSessionCountItem));
				base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.View_IturListDetails_tbLastSessionSumQuantityEdit, Convert.ToInt64(lastSessionSumQuantityEdit)));

				FileLogInfo fileLogInfo = new FileLogInfo();
				if (base._isDirectory == true)
				{
					fileLogInfo.Directory = this.Path;
				}
				else
				{
					fileLogInfo.File = this.Path;
				}

				//base.IsCopyFromSourceFolder = true;//tempCopyFromSourceFolder;//восстановили копирование на время импорта
				//RaisePropertyChanged(() => IsCopyFromSourceFolder);

				base.SaveFileLog(fileLogInfo);
				Thread.Sleep(500);

			}
			catch (Exception ex)
			{
				base.LogImport.Add(MessageTypeEnum.Error, String.Format("Import from PDA : ", ex.Message));
			}
			this._userSettingsManager.CountingFromSourceSet(saveCountingFromSource);
			this._userSettingsManager.CopyFromSourceSet(saveCopyFromSource); //востановить запомненное состояние
		}

        public override void Clear()
        {
            base.LogImport.Clear();
            //this._iturRepository.RefillApproveStatus = false;
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportInventProductMisAndDefaultADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "InventProduct"));
            this._iturRepository.ClearStatusBit(base.GetDbPath);
            UpdateLogFromILog();
        }

        #endregion

        public void RunPrintReport(string documentCode)
        {
            if (this._isAutoPrint == false) return;

            if (string.IsNullOrWhiteSpace(documentCode) == true) return;
            IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
            DocumentHeader documentHeader = documentHeaderRepository.GetDocumentHeaderByCode(documentCode, base.GetDbPath);
            if (documentHeader == null) return;

            IIturRepository iturRepository = base.ServiceLocator.GetInstance<IIturRepository>();
            Itur itur = iturRepository.GetIturByDocumentCode(documentCode, base.GetDbPath);

            Location location = null;
            if (itur != null)
            {
                ILocationRepository locationRepository = base.ServiceLocator.GetInstance<ILocationRepository>();
                location = locationRepository.GetLocationByCode(itur.LocationCode, base.GetDbPath);
            }

            try
            {
                GenerateReportArgs args = new GenerateReportArgs();
                args.Customer = base.CurrentCustomer;
                args.Branch = base.CurrentBranch;
                args.Inventor = base.CurrentInventor;
                args.DbPath = base.GetDbPath;
                args.Report = this._report;
                args.Doc = documentHeader;
				args.Device = null;
                args.ViewDomainContextType = ViewDomainContextEnum.ItursIturDoc;
                args.Itur = itur;
                args.Location = location;

                SelectParams spDoc = new SelectParams();
                List<string> searchDoc = new List<string> { documentCode };
                List<string> searchItur = new List<string> { documentHeader.IturCode };
                spDoc.FilterStringListParams.Add("DocumentCode", new FilterStringListParam()
                {
                    Values = searchDoc
                });
                spDoc.FilterStringListParams.Add("IturCode", new FilterStringListParam()
                {
                    Values = searchItur
                });
                args.SelectParams = spDoc;

                //this._generateReportRepository.RunPrintReport(args);
                //this._generateReportRepository.RunSaveReport(args, @"C:\Temp\testReport\output1.txt", ReportFileFormat.Excel);

                ImportPdaPrintQueue printQueue = _unityContainer.Resolve<ImportPdaPrintQueue>();
                printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
            }
            catch (Exception exc)
            {
                _logger.ErrorException("RunPrintReport", exc);
            }
        }


		//public string MISiDnextDataPath
		//{
		//	get
		//	{
		//		if (Directory.Exists(this._misiDnextDataPath) == false)
		//		{
		//			try { Directory.CreateDirectory(this._misiDnextDataPath); }
		//			catch { }
		//		}
		//		if (Directory.Exists(this._misiDnextDataPath) == true)
		//		{
		//			return this._misiDnextDataPath;
		//		}
		//		else return "";
		//	}
		//}

		//public string MISCommunicatorPath
		//{
		//	get
		//	{
		//		if (Directory.Exists(this._misCommunicatorPath) == false)
		//		{
		//			try { Directory.CreateDirectory(this._misCommunicatorPath); }
		//			catch { }
		//		}
		//		if (Directory.Exists(this._misCommunicatorPath) == true)
		//		{
		//			return this._misCommunicatorPath;
		//		}
		//		else return "";
		//	}
		//}
    }
}