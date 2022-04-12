using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Model.Transfer;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Common.Extensions;
using Count4U.Common.Events;
using NLog;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.Zip
{
    public class ZipExportViewModel : NotificationObject, INavigationAware
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IZip _zip;
        private readonly IEventAggregator _eventAggregator;
        private readonly ZipExclusionRules _zipExclusionRules;
        private readonly IDBSettings _dbSettings;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly ObservableCollection<ZipExportItemViewModel> _items;
        private readonly DelegateCommand _exportCommand;
        private readonly DelegateCommand _busyCancelCommand;

        private readonly InteractionRequest<MessageBoxNotification> _messageBoxRequest;

        private bool _isBusy;
        private CancellationTokenSource _cancellationTokenSource;

        private double _progress;
        private string _progressState;
        private string _progressFile;

        private bool _toggleCheck;
        private bool _toggle;        

        public ZipExportViewModel(
            IZip zip,
            IEventAggregator eventAggregator,
            ZipExclusionRules zipExclusionRules,
            IDBSettings dbSettings,
            IContextCBIRepository contextCbiRepository,
            IUserSettingsManager userSettingsManager
            )
        {
            _userSettingsManager = userSettingsManager;
            _contextCbiRepository = contextCbiRepository;
            _dbSettings = dbSettings;
            _zipExclusionRules = zipExclusionRules;
            this._eventAggregator = eventAggregator;
            this._zip = zip;

            this._items = new ObservableCollection<ZipExportItemViewModel>();
            this._exportCommand = new DelegateCommand(ExportCommandExecuted, ExportCommandCanExecute);
            this._busyCancelCommand = new DelegateCommand(ExportCancelCommandExecuted);
            this._messageBoxRequest = new InteractionRequest<MessageBoxNotification>();
        }

        public ObservableCollection<ZipExportItemViewModel> Items
        {
            get { return this._items; }
        }

        public DelegateCommand ExportCommand
        {
            get { return this._exportCommand; }
        }

        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {
                this._isBusy = value;
                RaisePropertyChanged(() => IsBusy);

                Progress = 0;
                ProgressState = Localization.Resources.ViewModel_ZipExport_Checking;
            }
        }

        public DelegateCommand BusyCancelCommand
        {
            get { return this._busyCancelCommand; }
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

        public string ProgressState
        {
            get { return this._progressState; }
            set
            {
                this._progressState = value;
                RaisePropertyChanged(() => ProgressState);
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
                foreach (ZipExportItemViewModel item in this._items)
                    item.IsChecked = this._toggleCheck;
                this._toggle = false;
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

        public string BusyText
        {
            get { return Localization.Resources.ViewModel_ZipExport_Packing; }
        }

        public InteractionRequest<MessageBoxNotification> MessageBoxRequest
        {
            get { return this._messageBoxRequest; }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            BuildRootNodes();
            BuildFilesIconsInTree();
            BuildTooltipsInTree();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        void BuildRootNodes()
        {
            List<Tuple<ZipRootFolder, string>> rootNodesInfo = new List<Tuple<ZipRootFolder, string>>();
            rootNodesInfo.Add(new Tuple<ZipRootFolder, string>(ZipRootFolder.Db, "database"));
            rootNodesInfo.Add(new Tuple<ZipRootFolder, string>(ZipRootFolder.Import, "importiturim"));
            rootNodesInfo.Add(new Tuple<ZipRootFolder, string>(ZipRootFolder.Reports, "report_small"));
            rootNodesInfo.Add(new Tuple<ZipRootFolder, string>(ZipRootFolder.ExportData, "export_data_zip_icon"));
#if DEBUG
            rootNodesInfo.Add(new Tuple<ZipRootFolder, string>(ZipRootFolder.Adapters, "module_small"));
            rootNodesInfo.Add(new Tuple<ZipRootFolder, string>(ZipRootFolder.ExportAdapters, "module_small"));
#endif

            foreach (Tuple<ZipRootFolder, string> info in rootNodesInfo)
            {
                ZipRootFolder rootFolder = info.Item1;
                string icon = info.Item2;

                string folderPath = this._zip.GetRootFolder(rootFolder).PathOnDisk;

                if (!String.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
                {
                    DirectoryInfo di = new DirectoryInfo(folderPath);
                    ZipExportItemViewModel rootNode = new ZipExportItemViewModel(di, null, NodePropertyChangedHandler, rootFolder, 0, null);
                    rootNode.Header = di.Name;
                    rootNode.Image = icon;
                    rootNode.IsExpanded = true;

                    this._items.Add(rootNode);

                    BuildChildNode(rootNode);
                }
            }
        }

        private void BuildChildNode(ZipExportItemViewModel parent)
        {
            if (parent.DirectoryInfo == null) return;

            int level = parent.Level + 1;

            foreach (DirectoryInfo di in parent.DirectoryInfo.EnumerateDirectories()) //build directories
            {
                try
                {
                    if (!this._zipExclusionRules.IsAcceptableForPack(parent.ZipRootFolder, di.FullName, level))
                        continue;

                    var zipItem = new ZipExportItemViewModel(di, null, this.NodePropertyChangedHandler, parent.ZipRootFolder, level, parent);
                    zipItem.Image = "folder";
                    zipItem.Header = di.Name;
                    zipItem.IsChecked = parent.IsChecked;
                    parent.Children.Add(zipItem);

                    BuildChildNode(zipItem);
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("BuildChildren, enumerating directories", exc);
                }
            }

            foreach (FileInfo fi in parent.DirectoryInfo.EnumerateFiles()) //build files
            {
                if (!this._zipExclusionRules.IsAcceptableForPack(parent.ZipRootFolder, fi.FullName, level))
                    continue;

                try
                {
                    var zipItem = new ZipExportItemViewModel(null, fi, this.NodePropertyChangedHandler, parent.ZipRootFolder, level, parent);
                    zipItem.Header = fi.Name;
                    zipItem.IsChecked = parent.IsChecked;

                    parent.Children.Add(zipItem);

                    BuildChildNode(zipItem);
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("BuildChildren, enumerating files", exc);
                }
            }
        }

        public void NodePropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                this._exportCommand.RaiseCanExecuteChanged();

                if (!_toggle)
                {
                    bool isAnyChecked = this._items.FlattenHierarchyNodes(r => r.GetChildren().OfType<ZipExportItemViewModel>()).Any(z => z.IsChecked);

                    this._toggleCheck = isAnyChecked;
                    RaisePropertyChanged(() => ToggleCheck);
                }
            }
        }

        private bool ExportCommandCanExecute()
        {
            return this._items.FlattenHierarchyNodes(r => r.GetChildren().Cast<ZipExportItemViewModel>()).Any(r => r.IsChecked);
        }

        void FillZipInfo(ZipPackInfo zipInfo, ZipExportItemViewModel rootNode)
        {
            var flatten = rootNode.FlattenHierarchyNode(r => r.GetChildren().Cast<ZipExportItemViewModel>()).Where(r => r.IsChecked && r != rootNode).ToList();
            foreach (ZipExportItemViewModel node in flatten)
            {
                if (node.FileInfo != null)
                {
                    zipInfo.Files.Add(node.FileInfo);
                }
                else
                {
                    if (node.DirectoryInfo != null && !node.GetChildren().Any(r => r.IsChecked))
                    {
                        zipInfo.EmptyFolders.Add(node.DirectoryInfo);
                    }
                }
            }
        }

        private void ExportCommandExecuted()
        {
            this._cancellationTokenSource = new CancellationTokenSource();

			Task.Factory.StartNew(Export, this._cancellationTokenSource.Token).LogTaskFactoryExceptions("ExportCommandExecuted");
        }

        void Export()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                     {
                                                                         IsBusy = true;
                                                                         this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(true);
                                                                     }));   
                            
                var packInfo = new List<ZipPackInfo>();

                foreach (ZipExportItemViewModel rootNode in this._items)
                {
                    ZipPackInfo pInfo = new ZipPackInfo();
                    pInfo.RootFolder = rootNode.ZipRootFolder;
                    FillZipInfo(pInfo, rootNode);
                    packInfo.Add(pInfo);
                }

                ZipPackInfo dbPackInfo = packInfo.FirstOrDefault(r => r.RootFolder == ZipRootFolder.Db);
                {
                    if (dbPackInfo != null && dbPackInfo.Files.Any())
                    {
                        _contextCbiRepository.RefillAllCBIInventorConfigs();
                    }
                }

                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                String path = this._zip.Pack(packInfo, this._cancellationTokenSource.Token, UpdateProgress, desktopFolder);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                     {
                                                                         IsBusy = false;
                                                                         this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(false);
                                                                     }));

                if (String.IsNullOrEmpty(path)) //open file in file system
                {
                    if (!_cancellationTokenSource.Token.IsCancellationRequested)
                        ShowPackErrorMessage();
                }
                else
                {
                    Utils.OpenFileInExplorer(path);                    
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Export", exc);
                ShowPackErrorMessage();
            }
        }

        private void ShowPackErrorMessage()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                                                                 {
                                                                     IsBusy = false;
                                                                     this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(false);

                                                                     string message = String.Format(Localization.Resources.Msg_Common_Error_Occured);
                                                                     MessageBoxNotification notification = new MessageBoxNotification
                                                                                                               {
                                                                                                                   Image = MessageBoxImage.Error, 
                                                                                                                   Content = message,
                                                                                                                   Settings = this._userSettingsManager,
                                                                                                               };
                                                                     this._messageBoxRequest.Raise(notification);
                                                                 }));
        }

        private void UpdateProgress(double progress, string processedTotal, string fileName)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Progress = progress;
                ProgressFile = fileName;
                ProgressState = processedTotal;
            }));
        }

        private void ExportCancelCommandExecuted()
        {
            this._cancellationTokenSource.Cancel();

            IsBusy = false;
        }

        private void BuildFilesIconsInTree()
        {
			Task.Factory.StartNew(BuildFilesIconsInTreeTask).LogTaskFactoryExceptions("BuildFilesIconsInTree");
        }

        private void BuildTooltipsInTree()
        {
			Task.Factory.StartNew(BuildTooltipsInTreeTask).LogTaskFactoryExceptions("BuildTooltipsInTree");
        }

        void BuildFilesIconsInTreeTask()
        {
            try
            {
                List<ZipExportItemViewModel> nodes = this._items.FlattenHierarchyNodes(r => r.GetChildren().OfType<ZipExportItemViewModel>()).Where(r => r.FileInfo != null).ToList();
                Dictionary<string, Icon> iconCache = new Dictionary<string, Icon>();

                foreach (ZipExportItemViewModel node in nodes)
                {
                    FileInfo fi = node.FileInfo;
                    string extension = fi.Extension;

                    if (String.IsNullOrEmpty(extension))
                        continue;

                    if (!iconCache.ContainsKey(extension))
                    {
                        Icon ico = Icon.ExtractAssociatedIcon(fi.FullName);
                        iconCache.Add(extension, ico);
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        BitmapImage bi = iconCache[extension].ToBitmapImage();
                        node.Image = bi;
                    }));
                }
            }
            catch (Exception e)
            {
                _logger.ErrorException("BuildFilesIconsInTreeTask", e);
            }
        }

        void BuildTooltipsInTreeTask()
        {
            try
            {
                List<ZipExportItemViewModel> nodes = this._items.FlattenHierarchyNodes(r => r.GetChildren().OfType<ZipExportItemViewModel>()).Where(r => r.FileInfo != null).ToList();
                foreach (ZipExportItemViewModel item in nodes)
                {
                    if (item.FileInfo.Name.ToLower() == this._dbSettings.Count4UDBFile.ToLower() && item.Parent != null && item.Parent.DirectoryInfo != null)
                    {
                        DirectoryInfo di = item.Parent.DirectoryInfo;
                        string objectCode = di.Name;
                        string tooltip = String.Empty;
                        Customer customer = this._contextCbiRepository.GetCustomerByCode(objectCode);
                        if (customer != null)
                        {
                            tooltip = customer.Name;
                        }
                        else
                        {
                            Branch branch = this._contextCbiRepository.GetBranchByCode(objectCode);
                            if (branch != null)
                            {
                                tooltip = branch.Name;
                            }
                            else
                            {
                                Inventor inventor = this._contextCbiRepository.GetInventorByCode(objectCode);
                                if (inventor != null)
                                {
                                    tooltip = UtilsConvert.DateToStringLong(inventor.InventorDate);
                                }
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(tooltip))
                            Utils.RunOnUI(() =>
                                              {
                                                  item.Tooltip = tooltip;
                                                  item.Parent.Tooltip = tooltip;
                                              });

                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildTooltipsInTreeTask", exc);
            }
        }
    }
}