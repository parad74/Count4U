using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTextAdapter.Otech
{
    public class ExportErpOtechAdapterViewModel : ExportErpMakatViewModel
    {

		public ExportErpOtechAdapterViewModel(IContextCBIRepository contextCbiRepository,
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
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpOtechAdapter.ini");
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

        protected override void RunExportInner(ExportErpCommandInfo info)
        {
            string branchCodeErp = string.Empty;
            if (base.CurrentBranch != null)
                branchCodeErp = base.CurrentBranch.BranchCodeERP;
            string inventorDate = string.Empty;
			string inventorDate1 = string.Empty;
			
            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
                inventorDate = dt.ToString("ddMMyyyy");
				inventorDate1 = dt.ToString("dd") + @"/" + dt.ToString("MM") + @"/" + dt.ToString("yy");//ToString("dd-MM-yy");
            }

            //FILE1
			//lum_xxx.txt (XXX – ERP Code Branch)
			string fileName1 = String.Format("lum_{0}", branchCodeErp); //result file only name without extension            
			string fileNameWithExtension1 = String.Format("{0}.txt", fileName1);
			string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductOtechERPFileProvider.ToString());
			provider.Parms.Clear();
			provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider.Parms[ImportProviderParmEnum.InventorDate1] = inventorDate1;
			provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider.ToPathFile = fullPath1;
			provider.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			//FILE1_W
			//string fileName1_W = String.Format("INV_COUNT{0}W_{1}", branchCodeErp, inventorDate); //result file only name without extension            
			//string fileNameWithExtension1_W = String.Format("{0}.dat", fileName1_W);
			//string fullPath1_W = Path.Combine(base.PathToExportErp, fileNameWithExtension1_W);

			//IExportERPProvider provider_W = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductOtechERPFileProvider_W.ToString());
			//provider_W.Parms.Clear();
			//provider_W.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider_W.FromPathDB = base.GetDbPath;
			//base.IsFromCatalog = true;
			//base.IsWithoutCatalog = true;
			//provider_W.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			//provider_W.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			//provider_W.Parms[ImportProviderParmEnum.InventorDate1] = inventorDate1;
			//provider_W.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			//provider_W.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//provider_W.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider_W.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider_W.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			//provider_W.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			//provider_W.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			//provider_W.ToPathFile = fullPath1_W;
			//provider_W.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			////FILE1_M
			//string fileName1_M = String.Format("INV_COUNT{0}M_{1}", branchCodeErp, inventorDate); //result file only name without extension            
			//string fileNameWithExtension1_M = String.Format("{0}.dat", fileName1_M);
			//string fullPath1_M = Path.Combine(base.PathToExportErp, fileNameWithExtension1_M);

			//IExportERPProvider provider_M = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductOtechERPFileProvider_M.ToString());
			//provider_M.Parms.Clear();
			//provider_M.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider_M.FromPathDB = base.GetDbPath;
			//base.IsFromCatalog = true;
			//base.IsWithoutCatalog = true;
			//provider_M.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			//provider_M.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			//provider_M.Parms[ImportProviderParmEnum.InventorDate1] = inventorDate1;
			//provider_M.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			//provider_M.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//provider_M.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider_M.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider_M.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			//provider_M.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			//provider_M.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			//provider_M.ToPathFile = fullPath1_M;
			//provider_M.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			//////FILE1_T
			//string fileName1_T = String.Format("INV_COUNT{0}T_{1}", branchCodeErp, inventorDate); //result file only name without extension            
			//string fileNameWithExtension1_T = String.Format("{0}.dat", fileName1_T);
			//string fullPath1_T = Path.Combine(base.PathToExportErp, fileNameWithExtension1_T);

			//IExportERPProvider provider_T = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductOtechERPFileProvider_T.ToString());
			//provider_T.Parms.Clear();
			//provider_T.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider_T.FromPathDB = base.GetDbPath;
			//base.IsFromCatalog = true;
			//base.IsWithoutCatalog = true;
			//provider_T.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			//provider_T.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			//provider_T.Parms[ImportProviderParmEnum.InventorDate1] = inventorDate1;
			//provider_T.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			//provider_T.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//provider_T.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider_T.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider_T.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			//provider_T.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			//provider_T.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			//provider_T.ToPathFile = fullPath1_T;
			//provider_T.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


            //FILE2
            string fileName2 = String.Format("NF_INV_COUNT{0}_{1}", branchCodeErp, inventorDate); //result file only name with extension            
            string fileNameWithExtension2 = String.Format("{0}.txt", fileName2);
            string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductOtechERPFileProvider1.ToString());
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = false;
			base.IsWithoutCatalog = true;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider1.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
 			provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider1.ToPathFile = fullPath2;
			provider1.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


			SaveFileLog(fileName2 + "_Log");            
        }

        
    }
}