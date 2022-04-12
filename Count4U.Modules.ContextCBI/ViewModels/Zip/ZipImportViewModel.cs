using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Model.Interface;
using Count4U.Model.Transfer;
using Ionic.Zip;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using NLog;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.Zip
{
    public class ZipImportViewModel : NotificationObject, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly ZipExclusionRules _zipExclusionRules;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IZip _zip;
        private readonly IAlterADOProvider _alterAdoProvider;

        private readonly DelegateCommand _browseCommand;
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _busyCancelCommand;

        private readonly ObservableCollection<ZipImportItemViewModel> _items;

        private readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;
        private readonly InteractionRequest<MessageBoxNotification> _messageBoxRequest;
        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;

        private string _path;
        private bool _toggleCheck;
        private bool _isBusy;
        private bool _isPathOk;
        private bool _isOverwrite;
        private bool _isClearPreviously;

        private string _busyText;
        private double _progress;
        private string _progressFile;
        private string _progressState;

        private bool _toggle;
        private string _pathValidationResult;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _isCancelDisabled;        

        public ZipImportViewModel(
            IZip zip,
            IEventAggregator eventAggregator,
            ZipExclusionRules zipExclusionRules,
            IAlterADOProvider alterAdoProvider,
            IUserSettingsManager userSettingsManager)
        {
            this._alterAdoProvider = alterAdoProvider;
            this._userSettingsManager = userSettingsManager;
            this._zipExclusionRules = zipExclusionRules;
            this._eventAggregator = eventAggregator;
            this._zip = zip;
            this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();
            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
            this._browseCommand = new DelegateCommand(BrowseCommandExecuted);
            this._importCommand = new DelegateCommand(ImportCommandExecuted, ImportCommandCanExecute);
            this._items = new ObservableCollection<ZipImportItemViewModel>();
            this._busyCancelCommand = new DelegateCommand(ImportCancelCommandExecuted, ImportCancelCommandCanExecute);
            this._messageBoxRequest = new InteractionRequest<MessageBoxNotification>();

            this._isOverwrite = true;
        }       

        public InteractionRequest<OpenFileDialogNotification> FileChooseDilogRequest
        {
            get { return this._fileChooseDilogRequest; }
        }

        public DelegateCommand BrowseCommand
        {
            get { return this._browseCommand; }
        }

        public string Path
        {
            get { return this._path; }
            set
            {
                this._path = value;
                RaisePropertyChanged(() => Path);

                this._items.Clear();
                this._pathValidationResult = CheckZip();
                if (String.IsNullOrEmpty(this._pathValidationResult))
                    BuildTree();

                this._importCommand.RaiseCanExecuteChanged();

                IsPathOk = String.IsNullOrEmpty(this._pathValidationResult) && !String.IsNullOrEmpty(this._path);
            }
        }

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

        public ObservableCollection<ZipImportItemViewModel> Items
        {
            get { return this._items; }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public DelegateCommand BusyCancelCommand
        {
            get { return this._busyCancelCommand; }
        }

        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {
                this._isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public double Progress
        {
            get { return this._progress; }
            set
            {
                this._progress = value;
                RaisePropertyChanged(() => Progress);
            }
        }

        public bool ToggleCheck
        {
            get { return this._toggleCheck; }
            set
            {
                this._toggleCheck = value;
                RaisePropertyChanged(() => ToggleCheck);

                this._toggle = true;
                foreach (ZipImportItemViewModel item in this._items)
                    item.IsChecked = this._toggleCheck;
                this._toggle = false;
            }
        }

        public bool IsPathOk
        {
            get { return this._isPathOk; }
            set
            {
                this._isPathOk = value;
                RaisePropertyChanged(() => IsPathOk);
            }
        }

        public string ProgressFile
        {
            get { return this._progressFile; }
            set
            {
                this._progressFile = value;
                RaisePropertyChanged(() => ProgressFile);
            }
        }

        public string ProgressState
        {
            get { return this._progressState; }
            set
            {
                this._progressState = value;
                RaisePropertyChanged(() => ProgressState);
            }
        }

        public string BusyText
        {
            get { return this._busyText; }
            set { this._busyText = value; }
        }

        public bool IsOverwrite
        {
            get { return this._isOverwrite; }
            set
            {
                this._isOverwrite = value;
                RaisePropertyChanged(() => IsOverwrite);
            }
        }

        public bool IsClearPreviously
        {
            get { return this._isClearPreviously; }
            set
            {
                this._isClearPreviously = value;
                RaisePropertyChanged(() => IsClearPreviously);
            }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return this._yesNoRequest; }
        }

        public InteractionRequest<MessageBoxNotification> MessageBoxRequest
        {
            get { return this._messageBoxRequest; }
        }

        public void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                this._importCommand.RaiseCanExecuteChanged();

                if (!_toggle)
                {
                    bool isAnyChecked = this._items.FlattenHierarchyNodes(r => r.GetChildren().OfType<ZipImportItemViewModel>()).Any(z => z.ZipEntry != null && z.IsChecked);

                    this._toggleCheck = isAnyChecked;
                    RaisePropertyChanged(() => ToggleCheck);
                }
            }
        }

        private void BrowseCommandExecuted()
        {
            OpenFileDialogNotification notification = new OpenFileDialogNotification();

            notification.DefaultExt = ".zip";
            notification.Filter = "Zip archive(*.zip)|*.zip|All files (*.*)|*.*";
            notification.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            this._fileChooseDilogRequest.Raise(notification, FileBrowse);
        }

        private void FileBrowse(OpenFileDialogNotification openFileDialogNotification)
        {
            if (!openFileDialogNotification.IsOK) return;
            Path = openFileDialogNotification.FileName;
        }

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

        string CheckZip()
        {
            if (String.IsNullOrEmpty(this._path))
                return String.Empty;

            if (!File.Exists(this._path))
                return Localization.Resources.ViewModel_ZipImport_FileNotExist;

            if (!ZipFile.IsZipFile(this._path))
                return Localization.Resources.ViewModel_ZipImport_FileNotZip;

            if (!_zip.IsZipInCorrectFormat(this._path))
                return Localization.Resources.ViewModel_ZipImport_FileNotCount4UZip;

            return String.Empty;
        }



        void BuildTree()
        {
            using (ZipFile zip = new ZipFile(this._path))
            {
                foreach (ZipEntry zipEntry in zip)
                {
                    BuildEntry(zipEntry);
                }
            }

            foreach (var item in _items.FlattenHierarchyNodes(r => r.GetChildren().Cast<ZipImportItemViewModel>()))
                item.IsChecked = true;
        }

        void BuildEntry(ZipEntry zipEntry)
        {
            string[] segments = zipEntry.FileName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (!segments.Any())
                return;

            try
            {
                ZipImportItemViewModel curNode = null;
                foreach (string segment in segments)
                {
                    bool isLast = segments.Last() == segment;
                    if (curNode == null)
                    {
                        curNode = this.Items.FirstOrDefault(r => r.Header == segment); //root node
                        if (curNode == null)
                        {
                            ZipRootFolder zipRootFolder = FromNameToZipRootFolder(segment);

                            curNode = new ZipImportItemViewModel(PropertyChangedHandler)
                                          {
                                              Header = segment,
                                              Image = null,
                                              IsExpanded = true,
                                              ZipEntry = isLast ? zipEntry : null,
                                              ZipRootFolder = zipRootFolder,
                                              Parent = null,
                                          };
                            SetItemImage(curNode);
                            this.Items.Add(curNode);
                        }
                    }
                    else //one of child nodes in hierarchy
                    {
                        ZipImportItemViewModel child = curNode.GetChildren().OfType<ZipImportItemViewModel>().FirstOrDefault(r => r.Header == segment);
                        if (child != null)
                            curNode = child;
                        else
                        {
                            if (!this._zipExclusionRules.IsAcceptableForUnpack(zipEntry))
                                continue;

                            child = new ZipImportItemViewModel(PropertyChangedHandler)
                                        {
                                            Header = segment,
                                            Image = null,
                                            IsExpanded = false,
                                            ZipEntry = segments.Last() == segment ? zipEntry : null,
                                            Parent = curNode,
                                        };
                            SetItemImage(child);
                            curNode.Children.Add(child);
                            curNode = child;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildEntry", exc);
                throw;
            }
        }

        private ZipRootFolder FromNameToZipRootFolder(string name)
        {
            if (name == _zip.AppData)
                return ZipRootFolder.Db;

            if (name == _zip.ImportData)
                return ZipRootFolder.Import;

            if (name == _zip.ReportTemplate)
                return ZipRootFolder.Reports;

            if (name == _zip.ExportAdapters)
                return ZipRootFolder.ExportAdapters;

            if (name == _zip.ExportData)
                return ZipRootFolder.ExportData;


            return ZipRootFolder.Db;
        }

        private string FromZipRootFolderToImage(ZipRootFolder folder)
        {
            switch (folder)
            {
                case ZipRootFolder.Db:
                    return "database";
                case ZipRootFolder.Import:
                    return "importiturim";
                case ZipRootFolder.Adapters:
                case ZipRootFolder.ExportAdapters:
                    return "module_small";
                case ZipRootFolder.Reports:
                    return "report_small";
                case ZipRootFolder.ExportData:
                    return "export_data_zip_icon";
                default:
                    throw new ArgumentOutOfRangeException("folder");
            }

        }

        void SetItemImage(ZipImportItemViewModel item)
        {
            if (item.ZipEntry == null || item.ZipEntry.IsDirectory)
            {
                if (item.Parent == null && item.ZipRootFolder != null) //root node
                    item.Image = FromZipRootFolderToImage(item.ZipRootFolder.Value);
                else
                    item.Image = "folder";
            }
            else
            {
                try
                {
                    FileInfo fi = new FileInfo(item.Header);

                    string extension = fi.Extension;
                    if (extension == ".layout")
                        return;

                    item.Image = Icons.IconFromExtensionShell(extension, Icons.SystemIconSize.Small).ToBitmapImage();
                }
                catch (Exception exc)
                {
                    _logger.InfoException("SetItemImage", exc);
                }
            }
        }

        private bool ImportCommandCanExecute()
        {
            bool isAnyChecked = this._items.FlattenHierarchyNodes(r => r.GetChildren().OfType<ZipImportItemViewModel>()).Any(z => z.ZipEntry != null && z.IsChecked);

            return !String.IsNullOrWhiteSpace(this._path) && String.IsNullOrEmpty(this._pathValidationResult) &&
                isAnyChecked;
        }


        private void ImportCancelCommandExecuted()
        {
            this._cancellationTokenSource.Cancel();
        }

        private bool ImportCancelCommandCanExecute()
        {
            return !_isCancelDisabled;
        }

        private void ImportCommandExecuted()
        {
            if (this._isClearPreviously)
            {
                var rootFolder = this._zip.GetRootFolder(ZipRootFolder.Import);
                DirectoryInfo di = new DirectoryInfo(rootFolder.PathOnDisk);
                MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
                notification.Settings = this._userSettingsManager;
                notification.Content = String.Format(Localization.Resources.Msg_Clear_Import_Directory, di.FullName);
                notification.Image = MessageBoxImage.Warning;
                this._yesNoRequest.Raise(notification, r =>
                                                      {
                                                          if (r.IsYes)
                                                              RunImport();
                                                      });
            }
            else
            {
                RunImport();
            }

        }

        void RunImport()
        {
            this._cancellationTokenSource = new CancellationTokenSource();
			Task.Factory.StartNew(Import, this._cancellationTokenSource.Token).LogTaskFactoryExceptions("RunImport");
        }

        void Import()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                 {
                                                                     BusyText = Localization.Resources.ViewModel_ZipImport_Unpacking;
                                                                     IsBusy = true;
                                                                     this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(true);
                                                                 }));

            try
            {
                ZipUnpackInfo info = new ZipUnpackInfo();
                info.ZipFilePath = this._path;
                info.IsOverwrite = this._isOverwrite;
                info.IsClearImportData = this._isClearPreviously;

                foreach (ZipRootFolder en in Enum.GetValues(typeof(ZipRootFolder)))
                {
                    ZipImportItemViewModel root = this._items.FirstOrDefault(r => r.ZipRootFolder == en);
                    if (root != null)
                    {
                        ZipUnpackInfoItem infoItem = new ZipUnpackInfoItem();
                        infoItem.RootFolder = en;
                        infoItem.Entries = root.FlattenHierarchyNode(r => r.GetChildren().OfType<ZipImportItemViewModel>())
                            .Where(r => r.ZipEntry != null && r.IsChecked)
                            .Select(r => r.ZipEntry).ToList();

                        info.Items.Add(infoItem);
                    }
                    else
                    {

                    }
                }


                if (info.Items.Any())
                    this._zip.Unpack(info, this._cancellationTokenSource.Token, UpdateProgress);

                if (_cancellationTokenSource.Token.IsCancellationRequested == false && 
                    info.Items.Any(r=>r.Entries.Any(z=>z.FileName.ToLower().EndsWith(".sdf"))))
                {
                    this._isCancelDisabled = true;
                    this._busyCancelCommand.RaiseCanExecuteChanged();
                    this.BusyText = Localization.Resources.Msg_CheckingDb;

                    _logger.Info("AlterAdoProvider.UpdateDBViaScript() start");

                    this._alterAdoProvider.UpdateDBViaScript();

                    _logger.Info("AlterAdoProvider.UpdateDBViaScript() end");
                }

                Utils.RunOnUI(()=>
                           {
                               IsBusy = false;
                               this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(false);

                               if (info.Items.Any(r => r.RootFolder == ZipRootFolder.Adapters || r.RootFolder == ZipRootFolder.ExportAdapters))
                               {
                                   string message = Localization.Resources.Msg_Zip_Changes_Apply_After_Restart;
                                   MessageBoxNotification notification = new MessageBoxNotification
                                   {
                                       Image = MessageBoxImage.Information,
                                       Content = message,
                                       Settings = this._userSettingsManager
                                   };
                                   this._messageBoxRequest.Raise(notification);
                               }
                           }
                );
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Import", exc);
                ShowUnpackErrorMessage();
            }
            finally
            {
                Utils.RunOnUI(() => IsBusy = false);
                this._isCancelDisabled = false;
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

        private void ShowUnpackErrorMessage()
        {
            Utils.RunOnUI(()=>
            {
                IsBusy = false;
                this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(false);

                string message = String.Format(Localization.Resources.Msg_Common_Error_Occured);
                MessageBoxNotification notification = new MessageBoxNotification { Image = MessageBoxImage.Error, Content = message, Settings = this._userSettingsManager };
                this.MessageBoxRequest.Raise(notification);
            });
        }
    }
}