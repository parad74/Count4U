using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.Constants;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.ExportErpTextAdapter;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTemplateAdapter.MerkavaGov
{
    public class ExportErpTemplateMerkavaGovAdapterViewModel : ExportErpMakatViewModel
    {
		protected string _photoPropertyName;
		protected string _pathPhoto;
		protected IConnectionDB _connection;
		public ExportErpTemplateMerkavaGovAdapterViewModel(IContextCBIRepository contextCbiRepository,
                                               ILog logImport,
                                               IServiceLocator serviceLocator,
                                               IUserSettingsManager userSettingsManager,
                                               IDBSettings dbSettings,
			IConnectionDB connection)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {
            this._makatOriginal = true;
			this._connection = connection; 
        }

      

        protected override string GetPathToIniFile()
        {
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpTemplateMerkavaGovAdapter.ini");
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
			IUserSettingsManager userSettingsManager = base.ServiceLocator.GetInstance<IUserSettingsManager>();
			//string Host = userSettingsManager.HostGet().Trim('\\');
			bool enableSsl;
			string Host = userSettingsManager.HostFtpGet(out enableSsl);
			string User = userSettingsManager.UserGet();
			this._photoPropertyName = userSettingsManager.InventProductPropertyPhotoSelectedItemGet();
			//this._pathPhoto = Host + @"/" + User + @"/mINV/InventoryImages/" + base.CurrentCustomer.Code;		    //для ссылки в отчете
			string relativeDb = base._contextCBIRepository.BuildLongCodesPath(base.CurrentInventor);
			string fromPhotoFolder = relativeDb.Trim('\\') + @"\" + FtpFolderName.Photo;
			this._pathPhoto = Host + @"/" + this._connection.RootFolderFtp(fromPhotoFolder).Replace(@"\", @"/"); //	 \mINV\Inventor\2018\1\1\<InventorCode>\Photo

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
			string nowDate2 = string.Empty;
			string inventorDatefileName = string.Empty;
			string selectLocationCode = string.Empty;
			string selectLocationCode1 = string.Empty;
			ILocationRepository locationRepository = base.ServiceLocator.GetInstance<ILocationRepository>();
			locationRepository.GetLocationCodeList(base.GetDbPath);

			//drop and create 	  currentInventoryAdvanced
			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
				this.ServiceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(base.GetDbPath);

			if (base.CurrentInventor != null)
			{
				DateTime dt = base.CurrentInventor.InventorDate;
				inventorDate = dt.ToString("ddMMyyyy");
				inventorDate1 = dt.ToString("yyyyMMdd");
				DateTime dtNow = DateTime.Now;
				nowDate2 = dtNow.ToString("dd") + @"-" + dtNow.ToString("MM") + @"-" + dtNow.ToString("yyyy") + @"_" + dtNow.ToString("hh") + @"-" + dtNow.ToString("mm");
				inventorDatefileName = dt.ToString("ddMMyy");
			}

			//{
				//FILE1
			//string fileName1 = String.Format("MerkavaCurrentInventory_AgriFormat_{0}", nowDate2); //result file only name without extension            
			//string fileNameWithExtension1 = String.Format("{0}.xlsx", fileName1);
			//string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			//IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductMerkavaProvider1.ToString());
			//provider.Parms.Clear();
			//provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider.FromPathDB = base.GetDbPath;
			//base.IsFromCatalog = true;
			//base.IsWithoutCatalog = true;
			//provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			//provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
			//provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			//provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			//provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			//provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			//provider.Parms[ImportProviderParmEnum.Path1] = this._pathPhoto;
			//provider.Parms[ImportProviderParmEnum.PropertyName] = this._photoPropertyName;
			//provider.Parms[ImportProviderParmEnum.FileXlsx] = "1"; // for test only

			//provider.ToPathFile = fullPath1;
			//provider.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);
			//}


			//{
				//FILE2
				//MerkavaCurrentInventory_MerkavaFormat_DD-MM-DDDD_HH-MM
				string fileName2 = String.Format("MerkavaCurrentInventory_MerkavaFormat_{0}", nowDate2); //result file only name without extension            
				string fileNameWithExtension2 = String.Format("{0}.xlsx", fileName2);
				string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

				IExportERPProvider provider2 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductMerkavaGovProvider2_1.ToString());
				provider2.Parms.Clear();
				provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider2.FromPathDB = base.GetDbPath;
				base.IsFromCatalog = false;
				base.IsWithoutCatalog = true;
				provider2.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
				provider2.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
				provider2.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
				provider2.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
				provider2.Parms[ImportProviderParmEnum.Path1] = this._pathPhoto;
				provider2.Parms[ImportProviderParmEnum.PropertyName] = this._photoPropertyName;

				provider2.Parms[ImportProviderParmEnum.FileXlsx] = "1"; // for test only

				provider2.ToPathFile = fullPath2;
				provider2.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);
			//}
				SaveFileLog(fileName2 + "_Log");

			{
				//FILE1
				//string fileName1 = String.Format("MerkavaCurrentInventory_AgriFormat_FIX_{0}", nowDate2); //result file only name without extension            
				//string fileNameWithExtension1 = String.Format("{0}.xlsx", fileName1);
				//string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

				//IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductMerkavaProvider3.ToString());
				//provider.Parms.Clear();
				//provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//provider.FromPathDB = base.GetDbPath;
				//base.IsFromCatalog = true;
				//base.IsWithoutCatalog = true;
				//provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
				//provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
				//provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
				//provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				//provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;

				//provider.Parms[ImportProviderParmEnum.FileXlsx] = "1"; // for test only

				//provider.ToPathFile = fullPath1;
			//	provider.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

				//SaveFileLog(fileName1 + "_Log");
			}

			{
				//FILE4
				//string fileName1 = String.Format("MerkavaCurrentInventory_AgriFormat_FIX_Supplier{0}", nowDate2); //result file only name without extension            
				//string fileNameWithExtension1 = String.Format("{0}.xlsx", fileName1);
				//string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

				//IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductMerkavaProvider4.ToString());
				//provider.Parms.Clear();
				//provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//provider.FromPathDB = base.GetDbPath;
				//base.IsFromCatalog = true;
				//base.IsWithoutCatalog = true;
				//provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
				//provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
				//provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
				//provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				//provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;

				//provider.Parms[ImportProviderParmEnum.FileXlsx] = "1"; // for test only

				//provider.ToPathFile = fullPath1;
			//	provider.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

				//SaveFileLog(fileName1 + "_Log");
			}


			{
				//FILE5
				//string fileName1 = String.Format("MerkavaCurrentInventory_AgriFormat_FIX_CurrentInventor{0}", nowDate2); //result file only name without extension            
				//string fileNameWithExtension1 = String.Format("{0}.xlsx", fileName1);
				//string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

				//IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductMerkavaProvider5.ToString());
				//provider.Parms.Clear();
				//provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				//provider.FromPathDB = base.GetDbPath;
				//base.IsFromCatalog = true;
				//base.IsWithoutCatalog = true;
				//provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
				//provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
				//provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
				//provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				//provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
				//provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;

				//provider.Parms[ImportProviderParmEnum.FileXlsx] = "1"; // for test only

				//provider.ToPathFile = fullPath1;
				//provider.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

				//SaveFileLog(fileName1 + "_Log");
			}
		
		}
    }
}