using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.Constants;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.ExportErpTextAdapter;
using Count4U.Model;
using Count4U.Model.Extensions;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTemplateAdapter.NativPlusLadpc
{
    public class ExportErpNativPlusLadpcAdapterViewModel : ExportErpMakatViewModel
    {

		//protected bool _propertyStr1;
		//protected bool _propertyStr2;
		//protected bool _propertyStr3;
		protected string _photoPropertyName;
		protected string _pathPhoto;
		protected IConnectionDB _connection;

		public ExportErpNativPlusLadpcAdapterViewModel(IContextCBIRepository contextCbiRepository,
                                               ILog logImport,
                                               IServiceLocator serviceLocator,
                                               IUserSettingsManager userSettingsManager,
                                               IDBSettings dbSettings,
			IConnectionDB connection)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {
            this._makatOriginal = false;
			this._connection = connection;  
        }

	

	
        protected override string GetPathToIniFile()
        {
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpNativPlusLadpcAdapter.ini");
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
			//this._pathPhoto = Host + @"/" + User + @"/mINV/InventoryImages/" + base.CurrentCustomer.Code;	  //для ссылки в отчете 
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
			string inventorDate2 = string.Empty;
			string inventorDate3 = string.Empty;
			string inventorDatefileName = string.Empty;
			IIturRepository iturRepository = base.ServiceLocator.GetInstance<IIturRepository>();
			List<string> listIturCode = iturRepository.GetIturCodeList(base.GetDbPath);

			//drop and create 	  currentInventoryAdvanced
			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
				this.ServiceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(base.GetDbPath);

            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
				DateTime dtNow = DateTime.Now;
                inventorDate = dt.ToString("ddMMyyyy");
                inventorDate1 = dt.ToString("yyyyMMdd");
				inventorDate2 = dt.ToString("dd") + @"-" + dt.ToString("MM") + @"-" + dt.ToString("yyyy") + @"_" + dt.ToString("HH") + @"-" + dt.ToString("mm");
				inventorDate3 = dtNow.ToString("dd") + @"-" + dtNow.ToString("MM") + @"-" + dtNow.ToString("yyyy");
				inventorDatefileName = dt.ToString("ddMMyy");
            }

            //FILE1
			//	Ladpc_DD-MM-YYYY_HH-MM.csv
			string fileName1 = String.Format("Ladpc_{0}", inventorDate2); //result file only name without extension            
			string fileNameWithExtension1 = String.Format("{0}.csv", fileName1);
			string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductNativPlusLadpcCsvProvider1.ToString());
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
			//provider.Parms[ImportProviderParmEnum.SourcePath] = GetCurrentProperty();
			//provider.Parms[ImportProviderParmEnum.Path1] = @"ftp://ftp.boscom.com/mINV/InventoryImages/" + base.CurrentCustomer.Code;
			provider.Parms[ImportProviderParmEnum.Path1] = this._pathPhoto;
			provider.Parms[ImportProviderParmEnum.PropertyName] = this._photoPropertyName;
			provider.Parms[ImportProviderParmEnum.FileXlsx] = "0"; 

			provider.ToPathFile = fullPath1;
			//if (info.IsFull == true)
			//{
				provider.Export();
			//}
			//provider.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);
			

			//FILE		2
			//	Ladpc_XLSX_CurrentInventory_DD-MM-YYYY_HH-MM.xlsx
			string fileName2 = String.Format("Ladpc_XLSX_CurrentInventory_{0}", inventorDate2); //result file only name without extension            
			string fileNameWithExtension2 = String.Format("{0}.xlsx", fileName2);
			string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider2 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductNativPlusLadpcXslxProvider2.ToString());
			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.FromPathDB = base.GetDbPath;
			base.IsFromCatalog = true;
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
			//provider2.Parms[ImportProviderParmEnum.SourcePath] = GetCurrentProperty();
			provider2.Parms[ImportProviderParmEnum.Path1] = this._pathPhoto;
			provider2.Parms[ImportProviderParmEnum.PropertyName] = this._photoPropertyName;

			provider2.Parms[ImportProviderParmEnum.FileXlsx] = "1"; // for test only

			provider2.ToPathFile = fullPath2;
			//if (info.IsFull == true)
			//{
				provider2.Export();
			//}

            //FILE2
			//string fileName2 = String.Format("MerkavaCurrentInventory_MerkavaFormat_{0}", inventorDate2); //result file only name without extension            
			//string fileNameWithExtension2 = String.Format("{0}.xlsx", fileName2);
			//string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			//IExportERPProvider provider2 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductMerkavaProvider2_1.ToString());
			//provider2.Parms.Clear();
			//provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider2.FromPathDB = base.GetDbPath;
			//base.IsFromCatalog = false;
			//base.IsWithoutCatalog = true;
			//provider2.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			//provider2.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
			//provider2.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			//provider2.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider2.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider2.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			//provider2.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			//provider2.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;

			//provider2.Parms[ImportProviderParmEnum.FileXlsx] = "1"; // for test only

			//provider2.ToPathFile = fullPath2;
			//provider2.Export();

			SaveFileLog(fileName1 + "_Log");
        }
    }
}