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

namespace Count4U.ExportErpTextAdapter.Unizag
{
    public class ExportErpUnizagAdapterViewModel : ExportErpMakatViewModel
    {
       
        public ExportErpUnizagAdapterViewModel(IContextCBIRepository contextCbiRepository,
                                               ILog logImport,
                                               IServiceLocator serviceLocator,
                                               IUserSettingsManager userSettingsManager,
                                               IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {
            this._makatOriginal = true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

        }
		
        protected override string GetPathToIniFile()
        {
            return Path.Combine(base.GetModulesFolderPath(), "ExportErpUnizagAdapter.ini");
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


        protected override void RunExportInner(ExportErpCommandInfo info)
        {
            //_makat
            //_makatOriginal
            //base.BarcodeWithoutMask = this.Makat ? "1" : "";

            string branchCodeErp = string.Empty;
            if (base.CurrentBranch != null)
                branchCodeErp = base.CurrentBranch.BranchCodeERP;
            string inventorDate = string.Empty;
            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
                inventorDate = dt.ToString("ddMMyyyy");
            }

			string fileName = String.Format("INV_{0}_{1}", branchCodeErp, inventorDate); //result file only name without extension    
			string fileNameWithExtension = String.Format("{0}.txt", fileName);
			string fullPath = Path.Combine(base.PathToExportErp, fileNameWithExtension);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductUnizagERPFileProvider.ToString());
			provider.Parms.Clear();
			provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.FromPathDB = base.GetDbPath;
			provider.ToPathFile = fullPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			//"INV_" & CodeERP & "_" (InventorDate)DDMMYYYY & "_TSV" & ".txt"
			string fileName1 = String.Format("INV_{0}_{1}_TSV", branchCodeErp, inventorDate); //result file only name without extension    
			string fileNameWithExtension1 = String.Format("{0}.txt", fileName1);
			string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductUnizagERPFileProvider1.ToString());
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.FromPathDB = base.GetDbPath;
			provider1.ToPathFile = fullPath1;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider1.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider1.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


            SaveFileLog(fileName + "_Log");            
        }
    }
}