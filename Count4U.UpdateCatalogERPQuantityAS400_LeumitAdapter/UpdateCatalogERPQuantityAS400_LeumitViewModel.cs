using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Count4U.Common.Helpers;
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
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;
using Count4U.Common.ViewModel;

namespace Count4U.UpdateCatalogERPQuantityAS400_LeumitAdapter
{																																				  
    public class UpdateCatalogERPQuantityAS400_LeumitViewModel : TemplateAdapterOneFileViewModel
    {
		public string _fileName {get; set;}
		private CatalogParserPoints _catalogParserPoints2;
		string _branchErpCode = String.Empty;

        public UpdateCatalogERPQuantityAS400_LeumitViewModel(IServiceLocator serviceLocator,
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
				this._branchErpCode = base.CurrentBranch.BranchCodeERP;
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

			this.PathFilter = "*.csv|*.csv|All files (*.*)|*.*";

            this._fileName = "TOXXX.csv";

			base.Encoding = System.Text.Encoding.GetEncoding(1255);
			base.IsInvertLetters = false;
			base.IsInvertWords = false;

			//this.InitCatalogParserPoints2();			

			base.StepTotal = 1;

        }

        public override void InitFromIni()
        {
			Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
	
			//init GUI
			this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName2, this._fileName);
				//todo
			if (this._fileName.Contains("XXX") == true)
			{
				this._fileName = this._fileName.Replace("XXX", this._branchErpCode);
			}
			base.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
		}

	
        protected override bool PreImportCheck()
        {
            try
            {
                string path = base.Path;

                if (!File.Exists(path))
                    return false;

				//if (base.CurrentBranch == null || String.IsNullOrWhiteSpace(base.CurrentBranch.BranchCodeERP))
				//    return false;

				if (base.CurrentBranch == null)
				{
					if (base.CurrentCustomer != null)
					{
						return true;
					}
					else
					{
						return false;
					}
				}

				if (String.IsNullOrWhiteSpace(base.CurrentBranch.BranchCodeERP) == true)
				{
					return false;
				}

                string line;

                using (StreamReader reader = new StreamReader(path))
                {
                    reader.ReadLine();
                    line = reader.ReadLine();
                }

                if (String.IsNullOrWhiteSpace(line))
                    return false;

                string[] split = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Count() < 5)
                    return false;

                string fileERP = split[0];
                string date = split[4];
                string warning = String.Empty;

                string branchERPTrimmed = base.CurrentBranch.BranchCodeERP.Trim().ToLower();

                if (branchERPTrimmed != fileERP.ToLower())
                {
                    warning = String.Format(Localization.Resources.ViewModel_XTechMeuhedet_ERP_Code_Different,
                              String.IsNullOrWhiteSpace(fileERP) ? "empty" : fileERP,
                              branchERPTrimmed);

                    if (base.ERPWarning(warning) == false)
                        return false;
                }

                int year = Int32.Parse(date.Substring(0, 4));
                int month = Int32.Parse(date.Substring(4, 2));
                int day = Int32.Parse(date.Substring(6, 2));

                DateTime inventorDate = base.CurrentInventor.InventorDate;
                if (inventorDate.Year != year || inventorDate.Month != month || inventorDate.Day != day)
                {
                    warning = Localization.Resources.ViewModel_ImportCatalogAs400_msgInventorDate;

                    return base.ERPWarning(warning);
                }
            }
            catch (Exception exc)
            {
                base.WriteErrorExceptionToAppLog("PreImportCheck", exc);

                return true;
            }

            return true;
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

			IImportProvider provider2 = this.GetProviderInstance(ImportProviderEnum.ImportCatalogAS400LeumitUpdateERPQuentetyADOProvider1);
			provider2.ToPathDB = base.GetDbPath;					   
			provider2.FastImport = base.IsTryFast;
			provider2.FromPathFile = this.Path;
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider2.Parms.AddCatalogParserPoints(this._catalogParserPoints2);
			provider2.ProviderEncoding = base.Encoding; //this._encoding2;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;	  // this._isInvertLetters2
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;		// this._isInvertWords2
			provider2.Parms[ImportProviderParmEnum.ERPNum] = branchErpCode;

			this.StepCurrent = 1;
			provider2.Import();

			FileLogInfo fileLogInfo = new FileLogInfo();
			fileLogInfo.File = this.Path;
			base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
        }

        protected override void OnPathChanged()
        {
            base.OnPathChanged();            
        }
    }
}
