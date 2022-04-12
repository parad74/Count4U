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

namespace Count4U.ExportErpTemplateAdapter.Clalit
{
    public class ExportErpTemplateClalitAdapterViewModel : ExportErpMakatViewModel
    {

		protected string _photoPropertyName;
		protected string _pathPhoto;
		protected IConnectionDB _connection;

		public ExportErpTemplateClalitAdapterViewModel(IContextCBIRepository contextCbiRepository,
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
			return Path.Combine(base.GetModulesFolderPath(), "ExportErpTemplateClalitAdapter.ini");
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
			
			//this._pathPhoto = Host + @"/" + User + @"/mINV/InventoryImages/" + base.CurrentCustomer.Code;				   //для ссылки в отчете
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
			string inventorDatefileName = string.Empty;
            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
				DateTime dtNow = DateTime.Now;
                inventorDate = dt.ToString("ddMMyyyy");
                inventorDate1 = dt.ToString("yyyyMMdd");
				inventorDate2 = dt.ToString("dd") + @"/" + dt.ToString("MM") + @"/" + dt.ToString("yyyy");
				inventorDatefileName = dtNow.ToString("dd") + @"-" + dtNow.ToString("MM") + @"-" + dtNow.ToString("yyyy") + @"-" + dtNow.ToString("hh") + @"-" + dtNow.ToString("mm");
            }

			//drop and create 	  currentInventoryAdvanced
			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
				this.ServiceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(base.GetDbPath);

			//CA_Sfira_FromDevices_DD-MM-YYYY-HH-MM.xlsx  //m
			string fileName1 = String.Format("CA_Sfira_FromDevices_{0}", inventorDatefileName); //result file only name without extension            
            string fileNameWithExtension1 = String.Format("{0}.xlsx", fileName1);
            string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductClalitXlsxTemplateProvider1.ToString());
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
			provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.Path1] = this._pathPhoto;
			provider.Parms[ImportProviderParmEnum.PropertyName] = this._photoPropertyName;
			provider.Parms[ImportProviderParmEnum.FileXlsx] = "1"; // for test only

			provider.Parms[ImportProviderParmEnum.FileXlsxTemplate] = "1";
			string xlsxTemplatePath = base._dbSettings.ReportTemplatePath() + @"\" + "XlsxTemplate" + @"\" + "Clalit" +@"\" + "CA_Sfira_FromDevices.xlsx";
			provider.Parms[ImportProviderParmEnum.XlsxTemplatePath] = xlsxTemplatePath;
            provider.ToPathFile = fullPath1;
			//provider.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);
			provider.Export();//info.IsFull, false, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


            //FILE2
			//string fileName2 = String.Format("NF_INV_COUNT{0}_{1}", branchCodeErp, inventorDate); //result file only name with extension            
			//string fileNameWithExtension2 = String.Format("{0}.dat", fileName2);
			//string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			//IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductMPLFileProvider1.ToString());
			//provider1.Parms.Clear();
			//provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//provider1.FromPathDB = base.GetDbPath;
			//base.IsFromCatalog = false;
			//base.IsWithoutCatalog = true;
			//provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
			//provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate1;
			//provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
			//provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			//provider1.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			//provider1.ToPathFile = fullPath2;
			//provider1.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);

	
            SaveFileLog(fileName1 + "_Log");
        }
    }
}