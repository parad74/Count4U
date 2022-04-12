using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTextAdapter.NimrodAviv
{
    public class ExportErpNimrodAvivAdapterViewModel : ExportErpMakatViewModel
    {

		public ExportErpNimrodAvivAdapterViewModel(IContextCBIRepository contextCbiRepository,
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
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpNimrodAvivAdapter.ini");
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
			string localBranchCode = string.Empty;
			if (base.CurrentBranch != null)
			{
				localBranchCode = base.CurrentBranch.BranchCodeLocal.TrimStart('0');
				branchCodeErp = base.CurrentBranch.BranchCodeERP;
			}
            string inventorDate = string.Empty;
            string inventorDate1 = string.Empty;
			string inventorDate2 = string.Empty;
            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
                inventorDate = dt.ToString("ddMMyyyy");
                inventorDate1 = dt.ToString("yyyyMMdd");
				//inventorDate2 = dt.ToString("yyyy") + @"-" + dt.ToString("MM") + @"-" + dt.ToString("dd") + @"T" + dt.ToString("HH") + @":" + dt.ToString("mm") + @":00";
            }

			//FILE sXXX_cYYYY_InvCount.txt
			string fileName = String.Format("s{0}_cNNNN_InvCount", localBranchCode); //result file only name without extension      NNNN - IturNumber
			string fileNameWithExtension = String.Format("{0}.txt", fileName);
			string fullPath = Path.Combine(base.PathToExportErp, fileNameWithExtension);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductNimrodAvivERPFileProvider.ToString());
			provider.Parms.Clear();
			provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider.Parms[ImportProviderParmEnum.ERPNum] = localBranchCode;
			provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
			provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = "1";//info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider.ToPathFile = fullPath;
			IIturRepository iturRepository = base.ServiceLocator.GetInstance<IIturRepository>();

			iturRepository.RefillIturStatistic(base.GetDbPath); // надо заполнить статистику - TODO добавть в БД и пересчитывать на рефреш статус как со статистикой документов
			List<string> iturCodes = iturRepository.GetIturCodesWithInventProduct(base.GetDbPath); // обязательно RefillIturStatistic до того как

			provider.Export(false, info.IsFilterByLocations, info.LocationCodeList, true, iturCodes);

	
			//XXX_CountManager.txt
			string fileName1 = String.Format("{0}_CountManager", localBranchCode); //result file only name without extension            
			string fileNameWithExtension1 = String.Format("{0}.txt", fileName1);
			string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductNimrodAvivERPFileProvider1.ToString());
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = localBranchCode;
			provider1.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
			provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = "1";//info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider1.ToPathFile = fullPath1;
			provider1.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


            //FILE2
			string fileName2 = "addcatalog"; //All the items that are not exist in Catalog        
			string fileNameWithExtension2 = String.Format("{0}.txt", fileName2);
			string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider2 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductNimrodAvivERPFileProvider2.ToString());
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = false;
			base.IsWithoutCatalog = true;
			provider2.Parms[ImportProviderParmEnum.ERPNum] = localBranchCode;
			provider2.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
			provider2.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider2.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider2.ToPathFile = fullPath2;
			provider2.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


			//FILE4
			//DATA_xxx.csv 
			string fileName4 = "DATA_" + branchCodeErp; //result file only name without extension            
			string fileNameWithExtension4 = String.Format("{0}.csv", fileName4);
			string fullPath4 = Path.Combine(base.PathToExportErp, fileNameWithExtension4);

			IExportERPProvider provider4 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductNimrodAvivERPFileProvider3.ToString());
			provider4.Parms.Clear();
			provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider4.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider4.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider4.Parms[ImportProviderParmEnum.InventorDate] = inventorDate2;
			provider4.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider4.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider4.ToPathFile = fullPath4;
			provider4.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);
		
            SaveFileLog(fileName + "_Log");
        }
    }
}