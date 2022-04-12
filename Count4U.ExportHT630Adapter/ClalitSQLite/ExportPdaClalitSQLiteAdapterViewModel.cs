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
using Count4U.Model.Transfer;
using System.Net;
using Count4U.Common.ViewModel.Adapters.Import;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Model.Interface.Count4U;

namespace Count4U.ExportPdaClalitSQLiteAdapter
{
	public class ExportPdaClalitSQLiteAdapterViewModel : ExportPdaModuleBaseViewModel	  //все для динамической смены\выбора  ExportPda Adapter
    {
		//private readonly IDBSettings _dbSettings;
		//private string _misCommunicatorPath = "";
		private readonly IZip _zip;
		private string _misiDnextDataPath = "";
		private string _host = "";
		private string _hostToApp = "";
		private string _hostmINV = "";
		private string _hostFromApp = "";
		private string _hostUser = "";
		private string _hostPassword = "";
		private string _hostConfig = "";
		private string _threadCode;
		

		public ExportPdaClalitSQLiteAdapterViewModel(
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
			this._hostToApp =  @"ToApp";
			this._hostFromApp = @"FromApp";
			this._hostConfig = @"Config";
			//this._threadCode = Guid.NewGuid().ToString();
			this._threadCode = "ThreadCode";
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


        protected override void InitDefault()		
		{
			base.IncludeCurrentInventor = false;
			base.IncludePreviousInventor = false;
			base.IncludeProfile = false;
			
			base.Encoding = System.Text.Encoding.GetEncoding(1255);
			base.InvertLetters  = false;
			base.InvertWords = false;
			base.CutAfterInvert = false;

			base.IncludeCurrentInventor = false;
			base.IncludePreviousInventor = false;
			base.IncludeProfile = false;
			base.CreateZipFile = false;
			
			base.CheckBaudratePDA = "0";
			//string exportDateTime = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
			//if (base.State != null)
			//{
			//	if (base.State.CurrentInventor != null) this._threadCode = base.State.CurrentInventor.Code + '^' + exportDateTime;
			//}
		}

		protected override void InitFromIniFile()	 //из кастомера  
		{	
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
			base.IncludePreviousInventor = info.IncludePreviousInventor ;
			base.IncludeProfile = info.IncludeProfile;
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
			string toObjectCodeFolder = base.GetExportToPDAFolderPath(true);
			string toFileName = this.GetFileBD3();
			string db3Path = processCont4uFolder + @"\" + toFileName;

			base.ClearFolder(processCont4uFolder);
			base.ClearFolder(toObjectCodeFolder);
			string customerCode = String.Empty;
			//DateTime dt = DateTime.Now;
			//string exportDateTime = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + "_" + dt.ToString("HH") + "_" + dt.ToString("mm") +"_" + dt.ToString("ss");


			if (base.CurrentCustomer != null)
			{
				customerCode = base.CurrentCustomer.Code;
			}

			string inventorCode = customerCode;

			if (base.CurrentInventor != null)
			{
				inventorCode = base.CurrentInventor.Code;
			}

			string branchErpCode = String.Empty;

			if (base.CurrentBranch != null)
			{
				branchErpCode = base.CurrentBranch.BranchCodeERP;
			}

			//drop and create 	  currentInventoryAdvanced
			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
				this.ServiceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(base.GetDbPath);

			//"Catalog";
			IImportProvider provider =																		   //CatalogClalitSdf2SqliteParser
			 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCatalogClalitSdf2SqliteProvider.ToString());

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
			provider.FromPathFile = base.GetDbPath;
			provider.ToPathDB = db3Path;

			provider.Clear();
			provider.Import();

			//   //"Location"
			IImportProvider provider10 =																   //LocationClalitSdf2SqliteParser
		 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportLocationClalitSdf2SqliteProvider.ToString());

			provider10.Parms.Clear();
			provider10.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider10.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			provider10.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			provider10.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			provider10.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			provider10.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			provider10.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			provider10.Parms[ImportProviderParmEnum.IncludeCurrentInventor] = base.IncludeCurrentInventor ? "1" : String.Empty;
			provider10.Parms[ImportProviderParmEnum.IncludeProfile] = base.IncludeProfile ? "1" : String.Empty;
			provider10.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider10.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider10.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider10.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			provider10.FromPathFile = base.GetDbPath;
			provider10.ToPathDB = db3Path;

			provider10.Clear();
			provider10.Import();

			//   //"BuildingConfig"
			IImportProvider provider20 =																			//BuildingConfigClalitSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportBuildingConfigClalitSdf2SqliteProvider.ToString());

			provider20.Parms.Clear();
			provider20.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			provider20.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			provider20.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			provider20.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			provider20.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			provider20.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			provider20.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			provider20.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			provider20.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			provider20.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			provider20.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			provider20.FromPathFile = base.GetDbPath;
			provider20.ToPathDB = db3Path;

			provider20.Clear();
			provider20.Import();

			{
				// "PropertyStr6"
				// "Sstatus"
				//Field4: configuration_stat_id
				//Field5: descr
				IImportProvider provider1 =																	   //PropertyStrListClalitSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider1.ToString());

				provider1.Parms.Clear();
				provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider1.Parms[ImportProviderParmEnum.FileType] = base.FileType;
				provider1.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
				provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
				provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
				provider1.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
				provider1.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
				provider1.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
				provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				provider1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider1.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
				provider1.FromPathFile = base.GetDbPath;
				provider1.ToPathDB = db3Path;

				provider1.Clear();
				provider1.Import();

				//PropertyStr2
				//"Sprod"
				//Field3: prod_id
				//Field4: descr
				IImportProvider provider2 =																		//PropertyStrListClalitSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider2.ToString());

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

				//PropertyStr4
				// "Smodel"
				//Field3: model_id
				//Field4: model_name
				IImportProvider provider3 =																		//PropertyStrListClalitSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider3.ToString());

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
				//PropertyStr8
				//"Szag"
				//Field6: entry_id
				//Field4: descr
				IImportProvider provider4 =																		//PropertyStrListClalitSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider4.ToString());

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

				//PropertyStr3
				//"Syatzran"
				//Field3: vend_id
				//Field4: descr
				IImportProvider provider5 =																		//PropertyStrListClalitSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider5.ToString());
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

			// "PreviousInventory"
			if (base.IncludePreviousInventor == true)
			{
				IImportProvider provider50 =																		//PreviousInventoryClalitSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPreviousInventoryClalitSdf2SqliteProvider.ToString());

				provider50.Parms.Clear();
				provider50.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider50.Parms[ImportProviderParmEnum.FileType] = base.FileType;
				provider50.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
				provider50.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
				provider50.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
				provider50.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
				provider50.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
				provider50.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
				provider50.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				provider50.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider50.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
				provider50.FromPathFile = base.GetDbPath;
				provider50.ToPathDB = db3Path;

				provider50.Clear();
				provider50.Import();
			}

			if (base.IncludeCurrentInventor == true)
			{
				IImportProvider provider60 =																  //CurrentInventoryClalitSdf2SqliteParser
			 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCurrentInventorClalitSdf2SqliteProvider.ToString());

				provider60.Parms.Clear();
				provider60.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
				provider60.Parms[ImportProviderParmEnum.FileType] = base.FileType;
				provider60.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
				provider60.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
				provider60.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
				provider60.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
				provider60.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
				provider60.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
				provider60.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
				provider60.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
				provider60.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
				provider60.FromPathFile = base.GetDbPath;
				provider60.ToPathDB = db3Path;

				provider60.Clear();
				provider60.Import();
			}

			string toObjectCodePathFile = toObjectCodeFolder + @"\" + toFileName;
			base.CopyFile(db3Path, toObjectCodePathFile);
   
			//this._hostmINV = @"mINV";
			//this._hostToApp = @"ToApp";
			//if (base.CreateZipFile == true)
			//{
			//	string mINV_Config = this._hostmINV + @"\" + this._hostConfig;						//  mINV
			//	string fromFtpCustomerConfigFolder = mINV_Config + @"\" + customerCode;			// mINV\customerCode
			//	// ------------------ mINV\Config\customerCode\profile.xml ---------------------   Забираем  profile.xml с сервера 
			//	List<string> filepathList = GetProfileFilesFromFtpAndSave(fromFtpCustomerConfigFolder, toObjectCodeFolder);
			//		// -------------- zip on Count4U --------------------------
			//	filepathList.Add(toObjectCodePathFile);		 // db3 на Count4U 
			//	string zipFile = this._threadCode + "^" + exportDateTime + "^.zip";			//	
			//	string zipPath = toObjectCodeFolder + @"\" + zipFile;			 //на Count4U 
			//	this._zip.DoZipFile(filepathList, zipPath);
			//}

			SaveFileLog(ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter);
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
				 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCatalogClalitSdf2SqliteProvider.ToString());
			provider.ToPathDB = db3Path;
			provider.Clear();

			IImportProvider provider1 =
				 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportLocationClalitSdf2SqliteProvider.ToString());
			provider1.ToPathDB = db3Path;
			provider1.Clear();

			IImportProvider provider2 =
			   base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportBuildingConfigClalitSdf2SqliteProvider.ToString());
			provider2.ToPathDB = db3Path;
			provider2.Clear();

			IImportProvider provider3 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider1.ToString());
			provider3.ToPathDB = db3Path;
			provider3.Clear();

			IImportProvider provider4 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider2.ToString());
			provider4.ToPathDB = db3Path;
			provider4.Clear();

			IImportProvider provider10 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider3.ToString());
			provider10.ToPathDB = db3Path;
			provider10.Clear();

			IImportProvider provider11 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider4.ToString());
			provider11.ToPathDB = db3Path;
			provider11.Clear();

			IImportProvider provider12 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider5.ToString());
			provider12.ToPathDB = db3Path;
			provider12.Clear();

			//if (base.IncludePreviousInventor == true)
			//{
				IImportProvider provider5 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPreviousInventoryClalitSdf2SqliteProvider.ToString());
				provider5.ToPathDB = db3Path;
				provider5.Clear();
			//}

			IImportProvider provider6 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCurrentInventorClalitSdf2SqliteProvider.ToString());
			provider6.ToPathDB = db3Path;
			provider6.Clear();
			

            UpdateLogFromILog();
        }


    }
}

//// ------------------ mINV\Config\customerCode\profile.xml ---------------------						   MOVE ToFtpViewModel
//string mINV_Config = this._hostmINV + @"\" + this._hostConfig;		//  mINV\\Config
////string ftpConfigFolder = mINV_Config + @"\" + customerCode;			// mINV\\Config\customerCode
//string result5 = client.ChangeWorkingDirectory(mINV_Config);		  //  mINV\\Config
//string fromFtpPathProfile = customerCode + @"\profile.xml";		    //  	 customerCode\profile.xml	   на ftp	  (from)
//string toPathProfile = objectCodeFolder + @"\profile.xml";				// на Count4U 
//string result6 = client.GetFileFromFtp(fromFtpPathProfile, toPathProfile);
//base.LogImport.Add(MessageTypeEnum.Trace, "Copy profile.xml from ftpServer[" + fromFtpPathProfile + "]");
//base.LogImport.Add(MessageTypeEnum.Trace, "with Result [" + result6 + "]");
//// -------------- zip on Count4U --------------------------
//List<string> filepathList = new List<string>();
//filepathList.Add(toObjectCodePathFile);		 // db3 на Count4U 
//filepathList.Add(toPathProfile);					// profile.xml на Count4U 
//string zipFile = inventorCode + ".zip";			//					??
//string zipPath = objectCodeFolder + @"\" + zipFile;			 //на Count4U 
//this._zip.DoZipFile(filepathList, zipPath);

//FtpClient client = new FtpClient(Host, HostUser, HostPassword);
////--------------- find or create folder on ftp  mINV\\ToApp\customerCode ----------------			move  ToFtpViewModel
//string mINV_toApp = this._hostmINV + @"\" + this._hostToApp;		//  mINV\\ToApp
//string ftpFolder = mINV_toApp + @"\" + customerCode;						// mINV\\ToApp\customerCode

//!!!!!!!!!! перенесла
//client.uri = Host;
//string result3 = client.ChangeWorkingDirectory(mINV_toApp);			// mINV\\ToApp
////string customerfolder = this._hostToApp + @"/" + customerCode;		 // ToApp/customerCode
//string customerfolder = customerCode;													// customerCode
//string[] listDirectory = client.ListDirectory();
//bool newDir = true;
//foreach (string dir in listDirectory)
//{																					 // проверяем есть ли такая папка
//	if (dir.ToLower() == customerfolder.ToLower())		//  ToApp/customerCode
//	{
//		newDir = false;
//	  }
//}

//if (newDir == true)
//{
//	string result1 = client.MakeDirectory(customerCode);
//	base.LogImport.Add(MessageTypeEnum.Trace, "Create folder on ftpServer[" + customerCode + "]");
//	base.LogImport.Add(MessageTypeEnum.Trace, "with Result [" + result1 + "]");
//}



//string fromFtpPathProfile = customerCode + @"\profile.xml";		    //  	 customerCode\profile.xml	   на ftp	  (from)
//string toPathProfile = objectCodeFolder + @"\profile.xml";				// на Count4U 
//FtpStatusCode result6 = client.GetFileFromFtp(fromFtpPathProfile, toPathProfile);
//base.LogImport.Add(MessageTypeEnum.Trace, "Copy profile.xml from ftpServer[" + fromFtpPathProfile + "]");
//base.LogImport.Add(MessageTypeEnum.Trace, "with Result [" + result6.ToString() + "]");


//---------------------  -------------------------------------------------------------				  move  ToFtpViewModel
//client.uri = Host;
//string toFTPzipPath = customerCode + @"\" + zipFile;		  //  	 customerCode\zipFile.zip
//string result7 = client.ChangeWorkingDirectory(mINV_toApp);
//string result = client.SaveFileToFtp(zipPath, toFTPzipPath);
//base.LogImport.Add(MessageTypeEnum.Trace, "Save file [" + zipPath + "]" + " to ftp [" + toFTPzipPath + "]");
// base.LogImport.Add(MessageTypeEnum.Trace, "with Result [" + result + "]");
