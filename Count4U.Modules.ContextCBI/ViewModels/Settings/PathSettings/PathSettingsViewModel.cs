using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using Count4U.Common.Helpers;
using Count4U.Common.Services.Ini;
using Count4U.Common.UserSettings;
using Count4U.GenerationReport.Settings;
using Count4U.Model;
using Count4U.Model.Interface;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels.Settings.PathSettings
{
    public class PathSettingsViewModel : NotificationObject, INavigationAware
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;
        private readonly IIniFileInventor _iniFileInventor;
        private readonly ISettingsRepository _settingsRepository;
		private readonly IUserSettingsManager _userSettingsManager;

        private readonly ObservableCollection<PathSettingsItemViewModel> _items;
        private PathSettingsItemViewModel _itemDb;
        private PathSettingsItemViewModel _itemImport;
        private PathSettingsItemViewModel _itemParam;
        private PathSettingsItemViewModel _itemSettings;
        private PathSettingsItemViewModel _itemLog;
        private PathSettingsItemViewModel _itemReport;
        private PathSettingsItemViewModel _itemExport;
        private PathSettingsItemViewModel _itemZip;
        private PathSettingsItemViewModel _itemImportAdapter;
        private PathSettingsItemViewModel _itemExportAdapter;
        private PathSettingsItemViewModel _itemUiPropertySet;
        private PathSettingsItemViewModel _itemFilterTemplate;
		private PathSettingsItemViewModel _itemUIConfigFolder;
		private PathSettingsItemViewModel _itemAdapterConfigFolder;
        private PathSettingsItemViewModel _itemPlanPicture;
		private PathSettingsItemViewModel _itemTerminalID;
		private PathSettingsItemViewModel _itemTransferFilesPath;
		private PathSettingsItemViewModel _itemCommunicatorPath;


		public PathSettingsViewModel(IDBSettings dbSettings, IIniFileInventor iniFileInventor, ISettingsRepository settingsRepository, IUserSettingsManager userSettingsManager)
        {
            this._settingsRepository = settingsRepository;
            this._iniFileInventor = iniFileInventor;
            this._dbSettings = dbSettings;
			this._userSettingsManager = userSettingsManager;

            this._items = new ObservableCollection<PathSettingsItemViewModel>();
        }

        public ObservableCollection<PathSettingsItemViewModel> Items
        {
            get { return _items; }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            try
            {
                Build();
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OnNavigatedTo", exc);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private void Build()
        {
            {
                string empty = this._dbSettings.EmptyCount4UDBFilePath();
                string fullPath = Path.GetFullPath(empty);
                string dbPath = Path.GetDirectoryName(fullPath);


                _itemDb = new PathSettingsItemViewModel();
                _itemDb.Name = Localization.Resources.View_PathSettings_tbDatabaseFolder;
                _itemDb.Path = dbPath;
                _itemDb.OpenCommand = new DelegateCommand(DbPathCommandExecuted, DbPathCommandCanExecute);
                _items.Add(_itemDb);
            }

            {
                string path = this._dbSettings.ImportFolderPath();
                string fullPath = Path.GetFullPath(path);
                string importPath = fullPath;

                _itemImport = new PathSettingsItemViewModel();
                _itemImport.Name = Localization.Resources.View_PathSettings_tbImportFolderPath;
                _itemImport.Path = importPath;
                _itemImport.OpenCommand = new DelegateCommand(ImportPathCommandExecuted, ImportPathCommandCanExecute);
                _items.Add(_itemImport);
            }

            {
                string path = _iniFileInventor.BuildParamsFolderPath();
                string paramsPath = path;

                _itemParam = new PathSettingsItemViewModel();
                _itemParam.Name = Localization.Resources.View_PathSettings_tbParamsIni;
                _itemParam.Path = paramsPath;
                _itemParam.OpenCommand = new DelegateCommand(ParamsPathCommandExecuted, ParamsPathCommandCanExecute);
                _items.Add(_itemParam);
            }

            {
                string settingsPath = FileSystem.UserCount4UFolder();

                _itemSettings = new PathSettingsItemViewModel();
                _itemSettings.Name = Localization.Resources.View_PathSettings_tbSettingsFolderPath;
                _itemSettings.Path = settingsPath;
                _itemSettings.OpenCommand = new DelegateCommand(SettingsPathCommandExecuted, SettingsPathCommandCanExecute);
                _items.Add(_itemSettings);
            }

            {
                string logPath = this._settingsRepository.LogPath;

                _itemLog = new PathSettingsItemViewModel();
                _itemLog.Name = Localization.Resources.View_Pathettings_tbLog;
                _itemLog.Path = logPath;
                _itemLog.OpenCommand = new DelegateCommand(LogPathCommandExecuted, LogPathCommandCanExecute);
                _items.Add(_itemLog);
            }

            {
                string reportTemplatePath = _dbSettings.ReportTemplatePath();

                _itemReport = new PathSettingsItemViewModel();
                _itemReport.Name = Localization.Resources.View_Pathettings_tbReportTemplate;
                _itemReport.Path = reportTemplatePath;
                _itemReport.OpenCommand = new DelegateCommand(ReportTemplateCommandExecuted, ReportTemplateCommandCanExecute);
                _items.Add(_itemReport);
            }

            {
                string exportPdaPath = _dbSettings.ExportErpFolderPath();

                _itemExport = new PathSettingsItemViewModel();
                _itemExport.Name = Localization.Resources.View_Pathettings_tbExport;
                _itemExport.Path = exportPdaPath;
                _itemExport.OpenCommand = new DelegateCommand(ExportPdaCommandExecuted, ExportPdaCommandCanExecute);
                _items.Add(_itemExport);
            }

            {
                string zipFileOfficePath = UtilsPath.ZipOfficeFolder(_dbSettings);

                _itemZip = new PathSettingsItemViewModel();
                _itemZip.Name = Localization.Resources.View_Pathettings_tbZipFileToOffice;
                _itemZip.Path = zipFileOfficePath;
                _itemZip.OpenCommand = new DelegateCommand(ZipFileOfficePathCommandExecuted, ZipFileOfficePathCommandCanExecute);
                _items.Add(_itemZip);
            }

            {
                string importAdapter = FileSystem.ImportModulesFolderPath();

                _itemImportAdapter = new PathSettingsItemViewModel();
                _itemImportAdapter.Name = Localization.Resources.View_Pathettings_tbImportAdapters;
                _itemImportAdapter.Path = importAdapter;
                _itemImportAdapter.OpenCommand = new DelegateCommand(ImportAdapterCommandExecuted, ImportAdapterCommandCanExecute);
                _items.Add(_itemImportAdapter);
            }

            {
                string exportAdapter = FileSystem.ExportModulesFolderPath();

                _itemExportAdapter = new PathSettingsItemViewModel();
                _itemExportAdapter.Name = Localization.Resources.View_Pathettings_tbExportAdapters;
                _itemExportAdapter.Path = exportAdapter;
                _itemExportAdapter.OpenCommand = new DelegateCommand(ExportAdapterCommandExecuted, ExportAdapterCommandCanExecute);
                _items.Add(_itemExportAdapter);
            }

            {
                string uiPropertySet = _dbSettings.UIPropertySetFolderPath();
                _itemUiPropertySet = new PathSettingsItemViewModel();
                _itemUiPropertySet.Name = Localization.Resources.View_Pathettings_tbUIPropertySet;
                _itemUiPropertySet.Path = uiPropertySet;
                _itemUiPropertySet.OpenCommand = new DelegateCommand(UIPropertySetOpenCommandExecuted, UIPropertySetOpenCommandCanExecute);
                _items.Add(_itemUiPropertySet);
            }

            {
                string filterTemplate = _dbSettings.UIFilterTemplateSetFolderPath();
                _itemFilterTemplate = new PathSettingsItemViewModel();
                _itemFilterTemplate.Name = Localization.Resources.View_Pathettings_tbFilterTemplates;
                _itemFilterTemplate.Path = filterTemplate;
                _itemFilterTemplate.OpenCommand = new DelegateCommand(FilterTemplateOpenCommandExecuted, FilterTemplateCommandCanExecute);
                _items.Add(_itemFilterTemplate);
            }
			//======
			{
				string uiConfig = _dbSettings.UIConfigSetFolderPath();
				_itemUIConfigFolder = new PathSettingsItemViewModel();
				_itemUIConfigFolder.Name = Localization.Resources.View_Pathettings_tbUIConfigFolder;
				_itemUIConfigFolder.Path = uiConfig;
				_itemUIConfigFolder.OpenCommand = new DelegateCommand(UIConfigOpenCommandExecuted, UIConfigCommandCanExecute);
				_items.Add(_itemUIConfigFolder);
			}

			{
				//string adapterConfig = _dbSettings.AdapterDefaultConfigFolderPath();
				//_itemAdapterConfigFolder = new PathSettingsItemViewModel();
				//_itemAdapterConfigFolder.Name = Localization.Resources.View_Pathettings_tbUIConfigFolder;
				//_itemAdapterConfigFolder.Path = adapterConfig;
				//_itemAdapterConfigFolder.OpenCommand = new DelegateCommand(AdapterConfigOpenCommandExecuted, AdapterConfigCommandCanExecute);
				//_items.Add(_itemAdapterConfigFolder);
			}

            {
                string planPicture = _dbSettings.PlanogramPictureFolderPath();
                _itemPlanPicture = new PathSettingsItemViewModel();
                _itemPlanPicture.Name = Localization.Resources.View_Pathettings_tbPlanPicture;
                _itemPlanPicture.Path = planPicture;
                _itemPlanPicture.OpenCommand = new DelegateCommand(PlanPictureOpenCommandExecuted, PlanPictureCommandCanExecute);
                _items.Add(_itemPlanPicture);
            }

            {
                string terminalID = _dbSettings.TerminalIDPath();

                _itemTerminalID = new PathSettingsItemViewModel();
                _itemTerminalID.Name = Localization.Resources.View_Pathettings_tbTerminalID;
                _itemTerminalID.Path = terminalID;
                _itemTerminalID.OpenCommand = new DelegateCommand(
                    ()=>Utils.OpenFolderInExplorer(_itemTerminalID.Path),
                    () => Directory.Exists(_itemTerminalID.Path));
                _items.Add(_itemTerminalID);
            }

			{
				string transferFilesPath = this._userSettingsManager.ImportPDAPathGet().Trim('\\') + @"\IDnextData";
				_itemTransferFilesPath = new PathSettingsItemViewModel();
				_itemTransferFilesPath.Name = Localization.Resources.View_Pathettings_tbTransferFilesPath;
				_itemTransferFilesPath.Path = transferFilesPath;
				_itemTransferFilesPath.OpenCommand = new DelegateCommand(TransferFilesPathCommandExecuted, ExportAdapterCommandCanExecute);
				_items.Add(_itemTransferFilesPath);
			}
			
			{
				string communicatorPath = this._userSettingsManager.ImportPDAPathGet().Trim('\\') + @"\MISCommunicator";
				_itemCommunicatorPath = new PathSettingsItemViewModel();
				_itemCommunicatorPath.Name = Localization.Resources.View_Pathettings_tbCommunicatorPath;
				_itemCommunicatorPath.Path = communicatorPath;
				_itemCommunicatorPath.OpenCommand = new DelegateCommand(CommunicatorPathCommandExecuted, CommunicatorPathCommandCanExecute);
				_items.Add(_itemCommunicatorPath);
			}
           
	


        }

        private bool UIPropertySetOpenCommandCanExecute()
        {
            return Directory.Exists(_itemUiPropertySet.Path);
        }

        private void UIPropertySetOpenCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemUiPropertySet.Path);
        }

        private bool ExportAdapterCommandCanExecute()
        {
            return Directory.Exists(_itemExportAdapter.Path);
        }

        private void ExportAdapterCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemExportAdapter.Path);
        }

		private void TransferFilesPathCommandExecuted()
        {
			Utils.OpenFolderInExplorer(_itemTransferFilesPath.Path);
        }

		private bool TransferFilesPathCommandCanExecute()
		{
			return Directory.Exists(_itemTransferFilesPath.Path);
		}

        private bool ImportAdapterCommandCanExecute()
        {
            return Directory.Exists(_itemImportAdapter.Path);
        }

		private void ImportAdapterCommandExecuted()
		{
			Utils.OpenFolderInExplorer(_itemImportAdapter.Path);
		}
		
		private bool CommunicatorPathCommandCanExecute()
		{
			return Directory.Exists(_itemCommunicatorPath.Path);
		}

		private void CommunicatorPathCommandExecuted()
        {
			Utils.OpenFolderInExplorer(_itemCommunicatorPath.Path);
        }

		
        private bool DbPathCommandCanExecute()
        {
            return Directory.Exists(_itemDb.Path);
        }

        private void DbPathCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemDb.Path);
        }

        private bool ImportPathCommandCanExecute()
        {
            return Directory.Exists(_itemImport.Path);
        }

        private void ImportPathCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemImport.Path);
        }

        private bool ParamsPathCommandCanExecute()
        {
            return Directory.Exists(_itemParam.Path);
        }

        private void ParamsPathCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemParam.Path);
        }

        private bool SettingsPathCommandCanExecute()
        {
            return Directory.Exists(_itemSettings.Path);
        }

        private void SettingsPathCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemSettings.Path);
        }

        private bool ExportPdaCommandCanExecute()
        {
            return Directory.Exists(_itemExport.Path);
        }

        private void ExportPdaCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemExport.Path);
        }

        private bool ReportTemplateCommandCanExecute()
        {
            return Directory.Exists(_itemReport.Path);
        }

        private void ReportTemplateCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemReport.Path);
        }

        private bool LogPathCommandCanExecute()
        {
            return Directory.Exists(_itemLog.Path);
        }

        private void LogPathCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemLog.Path);
        }

        private bool ZipFileOfficePathCommandCanExecute()
        {
            return Directory.Exists(_itemZip.Path);
        }

        private void ZipFileOfficePathCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemZip.Path);
        }

        private bool FilterTemplateCommandCanExecute()
        {
            return Directory.Exists(_itemFilterTemplate.Path);
        }

        private void FilterTemplateOpenCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemFilterTemplate.Path);
        }


		private bool UIConfigCommandCanExecute()
        {
			return Directory.Exists(_itemUIConfigFolder.Path);
        }

		private void UIConfigOpenCommandExecuted()
        {
			Utils.OpenFolderInExplorer(_itemUIConfigFolder.Path);
        }


		private bool AdapterConfigCommandCanExecute()
        {
			return Directory.Exists(_itemAdapterConfigFolder.Path);
        }

		private void AdapterConfigOpenCommandExecuted()
        {
			Utils.OpenFolderInExplorer(_itemAdapterConfigFolder.Path);
        }

        private bool PlanPictureCommandCanExecute()
        {
            return Directory.Exists(_itemPlanPicture.Path);
        }

        private void PlanPictureOpenCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_itemPlanPicture.Path);
        }
    }
}