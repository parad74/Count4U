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
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;

namespace Count4U.ImportIturDefaultAdapter
{
    public class ImportIturDefaultAdapterViewModel : TemplateAdapterFileFolderViewModel
    {
        private readonly IIturRepository _iturRepository;
		public string _fileName {get; set;}

        public ImportIturDefaultAdapterViewModel(IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
            this._iturRepository = iturRepository;
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

						this.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						//if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
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
            this._fileName = "ImportIturTest.txt";
            this.PathFilter = "*.txt|*.txt|All files (*.*)|*.*";
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;

            StepTotal = 1;
        }

        public override void InitFromIni()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			//init GUI
            this._fileName = iniData.SetValue(ImportProviderParmEnum.FileName1, this._fileName);
            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + this._fileName);
			base.IsInvertLetters = iniData.SetInvertValue(ImportProviderParmEnum.InvertLetters, base.IsInvertLetters);
			base.IsInvertWords = iniData.SetInvertValue(ImportProviderParmEnum.InvertWords, base.IsInvertWords);
		
			//init Provider Parms
        }

        public override void Import()
        {
			base.LogImport.Add(MessageTypeEnum.Trace, "Start ImportIturDefault Adapter");
			base.LogImport.Add(MessageTypeEnum.Trace, String.Format("Path : {0}", this.Path));

            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportIturADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;

            provider.ProviderEncoding = base.Encoding;

            if (this.IsSingleFile == true)
            {
                provider.FromPathFile = this.Path;
                provider.Import();                
            }
            else
            {
                var files = Directory.GetFiles(this.Path);
                StepTotal = files.Length;
                StepCurrent = 0;

                foreach (string filePath in files)
                {
                    StepCurrent++;

                    string finalPath = System.IO.Path.Combine(this.Path, filePath);
                    provider.FromPathFile = finalPath;
                    provider.Import();                    
                }
            }

            FileLogInfo fileLogInfo = new FileLogInfo();
            if (this._isDirectory == true)
            {
				fileLogInfo.Directory = base.GetImportPath().Trim('\\');
            }
            else
            {
                fileLogInfo.File = this.Path;
            }

            this._iturRepository.RefillApproveStatusBit(base.GetDbPath);

            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportIturADOProvider);
            provider.ToPathDB = base.GetDbPath;
            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}