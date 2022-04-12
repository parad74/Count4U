using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTextAdapter.NativExportErp
{
    public class ExportErpNativExportErpAdapterViewModel : ExportErpModuleBaseViewModel
    {
		public ExportErpNativExportErpAdapterViewModel(IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
            IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {
			//this._makatOriginal = false;
        }

        protected override string GetPathToIniFile()
        {
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpNativExportErpAdapter.ini");
        }

        protected override void InitDefault()
        {
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
			base.Encoding = System.Text.Encoding.GetEncoding(1255);
			base.IsInvertLetters = false;
			base.IsInvertWords = false;
			//init Provider Parms
        }

        protected override void InitFromIniFile()
        {            
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
			//init GUI
			//init Provider Parms
        }

		protected override void InitFromConfig(ExportErpCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = this.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						//TO DO
						//XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);
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

        protected override void RunExportInner(ExportErpCommandInfo info)
        {         
            string branchCodeErp = string.Empty;            
            if (base.CurrentBranch != null)
                branchCodeErp = base.CurrentBranch.BranchCodeERP;
            string inventorDate = string.Empty;
            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
                inventorDate = dt.ToString("ddMMyyyy");
            }

			string fileName2 = String.Format("INV_COUNT{0}_{1}", branchCodeErp, inventorDate); //result file only name with extension            
			string fileNameWithExtension2 = String.Format("{0}.xlsx", fileName2);
			string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductNativExportErpFileProvider.ToString());
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider1.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.FileXlsx] = "1";
			provider1.ToPathFile = fullPath2;
			provider1.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			SaveFileLog(fileName2 + "_Log");                        
        }       
    }
}