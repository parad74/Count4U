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

namespace Count4U.ImportSupplierSapb1ZometsfarimAdapter
{
    public class ImportSupplierSapb1ZometsfarimAdapterViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
		private string _branchErpCode = String.Empty;

		public ImportSupplierSapb1ZometsfarimAdapterViewModel(IServiceLocator serviceLocator,
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
						if (this._fileName.Contains("XXX") == true)
						{
							this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
						}
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
            this.Clear();
        }

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
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

			this._fileName = "FullCatalog.txt";
			this.PathFilter = "*.txt|*.txt|All files (*.*)|*.*";

            base.Encoding = System.Text.Encoding.GetEncoding(1255);
            base.IsInvertLetters = true;
			base.IsInvertWords = true;

            base.StepTotal = 1;
        }

        public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
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
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            //todo
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}
            base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);
        }


        public override void Import()
        {
			base.LogImport.Clear();
			base.LogImport.Add(MessageTypeEnum.Trace, "Start ImportSupplierSapb1Zometsfarim Adapter");
			IImportProvider provider6 = this.GetProviderInstance(ImportProviderEnum.ImportSupplierSapb1ZometsfarimProvider);
			provider6.ToPathDB = base.GetDbPath;
			provider6.FastImport = base.IsTryFast;
			provider6.FromPathFile = this.Path;
			provider6.Parms.Clear();
			provider6.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider6.ProviderEncoding = base.Encoding;
			provider6.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider6.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider6.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;

            this.StepCurrent = 1;
            provider6.Import();

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = this.Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
			base.LogImport.Clear();
			IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportSupplierSapb1ZometsfarimProvider);
			provider.ToPathDB = base.GetDbPath;
			provider.Clear();
			UpdateLogFromILog();

        }

    }
}
