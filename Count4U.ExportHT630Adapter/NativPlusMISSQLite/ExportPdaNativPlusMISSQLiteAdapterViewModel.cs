using System;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Common.ViewModel.Adapters.Import;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Model.Interface.Count4U;

namespace Count4U.ExportPdaNativPlusMISSQLiteAdapter
{
	public class ExportPdaNativPlusMISSQLiteAdapterViewModel : ExportPdaModuleBaseViewModel	  //все для динамической смены\выбора  ExportPda Adapter
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

		public ExportPdaNativPlusMISSQLiteAdapterViewModel(
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
			base.IncludePreviousInventor = false;
			base.IncludeCurrentInventor = false;
			base.IncludeProfile = false;
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
			//base.IncludeProfile = info.IncludeProfile;
			//base.IncludePreviousInventor = info.IncludePreviousInventor ;
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
			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(base.GetDbPath);

			//"Catalog";
			IImportProvider provider =																		   //CatalogNativPlusMISSdf2SqliteParser
			 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCatalogNativPlusMISSdf2SqliteProvider.ToString());

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
			IImportProvider provider1 =																   //LocationNativPlusMISSdf2SqliteParser
		 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportLocationNativPlusMISSdf2SqliteProvider.ToString());

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
			IImportProvider provider2 =																			//BuildingConfigNativSdf2SqliteParser
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportBuildingConfigNativPlusMISSdf2SqliteProvider.ToString());

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


			//{
			//	//   //"PropertyStr1List"
			//	IImportProvider providerPrp1 =																	   //PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr1ListNativSdf2SqliteProvider.ToString());

			//	providerPrp1.Parms.Clear();
			//	providerPrp1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp1.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp1.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp1.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp1.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp1.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp1.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp1.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp1.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp1.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp1.FromPathFile = base.GetDbPath;
			//	providerPrp1.ToPathDB = db3Path;

			//	providerPrp1.Clear();
			//	providerPrp1.Import();
			//}

			//{
			//	//   //"PropertyStr2List"
			//	IImportProvider providerPrp2 =																	   //PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr2ListNativSdf2SqliteProvider.ToString());

			//	providerPrp2.Parms.Clear();
			//	providerPrp2.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp2.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp2.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp2.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp2.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp2.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp2.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp2.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp2.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp2.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp2.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp2.FromPathFile = base.GetDbPath;
			//	providerPrp2.ToPathDB = db3Path;

			//	providerPrp2.Clear();
			//	providerPrp2.Import();
			//}

			//{
			//	//   //"PropertyStr3List"
			//	IImportProvider providerPrp3 =																	   //PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr3ListNativSdf2SqliteProvider.ToString());

			//	providerPrp3.Parms.Clear();
			//	providerPrp3.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp3.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp3.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp3.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp3.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp3.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp3.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp3.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp3.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp3.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp3.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp3.FromPathFile = base.GetDbPath;
			//	providerPrp3.ToPathDB = db3Path;

			//	providerPrp3.Clear();
			//	providerPrp3.Import();
			//}

			//{
			//	//   //"PropertyStr4List"
			//	IImportProvider providerPrp4 =																	   //PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr4ListNativSdf2SqliteProvider.ToString());

			//	providerPrp4.Parms.Clear();
			//	providerPrp4.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp4.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp4.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp4.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp4.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp4.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp4.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp4.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp4.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp4.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp4.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp4.FromPathFile = base.GetDbPath;
			//	providerPrp4.ToPathDB = db3Path;

			//	providerPrp4.Clear();
			//	providerPrp4.Import();
			//}

			//{
			//	//   //"PropertyStr5List"
			//	IImportProvider providerPrp5 =																	   //PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr5ListNativSdf2SqliteProvider.ToString());

			//	providerPrp5.Parms.Clear();
			//	providerPrp5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp5.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp5.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp5.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp5.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp5.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp5.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp5.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp5.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp5.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp5.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp5.FromPathFile = base.GetDbPath;
			//	providerPrp5.ToPathDB = db3Path;

			//	providerPrp5.Clear();
			//	providerPrp5.Import();
			//}


			//{
			//	//   //"PropertyStr6List"
			//	IImportProvider providerPrp6 =																	   //PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr6ListNativSdf2SqliteProvider.ToString());

			//	providerPrp6.Parms.Clear();
			//	providerPrp6.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp6.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp6.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp6.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp6.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp6.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp6.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp6.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp6.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp6.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp6.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp6.FromPathFile = base.GetDbPath;
			//	providerPrp6.ToPathDB = db3Path;

			//	providerPrp6.Clear();
			//	providerPrp6.Import();
			//}

			//{
			//	//   //"PropertyStr7List"
			//	IImportProvider providerPrp7 =																		//PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr7ListNativSdf2SqliteProvider.ToString());

			//	providerPrp7.Parms.Clear();
			//	providerPrp7.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp7.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp7.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp7.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp7.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp7.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp7.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp7.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp7.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp7.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp7.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp7.FromPathFile = base.GetDbPath;
			//	providerPrp7.ToPathDB = db3Path;

			//	providerPrp7.Clear();
			//	providerPrp7.Import();
			//}

			//{
			//	//   //"PropertyStr8List"
			//	IImportProvider providerPrp8 =																		//PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr8ListNativSdf2SqliteProvider.ToString());

			//	providerPrp8.Parms.Clear();
			//	providerPrp8.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp8.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp8.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp8.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp8.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp8.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp8.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp8.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp8.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp8.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp8.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp8.FromPathFile = base.GetDbPath;
			//	providerPrp8.ToPathDB = db3Path;

			//	providerPrp8.Clear();
			//	providerPrp8.Import();
			//}


			//{
			//	//   //"PropertyStr8List"
			//	IImportProvider providerPrp9 =																		//PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr9ListNativSdf2SqliteProvider.ToString());

			//	providerPrp9.Parms.Clear();
			//	providerPrp9.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp9.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp9.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp9.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp9.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp9.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp9.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp9.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp9.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp9.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp9.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp9.FromPathFile = base.GetDbPath;
			//	providerPrp9.ToPathDB = db3Path;

			//	providerPrp9.Clear();
			//	providerPrp9.Import();
			//}

			//{
			//	//   //"PropertyStr8List"
			//	IImportProvider providerPrp10 =																		//PropertyStrListNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr10ListNativSdf2SqliteProvider.ToString());

			//	providerPrp10.Parms.Clear();
			//	providerPrp10.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerPrp10.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerPrp10.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerPrp10.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerPrp10.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerPrp10.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerPrp10.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerPrp10.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerPrp10.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerPrp10.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerPrp10.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerPrp10.FromPathFile = base.GetDbPath;
			//	providerPrp10.ToPathDB = db3Path;

			//	providerPrp10.Clear();
			//	providerPrp10.Import();
			//}

			//{
			//	//   "TemplateInventory"
			//	IImportProvider providerTemplateInventory =																		
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportTemplateInventoryNativSdf2SqliteProvider.ToString());

			//	providerTemplateInventory.Parms.Clear();
			//	providerTemplateInventory.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	providerTemplateInventory.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	providerTemplateInventory.FromPathFile = base.GetDbPath;
			//	providerTemplateInventory.ToPathDB = db3Path;

			//	providerTemplateInventory.Clear();
			//	providerTemplateInventory.Import();
			//}

			//// "PreviousInventory"
			//if (base.IncludePreviousInventor == true)
			//{
			//	IImportProvider provider5 =																		//PreviousInventoryNativSdf2SqliteParser
			//	base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPreviousInventoryNativSdf2SqliteProvider.ToString());

			//	provider5.Parms.Clear();
			//	provider5.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	provider5.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	provider5.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	provider5.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	provider5.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	provider5.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	provider5.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	provider5.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	provider5.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	provider5.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	provider5.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	provider5.FromPathFile = base.GetDbPath;
			//	provider5.ToPathDB = db3Path;

			//	provider5.Clear();
			//	provider5.Import();
			//}

			//if (base.IncludeCurrentInventor == true)
			//{
			//	IImportProvider provider6 =																  //CurrentInventoryNativSdf2SqliteParser
			// base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCurrentInventorNativPlusSdf2SqliteProvider.ToString());

			//	provider6.Parms.Clear();
			//	provider6.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
			//	provider6.Parms[ImportProviderParmEnum.FileType] = base.FileType;
			//	provider6.Parms[ImportProviderParmEnum.BarcodeWithoutMask] = base.BarcodeWithoutMask;
			//	provider6.Parms[ImportProviderParmEnum.MakatWithoutMask] = base.MakatWithoutMask;
			//	provider6.Parms[ImportProviderParmEnum.InvertLetters] = base.InvertLetters ? "1" : String.Empty;
			//	provider6.Parms[ImportProviderParmEnum.InvertWords] = base.InvertWords ? "1" : String.Empty;
			//	provider6.Parms[ImportProviderParmEnum.CutAfterInvert] = base.CutAfterInvert ? "1" : String.Empty;
			//	provider6.Parms[ImportProviderParmEnum.MaxLen] = base.MaxLen;
			//	provider6.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
			//	provider6.Parms[ImportProviderParmEnum.DBPath] = base.GetDbPath;
			//	provider6.Parms[ImportProviderParmEnum.DB3Path] = db3Path;
			//	provider6.FromPathFile = base.GetDbPath;
			//	provider6.ToPathDB = db3Path;

			//	provider6.Clear();
			//	provider6.Import();
			//}

			string toObjectCodePathFile = objectCodeFolder + @"\" + toFileName;
			base.CopyFile(db3Path, toObjectCodePathFile);

			//this._hostmINV = @"mINV";
			//this._hostToApp = @"ToApp";
			//this._hostConfig = @"Config";
			//if (base.CreateZipFile == true)
			//{
			//	string mINV_Config = this._hostmINV + @"\" + this._hostConfig;						//  mINV\\Config
			//	string fromFtpCustomerConfigFolder = mINV_Config + @"\" + customerCode;			// mINV\\Config\customerCode
			//	// ------------------ mINV\Config\customerCode\profile.xml ---------------------   Забираем  profile.xml с сервера 
			//	List<string> filepathList = GetProfileFilesFromFtpAndSave(fromFtpCustomerConfigFolder, objectCodeFolder);
			//	// -------------- zip on Count4U --------------------------
			//	filepathList.Add(toObjectCodePathFile);		 // db3 на Count4U 
			//	string zipFile = this._threadCode + "^" + exportDateTime + "^.zip";				//	
			//	string zipPath = objectCodeFolder + @"\" + zipFile;			 //на Count4U 
			//	this._zip.DoZipFile(filepathList, zipPath);
			//}

			SaveFileLog(ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter);
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
				 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCatalogNativSdf2SqliteProvider.ToString());
			provider.ToPathDB = db3Path;
			provider.Clear();

			IImportProvider provider1 =
				 base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportLocationNativSdf2SqliteProvider.ToString());
			provider1.ToPathDB = db3Path;
			provider1.Clear();

			IImportProvider provider2 =
			   base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportBuildingConfigNativSdf2SqliteProvider.ToString());
			provider2.ToPathDB = db3Path;
			provider2.Clear();



			//if (base.IncludePreviousInventor == true)
			//{
			IImportProvider provider5 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPreviousInventoryNativSdf2SqliteProvider.ToString());
			provider5.ToPathDB = db3Path;
			provider5.Clear();
			//}

			IImportProvider provider6 =
			base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportCurrentInventorNativSdf2SqliteProvider.ToString());
			provider6.ToPathDB = db3Path;
			provider6.Clear();

			{
				IImportProvider providerPrp1 =
					base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr1ListNativSdf2SqliteProvider.ToString());
				providerPrp1.ToPathDB = db3Path;
				providerPrp1.Clear();
			}

			{
				IImportProvider providerPrp2 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr2ListNativSdf2SqliteProvider.ToString());
				providerPrp2.ToPathDB = db3Path;
				providerPrp2.Clear();
			}

			{
				IImportProvider providerPrp3 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr3ListNativSdf2SqliteProvider.ToString());
				providerPrp3.ToPathDB = db3Path;
				providerPrp3.Clear();
			}


			{
				IImportProvider providerPrp4 =
					base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr4ListNativSdf2SqliteProvider.ToString());
				providerPrp4.ToPathDB = db3Path;
				providerPrp4.Clear();
			}

			{
				IImportProvider providerPrp5 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr5ListNativSdf2SqliteProvider.ToString());
				providerPrp5.ToPathDB = db3Path;
				providerPrp5.Clear();
			}

			{
				IImportProvider providerPrp6 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr6ListNativSdf2SqliteProvider.ToString());
				providerPrp6.ToPathDB = db3Path;
				providerPrp6.Clear();
			}


			{
				IImportProvider providerPrp7 =
					base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr7ListNativSdf2SqliteProvider.ToString());
				providerPrp7.ToPathDB = db3Path;
				providerPrp7.Clear();
			}

			{
				IImportProvider providerPrp8 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr8ListNativSdf2SqliteProvider.ToString());
				providerPrp8.ToPathDB = db3Path;
				providerPrp8.Clear();
			}

			{
				IImportProvider providerPrp9 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr9ListNativSdf2SqliteProvider.ToString());
				providerPrp9.ToPathDB = db3Path;
				providerPrp9.Clear();
			}

			{
				IImportProvider providerPrp10 =
				base.ServiceLocator.GetInstance<IImportProvider>(ImportProviderEnum.ImportPropertyStr10ListNativSdf2SqliteProvider.ToString());
				providerPrp10.ToPathDB = db3Path;
				providerPrp10.Clear();
			}

			UpdateLogFromILog();
		}


    }
}