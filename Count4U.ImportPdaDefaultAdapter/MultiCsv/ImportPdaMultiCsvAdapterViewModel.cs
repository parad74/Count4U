using System;
using System.Linq;
using System.Collections.Generic;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using System.IO;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface.Count4U;
using Count4U.Common.Interfaces.Adapters;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Count4U.Common.Helpers.Actions;
using Count4U.GenerationReport;
using Count4U.Common.ViewModel.Adapters.Import;
using System.Threading;
using Count4U.Common;
using System.Xml.Linq;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;

namespace Count4U.ImportPdaMultiCsvAdapter
{
	public class ImportPdaMultiCsvAdapterViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
    {
		public string FileName {get; set;}
		private readonly IIturRepository _iturRepository;
		private bool XlsxFormat;
		private string _branchErpCode = String.Empty;
		private bool _withQuantityERP;
		private readonly IGenerateReportRepository _generateReportRepository;
		private readonly IUnityContainer _unityContainer;
		private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
		private readonly List<string> _newSessionCodeList;
		private readonly DelegateCommand _makatMaskSelectCommand;

		private bool _isAutoPrint;
		private Count4U.GenerationReport.Report _report;
		private bool _isContinueGrabFiles;


		public ImportPdaMultiCsvAdapterViewModel(
			IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
			IIturRepository iturRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			IUserSettingsManager userSettingsManager,
			IGenerateReportRepository generateReportRepository,
			ITemporaryInventoryRepository temporaryInventoryRepository,
			 IUnityContainer unityContainer
           ) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
			this._unityContainer = unityContainer;
			this._generateReportRepository = generateReportRepository;
			this._iturRepository = iturRepository;
			this._newSessionCodeList = new List<string>();

			base.ParmsDictionary.Clear();
			this._isDirectory = true;
			this._isSingleFile = false;
			this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
			//this._makatMaskSelectCommand = new DelegateCommand(MakatMaskSelectCommandExecuted);

        }


		//public DelegateCommand MakatMaskSelectCommand
		//{
		//	get { return this._makatMaskSelectCommand; }
		//}

		//private void MakatMaskSelectCommandExecuted()
		//{
		//	ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
		//	payload.Settings = new Dictionary<string, string>();
		//	Utils.AddContextToDictionary(payload.Settings, base.Context);
		//	Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
		//	payload.Callback = r =>
		//	{
		//		MaskSelectedData data = r as MaskSelectedData;
		//		if (data != null)
		//			MakatMask = data.Value;
		//	};
		//	OnModalWindowRequest(payload);
		//}

		private int _countFiles;
		public int CountFiles
		{
			get { return _countFiles; }
			set
			{
				_countFiles = value;
				RaisePropertyChanged(() => CountFiles);

			}
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


		//private string _mask;
		//public string Mask
		//{
		//	get { return _mask; }
		//	set
		//	{
		//		this._mask = value;

		//		RaisePropertyChanged(() => Mask);
		//	}
		//}

		//public bool WithQuantityErp
		//{
		//	get { return _withQuantityERP; }
		//	set
		//	{
		//		this._withQuantityERP = value;
		//		//if (value == true)
		//		//{
		//		//	base.StepTotal = 5;
		//		//}
		//		//else
		//		//{
		//		//	base.StepTotal = 4;
		//		//}
		//		RaisePropertyChanged(() => WithQuantityErp);

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
						this.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this.FileName);

						this.XlsxFormat = true;
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this._newSessionCodeList.Clear();
			this._maskViewModel = this.BuildMaskControl("1", this.BuildMaskRegionName());
			_maskViewModel.IsCustomerVisible = false;
			_maskViewModel.IsBranchVisible = false;
			_maskViewModel.IsInventorVisible = false;
			_maskViewModel.IsMakatMaskVisible = false;
			
			
			if (this._maskViewModel != null)
			{
				if (string.IsNullOrWhiteSpace(this._makatMask) == false)
				{
					this._maskViewModel.MakatMask = this._makatMask;   //init Default
				}
				if (string.IsNullOrWhiteSpace(this._barcodeMask) == false)
				{
					this._maskViewModel.BarcodeMask = this._barcodeMask; //init Default
				}
			}
        }

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);
		}
		public override bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
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
			this._withQuantityERP = true;
			base.ParmsDictionary.Clear();
			if (base.CurrentCustomer != null)
			{
				base.AddParamsInDictionary(base.CurrentCustomer.ImportCatalogAdapterParms);
			}

			if (base.CurrentBranch != null)
			{
				this._branchErpCode = base.CurrentBranch.BranchCodeERP;
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

			//init GUI
			this.FileName = FileSystem.inData;
			this.PathFilter = "*.csv|*.csv|All files (*.*)|*.*";
			this.XlsxFormat = true;

            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;

           
        }

	    public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            //init GUI
            this.FileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this.FileName);
            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this.FileName);
			
			 this.XlsxFormat = true;

         	try
			{
				this.CountFiles = 0;
				if (base._isDirectory)
				{
					if (!Directory.Exists(this.Path))
						Directory.CreateDirectory(this.Path);

					if (Directory.Exists(this.Path) == true)
					{
						DirectoryInfo dir = new System.IO.DirectoryInfo(this.Path);
						this.CountFiles = dir.GetFiles().Length;
					}
				}
			}
			catch (Exception exc)
			{
				WriteErrorExceptionToAppLog("Create inData directory", exc);
			}
		}

		protected override bool PreImportCheck()
		{
			return true;
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

				DateTime updateDateTime = DateTime.Now;
				base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

				string branchErpCode = String.Empty;
				if (base.CurrentBranch != null)
				{
					branchErpCode = base.CurrentBranch.BranchCodeERP;
				}


				//LocationMultyCsvParser
				IImportProvider importLocaton = this.GetProviderInstance(ImportProviderEnum.ImportLocationMultiCsvProvider);
				importLocaton.ToPathDB = base.GetDbPath;
				importLocaton.FromPathFile = base.GetDbPath;
				importLocaton.Parms.Clear();
				importLocaton.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				importLocaton.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				importLocaton.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				importLocaton.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				importLocaton.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;

				//ImportIturMultiCsvDelete9999999Provider

				IImportProvider importItur = this.GetProviderInstance(ImportProviderEnum.ImportIturMultiCsvProvider);
				importItur.ToPathDB = base.GetDbPath;
				importItur.FromPathFile = base.GetDbPath;
				importItur.Parms.Clear();
				importItur.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				importItur.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				importItur.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				importItur.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				importItur.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;

				IImportProvider importErpIturCode = this.GetProviderInstance(ImportProviderEnum.ImportIturMultiCsvProvider1);
				importErpIturCode.ToPathDB = base.GetDbPath;
				importErpIturCode.FromPathFile = base.GetDbPath;
				importErpIturCode.Parms.Clear();
				importErpIturCode.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				importErpIturCode.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				importErpIturCode.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				importErpIturCode.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				importErpIturCode.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;


				//IImportProvider providerDoc = this.GetProviderInstance(ImportProviderEnum.ImportDocumentHeaderAddSpetialDocToIturBlukProvider);
				//providerDoc.ToPathDB = base.GetDbPath;
				//providerDoc.FromPathFile = base.GetDbPath;
				//providerDoc.Parms.Clear();
				//providerDoc.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//providerDoc.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				//providerDoc.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//providerDoc.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				//providerDoc.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;

				IImportProvider providerInventProduct = this.GetProviderInstance(ImportProviderEnum.ImportInventProductMultiCsvProvider);
				providerInventProduct.ToPathDB = base.GetDbPath;
				providerInventProduct.FromPathFile = base.GetDbPath;
				providerInventProduct.Parms.Clear();
				providerInventProduct.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerInventProduct.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerInventProduct.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerInventProduct.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerInventProduct.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
		
				providerInventProduct.ProviderEncoding = base.Encoding;

				if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
				{
					MaskRecord makatMaskRecord1 = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
					providerInventProduct.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord1);
				}

				//ImportIturMultiCsvDelete9999999Provider

				IImportProvider deleteItur9999999 = this.GetProviderInstance(ImportProviderEnum.ImportIturMultiCsvDelete9999999Provider);
				deleteItur9999999.ToPathDB = base.GetDbPath;
				deleteItur9999999.FromPathFile = base.GetDbPath;
				deleteItur9999999.Parms.Clear();
				deleteItur9999999.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				deleteItur9999999.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				deleteItur9999999.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				deleteItur9999999.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				deleteItur9999999.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;

				StepCurrent = 0;
		

				//=============== IP =====================
				bool firstRun = true;
				base.Session = 0;
				while (true)
				{
					if (base.CancellationToken.IsCancellationRequested)
						break;

					List<string> files = new List<string>();
					List<string> unsureFiles = new List<string>();
					DirectoryInfo dir = new System.IO.DirectoryInfo(this.Path);
					FileInfo[] sourceFiles = dir.GetFiles().OrderBy(f => f.LastWriteTime).ToArray();

					foreach (FileInfo fi in sourceFiles)
					{
						string fileName = fi.Name;
						if (System.IO.Path.GetExtension(fileName) == ".csv")
						//&& fileName.Contains(inventorCode) == true)  //Проверка на  inventorCode
						{
							files.Add(fileName.ToLower());
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

										  
					StepTotal = 2 * files.Count  + 1 + 1 + 1;
					//Location
					StepCurrent++;
					importLocaton.Import();

					// =============== Doc ===================
					//StepCurrent++;
					//// Clear doc
					//IDocumentHeaderRepository docRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
					//docRepository.DeleteAllDocumentsWithoutAnyInventProduct(base.GetDbPath);

					////ImportDocumentHeaderAddFristDocToIturBlukProvider
					//providerDoc.Import();
					//==========================================
			
					if (base.CancellationToken.IsCancellationRequested)
						break;

					
					// =============== Itur ===================
					foreach (string filePath in files)
					{
						StepCurrent++;
						string finalPath = System.IO.Path.Combine(this.Path, filePath);
						importItur.FromPathFile = finalPath;
						string newDocumentCode = Guid.NewGuid().ToString();
						importItur.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;
						importItur.Import();
					}

					foreach (string filePath in files)
					{
						StepCurrent++;
						string finalPath = System.IO.Path.Combine(this.Path, filePath);
						importErpIturCode.FromPathFile = finalPath;
						string newDocumentCode = Guid.NewGuid().ToString();
						importErpIturCode.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;
						importErpIturCode.Import();
					}
					

					// =============== Doc ===================
					//StepCurrent++;
					//providerDoc.Import();

					// =============== InventProduct ===================
					foreach (string filePath in files)
					{
						StepCurrent++;
						string finalPath = System.IO.Path.Combine(this.Path, filePath);
						providerInventProduct.FromPathFile = finalPath;
						string newDocumentCode = Guid.NewGuid().ToString();
						providerInventProduct.Parms[ImportProviderParmEnum.NewDocumentCode] = newDocumentCode;
						providerInventProduct.Import();
					}

					deleteItur9999999.Import();

					base.UnsureSourceFilesAfterImport(this.Path, unsureFiles, true);
					base.BackupSourceFilesAfterImport(this.Path, files, true);

					firstRun = false;
				}

				IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
				documentHeaderRepository.DeleteAllDocumentsWithoutAnyInventProduct(base.GetDbPath);


				FileLogInfo fileLogInfo = new FileLogInfo();
				if (this._isDirectory == true)
				{
					fileLogInfo.Directory = this.Path;
				}
				else
				{
					fileLogInfo.File = this.Path;
				}

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
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportInventProductMultiCsvProvider);
			provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion

		#region IImportPdaAdapter Members

		public void RunPrintReport(string documentCode)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}