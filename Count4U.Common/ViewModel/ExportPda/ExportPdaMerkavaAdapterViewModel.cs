using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.Ini;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Interface;
using Microsoft.Practices.Unity;
using System.Linq;
using NLog;
using Count4U.Common.UserSettings;
using Count4U.Model.Extensions;

namespace Count4U.Common.ViewModel.ExportPda
{	  //NEW
    public class ExportPdaMerkavaAdapterViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		protected readonly IUserSettingsManager _userSettingsManager;
		protected readonly ICustomerConfigRepository _customerConfigRepository;
        private readonly IDBSettings _dbSettings;
        private readonly IUnityContainer _unityContainer;
        private readonly IImportAdapterRepository _importAdapterRepository;
        private readonly IIniFileParser _iniFileParser;

        private bool _isEditable;

        private string _adapterExportDir;

		private bool _includeCurrentInventor;
		private bool _includePreviousInventor;
		private bool _includeProfile;
		private bool _createZipFile;

		private bool _isNativMISVisible;

		public ExportPdaMerkavaAdapterViewModel(
			IUserSettingsManager userSettingsManager,
            IContextCBIRepository contextCbiRepository,
            IDBSettings dbSettings,
            IUnityContainer unityContainer,
            IImportAdapterRepository importAdapterRepository,
            IIniFileParser iniFileParser,
			ICustomerConfigRepository customerConfigRepository)
            : base(contextCbiRepository)
        {
			_userSettingsManager = userSettingsManager;
            _iniFileParser = iniFileParser;
            _importAdapterRepository = importAdapterRepository;
            _unityContainer = unityContainer;
            _dbSettings = dbSettings;
			_customerConfigRepository = customerConfigRepository;
        }

		[NotInludeAttribute]
		public bool IsNativMISVisible
		{
			get { return _isNativMISVisible; }
			set
			{
				_isNativMISVisible = value;
				RaisePropertyChanged(() => IsNativMISVisible);
			}
		}

		public bool IncludeCurrentInventor
		{
			get { return _includeCurrentInventor; }
			set { _includeCurrentInventor = value;
			RaisePropertyChanged(() => IncludeCurrentInventor);
			}
		}


		public bool IncludeProfile
		{
			get { return _includeProfile; }
			set { _includeProfile = value;
			RaisePropertyChanged(() => IncludeProfile);
			}
		}

		public bool IncludePreviousInventor
		{
			get { return _includePreviousInventor; }
			set { _includePreviousInventor = value;
			RaisePropertyChanged(() => IncludePreviousInventor);
			}
		}

		[NotInludeAttribute]
		public bool CreateZipFile
		{
			get { return _createZipFile; }
			set { _createZipFile = value;
			RaisePropertyChanged(() => CreateZipFile);
			}
		}

		[NotInludeAttribute]
        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; }
        }

	

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

         //   Build();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

		//private void Build()
		//{
		//	try
		//	{
		//		List<IExportPdaModuleInfo> exportPdaModuleInfo = Utils.GetExportPdaAdapters(_unityContainer, _importAdapterRepository, String.Empty, String.Empty, String.Empty);

		//		IExportPdaModuleInfo merkava = exportPdaModuleInfo.FirstOrDefault(r => r.Name == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter);

		//		if (merkava == null)
		//		{
		//			_logger.Warn("ExportPdaMerkavaSQLiteAdapter module is missing");
		//			return;
		//		}

		//		string name = merkava.UserControlType.Assembly.GetName().Name;
		//		string iniName = string.Format("{0}{1}", name, ".ini");

		//		string iniPath = Path.Combine(FileSystem.ExportModulesFolderPath(), iniName);

		//		if (!File.Exists(iniPath))
		//		{
		//			_logger.Warn("INI file is missing: {0}", iniPath);
		//			return;
		//		}

		//		List<IniFileData> iniData = _iniFileParser.Get(iniPath);

		//		foreach (IniFileData section in iniData)
		//		{
		//			//if (section.SectionName == SectionPDAType)
		//			//{
		//			//	foreach (KeyValuePair<string, string> kvp in section.Data)
		//			//	{
		//			//		_pdaTypeItems.Add(new ExportPdaProgramTypeItemViewModel() { Key = kvp.Key, Value = kvp.Value });
		//			//	}
		//			//}

               
				
		//		}               
		//	}
		//	catch (Exception e)
		//	{
		//		_logger.ErrorException("Build", e);
		//	}
		//}


		public void FillGUIAdapterData(IExportPdaModuleInfo adapter, ExportCommandInfo info)
        {
            if (adapter == null)
            {
                _adapterExportDir = String.Empty;
            }
            else
            {
                string sourceDir = _dbSettings.ExportToPdaFolderPath();
                _adapterExportDir = sourceDir = Path.Combine(sourceDir, adapter.Name);
                if (!Directory.Exists(sourceDir))
                {
                    Directory.CreateDirectory(sourceDir);
                }

				//ExportCommandInfo info = UtilsExport.GetExportPdaMerkavaSQLiteDefaultData(this._userSettingsManager);
				//ExportCommandInfo info = UtilsExport.GetExportPdaCommandInfoDefaultData(adapter.Name, this._userSettingsManager);


				this.IsNativMISVisible = true;
				if (adapter.Name == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter)
				{
					this.IsNativMISVisible = false;
				}
				this.IncludeCurrentInventor = info.IncludeCurrentInventor;// true;
				this.IncludePreviousInventor = info.IncludePreviousInventor;// true;
				this.IncludeProfile = info.IncludeProfile;// false;
				
				this.CreateZipFile = info.CreateZipFile;// false;
				
            }

	    }

		public void FillAdapterCustomerData(Customer customer)
		{
			if (customer == null || this._customerConfigRepository == null)
				return;
			//перед этим запонить данными по умолчанию для каждого авдптера отдельно.
			//if (selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMISAdapter)
			string keyCode = customer.Code + "|" + ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter;
			Dictionary<string, CustomerConfig> configDictionary = this._customerConfigRepository.GetCustomerConfigIniDictionary(keyCode);
			if (configDictionary != null)
			{
				this.IncludeCurrentInventor = configDictionary.GetBoolValue(this.IncludeCurrentInventor, CustomerConfigIniEnum.IncludeCurrentInventor);
				this.IncludePreviousInventor = configDictionary.GetBoolValue(this.IncludePreviousInventor, CustomerConfigIniEnum.IncludePreviousInventor);
				this.IncludeProfile = configDictionary.GetBoolValue(this.IncludeProfile, CustomerConfigIniEnum.IncludeProfile);
				this.CreateZipFile = configDictionary.GetBoolValue(this.CreateZipFile, CustomerConfigIniEnum.CreateZipFile);
				
			}
		}
		
 
        public void Save(Customer customer)
        {
              // customer.PDAType = pdaType;
          
        }

        public ExportCommandInfo  FillExportInfo(ExportCommandInfo info)
        {
			info.IncludeCurrentInventor = this.IncludeCurrentInventor;
			info.IncludePreviousInventor = this.IncludePreviousInventor;
			info.IncludeProfile = this.IncludeProfile;
			info.CreateZipFile = this.CreateZipFile;
			
			return info;
        }

   

        private bool CanOpenSubfolder(string subfolder)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(subfolder)) return false;

                if (String.IsNullOrWhiteSpace(_adapterExportDir)) return false;

                string finalDir = Path.Combine(_adapterExportDir, subfolder.Trim('"'));

                if (!Directory.Exists(finalDir))
                {
                    Directory.CreateDirectory(finalDir);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("CanOpenSubfolder", exc);

                return false;
            }

            return true;
        }

        private void OpenSubfolder(string subfolder)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(subfolder)) return;

                if (String.IsNullOrWhiteSpace(_adapterExportDir)) return;

                string finalDir = Path.Combine(_adapterExportDir, subfolder.Trim('"'));

                if (!Directory.Exists(finalDir))
                {
                    Directory.CreateDirectory(finalDir);
                }

                Utils.OpenFolderInExplorer(finalDir);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OpenSubfolder", exc);
            }
        }
    }
}