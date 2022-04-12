using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Common.Interfaces;
using Count4U.Model.Main;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;

namespace Count4U.ImportCatalogAvivPOSAdapter
{
    public class ImportAvivPOSAdapterViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
        string _branchErpCode = String.Empty;

        private bool _withQuantityErp;

        private string _makatMask2;
        private string _barcodeMask2;

        private bool _importSupplier;

        public ImportAvivPOSAdapterViewModel(IServiceLocator serviceLocator,
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

        public bool WithQuantityErp
        {
            get { return this._withQuantityErp; }
            set
            {
                this._withQuantityErp = value;
                if (value == true)
                {
                    base.StepTotal = 4;
                }
                else
                {
                    base.StepTotal = 3;
                }
				if (this.ImportSupplier == true) base.StepTotal = base.StepTotal + 1;

                this.RaisePropertyChanged(() => WithQuantityErp);

                if (base.RaiseCanImport != null)
                    base.RaiseCanImport();
            }
        }

        public string BarcodeMask2
        {
            get { return _barcodeMask2; }
            set
            {
                _barcodeMask2 = value;
                RaisePropertyChanged(() => BarcodeMask2);
            }
        }

        public string MakatMask2
        {
            get { return _makatMask2; }
            set
            {
                this._makatMask2 = value;
                RaisePropertyChanged(() => MakatMask2);
            }
        }

        public bool ImportSupplier
        {
            get { return _importSupplier; }
            set
            {
                this._importSupplier = value;
				if (value == true)
				{
					base.StepTotal = 4;
				}
				else
				{
					base.StepTotal = 3;
				}
				if (this.WithQuantityErp == true) base.StepTotal = base.StepTotal + 1;
                RaisePropertyChanged(() => ImportSupplier);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this._maskViewModel = this.BuildMaskControl("1", base.BuildMaskRegionName());

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

			this._maskViewModel.PropertyChanged += MaskViewModel_PropertyChanged;

        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            if (_maskViewModel != null)
            {
                this._maskViewModel.PropertyChanged -= MaskViewModel_PropertyChanged;
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
						base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						if (System.IO.Path.GetExtension(base.Path) == ".xlsx") XlsxFormat = true; else XlsxFormat = false;
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
			//base.
            this.Import();
        }

        protected override void RunClear()
        {
            this.Clear();
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
		

            this._withQuantityErp = false;

         

            //init GUI
            this.PathFilter = "*.dat|*.dat|All files (*.*)|*.*";
            this._fileName = "item.dat";
            base.IsInvertLetters = true;
            base.IsInvertWords = true;

            this.MakatMask = "0000000000000{S}";
            this.BarcodeMask = "7290000000000{S}";


            base.Encoding = System.Text.Encoding.GetEncoding(862);

            if (this._withQuantityErp == true)
            {
                base.StepTotal = 4;
            }
            else
            {
                base.StepTotal = 3;
            }
			if (this.ImportSupplier == true) base.StepTotal = base.StepTotal + 1;
        }

	

        public override void InitFromIni()
        {
	            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            //init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            if (this._fileName.Contains("XXX") == true)
            {
                this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
            }

			//string folder = base.GetImportFolder();

			this.Path = System.IO.Path.GetFullPath(base.GetImportPath() + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
            base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
            base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);
        }

		//private string GetImportFolder()
		//{
		//	string folder = base.GetImportPath().Trim('\\');
		//	if (this.ParmsDictionary.ContainsKey("ImportPath") == true)
		//	{
		//		string folderCustomer = this.ParmsDictionary["ImportPath"];
		//		if (string.IsNullOrWhiteSpace(folderCustomer) == false)
		//		{
		//			folder = folderCustomer.Trim('\\');
		//		}
		//	}
		//	return folder;
		//}

        public override void Import()
        {
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAvivPOSADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.FastImport = base.IsTryFast;
            provider.FromPathFile = this.Path;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.ProviderEncoding = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

            IImportProvider provider1 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAvivPOSADOProvider1);
            provider1.ToPathDB = base.GetDbPath;
            provider1.FastImport = base.IsTryFast;
            provider1.FromPathFile = this.Path;
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.ProviderEncoding = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

            IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAvivPOSADOProvider2);
            provider2.ToPathDB = base.GetDbPath;
            provider2.FromPathFile = this.Path;
            provider2.Parms.Clear();
            provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider2.ProviderEncoding = base.Encoding;
            provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;


            if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
            {
                MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
                provider.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
                provider1.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
                provider2.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
            }
            if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
            {
                MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
                provider.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
                provider1.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
                provider2.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
            }

            StepCurrent = 1;
            provider.Import();

            StepCurrent = 2;
            provider1.Import();

            StepCurrent = 3;
            provider2.Import();

            if (WithQuantityErp == true)
            {
                IImportProvider provider3 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAvivPOSUpdateERPQuentetyADOProvider1);
                provider3.ToPathDB = base.GetDbPath;
                provider3.FromPathFile = this.Path;
                provider3.Parms.Clear();
                provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
                provider3.ProviderEncoding = base.Encoding; //this._encoding2;
                provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
                provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
                provider3.Parms[ImportProviderParmEnum.WithQuantityERP] = this.WithQuantityErp ? "1" : String.Empty;
                provider3.Parms[ImportProviderParmEnum.ERPNum] = this._branchErpCode;

                if (string.IsNullOrWhiteSpace(this.MakatMask) == false)
                {
                    MaskRecord makatMaskRecord = MaskTemplateRepository.ToMaskRecord(this.MakatMask);
                    provider3.Parms.Add(ImportProviderParmEnum.MakatMaskRecord, makatMaskRecord);
                }
                if (string.IsNullOrWhiteSpace(this.BarcodeMask) == false)
                {
                    MaskRecord barcodeMaskRecord = MaskTemplateRepository.ToMaskRecord(this.BarcodeMask);
                    provider3.Parms.Add(ImportProviderParmEnum.BarcodeMaskRecord, barcodeMaskRecord);
                }

                this.StepCurrent = 4;
                provider3.Import();
	        }

			if (this.ImportSupplier == true)
			{
				IImportProvider provider4 = this.GetProviderInstance(ImportProviderEnum.ImportSupplierAvivPOSADOProvider);
				provider4.ToPathDB = base.GetDbPath;
				provider4.FromPathFile = this.Path;
				provider4.Parms.Clear();
				provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider4.ProviderEncoding = base.Encoding;
				provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider4.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

				if (this.WithQuantityErp == true) this.StepCurrent = 5;
				else this.StepCurrent = 4;
				provider4.Import();
			}
            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAvivPOSADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion

        void MaskViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BarcodeMask")
            {
                MakatMask2 = _maskViewModel.BarcodeMask;
            }

            if (e.PropertyName == "MakatMask")
            {
                BarcodeMask2 = _maskViewModel.MakatMask;
            }
        }
    }
}
