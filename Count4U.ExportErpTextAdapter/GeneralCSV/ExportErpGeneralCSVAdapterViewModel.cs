using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTextAdapter.GeneralCSV
{
    public class ExportErpGeneralCSVAdapterViewModel : ExportErpMakatViewModel
    {

        public ExportErpGeneralCSVAdapterViewModel(IContextCBIRepository contextCbiRepository,
                                               ILog logImport,
                                               IServiceLocator serviceLocator,
                                               IUserSettingsManager userSettingsManager,
                                               IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {
            this._makatOriginal = true;

        }

      

        protected override string GetPathToIniFile()
        {
            return Path.Combine(base.GetModulesFolderPath(), "ExportErpGeneralCSVAdapter.ini");
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
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

		}

        protected override void RunExportInner(ExportErpCommandInfo info)
        {
            string branchCodeErp = string.Empty;
            if (base.CurrentBranch != null)
                branchCodeErp = base.CurrentBranch.BranchCodeERP;
            string inventorDate = string.Empty;
            string inventorDate1 = string.Empty;
			string inventorDate2 = string.Empty;
            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
                inventorDate = dt.ToString("ddMMyyyy");
                inventorDate1 = dt.ToString("yyyyMMdd");
				inventorDate2 = dt.ToString("dd") + @"/" + dt.ToString("MM") + @"/" + dt.ToString("yyyy");
            }

            //FILE1
			string fileName1 = String.Format("KLITA_{0}", branchCodeErp); //result file only name without extension            
            string fileNameWithExtension1 = String.Format("{0}.dat", fileName1);
            string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductGeneralCSVERPFileProvider.ToString());
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.FromPathDB = base.GetDbPath;
            base.IsFromCatalog = true;
            base.IsWithoutCatalog = true;
            provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
            provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
            provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
            provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
            provider.ToPathFile = fullPath1;
			provider.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


            //FILE2
            string fileName2 = String.Format("NF_INV_COUNT{0}_{1}", branchCodeErp, inventorDate); //result file only name with extension            
            string fileNameWithExtension2 = String.Format("{0}.dat", fileName2);
            string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductGeneralCSVERPFileProvider1.ToString());
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.FromPathDB = base.GetDbPath;
            base.IsFromCatalog = false;
            base.IsWithoutCatalog = true;
            provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
            provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
            provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
            provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
            provider1.ToPathFile = fullPath2;
			provider1.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			//========PriorityGeneral
			string fileName3 = String.Format("KLITA_{0}", branchCodeErp); //result file only name without extension            
			string fileNameWithExtension3 = String.Format("{0}.tsv", fileName3);
			string fullPath3 = Path.Combine(base.PathToExportErp, fileNameWithExtension3);

			IExportERPProvider provider2 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductGeneralCSVERPFileProvider2.ToString());
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider2.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider2.Parms[ImportProviderParmEnum.InventorDate] = inventorDate2;
			provider2.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider2.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider2.ToPathFile = fullPath3;
			provider2.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			//// === MikiKupot
			////"FROM_" & CodeERP & "_" (InventorDate)DDMMYYYY & ".dat"
			string fileName4 = String.Format("FROM_{0}_{1}", branchCodeErp, inventorDate); //result file only name without extension            
			string fileNameWithExtension4 = String.Format("{0}.dat", fileName4);
			string fullPath4 = Path.Combine(base.PathToExportErp, fileNameWithExtension4);

			IExportERPProvider provider3 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductMikiKupotERPFileProvider.ToString());
			provider3.Parms.Clear();
			provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider3.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider3.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider3.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider3.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider3.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider3.ToPathFile = fullPath4;
			provider3.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			// === YellowPaz
			//SYYYYMMDD.XXX (XXX – Branch code ERP)	   inventorDate1
			string fileName5 = String.Format("S{0}.{1}", inventorDate1, branchCodeErp); //result file only name without extension            
			string fileNameWithExtension5 = fileName5; //String.Format("{0}.dat", fileName5);
			string fullPath5 = Path.Combine(base.PathToExportErp, fileNameWithExtension5);

			IExportERPProvider provider5 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductYellowPazERPFileProvider.ToString());
			provider5.Parms.Clear();
			provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider5.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider5.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider5.Parms[ImportProviderParmEnum.InventorDate] = inventorDate2;
			provider5.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider5.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider5.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider5.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider5.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider5.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider5.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider5.ToPathFile = fullPath5;
			provider5.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

            SaveFileLog(fileName1 + "_Log");
        }
    }
}