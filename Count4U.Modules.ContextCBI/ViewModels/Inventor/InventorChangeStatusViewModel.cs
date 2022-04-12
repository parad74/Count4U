using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System.Linq;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class InventorChangeStatusViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IStatusInventorRepository _statusInventorRepository;
        private readonly IDBSettings _dbSettings;

        private StatusInventors _statuses;
        private StatusInventor _currentStatus;
        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private readonly SendDataOffice _sendDataOffice;

        private DateTime _inventorDate;

        private bool _includeSdf;
        private bool _includeInventoryFiles;

        private string _zipPath;

        private bool _isBusy;
        private string _busyContent;

        private readonly DelegateCommand _sendDataCommand;
        private readonly DelegateCommand _printCommand;

        public InventorChangeStatusViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IStatusInventorRepository statusInventorRepository,
            SendDataOffice sendDataOffice,
            IDBSettings dbSettings)
            : base(contextCBIRepository)
        {
            this._dbSettings = dbSettings;
            this._sendDataOffice = sendDataOffice;
            this._statusInventorRepository = statusInventorRepository;
            this._eventAggregator = eventAggregator;
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._sendDataCommand = new DelegateCommand(SendDataCommandExecuted, SendDataCommandCanExecute);
            this._printCommand = new DelegateCommand(PrintCommandExecuted);
            this._includeSdf = true;
            this._includeInventoryFiles = true;
        }

        public string TextStatus
        {
            get { return string.Format(Localization.Resources.ViewModel_InventorStatusChange_Status, CurrentStatus == null ? String.Empty : CurrentStatus.Code); }
        }

        public StatusInventors Statuses
        {
            get { return _statuses; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public StatusInventor CurrentStatus
        {
            get { return this._currentStatus; }
            set
            {
                this._currentStatus = value;
                this.RaisePropertyChanged(() => this.CurrentStatus);
                this.RaisePropertyChanged(() => this.TextStatus);
                this._okCommand.RaiseCanExecuteChanged();
            }
        }
     
        public DateTime InventorDate
        {
            get { return _inventorDate; }
            set
            {
                _inventorDate = value;
                RaisePropertyChanged(() => InventorDate);
            }
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

        public string BusyContent
        {
            get { return _busyContent; }
            set
            {
                _busyContent = value;
                RaisePropertyChanged(() => BusyContent);
            }
        }

        public DelegateCommand SendDataCommand
        {
            get { return _sendDataCommand; }
        }

        public DelegateCommand PrintCommand
        {
            get { return _printCommand; }
        }

        public bool IncludeSdf
        {
            get { return _includeSdf; }
            set
            {
                _includeSdf = value;
                RaisePropertyChanged(() => IncludeSdf);
            }
        }

        public bool IncludeInventoryFiles
        {
            get { return _includeInventoryFiles; }
            set
            {
                _includeInventoryFiles = value;
                RaisePropertyChanged(() => IncludeInventoryFiles);
            }
        }

        public string ZipPath
        {
            get { return _zipPath; }
            set
            {
                _zipPath = value;
                RaisePropertyChanged(() => ZipPath);

                _sendDataCommand.RaiseCanExecuteChanged();
            }
        }

        public string CurrentCustomerString
        {
            get
            {
                Customer customer = base.CurrentCustomer;
                return String.Format("{0} [{1}]", customer.Name, customer.Code);
            }
        }

        public string CurrentBranchString
        {
            get
            {
                Branch branch = base.CurrentBranch;
                return String.Format("{0} [{1}]", branch.Name, branch.BranchCodeERP);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._statuses = this._statusInventorRepository.GetStatuses();

            Inventor inventor = base.CurrentInventor;

            this._currentStatus = Statuses.FirstOrDefault(r => r.Code == inventor.StatusCode);
            this._inventorDate = inventor.InventorDate;

            SetDefaultZipPath();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return _currentStatus != null;
        }

        private void OkCommandExecuted()
        {
            using (new CursorWait())
            {
                AuditConfig config = base.GetAuditConfigByCurrentContext();
//                config.StatusInventorCode = CurrentStatus.Code;
//                config.CreateDate = this.CreateDate;
//                config.InventorDate = this.InventorDate;
//                base.ContextCBIRepository.Update(config, CBIContext.History);

                Inventor inventor = base.ContextCBIRepository.GetInventorByCode(config.InventorCode);
                inventor.InventorDate = this.InventorDate;
                inventor.StatusCode = _currentStatus.Code;
                base.ContextCBIRepository.Update(inventor);

                this._eventAggregator.GetEvent<InventorEditedEvent>().Publish(inventor);
                this._eventAggregator.GetEvent<InventorStatusChangedEvent>().Publish(inventor);

                this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
            }
        }

        private void PrintCommandExecuted()
        {

        }

        private bool IsOkZipPath()
        {
            if (String.IsNullOrWhiteSpace(_zipPath))
                return false;

            try
            {
                FileInfo fi = new FileInfo(_zipPath);
                if (!fi.Directory.Exists)
                    return false;

                if (String.IsNullOrWhiteSpace(fi.Name))
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool SendDataCommandCanExecute()
        {
            return IsOkZipPath();
        }

        private void SendDataCommandExecuted()
        {
            this.IsBusy = true;

            Task.Factory.StartNew(BuildZip);
        }


        private void BuildZip()
        {
            _sendDataOffice.BuildZip(base.State, UpdateStatus, _includeSdf, _includeInventoryFiles, _zipPath);

            Utils.RunOnUI(() =>
                {
                    IsBusy = false;

                    if (File.Exists(_zipPath))
                    {
                        Utils.OpenFileInExplorer(_zipPath);
                    }
                });
        }

        private void UpdateStatus(string status)
        {
            Utils.RunOnUI(() => BusyContent = status);
        }

        private void SetDefaultZipPath()
        {
            string sendDataFolderPath = UtilsMisc.BuildPathToZipOfficeFolder(_dbSettings);

            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            string sendDataZipFileName = String.Format("{0}_{1}_{2}.zip",
                                                       base.CurrentCustomer.Code,
                                                       base.CurrentBranch.BranchCodeERP,
                                                       date);

            string sendDataZipFilePath = Path.Combine(sendDataFolderPath, sendDataZipFileName);

            this._zipPath = sendDataZipFilePath;
        }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "ZipPath":

                        if (!IsOkZipPath())
                            return Localization.Resources.ViewModel_InventorChangeStatus_InvalidPath;

                        break;
                }

                return String.Empty;
            }
        }

        public string Error { get; private set; }

    }
}