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

namespace Count4U.ImportPdaClalitSqliteAdapter
{
    public class ImportPdaClalitSqliteAdapterViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
    {
        private readonly IIturRepository _iturRepository;
		public string _fileName {get; set;}
        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
		private readonly List<string> _newSessionCodeList;
	    private bool _isAutoPrint;
        private Count4U.GenerationReport.Report _report;
        private bool _isContinueGrabFiles;
		private string _misiDnextDataPath = "";

		private InventProductAdvancedFieldName _inventProductAdvancedDBFieldName;

		public ImportPdaClalitSqliteAdapterViewModel(
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
			this._newSessionCodeList = new List<string>();

            this._isDirectory = true;
            this._isSingleFile = false;
			
			base.ParmsDictionary.Clear();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
	    }

		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
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
				//ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
				//List<string> newDocumentCodeList = sessionRepository.GetDocumentHeaderCodeList(this._newSessionCodeList, base.GetDbPath);
				//return newDocumentCodeList;
				IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
				System.Collections.Generic.List<string> docCodes = documentHeaderRepository.GetDocumentCodeList(base.GetDbPath);
				return docCodes; 
			}
        }

		public InventProductAdvancedFieldName InventProductAdvancedDBFieldName
		{
			get { return _inventProductAdvancedDBFieldName; }
			set { _inventProductAdvancedDBFieldName = value; }
		}

		
		public List<string> NewSessionCodeList
		{
			get { return this._newSessionCodeList; }
		}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this._newSessionCodeList.Clear();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

		//public void Refresh()
		//{
		//	IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
		//	List<string> docCodes = documentHeaderRepository.GetDocumentCodeList(base.GetDbPath);
		//}

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
            this._fileName = FileSystem.inData;
            this.PathFilter = "*.db3|*.db3|All files (*.*)|*.*";
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
			//base.ProcessLisner = 0;

			InitInventProductAdvancedField();

            base.IsInvertLetters = false;
            base.IsInvertWords = false;
            StepTotal = 1;
            Session = 0;
        }

		private void InitInventProductAdvancedField()
		{
			this.InventProductAdvancedDBFieldName = new InventProductAdvancedFieldName
			{
				IPValueStr1 = "PropertyStr1",
				IPValueStr2 = "PropertyStr2",
				IPValueStr3 = "PropertyStr3",  //было 12
				IPValueStr4 = "PropertyStr4",
				IPValueStr5 = "PropertyStr5",
				IPValueStr6 = "PropertyStr6",
				IPValueStr7 = "PropertyStr7",
				IPValueStr8 = "PropertyStr8",
				IPValueStr9 = "PropertyStr9",
				IPValueStr10 = "PropertyStr10",
				IPValueStr11 = "PropertyStr11",
				IPValueStr12 = "PropertyStr12",
				IPValueStr13 = "PropertyStr13",
				IPValueStr14 = "PropertyStr14",
				IPValueStr15 = "PropertyStr15",
				IPValueStr16 = "PropertyStr16",
				IPValueStr17 = "PropertyStr17",
				IPValueStr18 = "PropertyStr18",
				IPValueStr19 = "PropertyStr19",
				IPValueStr20 = "PropertyStr20",
				//IPValueFloat1 = "PropertyStr11",
				//IPValueFloat3 = "PropertyStr3" 
			};
		}

        public override void InitFromIni()
        {

			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);

			Dictionary<ImportProviderParmEnum, string> iniData1 = base.GetIniData("Count4U.ImportPdaDefaultAdapter");
			this.InventProductAdvancedDBFieldName = iniData1.SetValueName(this.InventProductAdvancedDBFieldName);
	
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

	
        public override void Import()
        {
			bool saveCountingFromSource = this._userSettingsManager.CountingFromSourceGet();
			bool saveCopyFromSource = this._userSettingsManager.CopyFromSourceGet();
			this._userSettingsManager.CountingFromSourceSet(false);//остановили подсчета на время импорта
			this._userSettingsManager.CopyFromSourceSet(false);//остановили копирование на время импорта
			try
			{

				if (this.IsCopyFromMainForm == true) //если это с белой кнопки запуск
				{
					Thread.Sleep(5000);
				}

				string inventorCode = base.CurrentInventor.Code;
				string newSessionCode = Guid.NewGuid().ToString();
				this._newSessionCodeList.Add(newSessionCode);

				IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportTemporaryInventoryClalitSqlite2SdfProvider);
				provider1.ToPathDB = base.GetDbPath;
				provider1.ProviderEncoding = base.Encoding;

				//IturUpdateClalitSqlite2SdfParser
				IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportLocationUpdateClalitSqlite2SdfProvider);
				provider2.ToPathDB = base.GetDbPath;
				provider2.ProviderEncoding = base.Encoding;
				//InventProductClalitSqlite2SdfParser
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductClalitSqlite2SdfProvider);
				provider3.ToPathDB = base.GetDbPath;
				provider3.ProviderEncoding = base.Encoding;

				base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("NewSessionCode : {0}", newSessionCode));
				base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Path : {0}", this.Path));

				IMakatRepository makatRepository = base.ServiceLocator.GetInstance<IMakatRepository>();
				makatRepository.ProductMakatDictionaryRefill(base.GetDbPath, true);

				if (base.IsSingleFile == true)
				{
					provider1.Parms.Clear();
					provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider1.FromPathFile = this.Path;

					provider3.Parms.Clear();
					provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider3.Parms.AddInventProductAdvancedField(this.InventProductAdvancedDBFieldName);
					provider3.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
					provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider3.FromPathFile = this.Path;

					provider2.Parms.Clear();
					provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					provider2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
					provider2.FromPathFile = this.Path;

					if (System.IO.Path.GetExtension(base.Path) == ".db3")
					{
						provider1.Parms[ImportProviderParmEnum.FileDB3] = "1";
						provider2.Parms[ImportProviderParmEnum.FileDB3] = "1";
						provider3.Parms[ImportProviderParmEnum.FileDB3] = "1";

						StepCurrent++;
						provider1.Import();
						StepCurrent++;
						provider2.Import();
						StepCurrent++;
						provider3.Import();
					}
					else
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("File : {0} Must have Extension .db3", base.Path));
					}
				}
				else
				{
					bool firstRun = true;
					base.Session = 0;
					while (true)
					{
						if (base.CancellationToken.IsCancellationRequested)
							break;

						//List<string> getFiles = Directory.GetFiles(this.Path).ToList();
						List<string> files = new List<string>();
						List<string> unsureFiles = new List<string>();
						DirectoryInfo dir = new System.IO.DirectoryInfo(this.Path);
						FileInfo[] sourceFiles = dir.GetFiles().OrderBy(f => f.LastWriteTime).ToArray();

						//foreach (string filePath in getFiles)
						//{
						foreach (FileInfo fi in sourceFiles)
						{
							string fileName = fi.Name;
							if (System.IO.Path.GetExtension(fileName) == ".db3")
							//&& fileName.Contains(inventorCode) == true)  //Проверка на  inventorCode
							{
								files.Add(fileName);
							}
							else
							{
								unsureFiles.Add(fileName);
							}
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
						base.StepTotal = files.Count * 3;
						base.StepCurrent = 0;
						//=====================	  Temporary 
						foreach (string filePath in files)
						{
							provider1.Parms.Clear();
							provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
							provider1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
							string finalPath = System.IO.Path.Combine(this.Path, filePath);
							provider1.FromPathFile = finalPath;

							if (System.IO.Path.GetExtension(finalPath) == ".db3")
							{
								provider1.Parms[ImportProviderParmEnum.FileDB3] = "1";
								base.StepCurrent++;
								provider1.Import();
							}
							else
							{
								base.LogImport.Add(MessageTypeEnum.Error, String.Format("File : {0} Must have Extension .db3", finalPath));
							}

							if (base.CancellationToken.IsCancellationRequested)
								break;
						}

						if (base.CancellationToken.IsCancellationRequested)
							break;

						//=====================	  Location 


						foreach (string filePath in files)
						{
							//string newDocumentCode = Guid.NewGuid().ToString();
							//newDocumentCodeList.Add(newDocumentCode);
							//this._newDocumentCodeList.Add(newDocumentCode);
							//provider.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;

							provider2.Parms.Clear();
							provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
							provider2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
							string finalPath = System.IO.Path.Combine(this.Path, filePath);
							provider2.FromPathFile = finalPath;

							if (System.IO.Path.GetExtension(finalPath) == ".db3")
							{
								provider2.Parms[ImportProviderParmEnum.FileDB3] = "1";
								base.StepCurrent++;
								provider2.Import();
							}
							else
							{
								base.LogImport.Add(MessageTypeEnum.Error, String.Format("File : {0} Must have Extension .db3", finalPath));
							}

							// multi doc RunPrintReport(newDocumentCode);
							// old-old Nikita printQueue.Enqueue(new PrintQueueItem() { DocumentCode = newDocumentCode, ViewModel = this });

							if (base.CancellationToken.IsCancellationRequested)
								break;
						}

						if (base.CancellationToken.IsCancellationRequested)
							break;

						//==============InventProduct================================
						//base.StepTotal = files.Count;
						//base.StepCurrent = 0;
						foreach (string filePath in files)
						{
							//string newDocumentCode = Guid.NewGuid().ToString();
							//newDocumentCodeList.Add(newDocumentCode);
							//this._newDocumentCodeList.Add(newDocumentCode);
							provider3.Parms.Clear();
							provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
							//provider.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;
							provider3.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
							provider3.Parms.AddInventProductAdvancedField(this.InventProductAdvancedDBFieldName);
							provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
							string finalPath = System.IO.Path.Combine(this.Path, filePath);
							provider3.FromPathFile = finalPath;

							if (System.IO.Path.GetExtension(finalPath) == ".db3")
							{
								provider3.Parms[ImportProviderParmEnum.FileDB3] = "1";
								base.StepCurrent++;
								provider3.Import();
							}
							else
							{
								base.LogImport.Add(MessageTypeEnum.Error, String.Format("File : {0} Must have Extension .db3", finalPath));
							}

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

				//foreach (var documentCode in currentDocumentCodeList)
				//{
				//	RunPrintReport(documentCode);
				//}

				base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import : {0}", currentDocumentCodeList.Count));
				IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
				long countDocumentWithoutError = documentHeaderRepository.GetCountDocumentWithoutError(currentSessionCodeList, base.GetDbPath);
				base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import correct : {0}", countDocumentWithoutError));
				long countDocumentWithError = documentHeaderRepository.GetCountDocumentWithError(currentSessionCodeList, base.GetDbPath);
				base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import with error:  {0}", countDocumentWithError));

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
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportInventProductClalitSqlite2SdfProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "InventProduct"));
            this._iturRepository.ClearStatusBit(base.GetDbPath);

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportLocationUpdateClalitSqlite2SdfProvider);
			provider2.ToPathDB = base.GetDbPath;
			provider2.Clear();
			base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Itur"));


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