using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using Count4U.Common.Interfaces;
using System.Collections.Generic;
using Type = System.Type;
using Count4U.Model.Main;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using System.Xml.Linq;
using Count4U.Common;

namespace Count4U.ImportCatalogGazitVerifoneAdapter
{
    public class ImportGazitVerifoneAdapterViewModel : TemplateAdapterTwoFilesViewModel
    {
		public string _fileName1 {get; set;}
		public string _fileName2 {get; set;}
		public string _fileName3 {get; set;}

		private string _path3;

        private CatalogParserPoints _gazitPoints;
		private string _branchErpCode = String.Empty;

        public ImportGazitVerifoneAdapterViewModel(IServiceLocator serviceLocator,
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
						this._path3 = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName3);
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this._maskViewModel1 = this.BuildMaskControl("1", base.BuildMaskRegionName("1"));
			this._maskViewModel2 = this.BuildMaskControl("2", base.BuildMaskRegionName("2"));
        }       

        #region IImportAdapter Members
        //catalog.dat
        //Point(0, 15, 16));	 //15 - 0 + 1
        //Point(16, 35, 20));	//35 - 16 + 1
        //Point(36, 45, 10));	//45 - 32 + 1
        //hamarot.dat
        //Point(0, 12, 13));	 //12 - 0 + 1
        //Point(13, 28, 16));	//28 - 13 + 1

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

		

			//init GUI
			//this.PathFilter = "*.dat|*.dat|All files (*.*)|*.*";
            this._fileName1 = "catalog.dat";
            this._fileName2 = "hamarot.dat";
			this._fileName3 = "multi_barcode.csv";

            base.IsInvertLetters = true;
            base.IsInvertWords = false;
            base.Encoding = System.Text.Encoding.GetEncoding(862);						

			//init Provider Parms
            this._gazitPoints = new CatalogParserPoints
            {
                CatalogMinLengthIncomingRow = 3,
                CatalogItemCodeStart = 1,
                CatalogItemCodeEnd = 16,
                CatalogItemNameStart = 17,
                CatalogItemNameEnd = 36,
                CatalogPriceBuyStart = 37,
                CatalogPriceBuyEnd = 46,
                HamarotBarcodeStart = 1,
                HamarotBarcodeEnd = 13,
                HamarotItemCodeStart = 14,
                HamarotItemCodeEnd = 29

            };

            base.StepTotal = 3;

        }

        public override void InitFromIni()
        {

            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			//init GUI
            this._fileName1 = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName1);
			if (this._fileName1.Contains("XXX") == true)
			{
				this._fileName1 = this._fileName1.Replace("XXX", this._branchErpCode);
			}
            this._fileName2 = iniData.SetValue(ImportProviderParmEnum.FileName2, this._fileName2);
			if (this._fileName2.Contains("XXX") == true)
			{
				this._fileName2 = this._fileName2.Replace("XXX", this._branchErpCode);
			}
            base.Path1 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName1);
            base.Path2 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName2);
			this._path3 = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName3);
			if (System.IO.Path.GetExtension(base.Path1) == ".xlsx") base.XlsxFormat1 = true; else base.XlsxFormat1 = false;
			if (System.IO.Path.GetExtension(base.Path2) == ".xlsx") base.XlsxFormat2 = true; else base.XlsxFormat2 = false;
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);

			//init Provider Parms
			this._gazitPoints = iniData.SetValue(this._gazitPoints);
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

			//ВНИМАНИЕ используется в 2 адаптерах
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogGazitVerifoneADOProvider);
            provider.ToPathDB = base.GetDbPath;
			provider.FastImport = base.IsTryFast;
            provider.FromPathFile = base.Path1;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.Parms.AddCatalogParserPoints(this._gazitPoints);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

            if (string.IsNullOrWhiteSpace(this.BarcodeMask1) == false)
            {
                MaskRecord barcodeMaskRecord1 = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask1);
                provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord1);
            }

            if (string.IsNullOrWhiteSpace(this.MakatMask1) == false)
            {
                MaskRecord makatMaskRecord1 = MaskTemplateRepository.ToMaskRecord(this.MakatMask1);
                provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord1);
            }

			//ВНИМАНИЕ используется в 2 адаптерах
            IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportHamarotGazitVerifoneADOProvider1);//!1
            provider1.ToPathDB = base.GetDbPath;
			provider1.FastImport = base.IsTryFast;
            provider1.FromPathFile = base.Path2;
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.Parms.AddCatalogParserPoints(this._gazitPoints);
            provider1.ProviderEncoding = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			//ВНИМАНИЕ используется в 2 адаптерах
            IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportHamarotGazitVerifoneADOProvider2);
            provider2.ToPathDB = base.GetDbPath;
			provider2.FastImport = base.IsTryFast;
            provider2.FromPathFile = base.Path2;
            provider2.Parms.Clear();
            provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider2.Parms.AddCatalogParserPoints(this._gazitPoints);
            provider2.ProviderEncoding = base.Encoding;
            provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

            if (string.IsNullOrWhiteSpace(this.BarcodeMask2) == false)
            {
                MaskRecord barcodeMaskRecord2 = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask2);
                provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord2);
                provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord2);
            }
            if (string.IsNullOrWhiteSpace(this.MakatMask2) == false)
            {
                MaskRecord makatMaskRecord2 = MaskTemplateRepository.ToMaskRecord(this.MakatMask2);
                provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord2);
                provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord2);
            }

	        StepCurrent = 1;
            provider.Import();
            StepCurrent = 2;
            provider1.Import();
            StepCurrent = 3;
            provider2.Import();

			//ВНИМАНИЕ используется в 2 адаптерах
			if (File.Exists(this._path3) == true)
			{
				IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogComaxASPMultiBarcodeADOProvider2);
				provider3.ToPathDB = base.GetDbPath;
				provider3.FastImport = base.IsTryFast;
				provider3.FromPathFile = this._path3;
				provider3.Parms.Clear();
				provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider3.ProviderEncoding = base.Encoding;
				provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider3.Parms[ImportProviderParmEnum.ERPNum] = _branchErpCode;
				StepCurrent = 4;
				provider3.Import();
			}

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = Path2;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogGazitVerifoneADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            //IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportHamarotGazitVerifoneADOProvider);
            //provider1.ToPathDB = base.GetDbPath;
            //provider1.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}