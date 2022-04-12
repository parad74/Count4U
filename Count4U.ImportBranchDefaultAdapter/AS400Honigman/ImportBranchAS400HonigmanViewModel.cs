using System;
using System.Collections.Generic;
using System.Threading;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;
using System.IO;
using Count4U.Common.ViewModel;

namespace Count4U.ImportBranchAS400HonigmanAdapter
{
    public class ImportBranchAS400HonigmanViewModel : TemplateAdapterOneFileViewModel    
    {
		public string _fileName {get; set;}
		private string _currentCode = String.Empty;
		private CatalogParserPoints	 _catalogParserPoints1;
        private bool _isCreateDb;

		public ImportBranchAS400HonigmanViewModel(IServiceLocator serviceLocator,
          IContextCBIRepository contextCBIRepository,
          IEventAggregator eventAggregator,
          IRegionManager regionManager,
          ILog logImport,
          IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
          IUserSettingsManager userSettingsManager) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
            _isCreateDb = false;
			base.ParmsDictionary.Clear();
        }

        public bool IsCreateDb
        {
            get { return _isCreateDb; }
            set
            {
                _isCreateDb = value;
                RaisePropertyChanged(() => IsCreateDb);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
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
						//if (this._fileName.Contains("XXX") == true)
						//{
						//	this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
						//}
						base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;

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
            //this.Clear();
        }
     
        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
			base.ParmsDictionary.Clear();
			if (base.CurrentCustomer != null)
			{
				this._currentCode = base.CurrentCustomer.Code;
				base.AddParamsInDictionary(base.CurrentCustomer.ImportCatalogAdapterParms);
			}

			if (base.CurrentBranch != null)
			{
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

			this.PathFilter = "*.txt|*.txt|All files (*.*)|*.*";
 			this._fileName = "sfstrpf.txt";

			base.Encoding = System.Text.Encoding.GetEncoding(862);
			base.IsInvertLetters = true;
			base.IsInvertWords = true;
            this.IsCreateDb = false;
 
			this.InitCatalogParserPoints1();			//this._catalogParserPoints1
			//this.InitCatalogParserPoints2();			//this._catalogParserPoints2

			base.StepTotal = 1;
        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();

            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            //todo
			//if (this._fileName.Contains("XXX") == true)
			//{
			//    this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			//}
            base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);

		}

		private void InitCatalogParserPoints1()
		{
			//Field1: ERP Code – Branch (col 1-4)
			//Field2: Branch Name (col 5-26)
			this._catalogParserPoints1 = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 3,
				CatalogItemCodeStart = 1,
				CatalogItemCodeEnd = 3, 
				CatalogItemNameStart = 4, 
				CatalogItemNameEnd = 23
			};

		}


        public override void Import()
        {
			IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportBranchAS400HonigmanProvider);
			provider1.ToPathDB = base.GetDbPath;
			provider1.FromPathFile = this.Path;
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints1);
			provider1.ProviderEncoding = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.IsCreateDb] = this.IsCreateDb ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.CustomerCode] = this._currentCode;

			this.StepCurrent = 1;
			provider1.Import();

			FileLogInfo fileLogInfo = new FileLogInfo();
			fileLogInfo.File = this.Path;
			base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
        }
         
    }
}