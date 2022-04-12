using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Abstract;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Common.Extensions;
using System.Xml.Linq;
using Count4U.Model.Extensions;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Main;

namespace Count4U.Common.ViewModel.Adapters.Export
{
	//все для динамической смены\выбора  ExportPda Adapter
    public abstract class ExportPdaModuleBaseViewModel : ExportModuleBaseViewModel, IExportPdaModule
	{
		protected readonly IUserSettingsManager _userSettingsManager;

	    protected ExportPdaModuleBaseViewModel(
			IContextCBIRepository contextCbiRepository,			
			ILog logImport,
			IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager
            )
			: base(contextCbiRepository, logImport, serviceLocator)
	    {
	        this._userSettingsManager = userSettingsManager;
	    }

		protected string WithoutProductName { get; set; }
		protected string MakatWithoutMask { get; set; }
		protected string BarcodeWithoutMask { get; set; }
		protected string CheckBaudratePDA { get; set; }
		protected int Hash { get; set; }
		protected int FileType { get; set; }
		protected int QType { get; set; }
		protected int UseAlphaKey { get; set; }
		protected int ClientId { get; set; }
		protected int NewItem { get; set; }
		protected string NewItemBool { get; set; }
		protected string ChangeQuantityType { get; set; }
		

		protected string Password { get; set; }
		protected string HTcalculateLookUp { get; set; }
		protected string LookUpEXE { get; set; }

		protected string AddNewLocation { get; set; }
		protected string AddExtraInputValueSelectFromBatchListForm { get; set; }
		protected string AllowNewValueFromBatchListForm { get; set; }
		protected string SearchIfExistInBatchList { get; set; }
		protected string AllowMinusQuantity { get; set; }
		protected string FractionCalculate { get; set; }
		protected string PartialQuantity { get; set; }
		protected string Host1 { get; set; }
		protected string Host2 { get; set; }
		protected int Timeout { get; set; }
		protected int Retry { get; set; }
		protected int SameBarcodeInLocation { get; set; }
		public string DefaultHost { get; set; }

		protected string AllowZeroQuantity { get; set; }
		protected int MaxQuantity { get; set; }
		protected string LastSync { get; set; }
		protected bool IsInvertWordsConfig { get; set; }
		protected bool IsInvertLettersConfig { get; set; }

		protected bool IturNameType { get; set; }
		protected string IturNamePrefix { get; set; }
		protected bool IsAddBinarySearch { get; set; }

		protected string CustomerCode { get; set; }
		protected string CustomerName { get; set; }

		protected int MaxLen { get; set; }
		protected bool InvertPrefix { get; set; }
		protected bool InvertWords { get; set; }
		protected bool InvertLetters { get; set; }
		protected bool CutAfterInvert { get; set; }

		protected string SearchDef { get; set; }


		protected string PDAType { get; set; }
		protected string MaintenanceType { get; set; }
		protected string ProgramType { get; set; }

		protected bool IncludeCurrentInventor { get; set; }
		protected bool IncludePreviousInventor { get; set; }
		protected bool IncludeProfile { get; set; }
		[NotInludeAttribute]
		protected bool CreateZipFile { get; set; }


		protected string ConfirmNewLocation { get; set; }
		protected string ConfirmNewItem { get; set; }
		protected string AutoSendData { get; set; }

		protected string AllowQuantityFraction { get; set; }
		protected string AddExtraInputValue { get; set; }
		protected string AddExtraInputValueHeaderName { get; set; }
		
	
        protected abstract void InitDefault();
        protected abstract void InitFromIniFile();
		

		public abstract bool CanExport();
        protected abstract void RunExportInner(ExportCommandInfo info);
		protected abstract ExportCommandInfo InitFromConfig(ExportCommandInfo info, CBIState state);  
		//public abstract void RunClear(ExportClearCommandInfo info);
		public abstract void RunClear();
		public abstract void ClearFolders(CBIState state);

	

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
  			Init1();
			Init2();
        }

		private void Init1()
		{
			try
			{
				this.Encoding = UserSettingsHelpers.GlobalEncodingGet(this._userSettingsManager);
				this.InvertWords = true;
				this.InvertLetters = true;
				this.InitDefault();
			}
			catch (Exception exc)
			{
				_logger.ErrorException("InitDefault", exc);
			}
		}


		private void Init2()
		{
			try
			{
				this.BaseInitFromIniFile();
				this.InitFromIniFile();
			}
			catch (Exception exc)
			{
				_logger.ErrorException("InitFromIni", exc);
			}
		}
		//protected void InitXDocumentConfig(XDocument configXDocument)
		//{

		//}

        private void BaseInitFromIniFile()
        {
            try
            {
                Dictionary<ImportProviderParmEnum, string> iniData = this.GetIniData();
                base.InitEncodingFromIniData(iniData);              
            }
            catch (Exception exc)
            {
                _logger.ErrorException("InitFromIniFile", exc);
            }
        }

		protected override void EncondingUpdated()
		{
			//	RaisePropertyChanged(() => Tooltip);
		}

        public void RunExportPdaBase(ExportCommandInfo info)
        {            
            base.SetIsBusy(true);

            base.IsSaveFileLog = info.IsSaveFileLog;
            base.CancellationToken = info.CancellationToken;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    base.LogImport.Clear();
                    DateTime timeStart = DateTime.Now;	

                    RunExportInner(info);

                    var fullTime = (DateTime.Now - timeStart);
                    string textFullTime = String.Format("{0:00}:{1:00}:{2:00}", fullTime.Hours, fullTime.Minutes, fullTime.Seconds);
					base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format(Localization.Resources.ViewModel_ExportModuleBase, textFullTime));

                    this.UpdateLogFromILog();
                }
                catch (Exception exc)
                {
					_logger.ErrorException("RunExportPdaBase", exc);
					base.LogImport.Add(MessageTypeEnum.Error, "RunExportPdaBase: " + exc.Message);
					this.UpdateLogFromILog();
                }

                Utils.RunOnUI(() => base.SetIsBusy(false));

                if (info.Callback != null)
                    Utils.RunOnUI(info.Callback);
			}).LogTaskFactoryExceptions("RunExportPdaBase");
        }

		public void RunExportPdaWithoutGUIBase(ExportCommandInfo info, 
			CBIState state)
		{
			//base.SetIsBusy(true);
			Utils.RunOnUI(() => base.SetIsBusy(true));

			base.IsSaveFileLog = info.IsSaveFileLog;
			base.CancellationToken = info.CancellationToken;
		//?? проверить	base._adapterName = info.AdapterName;
			
			//Task.Factory.StartNew(() =>
			//{
				try
				{
					base.LogImport.Clear();
					DateTime timeStart = DateTime.Now;
					//----------
					if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
					{
						Init1();

						try
						{
							info.FromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData; //пока фиксируем
							InitFromConfig(info, state);
							//info = RefillFromConfigExportPdaCommandInfo(info);	   //??
						}
						catch (Exception exc) { _logger.ErrorException("InitFromConfig", exc); }

					}
					//----------
					RunExportInner(info);

					var fullTime = (DateTime.Now - timeStart);
					string textFullTime = String.Format("{0:00}:{1:00}:{2:00}", fullTime.Hours, fullTime.Minutes, fullTime.Seconds);
					base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format(Localization.Resources.ViewModel_ExportModuleBase, textFullTime));

					this.UpdateLogFromILog();
				}
				catch (Exception exc)
				{
					_logger.ErrorException("RunExportPdaWithoutGUIBase", exc);
					base.LogImport.Add(MessageTypeEnum.Error, "RunExportPdaWithoutGUIBase: " + exc.Message);
					this.UpdateLogFromILog();
				}

				Utils.RunOnUI(() => base.SetIsBusy(false));

				if (info.Callback != null)
					Utils.RunOnUI(info.Callback);
		//	}).LogTaskFactoryExceptions("RunExportPdaWithoutGUIBase");
		}



		public void RunExportPdaClearWithoutGUIBase(ExportCommandInfo info,
			CBIState state)
		{
			base.LogImport.Clear();
			//this._adapterName = info.AdapterName;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				info.FromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData; //пока фиксируем
				try { InitFromConfig(info, state); }
				catch (Exception exc) { _logger.ErrorException("InitFromConfig", exc); }

			}
			RunClear();

			if (info.Callback != null)
				Utils.RunOnUI(info.Callback);
		}

		public void RunClear(ExportClearCommandInfo info)
		{
			RunClear();
			if (info.Callback != null)
				Utils.RunOnUI(info.Callback);
		}


		protected ExportCommandInfo RefillFromConfigExportPdaCommandInfo(ExportCommandInfo info)
		{
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = this.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(info, configXDoc);

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

		//public string GetConfigFolderPath(object currentDomainObject)
		//{

		//	if (currentDomainObject != null)
		//	{

		//		string dataInPath = base.ContextCBIRepository.GetImportFolderPath(currentDomainObject);

		//		string dataInConfigPath = dataInPath + @"\Config";

		//		if (string.IsNullOrWhiteSpace(dataInPath) == false)
		//		{
		//			if (Directory.Exists(dataInConfigPath) == false)
		//			{
		//				Directory.CreateDirectory(dataInConfigPath);
		//			}
		//		}
		//		return dataInConfigPath;
		//	}
		//	return String.Empty;
		//}

		//public string GetConfigFolderPath(object currentDomainObject)
		//{
		//	string subFolder = @"\Config";
		//	string path = "";
		//	if (currentDomainObject == null) return path;
		//	path = base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, true);
		//	if (string.IsNullOrWhiteSpace(subFolder) == true) return path;

		//	string pathWithSubFolder = path.TrimEnd('\\') + @"\" + subFolder.Trim('\\');
		//	if (Directory.Exists(pathWithSubFolder) == false)
		//	{
		//		try
		//		{
		//			Directory.CreateDirectory(pathWithSubFolder);
		//		}
		//		catch (Exception exp)
		//		{
		//			pathWithSubFolder = "";
		//		}
		//	}
		//	return pathWithSubFolder;
		//}

		public virtual string GetXDocumentConfigPath(ref ExportCommandInfo info)
		{
			string adapterName = info.AdapterName;
			string adapterConfigFileName = @"\" + adapterName + ".config";
			string fileConfigPath = String.Empty;

			//DBSettings dbSettings = ServiceLocator.GetInstance<DBSettings>();
			//string adapterDefaultParamFolderPath = dbSettings.AdapterDefaultConfigFolderPath().TrimEnd(@"\".ToCharArray());
			//if (string.IsNullOrWhiteSpace(adapterName) == false)		   //by default будет браться с адаптера
			//{
			//	fileConfigPath = adapterDefaultParamFolderPath + adapterConfigFileName; // @"\" + adapterName + @"\Config.xml";
			//}

			switch (info.FromConfigXDoc)
			{
				case ConfigXDocFromEnum.InitWithoutConfig:
					info.ConfigXDocumentPath = String.Empty;
					fileConfigPath = String.Empty;
					break;
				case ConfigXDocFromEnum.FromConfigXDocument:
				case ConfigXDocFromEnum.FromDefaultAdapter:
				case ConfigXDocFromEnum.FromRootPath:
				case ConfigXDocFromEnum.FromRootFolderAndCustomer:
				case ConfigXDocFromEnum.FromRootFolderAndBranch:
				case ConfigXDocFromEnum.FromRootFolderAndInventor:
				case ConfigXDocFromEnum.FromFtpCustomer:
				case ConfigXDocFromEnum.FromFtpBranch:
				case ConfigXDocFromEnum.FromFtpInventor:
					info.ConfigXDocumentPath = String.Empty;
					fileConfigPath = String.Empty;
					break;

				case ConfigXDocFromEnum.FromCustomerInData:
					fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath(base.CurrentCustomer) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
					info.ConfigXDocumentPath = fileConfigPath;
					break;
				case ConfigXDocFromEnum.FromBranchInData:
					fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath(base.CurrentBranch) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
					info.ConfigXDocumentPath = fileConfigPath;
					break;
				case ConfigXDocFromEnum.FromInventorInData:
					fileConfigPath = base.ContextCBIRepository.GetConfigFolderPath(base.CurrentInventor) + adapterConfigFileName;							 	// <ObjectCode>\InData\Config\<AdapterName>.Config
					info.ConfigXDocumentPath = fileConfigPath;
					break;
				//case ConfigXDocFromEnum.FromDomainObjectInData:
				//	object currentDomainObject = base.GetCurrentDomainObject();
				//	fileConfigPath = this.GetConfigFolderPath(currentDomainObject) + adapterConfigFileName;						 	// <ObjectCode>\InData\Config\<AdapterName>.Config
				//	info.ConfigXDocumentPath = fileConfigPath;
				//	break;
				case ConfigXDocFromEnum.FromFullPath:
					fileConfigPath = info.ConfigXDocumentPath;
					break;
			}

			return fileConfigPath;
		}

        protected void UpdateLogFromILog()
        {
          //  this.UpdateLog(base.LogImport.PrintLog(encoding: UserSettingsHelpers.GlobalEncodingGet(this._userSettingsManager)));
			this.UpdateLog(base.LogImport.PrintLog());			  //test
        }

        protected void SaveFileLog(string adapterName)
        {
            if (!IsSaveFileLog)
                return;

            try
            {
                string logText = base.LogImport.FileLog();

          //      string processFolder = this.GetExportToPDAFolderPath(false) + @"\" + "Process";
				string processFolder = this.GetExportToPDAFolderPath(true) + @"\Log\" + adapterName;

                if (!Directory.Exists(processFolder))
                {
                    Directory.CreateDirectory(processFolder);
                }

              //  string logFilePath = String.Format("{0}/{1}", processFolder, "log.txt");
				string logFilePath = Path.Combine(processFolder, adapterName + ".full.log.txt");
                File.WriteAllText(logFilePath, logText);

				//=============
				string logPrintText = LogImport.PrintLog();
				string logPrintFilePath = Path.Combine(processFolder, adapterName + ".log.txt");
				File.WriteAllText(logPrintFilePath, logPrintText);

            }
            catch(Exception exc)
            {
                _logger.ErrorException("SaveFileLog", exc);
            }
        }


		
	    public string GetExportToPDAFolderPath(bool withCurrentDomainObject = true)
		{
			if (String.IsNullOrEmpty(base.CBIDbContext) == false)
			{
				object currentDomainObject = base.GetCurrentDomainObject();
				if (currentDomainObject != null)
				{
                    return base.ContextCBIRepository.GetExportToPDAFolderPath(currentDomainObject, withCurrentDomainObject);
				}
			}
			return String.Empty;
		}

		protected void CopyFile(string fromFilePath, string toFilePath)
		{
			if (File.Exists(fromFilePath) == false) return;
			try
			{
				if (File.Exists(toFilePath) == true)
				{
					File.Delete(toFilePath);
				}
				string folder = Path.GetDirectoryName(toFilePath);
				if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
				File.Copy(fromFilePath, toFilePath);
				//GC.Collect();
			}
			catch { }
		}


		protected void ClearFolder(string pathFolder)
		{
			try
			{
				//string folder = Path.GetDirectoryName(pathFolder);
				if (Directory.Exists(pathFolder) == false) return;
				string[] fileNames = Directory.GetFiles(pathFolder);
				foreach (var fileName in fileNames)
				{
					string finalPath = Path.Combine(pathFolder, fileName);
					File.Delete(finalPath);
				}
			}
			catch(Exception exc)
			{
                _logger.ErrorException("RunClear", exc);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sectionName">sectionName in file</param>
		/// <param name="iniFilePath">iniFilePath = base.GetPathToIniFile("Count4U.<...>.ini")</param>
		/// <returns></returns>
		protected Dictionary<ImportProviderParmEnum, string> GetIniData(string sectionName = "", string iniFilePath ="")
        {
			if (String.IsNullOrEmpty(iniFilePath) == true) return new Dictionary<ImportProviderParmEnum, string>();
			Dictionary<ImportProviderParmEnum, string> result = base.ParseAdapterIniFile(sectionName, iniFilePath);

            return result;
        }

		public void ClearRegions()
		{
			//foreach (string maskRegion in this._maskRegions.ToList())
			//{
			//	this._regionManager.Regions.Remove(maskRegion);
			//	this._maskRegions.Remove(maskRegion);
			//}
		}


		
	}
}