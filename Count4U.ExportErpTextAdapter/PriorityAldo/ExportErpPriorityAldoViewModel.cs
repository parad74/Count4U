﻿using System;
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

namespace Count4U.ExportErpTextAdapter.PriorityAldo
{
    public class ExportErpPriorityAldoAdapterViewModel : ExportErpMakatViewModel
    {

		public ExportErpPriorityAldoAdapterViewModel(IContextCBIRepository contextCbiRepository,
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
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpPriorityAldoAdapter.ini");
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
			//string fileName1 = "From_NextLine_012_17012017.xlsx"; //result file only name without extension  
			string fileName1 = String.Format("From_NextLine_{0}_{1}", branchCodeErp, inventorDate);
			string fileNameWithExtension1 = fileName1 + ".xlsx";
            string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);
			base.ExportToExcel = true;

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductPriorityAldoERPFileProvider.ToString());
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
			provider.Parms[ImportProviderParmEnum.FileXlsx] = base.ExportToExcel ? "1" : String.Empty;		
			provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
            provider.ToPathFile = fullPath1;
			provider.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


            //FILE2
            string fileName2 = String.Format("NF_INV_COUNT{0}_{1}", branchCodeErp, inventorDate); //result file only name with extension            
            string fileNameWithExtension2 = String.Format("{0}.dat", fileName2);
            string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			//пусть
			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductSapb1ZometsfarimERPFileProvider1.ToString());
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.FromPathDB = base.GetDbPath;
            base.IsFromCatalog = false;
            base.IsWithoutCatalog = true;
            provider1.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
            provider1.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
            provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
            provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
            provider1.ToPathFile = fullPath2;
			provider1.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

			

			SaveFileLog(fileName1 + "_Log");
        }
    }
}