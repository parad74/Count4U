using System;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.ViewModel;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.ErpExpected
{
    public class ErpExpectedStep1ViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _backCommand;
        private readonly DelegateCommand _browseCommand;
        private readonly DelegateCommand _nextCommand;

        private readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;
        private string _filePath;
        private string _log;
        private readonly IRegionManager _regionManager;

        public ErpExpectedStep1ViewModel(IContextCBIRepository contextCBIRepository, 
			IEventAggregator eventAggregator,
             IRegionManager regionManager)
            : base(contextCBIRepository)
        {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

            this._backCommand = new DelegateCommand(BackCommandExecuted);
            this._browseCommand = new DelegateCommand(BrowseCommandExecuted);
            this._nextCommand = new DelegateCommand(NextCommandExecuted, NextCommandCanExecute);

            this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();
        }

        public DelegateCommand BackCommand
        {
            get { return this._backCommand; }
        }

        public string FilePath
        {
            get { return this._filePath; }
            set
            {
                this._filePath = value;
				this.RaisePropertyChanged(() => this.FilePath);

                this._nextCommand.RaiseCanExecuteChanged();
            }
        }

        public string Log
        {
            get { return this._log; }
            set
            {
                this._log = value;
				this.RaisePropertyChanged(() => this.Log);
            }
        }

        public new Customer CurrentCustomer
        {
            get { return base.ContextCBIRepository.GetCurrentCustomer(base.GetHistoryAuditConfig()); }
        }

        public new Branch CurrentBranch
        {
            get { return base.ContextCBIRepository.GetCurrentBranch(base.GetHistoryAuditConfig()); }
        }

        public new Inventor CurrentInventor
        {
            get { return base.ContextCBIRepository.GetCurrentInventor(base.GetHistoryAuditConfig()); }
        }

        public DelegateCommand BrowseCommand
        {
            get { return this._browseCommand; }
        }

        public DelegateCommand NextCommand
        {
            get { return this._nextCommand; }
        }

        public InteractionRequest<OpenFileDialogNotification> FileChooseDilogRequest
        {
            get { return this._fileChooseDilogRequest; }
        }

        private bool NextCommandCanExecute()
        {
            return !String.IsNullOrEmpty(this._filePath);
        }

        private void NextCommandExecuted()
        {

        }

        private void BrowseCommandExecuted()
        {
            OpenFileDialogNotification notification = new OpenFileDialogNotification();
            notification.DefaultExt = ".jpg";
            notification.Filter =
                "All files (*.*)|*.*";

            this._fileChooseDilogRequest.Raise(notification, this.FileUploadProcess);
        }

        private void FileUploadProcess(OpenFileDialogNotification notification)
        {
            if (notification.IsOK == false) return;

            this.FilePath = notification.FileName;
        }

        private void BackCommandExecuted()
        {
            UtilsNavigate.GoBack(this._regionManager);
        }
    }
}