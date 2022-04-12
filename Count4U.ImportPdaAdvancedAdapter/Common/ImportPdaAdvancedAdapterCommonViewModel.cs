using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common;
using System.Xml.Linq;
using Count4U.Common.ViewModel;

namespace Count4U.ImportPdaAdvancedAdapter
{
    public abstract class ImportPdaAdvancedAdapterCommonViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
    {
		public string _fileName {get; set;}
        protected bool _isAutoPrint;
        protected Count4U.GenerationReport.Report _report;

        //protected readonly List<string> _newDocumentCodeList;
		private readonly List<string> _newSessionCodeList;


        private readonly IIturRepository _iturRepository;

        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        private bool _isContinueGrabFiles;

        private InventProductAdvancedFieldIndex _inventProductAdvancedFieldIdx;

        protected ImportPdaAdvancedAdapterCommonViewModel(IServiceLocator serviceLocator,
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
            this._unityContainer = unityContainer;
            this._generateReportRepository = generateReportRepository;
            this._iturRepository = iturRepository;
            //this._newDocumentCodeList = new List<string>();
			this._newSessionCodeList = new List<string>();

            this._isDirectory = true;
            this._isSingleFile = false;
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
						base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						//if (System.IO.Path.GetExtension(base.Path) == ".xlsx") XlsxFormat = true; else XlsxFormat = false;
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

        public InventProductAdvancedFieldIndex InventProductAdvancedFieldIdx
        {
            get { return _inventProductAdvancedFieldIdx; }
            set { _inventProductAdvancedFieldIdx = value; }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return _yesNoRequest; }
        }

        public List<string> NewDocumentCodeList
        {
            get	{
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
            //this._newDocumentCodeList.Clear();
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
            ImportFromPdaCommandInfo pdaInfo = info as ImportFromPdaCommandInfo;
            if (pdaInfo != null)
            {
                this._isAutoPrint = pdaInfo.IsAutoPrint;
                this._report = pdaInfo.Report as Count4U.GenerationReport.Report;
                this._isContinueGrabFiles = pdaInfo.IsContinueGrabFiles;
            }
        }

        #region Implementation of IImportAdapter

        public abstract override void InitDefault(CBIState state = null);
        public abstract override void InitFromIni();

        public override void Import()
        {
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportInventProductAdvancedADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.ProviderEncoding = base.Encoding;
            string newSessionCode = Guid.NewGuid().ToString();
			this._newSessionCodeList.Add(newSessionCode);

            //List<string> newDocumentCodeList = new List<string>();
            //TODO Localization
            base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("NewSessionCode : {0}", newSessionCode));
            base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format("Path : {0}", this.Path));

            IMakatRepository makatRepository = base.ServiceLocator.GetInstance<IMakatRepository>();
            makatRepository.ProductMakatDictionaryRefill(base.GetDbPath, true);

            if (base.IsSingleFile == true)
            {
                //string newDocumentCode = Guid.NewGuid().ToString();
                //this._newDocumentCodeList.Add(newDocumentCode);
                //newDocumentCodeList.Add(newDocumentCode);
                provider.Parms.Clear();
                provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
                provider.Parms.AddInventProductAdvancedField(this._inventProductAdvancedFieldIdx);
               // provider.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;
                provider.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
                provider.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
                provider.FromPathFile = this.Path;
                StepCurrent = 1;
                provider.Import();
                //RunPrintReport(newDocumentCode);
            }
            else
            {
                bool firstRun = true;
                base.Session = 0;
                while (true)
                {
                    if (base.CancellationToken.IsCancellationRequested)
                        break;

                    List<string> files = Directory.GetFiles(this.Path).ToList();

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
                       // this._newDocumentCodeList.Add(newDocumentCode);
                        provider.Parms.Clear();
                        provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
						provider.Parms.AddInventProductAdvancedField(this._inventProductAdvancedFieldIdx);
                       // provider.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;
                        provider.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
                        provider.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
                        string finalPath = System.IO.Path.Combine(this.Path, filePath);
                        provider.FromPathFile = finalPath;
                        provider.Import();
                       // RunPrintReport(newDocumentCode);
                        //printQueue.Enqueue(new PrintQueueItem() { DocumentCode = newDocumentCode, ViewModel = this });


                        if (base.CancellationToken.IsCancellationRequested)
                            break;
                    }

                    if (base.CancellationToken.IsCancellationRequested)
                        break;

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
			}

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

            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            //this._iturRepository.RefillApproveStatus = false;
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportInventProductAdvancedADOProvider);
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
    }
}