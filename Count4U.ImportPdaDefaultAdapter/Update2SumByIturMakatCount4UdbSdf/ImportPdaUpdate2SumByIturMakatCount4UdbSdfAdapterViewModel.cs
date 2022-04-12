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
using System.Collections.ObjectModel;
using Count4U.Model.ExportImport.Items;
using Count4U.Model.Audit;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common;
using System.Xml.Linq;

namespace Count4U.ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapter
{
    public class ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapterViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
    {
        private readonly IIturRepository _iturRepository;
		public string _fileName {get; set;}
        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        //private  List<string> _newDocumentCodeList;
		private readonly List<string> _newSessionCodeList;

        private bool _isAutoPrint;
        private Count4U.GenerationReport.Report _report;
        private bool _isContinueGrabFiles;
		//private AdapterFileWatcher _pathFileWatcher;

		//protected readonly ObservableCollection<FileItemViewModel> _items;
		//protected bool _isChecked;

		//protected bool _isDbInventories;
		//protected bool _isDbFile;

		public ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapterViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager,
            IGenerateReportRepository generateReportRepository,
            IUnityContainer unityContainer) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
			//this._items = new ObservableCollection<FileItemViewModel>();
		//	this._items.Add(new FileItemViewModel { File = "test", Size = "10", Date = "1.1.1070" });

            this._unityContainer = unityContainer;
            this._generateReportRepository = generateReportRepository;
            this._iturRepository = iturRepository;
           // this._newDocumentCodeList = new List<string>();
			this._newSessionCodeList = new List<string>();

			//this._isDbInventories = true;
			//this._isDbFile = false;

			base.ParmsDictionary.Clear();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
        }

		//	if (this._items.Any(r => r.IsChecked) == false) TODO
		//				return false;

		//public ObservableCollection<FileItemViewModel> Items
		//{
		//	get { return this._items; }
		//}

		public override bool CanImport()
		{
			//var itemsCheck = this._items.Where(k => k.IsChecked == true).Select(k => k).ToList();
			//var count = itemsCheck.Count();
			//if (count > 0) return true;
			//return false;
			return true;
		}

		
		//public bool IsChecked
		//{
		//	get { return this._isChecked; }
		//	set
		//	{
		//		this._isChecked = value;
		//		RaisePropertyChanged(() => IsChecked);
		//		foreach (FileItemViewModel item in this._items)
		//		{
		//			item.IsChecked = this._isChecked;
		//		}
		//		if (base.RaiseCanImport != null)
		//			base.RaiseCanImport();
		//	}
		//}

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
				ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
				List<string> newDocumentCodeList = sessionRepository.GetDocumentHeaderCodeList(this._newSessionCodeList, base.GetDbPath);

				return newDocumentCodeList; 
			}
        }

		public List<string> NewSessionCodeList
		{
			get { return this._newSessionCodeList; }
		}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
		//	this._pathFileWatcher = new AdapterFileWatcher(this, TypedReflection<ImportPdaCount4UdbSdfAdapterViewModel>.GetPropertyInfo(r => r.Path), this._isDbFile);

          //  this._newDocumentCodeList.Clear();
			this._newSessionCodeList.Clear();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
			//this._pathFileWatcher.Clear();
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
            //init GUI
            this._fileName = FileSystem.inData;
			this.PathFilter = "*.sdf|*.sdf|All files (*.*)|*.*";
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;
            StepTotal = 1;
            Session = 0;
        }

        public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName) + @"\Count4UDB.sdf";
			//this.Build();
		//	this.RaisePropertyChanged(() => this.Items);
	
        }

		public override void Import()																				//ImportIturFromDBBlukProvider
		{							
			//ImportIturFromDBProvider
			string newSessionCode = Guid.NewGuid().ToString();
			this._newSessionCodeList.Add(newSessionCode);
			base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("NewSessionCode : {0}", newSessionCode));

			//IMakatRepository makatRepository = base.ServiceLocator.GetInstance<IMakatRepository>();
			//makatRepository.ProductMakatDictionaryRefill(base.GetDbPath, true);

			//string currentInventorCode = "";
			//if (base.State.CurrentInventor != null) currentInventorCode = base.State.CurrentInventor.Code;
			//var itemsCheck = this._items.Where(k => k.IsChecked == true).Select(k => k).ToList();
			//if (itemsCheck.Count() < 1) return;
			//if (currentInventorCode == "") return;
			string getDbPath = base.GetDbPath;

			//IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationFromDBADOProvider);
			//provider.ToPathDB = getDbPath;		//  base.GetDbPath;
			//provider.ProviderEncoding = base.Encoding;

			//IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportIturFromDBBlukProvider);
			//provider1.ToPathDB = getDbPath;		//  base.GetDbPath;
			//provider1.ProviderEncoding = base.Encoding;

			//IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportDocumentHeaderFromDBBlukProvider);
			//provider2.ToPathDB = getDbPath;		//  base.GetDbPath;
			//provider2.ProviderEncoding = base.Encoding;

			IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdate2SumByIturMakatDbBulkProvider);
			provider3.ToPathDB = getDbPath;		//  base.GetDbPath;
            provider3.ProviderEncoding = base.Encoding;
		   

                bool firstRun = true;
                base.Session = 0;
                while (true)
                {
                    if (base.CancellationToken.IsCancellationRequested)
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
					base.StepTotal = 1;
                    base.StepCurrent = 0;
					//foreach (FileItemViewModel item in itemsCheck)
					//{
						//base.StepCurrent++;
						//provider.Parms.Clear();
						//provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						//provider.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						//provider.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
						//provider.FromPathFile = item.Path;
						//provider.Import();

						//base.StepCurrent++;
						//provider1.Parms.Clear();
						//provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						//provider1.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						//provider1.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
						//provider1.FromPathFile = item.Path;
						//provider1.Import();

						//base.StepCurrent++;
						//provider2.Parms.Clear();
						//provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						//provider2.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						//provider2.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
						//provider2.FromPathFile = item.Path;
						//provider2.Import();

						base.StepCurrent++;
						provider3.Parms.Clear();
						provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						provider3.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						provider3.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
						provider3.FromPathFile = getDbPath;
						provider3.Import();

						base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Path : {0}", getDbPath));		  

                        if (base.CancellationToken.IsCancellationRequested)
                            break;
                  //  }

                    if (base.CancellationToken.IsCancellationRequested)
                        break;

                    firstRun = false;
                }
        
			// один импорт => одна сессия
			List<string> currentSessionCodeList = new List<string>();
			currentSessionCodeList.Add(newSessionCode);
			ISessionRepository sessionRepository = base.ServiceLocator.GetInstance<ISessionRepository>();
			List<string> currentDocumentCodeList = sessionRepository.Insert(currentSessionCodeList, base.GetDbPath);

			IDocumentHeaderRepository docRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
			docRepository.DeleteAllDocumentsWithoutAnyInventProduct(base.GetDbPath);

			IIturRepository iturRepository = base.ServiceLocator.GetInstance<IIturRepository>();
			iturRepository.RefillApproveStatusBit(base.GetDbPath);

			//foreach (var documentCode in currentDocumentCodeList)
			//{
			//	RunPrintReport(documentCode);
			//}

			//base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import : {0}", currentDocumentCodeList.Count));
			//IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
			//int countDocumentWithoutError = documentHeaderRepository.GetCountDocumentWithoutError(currentDocumentCodeList, base.GetDbPath);
			//base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import correct : {0}", countDocumentWithoutError));
			//int countDocumentWithError = documentHeaderRepository.GetCountDocumentWithError(currentDocumentCodeList, base.GetDbPath);
			//base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Total Document Import with error:  {0}", countDocumentWithError));
         

            FileLogInfo fileLogInfo = new FileLogInfo();
		    fileLogInfo.File = this.Path;

            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
			string getDbPath =  base.GetDbPath;

			//IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationFromDBADOProvider);
			//provider.ToPathDB = getDbPath;		//  base.GetDbPath;
			//provider.Clear();

			//IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportIturFromDBBlukProvider);
			//provider1.ToPathDB = getDbPath;		//  base.GetDbPath;
			//provider1.Clear();

			//IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportDocumentHeaderFromDBBlukProvider);
			//provider2.ToPathDB = getDbPath;		//  base.GetDbPath;
			//provider2.Clear();

			IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductFromDbBulkProvider);
			provider3.ToPathDB = getDbPath;		//  base.GetDbPath;
			provider3.Clear();

            base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "InventProduct"));
            this._iturRepository.ClearStatusBit(base.GetDbPath);
            UpdateLogFromILog();
        }

        #endregion

        public void RunPrintReport(string documentCode)
        {
			//if (this._isAutoPrint == false) return;

			//if (string.IsNullOrWhiteSpace(documentCode) == true) return;
			//IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
			//DocumentHeader documentHeader = documentHeaderRepository.GetDocumentHeaderByCode(documentCode, base.GetDbPath);
			//if (documentHeader == null) return;

			//IIturRepository iturRepository = base.ServiceLocator.GetInstance<IIturRepository>();
			//Itur itur = iturRepository.GetIturByDocumentCode(documentCode, base.GetDbPath);

			//Location location = null;
			//if (itur != null)
			//{
			//	ILocationRepository locationRepository = base.ServiceLocator.GetInstance<ILocationRepository>();
			//	location = locationRepository.GetLocationByCode(itur.LocationCode, base.GetDbPath);
			//}

			//try
			//{
			//	GenerateReportArgs args = new GenerateReportArgs();
			//	args.Customer = base.CurrentCustomer;
			//	args.Branch = base.CurrentBranch;
			//	args.Inventor = base.CurrentInventor;
			//	args.DbPath = base.GetDbPath;
			//	args.Report = this._report;
			//	args.Doc = documentHeader;
			//	args.ViewDomainContextType = ViewDomainContextEnum.ItursIturDoc;
			//	args.Itur = itur;
			//	args.Location = location;

			//	SelectParams spDoc = new SelectParams();
			//	List<string> searchDoc = new List<string> { documentCode };
			//	List<string> searchItur = new List<string> { documentHeader.IturCode };
			//	spDoc.FilterStringListParams.Add("DocumentCode", new FilterStringListParam()
			//	{
			//		Values = searchDoc
			//	});
			//	spDoc.FilterStringListParams.Add("IturCode", new FilterStringListParam()
			//	{
			//		Values = searchItur
			//	});
			//	args.SelectParams = spDoc;

			//	//this._generateReportRepository.RunPrintReport(args);
			//	//this._generateReportRepository.RunSaveReport(args, @"C:\Temp\testReport\output1.txt", ReportFileFormat.Excel);

			//	ImportPdaPrintQueue printQueue = _unityContainer.Resolve<ImportPdaPrintQueue>();
			//	printQueue.Enqueue(new PrintQueueItem() { GenerateReportArgs = args });
			//}
			//catch (Exception exc)
			//{
			//	_logger.ErrorException("RunPrintReport", exc);
			//}
        }
    }
}