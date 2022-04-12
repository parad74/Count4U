using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTextAdapter.AS400Jafora
{
    public class ExportErpAS400JaforaViewModel : ExportErpMakatViewModel
    {
		public ExportErpAS400JaforaViewModel(IContextCBIRepository contextCbiRepository,
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
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpAS400JaforaAdapter.ini");
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
			//var location = base.LocationItems;
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
			string inventorDate2 = string.Empty;

            if (base.CurrentInventor != null)
            {
				//DateTime dt = base.CurrentInventor.InventorDate;
				DateTime dt = DateTime.Now;
				inventorDate = dt.ToString("ddMM");
				inventorDate1 = dt.Year.ToString();
				inventorDate2 = dt.ToString("ddMMyyyy");
            }
			//FILE
			//JAFORA_Counted_XXX.CSV 
			string fileName = String.Format("JAFORA_Counted_{0}", branchCodeErp); //result file only name without extension            
			string fileNameWithExtension = String.Format("{0}.csv", fileName);
			string fullPath = Path.Combine(base.PathToExportErp, fileNameWithExtension);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductAS400JaforaERPFileProvider.ToString());
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
			provider.ToPathFile = fullPath;

			ILocationRepository locationRepository = base.ServiceLocator.GetInstance<ILocationRepository>();
			//provider.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);
			List<string> listLocationCode = locationRepository.GetLocationCodeList(base.GetDbPath);
			if (listLocationCode.Contains("none") == true) listLocationCode.Remove("none");
			provider.Export(info.IsFull,  true, listLocationCode, info.IsFilterByIturs, info.IturCodeList);

			//FILE1
			//JAFORA_Not_Counted_XXX.CSV 
			string fileName1 = String.Format("JAFORA_Not_Counted_{0}", branchCodeErp); //result file only name without extension            
			string fileNameWithExtension1 = String.Format("{0}.csv", fileName1);
			string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductAS400JaforaERPFileProvider1.ToString());
			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = true;
			provider1.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider1.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
			provider1.Parms[ImportProviderParmEnum.InventorDate1] = inventorDate1;
			provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider1.ToPathFile = fullPath1;
			provider1.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


			//FILE2
			string fileName2 = String.Format("NF_INV_COUNT{0}_{1}", branchCodeErp, inventorDate2); //result file only name with extension            
			string fileNameWithExtension2 = String.Format("{0}.dat", fileName2);
			string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider2 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductAS400JaforaERPFileProvider2.ToString());
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = false;
			base.IsWithoutCatalog = true;
			provider2.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider2.Parms[ImportProviderParmEnum.InventorDate] = inventorDate2;
			provider2.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider2.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider2.ToPathFile = fullPath2;
			provider2.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


			SaveFileLog(fileName + "_Log");
        }
    }
}