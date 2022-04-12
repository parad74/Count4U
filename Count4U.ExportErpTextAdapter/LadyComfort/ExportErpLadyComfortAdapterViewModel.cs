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

namespace Count4U.ExportErpTextAdapter.LadyComfort
{
    public class ExportErpLadyComfortAdapterViewModel : ExportErpMakatViewModel
    {

		public ExportErpLadyComfortAdapterViewModel(IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
            IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {
			//UseWithout Mask!
            this.MakatOriginal = true;
			this.Makat = false;
        }		

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this.MakatOriginal = true;
			this.Makat = false;
        }

        protected override string GetPathToIniFile()
        {
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpLadyComfortAdapter.ini");            
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
			this.MakatOriginal = true;
			this.Makat = false;
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
            string inventorDateFile = string.Empty;
			string inventorDateFile1 = string.Empty;
			string inventorDate = string.Empty;

            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
				inventorDate = dt.ToString("dd") + @"/" + dt.ToString("MM") + @"/" + dt.ToString("yy");
				inventorDateFile = dt.ToString("MMyy");
				inventorDateFile1 = dt.ToString("ddMMyyyy");
			}

			string fileName = String.Format("LadyComfot_{0}_{1}", branchCodeErp, inventorDateFile1); //result file only name without extension "LadyComfot_" & CodeERP & "_" (InventorDate)DDMMYYYY & ".CSV   
            string fileNameWithExtension = String.Format("{0}.csv", fileName);
            string fullPath = Path.Combine(base.PathToExportErp, fileNameWithExtension);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductLadyComfortERPFileProvider.ToString());
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
			base.IsWithoutCatalog = false;
			provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
            provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			provider.ToPathFile = fullPath;
			provider.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			//FILE2
			string fileName2 = String.Format("NF_INV_COUNT{0}_{1}", branchCodeErp, inventorDateFile1); //result file only name with extension            
			string fileNameWithExtension2 = String.Format("{0}.dat", fileName2);
			string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductLadyComfortERPFileProvider1.ToString());
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


			SaveFileLog(fileName + "_Log");         
        }

      
    }
}