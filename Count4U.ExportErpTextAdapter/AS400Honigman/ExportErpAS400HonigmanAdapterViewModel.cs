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

namespace Count4U.ExportErpTextAdapter.AS400Honigman
{
	//AS400Honigman
	public class ExportErpAS400HonigmanAdapterViewModel: ExportErpModuleBaseViewModel
    {
		public ExportErpAS400HonigmanAdapterViewModel(IContextCBIRepository contextCbiRepository,
			ILog logImport,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
            IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {

        }

        protected override string GetPathToIniFile()
        {
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpAS400HonigmanAdapter.ini");
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
            if (base.CurrentBranch != null)   branchCodeErp = base.CurrentBranch.BranchCodeERP;
            string inventorDate = string.Empty;
			string inventorDate1 = string.Empty;
			string inventorDate2 = string.Empty;
            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
				inventorDate = dt.ToString("yyyyMMdd");   //YYYYMMDD
				inventorDate1 = dt.ToString("ddMMyyyy");
            }

			//string fileName = String.Format("INV_COUNT_{0}_{1}", branchCodeErp, inventorDate); //result file only name without extension   
			//string fileNameWithExtension = String.Format("{0}.dat", fileName);
			string fileName = "Results";
			string fileNameWithExtension = "Results.txt";
			string fullPath = Path.Combine(base.PathToExportErp, fileNameWithExtension);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductAS400HonigmanERPFileProvider.ToString());
			provider.Parms.Clear();
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = "1";
			provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider.FromPathDB = base.GetDbPath;
			provider.ToPathFile = fullPath;
			provider.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			//FILE2
			string fileName1 = "Results_barcodes";
			string fileNameWithExtension1 = "Results_barcodes.txt";
			string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductAS400HonigmanERPFileProvider1.ToString());
			provider1.Parms.Clear();
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = "1";
			provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider1.FromPathDB = base.GetDbPath;
			provider1.ToPathFile = fullPath1;
			provider1.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			//FILE2
			string fileName2 = String.Format("NF_INV_COUNT{0}_{1}", branchCodeErp, inventorDate1); //result file only name with extension            
			string fileNameWithExtension2 = String.Format("{0}.dat", fileName2);
			string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider2 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductAS400HonigmanERPFileProvider2.ToString());
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = false;
			base.IsWithoutCatalog = true;
			provider2.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider2.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider2.Parms[ImportProviderParmEnum.MakatWithoutMask] = "1" ;
			provider2.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider2.ToPathFile = fullPath2;
			provider2.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			SaveFileLog(fileName + "_Log");                        
        }       
    }
}