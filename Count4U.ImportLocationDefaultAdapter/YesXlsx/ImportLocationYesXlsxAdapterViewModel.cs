using System;
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
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;

namespace Count4U.ImportLocationDefaultAdapter
{
    public class ImportLocationYesXlsxAdapterViewModel : TemplateAdapterFileFolderViewModel
    {
		public string _fileName {get; set;}
		private bool XlsxFormat;

		public ImportLocationYesXlsxAdapterViewModel(IServiceLocator serviceLocator,
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
			this._isDirectory = true;
			this._isSingleFile = false;
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
						this.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
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
            this.Import();
        }

        protected override void RunClear()
        {
            this.Clear();
        }      

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

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
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}
			//init GUI
			//this._fileName = "ImportLocation.xlsx";
			this._fileName = FileSystem.inData;
			this.PathFilter = "*.xlsx|*.xlsx|All files (*.*)|*.*";
			this.XlsxFormat = true;

            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;

            StepTotal = 2;
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
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			
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


        public override void Import()
        {
			base.LogImport.Clear();
			base.LogImport.Add(MessageTypeEnum.Trace, "Start ImportLocationYesXlsxAdapter");
			// now the same format providerLocationQ ==  providerLocationSN
			IImportProvider providerLocationQ = this.GetProviderInstance(ImportProviderEnum.ImportLocationYesXlsxProviderQ);
			providerLocationQ.ToPathDB = base.GetDbPath;
			providerLocationQ.Parms.Clear();
			providerLocationQ.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerLocationQ.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerLocationQ.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerLocationQ.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerLocationQ.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerLocationQ.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerLocationQ.ProviderEncoding = base.Encoding;

			IImportProvider providerLocationSN = this.GetProviderInstance(ImportProviderEnum.ImportLocationYesXlsxProviderSN);
			providerLocationSN.ToPathDB = base.GetDbPath;
			providerLocationSN.Parms.Clear();
			providerLocationSN.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			providerLocationSN.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			providerLocationSN.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			providerLocationSN.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			providerLocationSN.Parms[ImportProviderParmEnum.SheetNumberXlsx] = 1;
			providerLocationSN.Parms[ImportProviderParmEnum.SheetNameXlsx] = "גיליון1";
			providerLocationSN.ProviderEncoding = base.Encoding;


			//if (this.IsSingleFile == true)
			//{
			//	string fileTypeName = this.Path.ToUpper();

			//	if (fileTypeName.StartsWith("SN") == true)
			//	{
			//		providerLocationSN.FromPathFile = this.Path;
			//		StepCurrent++;
			//		providerLocationSN.Import();
			//	}

			//	if (fileTypeName.StartsWith("Q") == true)
			//	{
			//		providerLocationQ.FromPathFile = this.Path;
			//		StepCurrent++;
			//		providerLocationQ.Import();
			//	}

			//}
			//else
			//{
				var files = Directory.GetFiles(this.Path);
				StepTotal = files.Length;
				StepCurrent = 0;

				foreach (string filePath in files)
				{
					string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());
					if (fileTypeName.StartsWith("q_stock_") == true)
					{
						StepCurrent++;
						providerLocationQ.FromPathFile = filePath;
						providerLocationQ.Import();
					}
				}

				foreach (string filePath in files)
				{
					string fileTypeName = System.IO.Path.GetFileNameWithoutExtension(filePath.ToLower());

					if (fileTypeName.StartsWith("sn_stock_") == true)
					{
						StepCurrent++;
						providerLocationSN.FromPathFile = filePath;
						providerLocationSN.Import();
					}
				}
			//}

            FileLogInfo fileLogInfo = new FileLogInfo();
			if (this._isDirectory == true)
			{
				fileLogInfo.Directory = base.GetImportPath().Trim('\\');
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
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportLocationADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}