using System;
using System.Text;
using System.Collections.Generic;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Commands;
using Count4U.Common.Helpers.Actions;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;
using System.IO;
using Count4U.Common.ViewModel;

namespace Count4U.ImportCatalogPriorityRenuarAdapter
{
	public class ImportCatalogPriorityRenuarAdapterViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		public string _fileName2 {get; set;}
		private CatalogParserPoints _catalogParserPoints;
		private bool _withFamily;
		private string _branchErpCode = String.Empty;

        public ImportCatalogPriorityRenuarAdapterViewModel(IServiceLocator serviceLocator,
          IContextCBIRepository contextCBIRepository,
          IEventAggregator eventAggregator,
          IRegionManager regionManager,
          ILog logImport,
          IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
          IUserSettingsManager userSettingsManager) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
	     {
			 base.ParmsDictionary.Clear();
          }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this._maskViewModel1 = this.BuildMaskControl("1", base.BuildMaskRegionName("1"));
			if (this._maskViewModel1 != null)
			{
				if (string.IsNullOrWhiteSpace(this._makatMask1) == false)
				{
					this._maskViewModel1.MakatMask = this._makatMask1;   //init Default
				}
				if (string.IsNullOrWhiteSpace(this._barcodeMask1) == false)
				{
					this._maskViewModel1.BarcodeMask = this._barcodeMask1; //init Default
				}
			}

			//this._maskViewModel2 = this.BuildMaskControl("2", base.BuildMaskRegionName("2"));
			//if (this._maskViewModel2 != null)
			//{
			//	if (string.IsNullOrWhiteSpace(this._makatMask2) == false)
			//	{
			//		this._maskViewModel2.MakatMask = this._makatMask2;   //init Default
			//	}
			//	if (string.IsNullOrWhiteSpace(this._barcodeMask2) == false)
			//	{
			//		this._maskViewModel2.BarcodeMask = this._barcodeMask2; //init Default
			//	}
			//}
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
						if (this._fileName1.Contains("XXX") == true)
						{
							this._fileName1 = this._fileName1.Replace("XXX", this._branchErpCode);
						}

						if (this._fileName2.Contains("XXX") == true)
						{
							this._fileName2 = this._fileName2.Replace("XXX", this._branchErpCode);
						}
						base.Path1 = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName1);
						base.Path2 = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName2);
						if (System.IO.Path.GetExtension(base.Path1) == ".xlsx") base.XlsxFormat1 = true; else base.XlsxFormat1 = false;
						if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
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

		public override bool CanImport()
		{
			if (this.WithFamily == true)
				return (String.IsNullOrWhiteSpace(base.Path1) == false)
					   && (String.IsNullOrWhiteSpace(base.Path2) == false)
					   && this.IsOkPath(this.Path1)
					   && this.IsOkPath(this.Path2);

			return (String.IsNullOrWhiteSpace(base.Path1) == false)
					 && this.IsOkPath(this.Path1)
					 ;
		}

        #region Implementation of IImportAdapter

        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
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

       
            this.MakatMask1 = "-11{N}";
            this.BarcodeMask1 = "-11{N}";

			//init GUI
			this.PathFilter1 = "*.dat|*.dat|All files (*.*)|*.*";
			this._fileName1 = "items.dat";
            base.Encoding = System.Text.Encoding.GetEncoding(862);
            base.IsInvertLetters = true;
            base.IsInvertWords = true;

			this.PathFilter2 = "*.csv|*.csv|All files (*.*)|*.*";
			this._fileName2 = "Family.csv";
			//base.Encoding = System.Text.Encoding.GetEncoding(1255);

			this._withFamily = false;

			//init Provider Parms
			this._catalogParserPoints = new CatalogParserPoints
			{
				CatalogMinLengthIncomingRow = 65,
				CatalogItemCodeStart = 1,
				CatalogItemCodeEnd = 13,
				CatalogItemNameStart = 35,
				CatalogItemNameEnd = 65,
				CatalogPriceBuyStart = 115,
				CatalogPriceBuyEnd = 124,
				HamarotBarcodeStart = 17,
				HamarotBarcodeEnd = 29
			};

			base.StepTotal = 2;

			if (this.WithFamily == true) base.StepTotal = base.StepTotal + 1;
        }             

        public override void InitFromIni()
        {
	        Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();

			//init GUI
			this._fileName1 = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName1);
			this._fileName2 = iniData.SetValue(ImportProviderParmEnum.FileName2, this._fileName2);
			if (this._fileName1.Contains("XXX") == true)
			{
				this._fileName1 = this._fileName1.Replace("XXX", this._branchErpCode);
			}
			if (this._fileName2.Contains("XXX") == true)
			{
				this._fileName2 = this._fileName2.Replace("XXX", this._branchErpCode);
			}
			base.Path1 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName1);
			base.Path2 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName2);
			if (System.IO.Path.GetExtension(base.Path1) == ".xlsx") base.XlsxFormat1 = true; else base.XlsxFormat1 = false;
			if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);

		
			//init Provider Parms
            this._catalogParserPoints = iniData.SetValue(this._catalogParserPoints);
        }

		public bool WithFamily
		{
			get { return this._withFamily; }
			set
			{
				this._withFamily = value;
				if (this.WithFamily == true) base.StepTotal = base.StepTotal + 1;
				RaisePropertyChanged(() => WithFamily);
			}
		}

        public override void Import()
        {
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

			string branchErpCode = String.Empty;
			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPriorityRenuarADOProvider);
            provider.ToPathDB = base.GetDbPath;
			provider.FastImport = base.IsTryFast;
            provider.Clear();
            provider.FromPathFile = this.Path1;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.Parms.AddCatalogParserPoints(this._catalogParserPoints);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.WithFamily] = this.WithFamily ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;            

            IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPriorityRenuarADOProvider1);
            provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
            provider1.FromPathFile = this.Path1;
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.Parms.AddCatalogParserPoints(this._catalogParserPoints);
            provider1.ProviderEncoding = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.WithFamily] = this.WithFamily ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;            

			//string barcodeMask = @"11{N}";
			//string makatMask = @"11{N}";
			if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
			{
			    //11{N};
				MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
				//MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(barcodeMask);
				provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
				provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
			}
			if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
			{
               MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
				//MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(makatMask);
			   provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			   provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
			}

            StepCurrent = 1;
            provider.Import();
            StepCurrent = 2;
            provider1.Import();


			if (this.WithFamily == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportFamilyPriorityRenuarADOProvider);
				provider3.ToPathDB = base.GetDbPath;
				//provider3.FastImport = false;
				provider3.FromPathFile = this.Path2;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = System.Text.Encoding.GetEncoding(1255);		//!! не берется с интерфейса
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = String.Empty; //false
				provider3.Parms[ImportProviderParmEnum.InvertWords] =  String.Empty; //false
				
				StepCurrent = 3;
				provider3.Import();
			}

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path1;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogPriorityRenuarADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();

			IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportFamilyPriorityRenuarADOProvider);
			provider3.ToPathDB = base.GetDbPath;
			provider3.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}
