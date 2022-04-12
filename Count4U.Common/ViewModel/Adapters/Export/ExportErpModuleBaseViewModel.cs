using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters.Abstract;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Common.Extensions;
using Count4U.Model.Main;
using Count4U.Model.Audit;
using System.Xml.Linq;
using Count4U.Localization;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Extensions;

namespace Count4U.Common.ViewModel.Adapters.Export
{
    public abstract class ExportErpModuleBaseViewModel : ExportModuleBaseViewModel, IExportErpModule
    {
        private readonly IUserSettingsManager _userSettingsManager;
        protected readonly IDBSettings _dbSettings;
		private Dictionary<string, string> _parmsDictionary;

		private string _adapterName;

        protected ExportErpModuleBaseViewModel(
            IContextCBIRepository contextCbiRepository,
            ILog logImport,
            IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager,
            IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator)
        {
            _dbSettings = dbSettings;
            _userSettingsManager = userSettingsManager;
			_parmsDictionary = new Dictionary<string, string>();
			
        }

        protected bool IsInvertWords { get; set; }
        protected bool IsInvertLetters { get; set; }
        protected bool IsFromCatalog { get; set; }
        protected bool IsWithoutCatalog { get; set; }
		protected bool ExportToExcel { get; set; }
		protected string PathToExportErp { get; set; }

		protected abstract void InitFromConfig(ExportErpCommandInfo info, CBIState state);  
        protected abstract void RunExportInner(ExportErpCommandInfo info);
        protected abstract void InitDefault();
        protected abstract void InitFromIniFile();
		protected abstract string GetPathToIniFile();

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			this.PathToExportErp = BuildPathToExportErpDataFolder();
			InitRegular();
			//Init1();
			//try
			//{
			//	this.Encoding = UserSettingsHelpers.GlobalEncodingGet(this._userSettingsManager);
			//	this.IsInvertWords = true;
			//	this.ExportToExcel = false;
			//	this.IsInvertLetters = true;

			//	this.InitDefault();
			//}
			//catch (Exception exc)
			//{
			//	_logger.ErrorException("InitDefault", exc);
			//}

			//Init2();
			//try
			//{
			//	this.BaseInitFromIniFile();
			//	this.InitFromIniFile();
			//}
			//catch (Exception exc)
			//{
			//	_logger.ErrorException("InitConfig", exc);
			//}
        }

		[NotInludeAttribute]
		public string AdapterName
		{
			get { return _adapterName; }
		}

		private void InitRegular()
		{
			Init1();
			Init2();
		}

		private void Init1()
		{
			try
			{
				this.Encoding = UserSettingsHelpers.GlobalEncodingGet(this._userSettingsManager);
				this.IsInvertWords = true;
				this.ExportToExcel = false;
				this.IsInvertLetters = true;

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


        public void RunExportBase(ExportErpCommandInfo info)
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
					_logger.ErrorException("RunExportBase", exc);
					base.LogImport.Add(MessageTypeEnum.Error, "RunExportBase: " + exc.Message);
					this.UpdateLogFromILog();
                }

                Utils.RunOnUI(() => base.SetIsBusy(false));

                if (info.Callback != null)
                    Utils.RunOnUI(info.Callback);
			}).LogTaskFactoryExceptions("RunExportBase");
        }


		public void RunExportErpWithoutGUIBase(ExportErpCommandInfo info, 
			CBIState state)
		{
			base.SetIsBusy(true);

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
							info = RefillFromConfigExportErpCommandInfo(info);
						}
						catch (Exception exc) { _logger.ErrorException("InitFromConfig", exc); }

					}
					this.PathToExportErp = BuildPathToExportErpDataFolder();
			 		RunExportInner(info);

					var fullTime = (DateTime.Now - timeStart);
					string textFullTime = String.Format("{0:00}:{1:00}:{2:00}", fullTime.Hours, fullTime.Minutes, fullTime.Seconds);
					base.LogImport.Add(MessageTypeEnum.EndTrace, String.Format(Localization.Resources.ViewModel_ExportModuleBase, textFullTime));

					this.UpdateLogFromILog();
				}
				catch (Exception exc)
				{
					_logger.ErrorException("RunExportErpWithoutGUIBase", exc);
					base.LogImport.Add(MessageTypeEnum.Error, "RunExportErpWithoutGUIBase: " + exc.Message);
					this.UpdateLogFromILog();
				}

				Utils.RunOnUI(() => base.SetIsBusy(false));

				if (info.Callback != null)
					Utils.RunOnUI(info.Callback);
			//}).LogTaskFactoryExceptions("RunExportWithoutGUIBase");
		}


		protected ExportErpCommandInfo RefillFromConfigExportErpCommandInfo(ExportErpCommandInfo info)
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

        protected void UpdateLogFromILog()
        {
            //base.UpdateLog(base.LogImport.PrintLog(encoding: UserSettingsHelpers.GlobalEncodingGet(this._userSettingsManager)));
			base.UpdateLog(base.LogImport.PrintLog()); //test
        }

		public void RunExportErpClearWithoutGUIBase(ExportErpCommandInfo info, CBIState state)
		{
			try
			{
				base.LogImport.Clear();

				this._adapterName = info.AdapterName;
				if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
				{
					info.FromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData; //пока фиксируем
					try { InitFromConfig(info, state); }
					catch (Exception exc) { _logger.ErrorException("InitFromConfig", exc); }

				}

				string folder = BuildPathToExportErpDataFolder();//_dbSettings.ExportErpFolderPath();

				if (Directory.Exists(folder) == false) return;
				string[] fileNames = Directory.GetFiles(folder);
				foreach (var fileName in fileNames)
				{
					string finalPath = Path.Combine(folder, fileName);
					File.Delete(finalPath);
				}

				UpdateLogFromILog();
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunClear", exc);
			}

			if (info.Callback != null)
				Utils.RunOnUIAsync(info.Callback);
		}


        public void RunClear(ExportClearCommandInfo info)
        {
            try
            {
                base.LogImport.Clear();

                string folder = BuildPathToExportErpDataFolder();//_dbSettings.ExportErpFolderPath();

                if (Directory.Exists(folder) == false) return;
                string[] fileNames = Directory.GetFiles(folder);
                foreach (var fileName in fileNames)
                {
                    string finalPath = Path.Combine(folder, fileName);
                    File.Delete(finalPath);
                }

                UpdateLogFromILog();
            }
            catch (Exception exc)
            {
                _logger.ErrorException("RunClear", exc);
            }

            Utils.RunOnUIAsync(info.Callback);
        }

        protected void SaveFileLog(string fileName)
        {
            if (!IsSaveFileLog)
                return;

            try
            {
				string fileNameWithExtension = String.Format("{0}.log.txt", fileName);
                string logText = base.LogImport.FileLog();

				string processFolder = this.BuildPathToExportErpDataFolder() + @"\Log" ;

				if (!Directory.Exists(processFolder))
				{
					Directory.CreateDirectory(processFolder);
				}

				string logFilePath = Path.Combine(processFolder, fileNameWithExtension);
                File.WriteAllText(logFilePath, logText);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("SaveFileLog", exc);
            }
        }



		public Dictionary<string, string> ParmsDictionary
		{
			get { return _parmsDictionary; }
			set { _parmsDictionary = value; }
		}

		public void AddParamsInDictionary(string parameters)
		{
			if (string.IsNullOrWhiteSpace(parameters) == true) return;
			string[] parmArray = parameters.Split(';');   //ImportPath=C\:temp;ExportPath=C\:temp;
			foreach (var parm in parmArray)
			{
				string[] keyVal = parm.Split('=');
				if (keyVal.Length > 1)
				{
					this.ParmsDictionary[keyVal[0]] = keyVal[1];
				}
			}
		}

		public string BuildPathToExportErpDataFolder()
		{
			string code = "";
			string currentObjectType = "";
			object domainObject = base.GetCurrentDomainObject();
			Customer customer = domainObject as Customer;
			if (customer != null)
			{
				code = customer.Code;
				currentObjectType = "Customer";
			}

			Branch branch = domainObject as Branch;
			if (branch != null)
			{
				code = branch.Code;
				currentObjectType = "Branch";
			}

			Inventor inventor = domainObject as Inventor;
			if (inventor != null)
			{
				code = inventor.Code;
				currentObjectType = "Inventor";
			}

			string folder = GetExportFolderPath(currentObjectType, code);

			if (this.ParmsDictionary.ContainsKey("ExportPath") == true)
			{
				string exportPath = this.ParmsDictionary["ExportPath"];
				if (string.IsNullOrWhiteSpace(exportPath) == false)
				{
					folder = exportPath.Trim('\\');
				}
			}
			return folder;
		}
				
		//Путь по которому экспортируется файл - эта функция вызывается из VMExportAdaptera
		public string GetExportFolderPath(string currentObjectType,  string code) //было BuildPathToExportErpDataFolder() - вынесли в обертку с доп азванием
        {
			//if (base.CurrentInventor == null)
			//	return String.Empty;
			if (string.IsNullOrWhiteSpace(code) == true)
				return String.Empty;

			return UtilsPath.ExportErpFolder(this._dbSettings, currentObjectType, code);
        }

        protected Dictionary<ImportProviderParmEnum, string> GetIniData()
        {
            string iniFilePath = this.GetPathToIniFile();
			if (String.IsNullOrEmpty(iniFilePath) == true) return new Dictionary<ImportProviderParmEnum, string>();
            Dictionary<ImportProviderParmEnum, string> result = base.ParseAdapterIniFile(String.Empty, iniFilePath);

            return result;
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

		//public string GetConfigFolderPath(object domainObject)
		//{
		//	string subFolder = @"\Config";
		//	string path = "";
		//	if (domainObject == null) return path;
		//	string code = "";
		//	string currentObjectType = "";
		//	Customer customer = domainObject as Customer;
		//	if (customer != null)
		//	{
		//		code = customer.Code;
		//		currentObjectType = "Customer";
		//	}

		//	Branch branch = domainObject as Branch;
		//	if (branch != null)
		//	{
		//		code = branch.Code;
		//		currentObjectType = "Branch";
		//	}

		//	Inventor inventor = domainObject as Inventor;
		//	if (inventor != null)
		//	{
		//		code = inventor.Code;
		//		currentObjectType = "Inventor";
		//	}

		//	if (string.IsNullOrWhiteSpace(code) == true)
		//		return String.Empty;

		//	path = UtilsPath.ExportErpFolder(this._dbSettings, currentObjectType, code);

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

		public virtual  string GetXDocumentConfigPath(ref ExportErpCommandInfo info)
		{
			string adapterName = info.AdapterName;
			string adapterConfigFileName = @"\" + adapterName + ".config";
			string fileConfigPath = String.Empty;

			//string adapterDefaultParamFolderPath = _dbSettings.AdapterDefaultConfigFolderPath().TrimEnd(@"\".ToCharArray());
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
    }
}