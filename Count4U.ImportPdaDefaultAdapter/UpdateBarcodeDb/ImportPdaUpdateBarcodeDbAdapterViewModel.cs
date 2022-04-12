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

namespace Count4U.ImportPdaUpdateBarcodeDbAdapter
{
    public class ImportPdaUpdateBarcodeDbAdapterViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
    {
        private readonly IUnityContainer _unityContainer;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
		private readonly List<string> _newSessionCodeList;
		private bool _isContinueGrabFiles;
		private bool _step1;
		private bool _step2;
		private bool _step3;

		public ImportPdaUpdateBarcodeDbAdapterViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
            IUserSettingsManager userSettingsManager,
			ITemporaryInventoryRepository temporaryInventoryRepository,
             IUnityContainer unityContainer) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
            this._unityContainer = unityContainer;
     		this._newSessionCodeList = new List<string>();

			base.ParmsDictionary.Clear();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

			_step1 = true;
			_step2 = false;
			_step3 = false;
        }

		//	if (this._items.Any(r => r.IsChecked) == false) TODO
		//				return false;


		public bool Step1
		{
			get { return _step1; }
			set
			{
				this._step1 = value;
				base.StepTotal = 0;
				if (this._step1 == true) base.StepTotal ++;
				if (this._step2 == true) base.StepTotal++;
				if (this._step3 == true) base.StepTotal++;
				RaisePropertyChanged(() => Step1);
			}
		}

		public bool Step2
		{
			get { return _step2; }
			set
			{
				this._step2 = value;
				base.StepTotal = 0;
				if (this._step1 == true) base.StepTotal++;
				if (this._step2 == true) base.StepTotal++;
				if (this._step3 == true) base.StepTotal++;
				RaisePropertyChanged(() => Step2);
			}
		}

		public bool Step3
		{
			get { return _step3; }
			set
			{
				this._step3 = value;
				base.StepTotal = 0;
				if (this._step1 == true) base.StepTotal++;
				if (this._step2 == true) base.StepTotal++;
				if (this._step3== true) base.StepTotal++;
				RaisePropertyChanged(() => Step3);
			}
		}

		public override bool CanImport()
		{
			return true;
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
			this._newSessionCodeList.Clear();
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
			this._isContinueGrabFiles = false;
			//ImportFromPdaCommandInfo pdaInfo = info as ImportFromPdaCommandInfo;
			//if (pdaInfo != null)
			//{
			//	this._report = pdaInfo.Report as Count4U.GenerationReport.Report;
			//	this._isAutoPrint = pdaInfo.IsAutoPrint;
			//	this._isContinueGrabFiles = pdaInfo.IsContinueGrabFiles;
			//}
		}

  

        #region Implementation of IImportAdapter

        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
            //init GUI
			//this._fileName = FileSystem.inData;
			//this.PathFilter = "*.sdf|*.sdf|All files (*.*)|*.*";
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;
            StepTotal = 1;
            Session = 0;
        }

        public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
  
        }

		public override void Import()																				//ImportIturFromDBBlukProvider
		{							
			//ImportIturFromDBProvider
			string newSessionCode = Guid.NewGuid().ToString();
			this._newSessionCodeList.Add(newSessionCode);
			base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("NewSessionCode : {0}", newSessionCode));

			IMakatRepository makatRepository = base.ServiceLocator.GetInstance<IMakatRepository>();
			makatRepository.ProductMakatDictionaryRefill(base.GetDbPath, true);

			string currentInventorCode = "";
			if (base.State.CurrentInventor != null) currentInventorCode = base.State.CurrentInventor.Code;

			if (currentInventorCode == "") return;
			string getDbPath = base.GetDbPath;


			IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductUpdateBarcodeFromDbBulkProvider);
			provider3.ToPathDB = getDbPath;		//  base.GetDbPath;
            provider3.ProviderEncoding = base.Encoding;

			//IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductBulkUpdateProvider2);
			//provider4.ToPathDB = getDbPath;		//  base.GetDbPath;
			//provider4.ProviderEncoding = base.Encoding;

			//IImportProvider provider5 = this.GetProviderInstance(ImportProviderEnum.ImportInventProductBulkUpdateProvider3);
			//provider5.ToPathDB = getDbPath;		//  base.GetDbPath;
			//provider5.ProviderEncoding = base.Encoding;
		   

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
					//base.StepTotal = 2;
                    base.StepCurrent = 0;

					if (Step1 == true)
					{
						base.StepCurrent++;
						provider3.Parms.Clear();
						provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						provider3.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
						provider3.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
						provider3.FromPathFile = getDbPath;
						provider3.Import();
					}

					//if (Step2 == true)
					//{
					//	base.StepCurrent++;
					//	provider4.Parms.Clear();
					//	provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					//	provider4.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
					//	provider4.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
					//	provider4.FromPathFile = getDbPath;
					//	provider4.Import();
					//}

					//if (Step3 == true)
					//{
					//	base.StepCurrent++;
					//	provider5.Parms.Clear();
					//	provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					//	provider5.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
					//	provider5.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
					//	provider5.FromPathFile = getDbPath;
					//	provider5.Import();
					//}

					FileLogInfo fileLogInfo = new FileLogInfo();
					if (base._isDirectory == true)
					{
						fileLogInfo.Directory = this.Path;
					}
					else
					{
						fileLogInfo.File = this.Path;
					}
					base.SaveFileLog(fileLogInfo);
					Thread.Sleep(500);

					//if (Step3 == true)
					//{
					//	base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
					//	string fileName = "MerkavaCurrentInventory_AgriFormat_FIX_CurrentInventor.xlsx";
					//	this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\inData\" + fileName);
					//	base.StepCurrent++;
					//	provider5.Parms.Clear();
					//	provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					//	provider5.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
					//	provider5.Parms[ImportProviderParmEnum.DBPath] = getDbPath;		//base.GetDbPath;
					//	provider5.ToPathDB = base.GetDbPath;
					//	provider5.FastImport = base.IsTryFast;
					//	provider5.FromPathFile = this.Path;
					//	provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
					//	provider5.ProviderEncoding = base.Encoding;
					//	provider5.Parms[ImportProviderParmEnum.InvertLetters] = "0";
					//	provider5.Parms[ImportProviderParmEnum.InvertWords] = "0";
					//	//provider5.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;
					//	provider5.Parms[ImportProviderParmEnum.FileXlsx] = "1" ;
					//	provider5.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
					//	provider5.Parms[ImportProviderParmEnum.SheetNameXlsx] = "PreviousInventory";

					//	provider5.Import();
					//}

						base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Path : {0}", getDbPath));		  

                        if (base.CancellationToken.IsCancellationRequested)
                            break;

                    if (base.CancellationToken.IsCancellationRequested)
                        break;

                    firstRun = false;
                }

			FileLogInfo fileLog = new FileLogInfo();

			this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\inData");
			fileLog.Directory = this.Path;
			base.SaveFileLog(fileLog);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
	
			base.LogImport.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "InventProduct"));

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