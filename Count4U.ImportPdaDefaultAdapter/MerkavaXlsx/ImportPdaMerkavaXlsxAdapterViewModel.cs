using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
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

namespace Count4U.ImportPdaMerkavaXlsxAdapter
{
	public class ImportPdaMerkavaXlsxAdapterViewModel : TemplateAdapterFileFolderViewModel, IImportPdaAdapter
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

		private bool _isAutoPrint;
		private Count4U.GenerationReport.Report _report;
		private bool _isContinueGrabFiles;


		public ImportPdaMerkavaXlsxAdapterViewModel(
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
			this._isDirectory = false;
			this._isSingleFile = true;	
			this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

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

		public bool WithQuantityErp
		{
			get { return _withQuantityERP; }
			set
			{
				this._withQuantityERP = value;
				//if (value == true)
				//{
				//	base.StepTotal = 5;
				//}
				//else
				//{
				//	base.StepTotal = 4;
				//}
				RaisePropertyChanged(() => WithQuantityErp);

				if (base.RaiseCanImport != null)
					base.RaiseCanImport();
			}
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
			this.FileName = FileSystem.inData + @"\" + @"Merkava-ToInventProduct.xlsx";
			this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
			this.XlsxFormat = true;

            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;

           
        }

		//public override void InitConfig()
		//{
		//	Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			//init GUI
			//this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
			//this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			//base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			//base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);
			//if (System.IO.Path.GetExtension(base.Path) == ".xlsx") this.XlsxFormat = true; else this.XlsxFormat = false;
		//	this.XlsxFormat = true;
		//}

		    public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            //init GUI
            this.FileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this.FileName);
            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this.FileName);
			
			 this.XlsxFormat = true;

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

		
				IImportProvider providerInventProductSN = this.GetProviderInstance(ImportProviderEnum.ImportInventProductMerkavaXlsxProviderSN);
				providerInventProductSN.ToPathDB = base.GetDbPath;
				providerInventProductSN.Parms.Clear();
				providerInventProductSN.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				providerInventProductSN.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				providerInventProductSN.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				providerInventProductSN.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				providerInventProductSN.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
				providerInventProductSN.Parms[ImportProviderParmEnum.FileXlsx] = "1";
				providerInventProductSN.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				providerInventProductSN.Parms[ImportProviderParmEnum.SheetNameXlsx] = "GoToInventProduct";
				providerInventProductSN.ProviderEncoding = base.Encoding;

				//IImportProvider providerInventProductQ = this.GetProviderInstance(ImportProviderEnum.ImportInventProductMerkavaXlsxProviderQ);
				//providerInventProductQ.ToPathDB = base.GetDbPath;
				//providerInventProductQ.Parms.Clear();
				//providerInventProductQ.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//providerInventProductQ.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				//providerInventProductQ.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//providerInventProductQ.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				//providerInventProductQ.Parms[ImportProviderParmEnum.NewSessionCode] = newSessionCode;
				//providerInventProductQ.Parms[ImportProviderParmEnum.FileXlsx] = "1";
				//providerInventProductQ.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
				//providerInventProductQ.Parms[ImportProviderParmEnum.SheetNameXlsx] = "GoToInventProduct";
				//providerInventProductQ.ProviderEncoding = base.Encoding;

				StepCurrent = 0;
				if (this.IsSingleFile == true)
				{
					StepTotal = 1;

					//StepCurrent++;
					//providerInventProductQ.FromPathFile = this.Path;
					//providerInventProductQ.Import();

					StepCurrent++;
					providerInventProductSN.FromPathFile = this.Path;
					providerInventProductSN.Import();

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
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportInventProductMerkavaXlsxProviderQ);
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