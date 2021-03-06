using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using System.Text;
using Count4U.Common.Services.Ini;
using Count4U.Model.Count4U.Validate;
using Count4U.Common.Helpers.Ftp;
using System.Net;
using Count4U.Common.ViewModel.Adapters.Import;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Model.Interface.Count4U;

namespace Count4U.ExportPdaMerkavaSQLiteAdapter
{
	public class ExportPdaMerkavaSQLiteAdapterViewModel : ExportPdaModuleBaseViewModel	  //все для динамической смены\выбора  ExportPda Adapter
    {
		//private readonly IDBSettings _dbSettings;
		//private string _misCommunicatorPath = "";
		private string _misiDnextDataPath = "";

		private string _host = "";
		private string _hostToApp = "";
		private string _hostmINV = "";
		private string _hostFromApp = "";
		private string _hostUser = "";
		private string _hostPassword = "";
		private string _hostConfig = "";
		private readonly IZip _zip;
		private string _threadCode;

		public ExportPdaMerkavaSQLiteAdapterViewModel(
            IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
			IDBSettings dbSettings,
			IZip zip)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager)
        {
			this._zip = zip;
			//this._host = this._userSettingsManager.HostGet().Trim('\\');
			bool enableSsl;
			this._host = _userSettingsManager.HostFtpGet(out enableSsl);
			this._hostUser = this._userSettingsManager.UserGet();
			this._hostPassword = this._userSettingsManager.PasswordGet();
			this._hostmINV = @"mINV";
			this._hostToApp = @"ToApp";
			this._hostFromApp = @"FromApp";
			this._hostConfig = @"Config";
			//this._threadCode = Guid.NewGuid().ToString();
			this._threadCode = "ThreadCode";
        }


        protected override void InitDefault()		
		{
			base.IncludeCurrentInventor = false;
			base.IncludePreviousInventor = false;
			base.IncludeProfile = false;
			base.CreateZipFile = false;
			
			base.Encoding = System.Text.Encoding.GetEncoding(1255);
			base.InvertLetters  = false;
			base.InvertWords = false;
			base.CutAfterInvert = false;

			base.IncludeCurrentInventor = false;
			base.IncludePreviousInventor = false;
			base.IncludeProfile = false;
			base.CheckBaudratePDA = "0";
			//string exportDateTime = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
			//if (base.State != null)
			//{
			//	if (base.State.CurrentInventor != null) this._threadCode = base.State.CurrentInventor.Code + '^' + exportDateTime;
			//}
		}

		protected override ExportCommandInfo InitFromConfig(ExportCommandInfo info, CBIState state)
		{
			if (state == null) return info;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = this.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						//string exportPath = XDocumentConfigRepository.GetExportPath(this, configXDoc);
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
			return info;
		}
		public string Host
		{
			get { return _host; }
			set { _host = value; }
		}

		public string HostUser
		{
			get { return _hostUser; }
			set { _hostUser = value; }
		}

		public string HostPassword
		{
			get { return _hostPassword; }
			set { _hostPassword = value; }
		}

		
		protected override void InitFromIniFile()	 //из кастомера  
		{	
        }

        public override bool CanExport()
        {
            return true;
        }

	
        protected override void RunExportInner(ExportCommandInfo info)
        {
            base.WithoutProductName = info.WithoutProductName ? "1" : "";
            base.MakatWithoutMask = info.MakatWithoutMask ? "1" : "";
			//base.CheckBaudratePDA = info.CheckBaudratePDA ? "1" : "";
            base.BarcodeWithoutMask = info.BarcodeWithoutMask ? "1" : "";

			base.IncludeCurrentInventor = info.IncludeCurrentInventor;
			base.IncludePreviousInventor = info.IncludePreviousInventor;
			//base.IncludeProfile = info.IncludeProfile;
			base.CreateZipFile = info.CreateZipFile;
																		 
            this.Export();
        }

		//public override void RunClear(ExportClearCommandInfo info)
		//{
		//	this.Clear();
		//	if (info.Callback != null) Utils.RunOnUI(info.Callback);
		//}

		public override void RunClear()
		{
			this.Clear();
		}

		public override void ClearFolders(CBIState state) { }

		public string GetFileBD3()
		{
			string fileNameBD3 = this._threadCode + ".db3";
			return fileNameBD3;
		}

		//public string GetFileBD3()
		//{
		//	string inventorCode = String.Empty;
		//	string branchCode = String.Empty;
		//	string customerCode = String.Empty;

		//	if (base.CurrentInventor != null)
		//	{
		//		inventorCode = base.CurrentInventor.Code;
		//	}
		//	if (base.CurrentBranch != null)
		//	{
		//		branchCode = base.CurrentBranch.Code;
		//	}
		//	if (base.CurrentCustomer != null)
		//	{
		//		customerCode = base.CurrentCustomer.Code;
		//	}

		//	string objectCode = inventorCode;
		//	if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = branchCode;
		//	if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = customerCode;
		//	//string pathBD3 = System.IO.Path.GetDirectoryName(folder) + @"\" + objectCode + ".db3";
			
		//	string fileNameBD3 = objectCode + ".db3";
		//	return fileNameBD3;
		//}


		public void Export()
		{
			string exportDateTime = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
			if (base.State != null)
			{
				if (base.State.CurrentInventor != null) this._threadCode = base.State.CurrentInventor.Code + '^' + exportDateTime;
			}

			base.LogImport.Clear();
			string processCont4uFolder = base.GetExportToPDAFolderPath(false) + @"\Process";
			string objectCodeFolder = base.GetExportToPDAFolderPath(true);
			string toFileName = this.GetFileBD3();
			string db3Path = processCont4uFolder + @"\" + toFileName;
			//DateTime dt = DateTime.Now;
			//string exportDateTime = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + "_" + dt.ToString("HH") + "_" + dt.ToString("mm") + "_" + dt.ToString("ss");


			base.ClearFolder(processCont4uFolder);
			base.ClearFolder(objectCodeFolder);

			string customerCode = String.Empty;
			string branchErpCode = String.Empty;
			string inventorCode = String.Empty;

			if (base.CurrentCustomer != null)
			{
				customerCode = base.CurrentCustomer.Code;
			}

			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

			if (base.CurrentInventor != null)
			{
				inventorCode = base.CurrentInventor.Code;
			}

			//drop and create 	  currentInventoryAdvanced
			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
				this.ServiceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced( base.GetDbPath);

			//"Catalog";
			IImportProvider provider =																		   //CatalogMerkavaSdf2SqliteParser
			 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCatalogMerkavaSdf2SqliteProvider.ToString());

			provider.Parms.Clear();
			provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			provider.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			provider.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			provider.FromPathFile  = base.GetDbPath;
			provider.ToPathDB = db3Path;

			provider.Clear();
			provider.Import();

		 //   //"Location"
			IImportProvider provider1 =																   //LocationMerkavaSdf2SqliteParser
		 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportLocationMerkavaSdf2SqliteProvider.ToString());

			provider1.Parms.Clear();
			provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider1.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			provider1.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.IncludeCurrentInventor] = base.IncludeCurrentInventor ? "1" : String.Empty;
			provider1.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider1.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			provider1.FromPathFile = base.GetDbPath;
			provider1.ToPathDB = db3Path;

		   provider1.Clear();
		   provider1.Import();

		 //   //"BuildingConfig"
		   IImportProvider provider2 =																			//BuildingConfigMerkavaSdf2SqliteParser
			   base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportBuildingConfigMerkavaSdf2SqliteProvider.ToString());

			provider2.Parms.Clear();
			provider2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider2.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			provider2.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			provider2.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			provider2.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			provider2.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider2.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider2.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			provider2.FromPathFile = base.GetDbPath;
			provider2.ToPathDB = db3Path;

			provider2.Clear();
			provider2.Import();

		 //   //"PropertyStr6List"
			IImportProvider provider3 =																	   //PropertyStrListMerkavaSdf2SqliteParser
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr6ListMerkavaSdf2SqliteProvider.ToString());

			provider3.Parms.Clear();
			provider3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider3.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			provider3.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			provider3.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			provider3.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			provider3.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider3.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider3.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			provider3.FromPathFile = base.GetDbPath;
			provider3.ToPathDB = db3Path;

			provider3.Clear();
			provider3.Import();

		 //   //"PropertyStr7List"
			IImportProvider provider4 =																		//PropertyStrListMerkavaSdf2SqliteParser
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr7ListMerkavaSdf2SqliteProvider.ToString());

			provider4.Parms.Clear();
			provider4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider4.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			provider4.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			provider4.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			provider4.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			provider4.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider4.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider4.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider4.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			provider4.FromPathFile = base.GetDbPath;
			provider4.ToPathDB = db3Path;

			provider4.Clear();
			provider4.Import();

			//   //"PropertyStr18List"
			IImportProvider provider18 =																		//PropertyStrListMerkavaSdf2SqliteParser
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr18ListMerkavaSdf2SqliteProvider.ToString());

			provider18.Parms.Clear();
			provider18.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider18.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			provider18.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			provider18.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			provider18.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			provider18.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			provider18.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			provider18.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider18.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider18.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider18.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			provider18.FromPathFile = base.GetDbPath;
			provider18.ToPathDB = db3Path;

			provider18.Clear();
			provider18.Import();

			// "PreviousInventory"
			if (base.IncludePreviousInventor == true)
			{
				IImportProvider provider5 =																		//PreviousInventoryMerkavaSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPreviousInventoryMerkavaSdf2SqliteProvider.ToString());

				provider5.Parms.Clear();
				provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider5.Parms[ImportProviderParmEnum.FileType] = base.FileType;
				provider5.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
				provider5.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
				provider5.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
				provider5.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
				provider5.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				provider5.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider5.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
				provider5.FromPathFile = base.GetDbPath;
				provider5.ToPathDB = db3Path;

				provider5.Clear();
				provider5.Import();
			}

			if (base.IncludeCurrentInventor == true)					
			{
				IImportProvider provider6 =																  //CurrentInventoryMerkavaSdf2SqliteParser
			 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCurrentInventorMerkavaSdf2SqliteProvider.ToString());

				provider6.Parms.Clear();
				provider6.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider6.Parms[ImportProviderParmEnum.FileType] = base.FileType;
				provider6.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
				provider6.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
				provider6.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
				provider6.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
				provider6.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				provider6.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider6.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
				provider6.FromPathFile = base.GetDbPath;
				provider6.ToPathDB = db3Path;

				provider6.Clear();
				provider6.Import();
			}

			string toObjectCodePathFile = objectCodeFolder + @"\" + toFileName;
			base.CopyFile(db3Path, toObjectCodePathFile);

			//this._hostmINV = @"mINV";
			//this._hostToApp = @"ToApp";
			//if (base.CreateZipFile == true)
			//{
			//	string mINV_Config = this._hostmINV + @"\" + this._hostConfig;						//  mINV
			//	string fromFtpCustomerConfigFolder = mINV_Config + @"\" + customerCode;			// mINV\customerCode
			//	// ------------------ mINV\Config\customerCode\profile.xml ---------------------   Забираем  profile.xml с сервера 
			//	List<string> filepathList = GetProfileFilesFromFtpAndSave(fromFtpCustomerConfigFolder, objectCodeFolder);
			//	// -------------- zip on Count4U --------------------------
			//	filepathList.Add(toObjectCodePathFile);		 // db3 на Count4U 
			//	string zipFile = this._threadCode + "^" + exportDateTime + "^.zip";				//	
			//	string zipPath = objectCodeFolder + @"\" + zipFile;			 //на Count4U 
			//	this._zip.DoZipFile(filepathList, zipPath);
			//}

			SaveFileLog(ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter);
		}

		//OLD
		//private List<string> GetProfileFilesFromFtpAndSave(string fromFtpCustomerConfigFolder, string toObjectCodeFolder)
		//{
		//	List<string> filepathList = new List<string>();
		//	FtpClient client = new FtpClient(Host, HostUser, HostPassword);
		//	client.uri = Host;
		//	FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(fromFtpCustomerConfigFolder);		  // mINV\\Config\customerCode
		//	if (statusCode == FtpStatusCode.PathnameCreated)
		//	{
		//		List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(fromFtpCustomerConfigFolder, Host, HostUser, HostPassword);
		//		if (listInfoDirectory.Count >= 1)
		//		{
		//			foreach (FileDirectoryInfo fi in listInfoDirectory)
		//			{
		//				if (fi == null) continue;
		//				if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
		//				if (fi.IsDirectory == true) continue;
		//				string fromFtpFileName = fi.Name;
		//				string toFilePath = toObjectCodeFolder + @"\" + fi.Name;

		//				FtpStatusCode result6 = client.GetFileFromFtp(fromFtpFileName, toFilePath);
		//				base.LogImport.Add(MessageTypeEnum.Trace, "Copy from ftpServer[" + fromFtpFileName + "]");
		//				base.LogImport.Add(MessageTypeEnum.Trace, "with Result [" + result6.ToString() + "]");
		//				if (result6 == FtpStatusCode.ClosingData)
		//				{
		//					filepathList.Add(toFilePath);
		//				}
		//			}
		//		}
		//	}
		//	return filepathList;
		//}

        public void Clear()
        {
			base.LogImport.Clear();

			string processCont4uFolder = base.GetExportToPDAFolderPath(false) + @"\Process";
			string objectCodeFolder = base.GetExportToPDAFolderPath(true);
		
			//base.ClearFolder(processCont4uFolder);
			base.ClearFolder(objectCodeFolder);
	 		
			string toFileName = this.GetFileBD3();
			string db3Path = processCont4uFolder + @"\" + toFileName;


			IImportProvider provider =
				 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCatalogMerkavaSdf2SqliteProvider.ToString());
			provider.ToPathDB = db3Path;
			provider.Clear();

			IImportProvider provider1 =
				 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportLocationMerkavaSdf2SqliteProvider.ToString());
			provider1.ToPathDB = db3Path;
			provider1.Clear();

			IImportProvider provider2 =
			   base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportBuildingConfigMerkavaSdf2SqliteProvider.ToString());
			provider2.ToPathDB = db3Path;
			provider2.Clear();

			IImportProvider provider3 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr6ListMerkavaSdf2SqliteProvider.ToString());
			provider3.ToPathDB = db3Path;
			provider3.Clear();

			IImportProvider provider4 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr7ListMerkavaSdf2SqliteProvider.ToString());
			provider4.ToPathDB = db3Path;
			provider4.Clear();

			//if (base.IncludePreviousInventor == true)
			//{
				IImportProvider provider5 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPreviousInventoryMerkavaSdf2SqliteProvider.ToString());
				provider5.ToPathDB = db3Path;
				provider5.Clear();
			//}

			IImportProvider provider6 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCurrentInventorMerkavaSdf2SqliteProvider.ToString());
			provider6.ToPathDB = db3Path;
			provider6.Clear();
			

            UpdateLogFromILog();
        }


    }
}