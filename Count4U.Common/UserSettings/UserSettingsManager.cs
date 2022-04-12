using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings.LogType;
using Count4U.Common.UserSettings.Menu;
using Count4U.GenerationReport.Settings;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.ProcessC4U;
using NLog;
using Zen.Barcode;

namespace Count4U.Common.UserSettings
{
    public class UserSettingsManager : IUserSettingsManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;
        private readonly ISettingsRepository _settingsRepository;
		private readonly IProcessRepository _processRepository;

        private Configuration _config;
        private const string CommonSectionName = "CommonSection";
        private const string StatusSectionName = "StatusSection";
        private const string StatusGroupSectionName = "StatusGroupSection";
        private const string LogTypeSectionName = "LogTypeSection";
        private const string MenuSectionName = "MenuSection";

		private DateTime _startInventorDateTimeGet = DateTime.Now;
		private DateTime _endInventorDateTimeGet = DateTime.Now;

		public UserSettingsManager(IDBSettings dbSettings, ISettingsRepository settingsRepository, IProcessRepository processRepository)
        {
            this._settingsRepository = settingsRepository;
			this._processRepository = processRepository;
            this._dbSettings = dbSettings;
            this.RefreshedReport = false;
            this.AdminInitOnStart();
        }

        #region Admin methods

        private void AdminInitOnStart()
        {
            try
            {
				string inProcess = this._processRepository.GetProcessCode_InProcess();
				Configuration defaultConfiguration = AdminOpen(FileSystem.UserSettingsFile(inProcess));
				if (defaultConfiguration == null)
				{
					_logger.Info("AdminInitOnStart - Can't open Default Config Set - !!! can be problem");
					return;
				}

                CommonSection commonSection = (CommonSection)defaultConfiguration.GetSection(CommonSectionName);
                string currentConfiguration = commonSection.CommonElement.CurrentConfigurationSet;
                if (currentConfiguration == CommonElement.DefaultConfSet)
                {
                    this._config = defaultConfiguration;
                }
                else
                {
                    string confFileName = currentConfiguration;
                    string fullPath = Path.Combine(_dbSettings.UIConfigSetFolderPath(), confFileName);

                    if (File.Exists(fullPath) == false)
                    {
                        commonSection.CommonElement.CurrentConfigurationSet = CommonElement.DefaultConfSet;
                        defaultConfiguration.Save(ConfigurationSaveMode.Full);
                        this._config = defaultConfiguration;
                    }
                    else
                    {
                        this._config = AdminOpen(fullPath);

						if (this._config == null)
						{
							_logger.Info("AdminInitOnStart - Can't open userSetting by " + fullPath + " So try open Default Config Set");
							commonSection.CommonElement.CurrentConfigurationSet = CommonElement.DefaultConfSet;
							defaultConfiguration.Save(ConfigurationSaveMode.Full);
							this._config = defaultConfiguration;
							if (this._config == null)
							{
								_logger.Info("AdminInitOnStart - Can't open Default Config Set ");
							}
						}
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("AdminInitOnStart", exc);
            }
        }

        private static Configuration AdminOpen(string fileNamePath)
        {
            Configuration result = null;

            try
            {
                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = fileNamePath;

                result = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                CommonSection commonSection = (CommonSection)result.GetSection(CommonSectionName);
                if (commonSection == null)
                {
                    AdminCommonSectionInit(result);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("AdminOpen", exc);
            }

            return result;
        }


        public void AdminSave()
        {
            try
            {
                _config.Save(ConfigurationSaveMode.Full);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("AdminSave", exc);
            }
        }

        public void AdminCommitSave(string section)
        {
            ConfigurationManager.RefreshSection(section);
        }

        public void AdminAddConfiguration(string fileName)
        {
            try
            {

                string fullPathWithFolder = Path.Combine(_dbSettings.UIConfigSetFolderPath(), fileName);
                this._config.SaveAs(fullPathWithFolder, ConfigurationSaveMode.Full);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("AdminAddConfiguration", exc);
            }
        }

        public List<string> AdminListConfiguration()
        {
            List<string> result = new List<string>();

            result.Add(CommonElement.DefaultConfSet);

            try
            {
                DirectoryInfo di = new DirectoryInfo(_dbSettings.UIConfigSetFolderPath());
                foreach (FileInfo fi in di.GetFiles())
                {
                    if (fi.Extension == FileSystem.ConfigSetFileExtension)
                    {
                        result.Add(fi.Name);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("AdminListConfigurationSet", exc);
            }

            return result;
        }

        public string AdminGetCurrentConfiguration()
        {
			Configuration defaultConfiguration = AdminOpen(FileSystem.UserSettingsFile());
			if (defaultConfiguration == null)
			{
				_logger.Info("AdminInitOnStart - Can't open Default Config Set - !!! can be problem");
				return "";
			}

            CommonSection commonSection = (CommonSection)defaultConfiguration.GetSection(CommonSectionName);
            if (commonSection == null)
                commonSection = AdminCommonSectionInit(defaultConfiguration);
            string currentConfiguration = commonSection.CommonElement.CurrentConfigurationSet;

            return AdminListConfiguration().Any(r => r == currentConfiguration) ? currentConfiguration : CommonElement.DefaultConfSet;
        }

        public void AdminSetCurrentConfiguration(string fileName)
        {
            try
            {
                this._config.Save(ConfigurationSaveMode.Full);

				Configuration defaultConfiguration = AdminOpen(FileSystem.UserSettingsFile());
				if (defaultConfiguration == null)
				{
					_logger.Info("AdminInitOnStart - Can't open Default Config Set - !!! can be problem");
					return;
				}
                CommonSection commonSection = (CommonSection)defaultConfiguration.GetSection(CommonSectionName);

                if (commonSection == null)
                    commonSection = AdminCommonSectionInit(defaultConfiguration);

                commonSection.CommonElement.CurrentConfigurationSet = fileName;
                defaultConfiguration.Save(ConfigurationSaveMode.Full);

                string fullPath;
                if (fileName == CommonElement.DefaultConfSet)
					fullPath = FileSystem.UserSettingsFile();
                else
                    fullPath = Path.Combine(_dbSettings.UIConfigSetFolderPath(), fileName);

                this._config = AdminOpen(fullPath);
				if (this._config == null)
				{
					_logger.Info("AdminSetCurrentConfigurationSet - Can't open userSetting by " + fullPath + " So try open Default Config Set");
					this._config = AdminOpen(FileSystem.UserSettingsFile());
					if (this._config == null)
					{
						_logger.Info("AdminSetCurrentConfigurationSet - Can't open Default Config Set ");
					}
				}
            }
            catch (Exception exc)
            {
                _logger.ErrorException("AdminSetCurrentConfigurationSet", exc);
            }
        }

        private static CommonSection AdminCommonSectionInit(Configuration configuration)
        {
            CommonSection section = new CommonSection();
            section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
            section.SectionInformation.AllowOverride = true;

            section.CommonElement.ItursPortion = CommonElement.DefaultItursPortion;
            section.CommonElement.ItursPortionList = CommonElement.DefaultItursPortionList;
            section.CommonElement.CBIPortion = CommonElement.DefaultCBIPortion;
            section.CommonElement.InventProductsPortion = CommonElement.DefaultInventProductsPortion;
            section.CommonElement.InventProductsPortion = CommonElement.DefaultInventProductsPortion;
            section.CommonElement.ProductsPortion = CommonElement.DefaultProductsPortion;
            section.CommonElement.ProductsPortion = CommonElement.DefaultProductsPortion;
            section.CommonElement.GlobalEncoding = CommonElement.DefaultGlobalEncoding;
            section.CommonElement.ImportEncoding = CommonElement.DefaultImportEncoding;
            section.CommonElement.Language = CommonElement.DefaultLanguage;
            section.CommonElement.CurrentConfigurationSet = CommonElement.DefaultConfSet;
            section.CommonElement.Delay = CommonElement.DefaultDelay;
            section.CommonElement.IturSort = CommonElement.DefaultIturSort;
            section.CommonElement.IturGroup = CommonElement.DefaultIturGroup;
			section.CommonElement.CopyFromSource = CommonElement.DefaultCopyFromSource;
			section.CommonElement.CopyFromHost = CommonElement.DefaultCopyFromHost;
			section.CommonElement.CountingFromSource = CommonElement.DefaultCountingFromSource;
			section.CommonElement.SendToFtpOffice = CommonElement.DefaultSendToFtpOffice;
			section.CommonElement.ForwardResendData = CommonElement.DefaultForwardResendData;
			section.CommonElement.ShowMark = CommonElement.DefaultShowMark;
			section.CommonElement.CopyByCodeInventor = CommonElement.DefaultCopyByCodeInventor;
			section.CommonElement.ImportPDAPath = CommonElement.DefaultImportPDAPath;
			section.CommonElement.ExportPDAPath = CommonElement.DefaultExportPDAPath;
            section.CommonElement.ImportTCPPath = CommonElement.DefaultImportTCPPath;
            section.CommonElement.ExportTCPPath = CommonElement.DefaultExportTCPPath;
            section.CommonElement.TcpServerPort = CommonElement.DefaultTcpServerPort;
            section.CommonElement.WebServiceLink = CommonElement.DefaultWebServiceLink;
            section.CommonElement.WebServiceDeveloperLink = CommonElement.DefaultWebServiceDeveloperLink;
            section.CommonElement.UseToo = CommonElement.DefaultUseToo;
            section.CommonElement.TcpServerOn = CommonElement.DefaultTcpServerOn;

        section.CommonElement.Host = CommonElement.DefaultHost;
			section.CommonElement.User = CommonElement.DefaultUser;
			section.CommonElement.Password = CommonElement.DefaultPassword;

            configuration.Sections.Add(CommonSectionName, section);
            configuration.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection(CommonSectionName);

            return section;
        }

        #endregion


        #region regions
        public RegionElement RegionElementGet(string sectionName)
        {
            RegionSection section = (RegionSection)this._config.GetSection(sectionName);
            return section == null ? null : section.RegionElement;
        }

        public void RegionElementSet(string sectionName, RegionElement regionElement)
        {
            RegionSection section = (RegionSection)this._config.GetSection(sectionName);
            if (section == null)
            {
                section = new RegionSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;
                this._config.Sections.Add(sectionName, section);
            }

            if (regionElement != null)
            {
                //                section.RegionElement.ColorB = regionElement.ColorB;
                //                section.RegionElement.ColorG = regionElement.ColorG;
                //                section.RegionElement.ColorR = regionElement.ColorR;
                section.RegionElement.Height = regionElement.Height;
                section.RegionElement.Width = regionElement.Width;
                section.RegionElement.X = regionElement.X;
                section.RegionElement.Y = regionElement.Y;
                section.RegionElement.Theme = regionElement.Theme;
                section.RegionElement.IsOpen = regionElement.IsOpen;
            }
        }

        #endregion

        #region iturs
        public int PortionItursGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
          //  return section.CommonElement.ItursPortion;
			int portionIturs = section.CommonElement.ItursPortion;
			if (portionIturs == 0) portionIturs =CommonElement.DefaultItursPortion;
			return portionIturs;
        }

        public void PortionItursSet(int porition)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ItursPortion = porition;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public int PortionItursListGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.ItursPortionList;
        }

        public void PortionItursListSet(int porition)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ItursPortionList = porition;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
        #endregion

        public int PortionCBIGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            return section.CommonElement.CBIPortion;
        }

        public void PortionCBISet(int porition)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.CBIPortion = porition;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public int PortionSectionsGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.SectionsPortion;
        }

        public void PortionSectionsSet(int porition)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.SectionsPortion = porition;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        #region main window
        public MainWindowSettings MainWindowSettingsGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            if (section == null)
            {
                return null;
            }

            MainWindowSettings result = new MainWindowSettings();
            result.Top = section.CommonElement.MainWindowTop;
            result.Left = section.CommonElement.MainWindowLeft;
            result.Width = section.CommonElement.MainWindowWidth;
            result.Height = section.CommonElement.MainWindowHeight;
            //result.IsMaximized = section.CommonElement.IsMaximized;
			result.IsMaximized = true;

            return result;
        }

        public void MainWindowSettingsSet(MainWindowSettings settings)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            if (settings != null)
            {
                section.CommonElement.MainWindowTop = settings.Top;
                section.CommonElement.MainWindowLeft = settings.Left;
                section.CommonElement.MainWindowWidth = settings.Width;
                section.CommonElement.MainWindowHeight = settings.Height;
                section.CommonElement.IsMaximized = settings.IsMaximized;
            }
            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        #endregion

        public int PortionInventProductsGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.InventProductsPortion;
        }

        public void PortionInventProdutsSet(int porition)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.InventProductsPortion = porition;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public int PortionProductsGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.ProductsPortion;
        }

        public void PortionProdutsSet(int porition)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ProductsPortion = porition;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        #region statuses

        public string StatusColorGet(string statusName)
        {
            StatusSection section = this._config.GetSection(StatusSectionName) as StatusSection;
            if (section == null)
            {
                section = new StatusSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;
                this._config.Sections.Add(StatusSectionName, section);
                this.AdminSave();
            }

            StatusElement element = section.StatusElementCollection[statusName];
            return element == null ? string.Empty : element.Color;
        }

        public void StatusColorSet(string statusName, string color)
        {
            StatusSection section = this._config.GetSection(StatusSectionName) as StatusSection;
            if (section == null)
            {
                section = new StatusSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;
                this._config.Sections.Add(StatusSectionName, section);

            }

            StatusElement element = section.StatusElementCollection[statusName];
            if (element == null)
            {
                element = new StatusElement();
                element.Name = statusName;
                section.StatusElementCollection.Add(element);
            }

            element.Color = color;

            this.AdminSave();
            ConfigurationManager.RefreshSection(StatusSectionName);
        }

        public string StatusGroupColorGet(string statusGroupName)
        {
            StatusGroupSection section = this._config.GetSection(StatusGroupSectionName) as StatusGroupSection;
            if (section == null)
            {
                section = new StatusGroupSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;
                this._config.Sections.Add(StatusGroupSectionName, section);
                this.AdminSave();
            }

            StatusGroupElement element = section.StatusGroupElementCollection[statusGroupName];
            return element == null ? string.Empty : element.Color;
        }

        public void StatusGroupColorSet(string statusName, string color)
        {
            StatusGroupSection section = this._config.GetSection(StatusGroupSectionName) as StatusGroupSection;
            if (section == null)
            {
                section = new StatusGroupSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;
                this._config.Sections.Add(StatusGroupSectionName, section);

            }

            StatusGroupElement element = section.StatusGroupElementCollection[statusName];
            if (element == null)
            {
                element = new StatusGroupElement();
                element.Name = statusName;
                section.StatusGroupElementCollection.Add(element);
            }

            element.Color = color;

            this.AdminSave();
            ConfigurationManager.RefreshSection(StatusSectionName);
        }

        #endregion

        public int GlobalEncodingGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            return section.CommonElement.GlobalEncoding;
        }

        public void GlobalEncodingSet(int encoding)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.GlobalEncoding = encoding;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public int ImportEncodingGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            return section.CommonElement.ImportEncoding;
        }

        public void ImportEncodingSet(int encoding)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ImportEncoding = encoding;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public LogTypeElementCollection LogTypeGet()
        {
            LogTypeSection section = this._config.GetSection(LogTypeSectionName) as LogTypeSection;
            if (section == null)
            {
                section = new LogTypeSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;

                LogTypeElement simple = new LogTypeElement();
                simple.IsEnabled = true;
                simple.Name = MessageTypeEnum.SimpleTrace.ToString();
                section.LogTypeElementCollection.Add(simple);

                this._config.Sections.Add(LogTypeSectionName, section);
                this.AdminSave();
            }

            return section.LogTypeElementCollection;
        }

        public void LogTypeSet(string logTypeName, bool isEnabled)
        {
            LogTypeSection section = this._config.GetSection(LogTypeSectionName) as LogTypeSection;
            if (section == null)
            {
                section = new LogTypeSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;

                LogTypeElement simple = new LogTypeElement();
                simple.IsEnabled = true;
                simple.Name = MessageTypeEnum.SimpleTrace.ToString();
                section.LogTypeElementCollection.Add(simple);
                this._config.Sections.Add(LogTypeSectionName, section);

            }

            LogTypeElement element = section.LogTypeElementCollection[logTypeName];
            if (element == null)
            {
                element = new LogTypeElement();
                element.Name = logTypeName;
                section.LogTypeElementCollection.Add(element);
            }

            element.IsEnabled = isEnabled;

            this.AdminSave();
            ConfigurationManager.RefreshSection(StatusSectionName);
        }

        private enLanguage? _temporaryLanguagedUntilRestart;

        public enLanguage LanguageGet()
        {
            if (this._temporaryLanguagedUntilRestart != null)
                return this._temporaryLanguagedUntilRestart.Value;

            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            string language = section.CommonElement.Language;

            return FromStringToLanguage(language);
        }

        public void LanguageSet(enLanguage language)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);



            string strLng = section.CommonElement.Language;
            this._temporaryLanguagedUntilRestart = FromStringToLanguage(strLng);

            string languageStr = "en";
            switch (language)
            {
                case enLanguage.English:
                    languageStr = "en";
                    break;
                case enLanguage.Hebrew:
                    languageStr = "he";
                    break;
                case enLanguage.Italian:
                    languageStr = "it";
                    break;
                case enLanguage.Russian:
                    languageStr = "ru";
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException("language");
            }

            section.CommonElement.Language = languageStr;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        private static enLanguage FromStringToLanguage(string language)
        {
            switch (language)
            {
                case "en":
                    return enLanguage.English;
                case "he":
                    return enLanguage.Hebrew;
                case "it":
                    return enLanguage.Italian;
                case "ru":
                    return enLanguage.Russian;
            }
            return enLanguage.English;
        }

        public int DelayGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			int delay = section.CommonElement.Delay;
			if (delay == 0)  delay=  CommonElement.DefaultDelay;
            return delay;
        }

        public void DelaySet(int delay)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.Delay = delay;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public string IturSortGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            return section.CommonElement.IturSort;
        }

        public void IturSortSet(string sort)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.IturSort = sort;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public string IturGroupGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            return section.CommonElement.IturGroup;
        }

        public void IturGroupSet(string group)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.IturGroup = group;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public string IturModeGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            return section.CommonElement.IturMode;
        }

        public void IturModeSet(string mode)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.IturMode = mode;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public bool IsExpandedBottomGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            return section.CommonElement.IsExpandedBottom;
        }

        public void IsExpandedBottomSet(bool isExpanded)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.IsExpandedBottom = isExpanded;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public int PortionSuppliersGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.SuppliersPortion;
        }

		
        public void PortionSuppliersSet(int portion)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.SuppliersPortion = portion;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }


		public int PortionFamilysGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.FamilysPortion;
		}
  
		public void PortionFamilysSet(int portion)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.FamilysPortion = portion;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

        public char CurrencyGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.Currency;
        }

        public void CurrencySet(char currency)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.Currency = currency;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public string BarcodeTypeGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.BarcodeType;
        }

        public void BarcodeTypeSet(string barcodeType)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.BarcodeType = barcodeType;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

		public string PrinterGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.Printer;
		}

		public void PrinterSet(string printer)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.Printer = printer;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

        public string BarcodePrefixGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.BarcodePrefix;
        }

        public void BarcodePrefixSet(string barcodePrefix)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.BarcodePrefix = barcodePrefix;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }


		public string CustomerFilterCodeGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.CustomerFilterCode;
		}

		public void CustomerFilterCodeSet(string customerFilterCode)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.CustomerFilterCode = customerFilterCode;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string CustomerFilterNameGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.CustomerFilterName;
		}

		public void CustomerFilterNameSet(string customerFilterName)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.CustomerFilterName = customerFilterName;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


        public string IturNamePrefixGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.IturNamePrefix;
        }

        public void IturNamePrefixSet(string iturNamePrefix)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.IturNamePrefix = iturNamePrefix;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public bool NavigateBackImportPdaFormGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.NavigateBackImportPdaForm;
        }

        public void NavigateBackImportPdaFormSet(bool navigateBack)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.NavigateBackImportPdaForm = navigateBack;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

	    public bool UseCustomerFilterGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UseCustomerFilter;
        }

        public void UseCustomerFilterSet(bool useCustomerFilter)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UseCustomerFilter = useCustomerFilter;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }


		public bool SearchDialogIsModalGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.SearchDialogIsModal;
        }

		public void SearchDialogIsModalSet(bool searchDialogIsModal)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.SearchDialogIsModal = searchDialogIsModal;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
	
		public bool CopyFromSourceGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.CopyFromSource;
		}

	
		public void CopyFromSourceSet(bool copyFromSource)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.CopyFromSource = copyFromSource;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public bool SendToFtpOfficeGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.SendToFtpOffice;
		}


		public void SendToFtpOfficeSet(bool sendToFtpOffice)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.SendToFtpOffice = sendToFtpOffice;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public bool TagSubstringGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.TagSubstring;
		}

	
		public void TagSubstringSet(bool tagSubstring)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.TagSubstring = tagSubstring;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public bool ForwardResendDataGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.ForwardResendData;
		}


		public void ForwardResendDataSet(bool forwardResendData)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.ForwardResendData = forwardResendData;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public bool CopyFromHostGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.CopyFromHost;
		}

		public void CopyFromHostSet(bool copyFromHost)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.CopyFromHost = copyFromHost;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public bool CopyByCodeInventorGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.CopyByCodeInventor;
		}

		public void CopyByCodeInventorSet(bool copyByCodeInventor)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.CopyByCodeInventor = copyByCodeInventor;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public bool CountingFromSourceGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.CountingFromSource;
		}

		public void CountingFromSourceSet(bool сountingFromSource)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.CountingFromSource = сountingFromSource;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public bool ShowMarkGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.ShowMark;
		}

		public void ShowMarkSet(bool showMark)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.ShowMark = showMark;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public bool PropertyIsEmptyGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.PropertyIsEmpty;
		}

		public void PropertyIsEmptySet(bool propertyIsEmpty)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.PropertyIsEmpty = propertyIsEmpty;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string ImportPDAPathGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.ImportPDAPath;
		}
		public void ImportPDAPathSet(string path)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.ImportPDAPath = path;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

        public string ImportTCPPathGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.ImportTCPPath;
        }
        public void ImportTCPPathSet(string path)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ImportTCPPath = path;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public string ExportTCPPathGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.ExportTCPPath;
        }
        public void ExportTCPPathSet(string path)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ExportTCPPath = path;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        
        public string TcpServerPortGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.TcpServerPort;
        }
        public void TcpServerPortSet(string port)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.TcpServerPort = port;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public bool UseTooGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.UseToo;
        }

        public void UseTooSet(bool useToo)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.UseToo = useToo;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public bool TcpServerOnGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.TcpServerOn;
        }

        public void TcpServerOnSet(bool tcpServerOn)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.TcpServerOn = tcpServerOn;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
        

        public string WebServiceLinkGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.WebServiceLink;
        }
        public void WebServiceLinkSet(string link)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.WebServiceLink = link;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }



        //WebServiceDeveloperLink

        public string WebServiceDeveloperLinkGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.WebServiceDeveloperLink;
        }
        public void WebServiceDeveloperLinkSet(string link)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.WebServiceDeveloperLink = link;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
        public string ExportPDAPathGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.ExportPDAPath;
		}
		public void ExportPDAPathSet(string path)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.ExportPDAPath = path;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string HostGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.Host;
		}

		public void HostSet(string host)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.Host = host;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string HostFtpGet(out bool enableSsl)
		{
			enableSsl = false;
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			string host = section.CommonElement.Host.ToLower();
			if (host.StartsWith ("ftps://") == true)
			{
				host = host.Replace("ftps://", "ftp://");
				enableSsl = true;
			}
			return host.TrimEnd('/');
		}

		public string UserGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.User;
		}
		public void UserSet(string user)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.User = user;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public string PasswordGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.Password;
		}
		public void PasswordSet(string password)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.Password = password;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

        #region menu

        public MenuElement MenuGet(string name, string partName, string dashboardName)
        {
            MenuSection section = this._config.GetSection(MenuSectionName) as MenuSection;
            if (section == null)
            {
                section = new MenuSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;
                this._config.Sections.Add(MenuSectionName, section);
                this.AdminSave();
            }

            MenuElement element = section.MenuElementCollection.Cast<MenuElement>().FirstOrDefault(r => r.Name == name && r.PartName == partName && r.DashboardName == dashboardName);
            return element;
        }

        public void MenuInsert(MenuElement menu)
        {
            MenuSection section = this._config.GetSection(MenuSectionName) as MenuSection;
            if (section == null)
            {
                section = new MenuSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;
                this._config.Sections.Add(MenuSectionName, section);
            }

            MenuElement element = section.MenuElementCollection.
                Cast<MenuElement>().
                FirstOrDefault(r => r.Name == menu.Name && r.PartName == menu.PartName && r.DashboardName == menu.DashboardName);

            if (element != null)
            {
                throw new InvalidOperationException("Menu already exists");
            }

            section.MenuElementCollection.Add(menu);

            this.AdminSave();
            ConfigurationManager.RefreshSection(MenuSectionName);
        }

        public void MenuUpdate(MenuElement menu)
        {
            MenuSection section = this._config.GetSection(MenuSectionName) as MenuSection;
            if (section == null)
            {
                section = new MenuSection();
                section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                section.SectionInformation.AllowOverride = true;
                this._config.Sections.Add(MenuSectionName, section);
            }

            MenuElement element = section.MenuElementCollection.
                Cast<MenuElement>().
                FirstOrDefault(r => r.Name == menu.Name && r.PartName == menu.PartName && r.DashboardName == menu.DashboardName);

            if (element == null)
            {
                throw new InvalidOperationException("Menu doesn't exist");
            }

            element.BackgroundColor = menu.BackgroundColor;
            element.IsVisible = menu.IsVisible;
            element.SortIndex = menu.SortIndex;

            this.AdminSave();
            ConfigurationManager.RefreshSection(MenuSectionName);
        }

        #endregion

        #region dashboard background
        //home

	

        public string DashboardHomeBackgroundGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.DashboardHomeBackground;
        }
        public void DashboardHomeBackgroundSet(string path)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.DashboardHomeBackground = path;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
        public double DashboardHomeBackgroundOpacityGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.DashboardHomeBackgroundOpacity;
        }
        public void DashboardHomeBackgroundOpacitySet(double opacity)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.DashboardHomeBackgroundOpacity = opacity;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
        //customer
        public string DashboardCustomerBackgroundGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.DashboardCustomerBackground;
        }

        public void DashboardCustomerBackgroundSet(string path)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.DashboardCustomerBackground = path;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public double DashboardCustomerBackgroundOpacityGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.DashboardCustomerBackgroundOpacity;
        }

        public void DashboardCustomerackgroundOpacitySet(double opacity)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.DashboardCustomerBackgroundOpacity = opacity;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
        //branch
        public string DashboardBranchBackgroundGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.DashboardBranchBackground;
        }

        public void DashboardBranchBackgroundSet(string path)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.DashboardBranchBackground = path;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public double DashboardBranchBackgroundOpacityGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.DashboardBranchBackgroundOpacity;
        }

        public void DashboardBranchBackgroundOpacitySet(double opacity)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.DashboardBranchBackgroundOpacity = opacity;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
        //inventor
        public string DashboardInventorBackgroundGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.DashboardInventorBackground;
        }
        public void DashboardInventorBackgroundSet(string path)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.DashboardInventorBackground = path;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
        public double DashboardInventorBackgroundOpacityGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.DashboardInventorBackgroundOpacity;
        }
        public void DashboardInventorBackgroundOpacitySet(double opacity)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.DashboardInventorBackgroundOpacity = opacity;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public IturAnalyzesRepositoryEnum ReportRepositoryGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);

            IturAnalyzesRepositoryEnum en;
			//if (Enum.TryParse(section.CommonElement.ReportRepository, out en))
			//{
			//	return en;
			//}
            return IturAnalyzesRepositoryEnum.IturAnalyzesBulkRepository;
        }

        public void ReportRepositorySet(IturAnalyzesRepositoryEnum en)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ReportRepository = en.ToString();

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);

            _settingsRepository.ReportRepositoryGet = en;
        }

        public bool ShowIturERPGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.ShowIturERP;
        }

		public bool PackDataFileCatalogGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.PackDataFileCatalog;
		}

        public void ShotIturERPSet(bool isShow)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ShowIturERP = isShow;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

		public void PackDataFileCatalogSet(bool isPackDataFileCatalog)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.PackDataFileCatalog = isPackDataFileCatalog;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }
		

        public string IturFilterSelectedGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.IturFilterSelected;
        }

        public void IturFilterSelectedSet(string selectedValue)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.IturFilterSelected = selectedValue;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

		public string IturFilterSortSelectedGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.IturFilterSortSelected;
		}

		public void IturFilterSortSelectedSet(string selectedValue)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.IturFilterSortSelected = selectedValue;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string IturFilterSortAZSelectedGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.IturFilterSortAZSelected;
		}

		public void IturFilterSortAZSelectedSet(string selectedValue)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.IturFilterSortAZSelected = selectedValue;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

        public string InventProductFilterFocusGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.InventProductFilterFocus;
        }

        public void InventProductFilterFocusSet(string focusValue)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.InventProductFilterFocus = focusValue;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public string ReportAppNameGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return section.CommonElement.ReportAppName;
        }

        public void ReportAppNameSet(string value)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.ReportAppName = value;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public Color PlanEmptyColorGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return ColorParser.StringToColor(section.CommonElement.PlanEmptyColor);
        }

        public void PlanEmptyColorSet(Color color)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.PlanEmptyColor = ColorParser.ColorToString(color);

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public Color PlanZeroColorGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return ColorParser.StringToColor(section.CommonElement.PlanZeroColor);
        }

        public void PlanZeroColorSet(Color color)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.PlanZeroColor = ColorParser.ColorToString(color);

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public Color PlanHundredColorGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            return ColorParser.StringToColor(section.CommonElement.PlanHundredColor);
        }

        public void PlanHundredSet(Color color)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.PlanHundredColor = ColorParser.ColorToString(color);

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }


		public Color InventProductMarkColorGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return ColorParser.StringToColor(section.CommonElement.InventProductMarkColor);
        }

		public void InventProductMarkColorSet(Color color)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.InventProductMarkColor = ColorParser.ColorToString(color);

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

        public int UploadWakeupTimeGet()
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			if (section.CommonElement.UploadWakeupTime < 5)
				return 5;
			else 
				return	section.CommonElement.UploadWakeupTime;
        }

        public void UploadWakeupTimeSet(int time)
        {
            CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
            section.CommonElement.UploadWakeupTime = time;

            this.AdminSave();
            ConfigurationManager.RefreshSection(CommonSectionName);
        }

	
		public bool UploadOptionsHT630_CurrentDataPDAGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsHT630_CurrentDataPDA;
		}

		public void UploadOptionsHT630_CurrentDataPDASet(bool currentDataPDA)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsHT630_CurrentDataPDA = currentDataPDA;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public bool UploadOptionsHT630_BaudratePDAGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsHT630_BaudratePDA;
		}

		public void UploadOptionsHT630_BaudratePDASet(bool baudratePDA)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsHT630_BaudratePDA = baudratePDA;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public bool UploadOptionsHT630_DeleteAllFilePDAGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsHT630_DeleteAllFilePDA;
		}

		public void UploadOptionsHT630_DeleteAllFilePDASet(bool deleteAllFilePDA)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsHT630_DeleteAllFilePDA = deleteAllFilePDA;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string UploadOptionsHT630_ExeptionFileNotDeleteGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsHT630_ExeptionFileNotDelete;
		}

		public void UploadOptionsHT630_ExeptionFileNotDeleteSet(string fileNames)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsHT630_ExeptionFileNotDelete = fileNames;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public bool UploadOptionsHT630_AfterUploadPerformWarmStartGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsHT630_AfterUploadPerformWarmStart;
		}

		public void UploadOptionsHT630_AfterUploadPerformWarmStartSet(bool performWarmStart)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsHT630_AfterUploadPerformWarmStart = performWarmStart;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

			

		//public string UploadOptionsHT630_AfterUploadRunExeFileGet()
		//{
		//	CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
		//	return section.CommonElement.UploadOptionsHT630_AfterUploadRunExeFile;
		//}

		//public void UploadOptionsHT630_AfterUploadRunExeFileSet(string fileName)
		//{
		//	CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
		//	section.CommonElement.UploadOptionsHT630_AfterUploadRunExeFile = fileName;

		//	this.AdminSave();
		//	ConfigurationManager.RefreshSection(CommonSectionName);
		//}

		public string UploadOptionsHT630_BaudratePDAItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsHT630_BaudratePDAItem;
		}

		public void UploadOptionsHT630_BaudratePDAItemSet(string baudratePDAItem)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsHT630_BaudratePDAItem = baudratePDAItem;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public string UploadOptionsRunMemoryItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsRunMemoryItem;
		}

		public void UploadOptionsRunMemoryItemSet(string memory)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsRunMemoryItem = memory;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public string DomainObjectSelectedItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.DomainObjectSelectedItem;
		}

		public void DomainObjectSelectedItemSet(string domainObject)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.DomainObjectSelectedItem = domainObject;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string InventProductPropertyMarkSelectedItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.InventProductPropertyMarkSelectedItem;
		}

		public void InventProductPropertyMarkSelectedItemSet(string propertyName)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.InventProductPropertyMarkSelectedItem = propertyName;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string InventProductPropertyFilterSelectedItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.InventProductPropertyFilterSelectedItem;
		}

		public void InventProductPropertyFilterSelectedItemSet(string propertyName)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.InventProductPropertyFilterSelectedItem = propertyName;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public string InventProductPropertyFilterSelectedNumberItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.InventProductPropertyFilterSelectedNumberItem;
		}

		public void InventProductPropertyFilterSelectedNumberItemSet(string propertyName)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.InventProductPropertyFilterSelectedNumberItem = propertyName;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string InventProductPropertyPhotoSelectedItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.InventProductPropertyPhotoSelectedItem;
		}

		public void InventProductPropertyPhotoSelectedItemSet(string propertyName)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.InventProductPropertyPhotoSelectedItem = propertyName;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string InventProductPropertySelectedItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.InventProductPropertySelectedItem;
		}

		public void InventProductPropertySelectedItemSet(string propertyName)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.InventProductPropertySelectedItem = propertyName;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public string EditorTemplateSelectedItemGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.EditorTemplateSelectedItem;
		}

		public void EditorTemplateSelectedItemSet(string templateName)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.EditorTemplateSelectedItem = templateName;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}


		public bool UploadOptionsHT630_AfterUploadRunExeFileNeedDoGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsHT630_AfterUploadRunExeFileNeedDo;
		}

		public void UploadOptionsHT630_AfterUploadRunExeFileNeedDoSet(bool value)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsHT630_AfterUploadRunExeFileNeedDo = value;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public string UploadOptionsHT630_AfterUploadRunExeFileListGet()
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			return section.CommonElement.UploadOptionsHT630_AfterUploadRunExeFileList;
		}

		public void UploadOptionsHT630_AfterUploadRunExeFileListSet(string value)
		{
			CommonSection section = (CommonSection)this._config.GetSection(CommonSectionName);
			section.CommonElement.UploadOptionsHT630_AfterUploadRunExeFileList = value;

			this.AdminSave();
			ConfigurationManager.RefreshSection(CommonSectionName);
		}

		public DateTime StartInventorDateTimeGet()
		{
            int sec = _startInventorDateTimeGet.Second;
            _startInventorDateTimeGet = _startInventorDateTimeGet.AddSeconds(-1 * sec);
            return _startInventorDateTimeGet;
		}
		public void StartInventorDateTimeSet(DateTime startInventorDateTime)
		{
			_startInventorDateTimeGet = startInventorDateTime;
		}

		public DateTime EndInventorDateTimeGet()
		{
            int sec = _endInventorDateTimeGet.Second;
            _endInventorDateTimeGet = _endInventorDateTimeGet.AddSeconds(-1 * sec);
            return _endInventorDateTimeGet;
		}
		public void EndInventorDateTimeSet(DateTime endInventorDateTime)
		{
			_endInventorDateTimeGet = endInventorDateTime;
		}

        #endregion

        public bool RefreshedReport { get; set; } 

    }
}