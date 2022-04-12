using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Ionic.Zip;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using NLog;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Extensions;
using Count4U.GenerationReport.Settings;

namespace Count4U.Modules.ContextCBI.ViewModels.Pack.Unpack
{
    public class UnpackViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const string LogoFile = "LogoFile";

        private readonly DelegateCommand _unpackCommand;
        private readonly DelegateCommand _browseCommand;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IDBSettings _dbSettings;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly INavigationRepository _navigationRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly IInventorConfigRepository _inventorConfigRepository;
        private readonly IAlterADOProvider _alterAdoProvider;

        private string _path;

        private readonly ObservableCollection<UnpackOverwriteItem> _itemsOverwrite;
        private readonly ObservableCollection<UnpackItem> _items;
        private readonly ICollectionView _itemsView;
        private readonly ICollectionView _itemsOverwriteView;
		private readonly ISettingsRepository _settingsRepository;

        private readonly DelegateCommand _busyCancelCommand;
        private bool _isBusy;
        private CancellationTokenSource _cancellationTokenSource;

        private double _progress;
        private string _progressState;
        private string _progressFile;

        private string _busyText;
        private ILog _ilog;

        private string _warningText;

        private bool _isAllOverwrite;

        private bool _isGeneratedByCode;

        private bool _overwriteImportIsVisible;
        private bool _overwriteImportIsChecked;

        private bool _overwriteInDataIsVisible;
        private bool _overwriteInDataIsChecked;

		private bool _overwriteConfigIsVisible;
        private bool _overwriteConfigIsChecked;

		private bool _overwriteProfileIsVisible;
		private bool _overwriteProfileIsChecked;
		


        public UnpackViewModel(
            IContextCBIRepository contextCbiRepository,
            IUserSettingsManager userSettingsManager,
            IDBSettings dbSettings,
            ModalWindowLauncher modalWindowLauncher,
            INavigationRepository navigationRepository,
            IUnityContainer unityContainer,
            IInventorConfigRepository inventorConfigRepository,
			ISettingsRepository settingsRepository ,
            IAlterADOProvider alterAdoProvider)
            : base(contextCbiRepository)
        {
            _alterAdoProvider = alterAdoProvider;
            _inventorConfigRepository = inventorConfigRepository;
            _unityContainer = unityContainer;
            _navigationRepository = navigationRepository;
            _modalWindowLauncher = modalWindowLauncher;
			_settingsRepository = settingsRepository;
			_dbSettings = dbSettings;
            _userSettingsManager = userSettingsManager;
            _unpackCommand = new DelegateCommand(UnpackCommandExecuted, UnpackCommandCanExecute);
            _browseCommand = new DelegateCommand(BrowseCommandExecuted);

            _itemsOverwrite = new ObservableCollection<UnpackOverwriteItem>();
            _items = new ObservableCollection<UnpackItem>();

            ListCollectionView viewOverwrite = ((ListCollectionView)CollectionViewSource.GetDefaultView(_itemsOverwrite));
            viewOverwrite.CustomSort = new EntryComparer();
            _itemsOverwriteView = viewOverwrite;

            ListCollectionView view = ((ListCollectionView)CollectionViewSource.GetDefaultView(_items));
            view.CustomSort = new EntryComparer();
            _itemsView = view;

            _busyCancelCommand = new DelegateCommand(BusyCancelCommandExecuted, BusyCancelCommandCanExecute);
			
        }

        #region public properties

        public DelegateCommand UnpackCommand
        {
            get { return _unpackCommand; }
        }

        public DelegateCommand BrowseCommand
        {
            get { return _browseCommand; }
        }

        public ICollectionView ItemsView
        {
            get { return _itemsView; }
        }

        public ICollectionView ItemsOverwriteView
        {
            get { return _itemsOverwriteView; }
        }

        public string Path
        {
            get { return this._path; }
            set
            {
                this._path = value;
                RaisePropertyChanged(() => Path);

                this._unpackCommand.RaiseCanExecuteChanged();
                Build();
            }
        }

        public DelegateCommand BusyCancelCommand
        {
            get { return _busyCancelCommand; }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged(() => Progress);
            }
        }

        public string ProgressState
        {
            get { return _progressState; }
            set
            {
                _progressState = value;
                RaisePropertyChanged(() => ProgressState);
            }
        }

        public string ProgressFile
        {
            get { return _progressFile; }
            set
            {
                _progressFile = value;
                RaisePropertyChanged(() => ProgressFile);
            }
        }

        public string BusyText
        {
            get { return _busyText; }
            set
            {
                _busyText = value;
                RaisePropertyChanged(() => BusyText);
            }
        }

        public string WarningText
        {
            get { return _warningText; }
            set
            {
                _warningText = value;
                RaisePropertyChanged(() => WarningText);
            }
        }

        public bool IsAllOverwrite
        {
            get { return _isAllOverwrite; }
            set
            {
                _isAllOverwrite = value;
                RaisePropertyChanged(() => IsAllOverwrite);

                using (new CursorWait())
                {
                    _isGeneratedByCode = true;
                    foreach (UnpackOverwriteItem item in _itemsOverwrite)
                    {
                        item.IsOverwrite = _isAllOverwrite;
                    }
                    _isGeneratedByCode = false;
                }
            }
        }

        public bool OverwriteImportIsVisible
        {
            get { return _overwriteImportIsVisible; }
            set
            {
                _overwriteImportIsVisible = value;
                RaisePropertyChanged(() => OverwriteImportIsVisible);
            }
        }

        public bool OverwriteImportIsChecked
        {
            get { return _overwriteImportIsChecked; }
            set
            {
                _overwriteImportIsChecked = value;
                RaisePropertyChanged(() => OverwriteImportIsChecked);
				this._unpackCommand.RaiseCanExecuteChanged();
            }
        }

        public bool OverwriteInDataIsVisible
        {
            get { return _overwriteInDataIsVisible; }
            set
            {
                _overwriteInDataIsVisible = value;
                RaisePropertyChanged(() => OverwriteInDataIsVisible);
            }
        }

        public bool OverwriteInDataIsChecked
        {
            get { return _overwriteInDataIsChecked; }
            set
            {
                _overwriteInDataIsChecked = value;
                RaisePropertyChanged(() => OverwriteInDataIsChecked);
				this._unpackCommand.RaiseCanExecuteChanged();
            }
        }

		  public bool OverwriteConfigIsChecked
        {
            get { return _overwriteConfigIsChecked; }
            set
            {
                _overwriteConfigIsChecked = value;
                RaisePropertyChanged(() => OverwriteConfigIsChecked);
				this._unpackCommand.RaiseCanExecuteChanged();
            }
        }

		  public bool OverwriteProfileIsChecked
		  {
			  get { return _overwriteProfileIsChecked; }
			  set
			  {
				  _overwriteProfileIsChecked = value;
				  RaisePropertyChanged(() => OverwriteProfileIsChecked);
				  this._unpackCommand.RaiseCanExecuteChanged();
			  }
		  }


		  public bool OverwriteConfigIsVisible
        {
			get { return _overwriteConfigIsVisible; }
            set
            {
				_overwriteConfigIsVisible = value;
				RaisePropertyChanged(() => OverwriteConfigIsVisible);
            }
        }


		  public bool OverwriteProfileIsVisible
        {
			get { return _overwriteProfileIsVisible; }
            set
            {
				_overwriteProfileIsVisible = value;
				RaisePropertyChanged(() => OverwriteProfileIsVisible);
            }
        }

        #endregion

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Path":
                        return CheckZip();
                }
                return String.Empty;
            }
        }

        public string Error { get; private set; }

        private void BrowseCommandExecuted()
        {
            OpenFileDialogNotification notification = new OpenFileDialogNotification();

            notification.DefaultExt = ".zip";
            notification.Filter = "Zip archive(*.zip)|*.zip|All files (*.*)|*.*";

            string outFolder = UtilsPath.ZipOfficeFolder(_dbSettings);
            notification.InitialDirectory = outFolder;

            Tuple<bool, string> dialogResult = UtilsMisc.OpenFileDialog(notification);
            bool isOk = dialogResult.Item1;
            string path = dialogResult.Item2;

            if (!isOk)
                return;

            this.Path = path;
        }
        string CheckZip()
        {
            if (String.IsNullOrEmpty(this._path))
                return String.Empty;

            if (!File.Exists(this._path))
                return Localization.Resources.ViewModel_ZipImport_FileNotExist;

            if (!ZipFile.IsZipFile(this._path))
                return Localization.Resources.ViewModel_ZipImport_FileNotZip;

            //check format

            return String.Empty;
        }

        private void SubscribeOverwriteItems()
        {
            foreach (UnpackOverwriteItem item in _itemsOverwrite)
            {
                item.PropertyChanged += OverwriteItem_PropertyChanged;
            }
        }

        private void UnsubscribeOverwriteItems()
        {
            foreach (UnpackOverwriteItem item in _itemsOverwrite)
            {
                item.PropertyChanged -= OverwriteItem_PropertyChanged;
            }
        }

        private string RemoveFolderSlashes(string path)
        {
            return path.Replace("/", String.Empty).Replace("\\", String.Empty);
        }

        void OverwriteItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsOverwrite")
            {
                UnpackOverwriteItem overwriteItem = sender as UnpackOverwriteItem;
                if (overwriteItem == null)
                    return;

                if (overwriteItem.IsOverwrite)
                {
                    if (!_items.Any(r => r.Path == overwriteItem.Path))
                    {
                        UnpackItem unpackItem = new UnpackItem();
                        unpackItem.Image = overwriteItem.ImageShort;
                        unpackItem.Path = overwriteItem.Path;

                        _items.Add(unpackItem);
                    }
                }
                else
                {
                    UnpackItem unpackItem = _items.FirstOrDefault(r => r.Path == overwriteItem.Path);
                    if (unpackItem != null)
                    {
                        _items.Remove(unpackItem);
                    }
                }

                _unpackCommand.RaiseCanExecuteChanged();

                if (_isGeneratedByCode == false)
                {
                    if (!_itemsOverwrite.Any())
                    {
                        _isAllOverwrite = false;
                    }
                    else
                    {
                        _isAllOverwrite = _itemsOverwrite.All(r => r.IsOverwrite == true);
                    }

                    RaisePropertyChanged(() => IsAllOverwrite);
                }

            }
        }

        private string ImageFromEntry(ZipEntry entry)
        {
            if (entry.FileName.ToLower().Contains("maindb"))
                return PackIcons.Database;

            if (entry.FileName.ToLower().Contains("customer"))
                return PackIcons.Customer;

            if (entry.FileName.ToLower().Contains("branch"))
                return PackIcons.Branch;

            if (entry.FileName.ToLower().Contains("inventor"))
                return PackIcons.Inventor;

            return String.Empty;
        }

        private bool UnpackCommandCanExecute()
        {
			bool config = OverwriteConfigIsChecked;
			bool inData = OverwriteInDataIsChecked;
			bool profile = OverwriteProfileIsChecked;
            bool cbi =  _items.Any();
			return config || cbi || inData || profile; 
            //return false;
        }

        private void UnpackCommandExecuted()
        {
            BusyText = Localization.Resources.ViewModel_Unpack_busyText;
            IsBusy = true;

            Task.Factory.StartNew(() =>
                {
                    _ilog = _unityContainer.Resolve<ILog>();
                    _ilog.Clear();
                    _isUnpackOk = true;
                    _totalFilesUnpacked = 0;
                    _customersAffected = 0;
                    _branchesAffected = 0;
                    _inventorsAffected = 0;
                    _cancellationTokenSource = new CancellationTokenSource();

                    List<string> entriesToPostProcess = Unpack();

                    Utils.RunOnUI(() =>
                        {
                            BusyText = Localization.Resources.ViewModel_Unpack_PostProcess;
                            ProgressFile = String.Empty;
                        });

                    if (_isUnpackOk)
                    {
                        PostUnpackDbProcess(entriesToPostProcess);
                    }

                    WriteLogFooter();
                    WriteLogToFile();
					WriteUnpackLogToFile();

                    Utils.RunOnUI(() =>
                        {
                            IsBusy = false;
                            ShowLog();
                        });
				}).LogTaskFactoryExceptions("UnpackCommandExecuted");
        }

        private bool _isUnpackOk;
        private int _totalFilesUnpacked;
        private int _customersAffected;
        private int _branchesAffected;
        private int _inventorsAffected;

        private void WriteLogFooter()
        {
            //_path
            //_isUnpackOk
            //_totalFilesUnpacked ;
            _ilog.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.ViewModel_Unpack_summaryPath, _path));
            _ilog.Add(MessageTypeEnum.SimpleTrace, _isUnpackOk ? Localization.Resources.ViewModel_Unpack_summaryStatusOk : Localization.Resources.ViewModel_Unpack_summaryStatusFailed);
            _ilog.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.ViewModel_Unpack_summaryTotalUnpacked, _totalFilesUnpacked));

            _ilog.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.ViewModel_Unpack_summaryNumberCustomers, _customersAffected));
            _ilog.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.ViewModel_Unpack_summaryNumberBranches, _branchesAffected));
            _ilog.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.ViewModel_Unpack_summaryNumberInventors, _inventorsAffected));
        }

        private void CalculateIsShowMainDbWarning(ZipFile zipFile)
        {
            string result = String.Empty;

            if (IsMainDbSdfInArchive(zipFile))
            {
                result = FileSystem.IsAppRedactionOffice() ? Localization.Resources.View_Unpack_tbMainSdfWarningOffice : Localization.Resources.View_Unpack_tbMainSdfWarning;
            }

            WarningText = result;
        }

        private bool IsEntryNeedOverwrite(ZipEntry entry)
        {
            //            string basePath = _dbSettings.BuildAppDataFolderPath();
            //
            //            DirectoryInfo di = new DirectoryInfo(basePath);
            //
            //            string fullPath = System.IO.Path.Combine(di.Parent.FullName, entry.FileName);
            //
            //            return File.Exists(fullPath);

            string appData = _dbSettings.FolderApp_Data.ToLower();
            string importData = _dbSettings.FolderImport.ToLower();
            string basePath = String.Empty;

            if (entry.FileName.ToLower().StartsWith(appData))
            {
                basePath = new DirectoryInfo(_dbSettings.BuildAppDataFolderPath()).Parent.FullName; //App_Data
            }
            else if (entry.FileName.ToLower().StartsWith(importData))
            {
                basePath = new DirectoryInfo(_dbSettings.ImportFolderPath()).Parent.FullName; //ImportData
            }

            string fullPath = System.IO.Path.Combine(basePath, entry.FileName);
            return File.Exists(fullPath);
        }

        private bool IsEntryMainDbSdf(ZipEntry entry)
        {
            string mainDbSdf = RemoveFolderSlashes(System.IO.Path.Combine(_dbSettings.FolderApp_Data, _dbSettings.MainDBFile.ToLower())).ToLower();

            return RemoveFolderSlashes(entry.FileName).ToLower() == mainDbSdf;
        }

        private bool IsMainDbSdfInArchive(ZipFile zipFile)
        {
            string mainDbSdf = RemoveFolderSlashes(System.IO.Path.Combine(_dbSettings.FolderApp_Data, _dbSettings.MainDBFile.ToLower())).ToLower();

            return zipFile.Entries.Any(r => RemoveFolderSlashes(r.FileName.ToLower()) == mainDbSdf);
        }

        private bool IsEntryLogoFile(ZipEntry entry)
        {
            string logoFolder = RemoveFolderSlashes(System.IO.Path.Combine(_dbSettings.FolderApp_Data, _dbSettings.FolderLogoFile).ToLower());
            return RemoveFolderSlashes(entry.FileName.ToLower()).StartsWith(logoFolder);
        }

        private bool IsEntryImportData(ZipEntry entry)
        {
            string importFolder = _dbSettings.FolderImport;

            return entry.FileName.ToLower().StartsWith(importFolder.ToLower());
        }

        private void CalculateIsShowImportDataCheckboxes(ZipFile zipFile)
        {
            OverwriteImportIsVisible = zipFile.Entries.Any(r => IsFileInImportDataFolder(r.FileName));
            OverwriteInDataIsVisible = zipFile.Entries.Any(r => IsFileInImportInDataFolder(r.FileName));
			OverwriteConfigIsVisible = zipFile.Entries.Any(r => IsFileInImportConfigFolder(r.FileName));
			OverwriteProfileIsVisible = zipFile.Entries.Any(r => IsFileInImportProfileFolder(r.FileName));

            OverwriteImportIsChecked = _overwriteImportIsVisible;
            OverwriteInDataIsChecked = _overwriteInDataIsVisible;
			OverwriteConfigIsChecked = _overwriteConfigIsVisible;
			OverwriteProfileIsChecked = _overwriteProfileIsVisible;
        }

        private bool IsFileInImportDataFolder(string zipRelativePath)
        {
            string importFolder = _dbSettings.FolderImport.ToLower();

            return zipRelativePath.ToLower().StartsWith(importFolder);
        }

        private bool IsFileInImportInDataFolder(string zipRelativePath)
        {
            string importFolder = _dbSettings.FolderImport.ToLower();
            string inData = FileSystem.inData.ToLower();

            return zipRelativePath.ToLower().StartsWith(importFolder) && zipRelativePath.ToLower().Contains(inData);
        }


		private bool IsFileInImportConfigFolder(string zipRelativePath)
        {
            string importFolder = _dbSettings.FolderImport.ToLower();
			string configFolderName = FileSystem.ConfigFolderName.ToLower();

			bool ret = zipRelativePath.ToLower().StartsWith(importFolder) && zipRelativePath.ToLower().Contains(configFolderName);
			return ret;
        }

		private bool IsFileInImportProfileFolder(string zipRelativePath)
		{
			string importFolder = _dbSettings.FolderImport.ToLower();
			string profileFolderName = FileSystem.ProfileFolderName.ToLower();

			bool ret = zipRelativePath.ToLower().StartsWith(importFolder) && zipRelativePath.ToLower().Contains(profileFolderName);
			return ret;
		}

        private void Build()
        {
            using (new CursorWait())
            {
                try
                {
                    UnsubscribeOverwriteItems();

                    _items.Clear();
                    _itemsOverwrite.Clear();
                    OverwriteImportIsVisible = false;
                    OverwriteImportIsVisible = false;
					IsAllOverwrite = false;
                    RaisePropertyChanged(() => IsAllOverwrite);

                    if (String.IsNullOrEmpty(CheckZip()) == false)
                    {
                        return;
                    }

                    using (ZipFile zip = new ZipFile(_path))
                    {
                        CalculateIsShowMainDbWarning(zip);
                        CalculateIsShowImportDataCheckboxes(zip);

                        foreach (ZipEntry entry in zip.Entries)
                        {
                            if (entry.IsDirectory)
                                continue;

                            if (IsEntryMainDbSdf(entry) && FileSystem.IsAppRedactionOffice())
                                continue;

                            if (IsEntryLogoFile(entry)) //do not show logo files
                                continue;

                            if (IsEntryImportData(entry)) //do not show import data files
                                continue;

                            string image = ImageFromEntry(entry);

                            if (IsEntryNeedOverwrite(entry))
                            {
                                UnpackOverwriteItem overwriteItem = new UnpackOverwriteItem();
                                overwriteItem.Path = entry.FileName;
                                overwriteItem.Image = image;
                                _itemsOverwrite.Add(overwriteItem);
                            }
                            else
                            {
                                UnpackItem item = new UnpackItem();
                                item.Path = entry.FileName;
                                item.Image = image;
                                _items.Add(item);
                            }
                        }
                    }

                    SubscribeOverwriteItems();

					IsAllOverwrite = true;
					RaisePropertyChanged(() => IsAllOverwrite);

                    _unpackCommand.RaiseCanExecuteChanged();
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("Build", exc);
                }
            }
        }

        private List<string> Unpack()
        {
            List<string> entriesToPostProcess = new List<string>();

            try
            {
                string appData = _dbSettings.FolderApp_Data;

                using (ZipFile zip = new ZipFile(_path))
                {
                    bool mainDbSdfInArchive = IsMainDbSdfInArchive(zip);
                    bool willUnpackMainSdf = !FileSystem.IsAppRedactionOffice() && mainDbSdfInArchive;

                    List<ZipEntry> entriesToUnpack = new List<ZipEntry>();

                    foreach (ZipEntry entry in zip.Entries)
                    {
						
                        if (entry.IsDirectory)
                            continue;

                        if (IsEntryMainDbSdf(entry) && FileSystem.IsAppRedactionOffice()) //do not unpack maindb.sdf
                            continue;

                        if (IsEntryLogoFile(entry) == true) //do not unpack logo files if main.sdf is not unpacked
                        {
                            if (!willUnpackMainSdf)
                                continue;
                        }
						else if (IsFileInImportConfigFolder(entry.FileName) == true) //Config 
						{
							if (_overwriteConfigIsChecked == false)
							continue;
						}
						else if (IsFileInImportProfileFolder(entry.FileName) == true) //Profile 
						{
							if (_overwriteProfileIsChecked == false)
								continue;
						}
                        else if (IsEntryImportData(entry) == true)
                        {
							bool entryIsInData = IsFileInImportInDataFolder(entry.FileName);

                            if (entryIsInData == true) //inData 
                            {
                                if (_overwriteInDataIsChecked == false)
                                    continue;
                            }
                            else //plain Importdata
                            {
                                if (_overwriteImportIsChecked == false)
                                    continue;
                            }
                        }
                        else if (!_items.Any(r => r.Path.ToLower() == entry.FileName.ToLower())) //not in result grid
                        {
                            continue;
                        }

                        entriesToUnpack.Add(entry);

                        //files to postprocess
                        string fileName = RemoveFolderSlashes(entry.FileName.ToLower());
                        string customer = RemoveFolderSlashes(System.IO.Path.Combine(appData, "customer").ToLower());
                        string branch = RemoveFolderSlashes(System.IO.Path.Combine(appData, "branch").ToLower());
                        string inventor = RemoveFolderSlashes(System.IO.Path.Combine(appData, "inventor").ToLower());
                        if (fileName.StartsWith(customer) || fileName.StartsWith(branch) || fileName.StartsWith(inventor))
                        {
                            if (fileName.StartsWith(customer))
                                _customersAffected++;
                            if (fileName.StartsWith(branch))
                                _branchesAffected++;
                            if (fileName.StartsWith(inventor))
                                _inventorsAffected++;

                            string entryToPostProcesss = entry.FileName.Remove(0, appData.Length).Trim(new[] { '/' });
                            if (entryToPostProcesss.Contains(".sdf") == true)
                            {
                                entriesToPostProcess.Add(entryToPostProcesss);
                            }
                        }
                    }

                    UnpackInner(zip, entriesToUnpack);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Unpack", exc);
                _isUnpackOk = false;
            }

            return entriesToPostProcess;
        }

        private void UnpackInner(ZipFile zip, List<ZipEntry> entriesToUnpack)
        {
            int totalEntries = entriesToUnpack.Count();
            int processed = 0;
            ZipEntry curEntry = null;

            zip.ExtractProgress += (s, e) =>
            {
                if (e.CurrentEntry == null)
                    return;

                if (curEntry != e.CurrentEntry)
                {
                    _totalFilesUnpacked++;
                    processed++;
                    curEntry = e.CurrentEntry;
                }
                double v = e.BytesTransferred * 100 / (double)e.TotalBytesToTransfer;

                string fileName = curEntry.FileName;
                UpdateProgress(v, String.Format("{0}/{1}", processed, totalEntries), fileName);

                if (_cancellationTokenSource.IsCancellationRequested)
                    e.Cancel = true;
            };

            string basePath = String.Empty; //base path is calculated

            string appData = _dbSettings.FolderApp_Data.ToLower();
            string importData = _dbSettings.FolderImport.ToLower();

            foreach (ZipEntry zipEntry in entriesToUnpack)
            {
                if (zipEntry.FileName.ToLower().StartsWith(appData))
                {
                    basePath = new DirectoryInfo(_dbSettings.BuildAppDataFolderPath()).Parent.FullName; //App_Data
                }
                else if (zipEntry.FileName.ToLower().StartsWith(importData))
                {
                    basePath = new DirectoryInfo(_dbSettings.ImportFolderPath()).Parent.FullName; //ImportData
                }
                else
                {
                    throw new InvalidOperationException();
                }

                if (!zipEntry.IsDirectory) //check for .tmp files
                {
                    string tmpFileName = String.Format("{0}.tmp", zipEntry.FileName);
                    string tmpFilePath = System.IO.Path.Combine(basePath, tmpFileName);
                    if (File.Exists(tmpFilePath))
                    {
                        _logger.Info("Unpack - .tmp file found: {0}", tmpFilePath);
                        File.Delete(tmpFilePath);
                    }
                }

                try
                {
                    zipEntry.Extract(basePath, ExtractExistingFileAction.OverwriteSilently);
                    _logger.Info(String.Format("Unpacked: basePath={0} entry={1}", basePath, zipEntry.FileName));
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("zipEntry.Extract", exc);

                    if ((exc is ArgumentException) == false)
                    {
                        throw;
                    }
                }

                //  _ilog.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.ViewModel_Unpack_fileDone, zipEntry.FileName));
            }
        }

        private bool BusyCancelCommandCanExecute()
        {
            return false;
        }

        private void BusyCancelCommandExecuted()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        void UpdateProgress(double progress, string currentInTotal, string file)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Progress = progress;
                ProgressFile = file;
                ProgressState = currentInTotal;
            }));
        }

        private void ShowLog()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(settings, base.Context);
            Utils.AddDbContextToDictionary(settings, base.CBIDbContext);
            UtilsConvert.AddObjectToDictionary(settings, _navigationRepository, _ilog.PrintLog(ImportExportLoglevel.Advanced));

            object result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.LogView,
                WindowTitles.ViewUnpackLog,
                540,
                500,
                ResizeMode.CanResize,
                settings,
                Application.Current.MainWindow);
        }

        private void PostUnpackDbProcess(List<string> fileNameList)
        {
            try
            {
                string count4uFile = _dbSettings.Count4UDBFile.ToLower();

                List<string> fileNameListSorted = fileNameList.OrderBy(r => r, new CBIComparer()).ToList();
                List<string> relativePathList = new List<string>();

                foreach (string fileName in fileNameListSorted)
                {
					if (fileName.ToLower().Contains(count4uFile) == true)
					{
						relativePathList.Add(fileName.ToLower().Replace(count4uFile, String.Empty).Trim('/'));
					}
                }

                _logger.Info("_alterAdoProvider.UpdateCount4UDBViaScript");
                _alterAdoProvider.UpdateCount4UDBViaScript(relativePathList);
				_alterAdoProvider.AlterTableIturAnalyzesCount4UDBViaScript(relativePathList);
				_alterAdoProvider.UpdateAnalyticDBViaScript(relativePathList);
				

                _logger.Info("_inventorConfigRepository.RestoreDomainObject");
                foreach (string relative in relativePathList)
                {
                    string slashesFixed = relative.Replace('/', '\\');
                    _inventorConfigRepository.RestoreDomainObject(slashesFixed);
                }

            }
            catch (Exception e)
            {
                _logger.ErrorException("PostUnpackDbProcess", e);
                _isUnpackOk = false;
            }
        }

        private class CBIComparer : IComparer<string>
        {
            private int GetWeight(string str)
            {
                if (str.ToLower().StartsWith("customer"))
                    return 10;

                if (str.ToLower().StartsWith("branch"))
                    return 20;

                if (str.ToLower().StartsWith("inventor"))
                    return 30;

                return 0;
            }

            public int Compare(string x, string y)
            {
                int xWeight = GetWeight(x);
                int yWeight = GetWeight(y);

                return xWeight.CompareTo(yWeight);
            }
        }

        private void WriteLogToFile()
        {
            try
            {
                string log = _ilog.PrintLog(ImportExportLoglevel.Advanced);

                FileInfo fi = new FileInfo(_path);
                string fileName = String.Format("{0}.log.txt", fi.Name);
                string filePath = System.IO.Path.Combine(fi.Directory.FullName, fileName);

                File.WriteAllText(filePath, log);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("WriteLogToFile", exc);
            }
        }


		public String GetEnglishMonth(int iMonthNo)
		{
			String[] months = new String[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
			return months[iMonthNo - 1];
		}

		private void WriteUnpackLogToFile()
		{

			try
			{
				string logPath = this._settingsRepository.LogPath;
				if (Directory.Exists(logPath) == false)
				{
					Directory.CreateDirectory(logPath);
				}
				string filePathName = System.IO.Path.GetFileName(_path); 
				//DateTime now = DateTime.Now;
				//string log = GetEnglishMonth(now.Month) + "_" + now.Day.ToString() + "_" + filePathName;
				string log = filePathName;

				string fileName = String.Format("Unpack.log.txt");
				string filePath = System.IO.Path.Combine(logPath, fileName);
				if (File.Exists(filePath) == true)
				{
					string logOld =  File.ReadAllText(filePath);
					log = log + Environment.NewLine + logOld;
				}
				File.WriteAllText(filePath, log);
			}
			catch (Exception exc)
			{
				_logger.ErrorException("WriteLogToFile", exc);
			}
		}


    }
}