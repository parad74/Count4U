using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using NLog;
using Count4U.Common.Extensions;

namespace Count4U.Common.ViewModel.Script
{
    public abstract class ScriptSaveBaseViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly IEventAggregator _eventAggregator;

        protected readonly DelegateCommand _runCommand;
        protected readonly DelegateCommand _cancelCommand;
        protected readonly DelegateCommand _browseCommand;

        protected string _path;

        protected readonly InteractionRequest<SaveFileDialogNotification> _fileSaveDialogRequest;

        protected bool _isBusy;
        protected string _log;

        protected ScriptSaveBaseViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            this._eventAggregator = eventAggregator;
            this._fileSaveDialogRequest = new InteractionRequest<SaveFileDialogNotification>();

            this._runCommand = new DelegateCommand(RunCommandExecuted, RunCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._browseCommand = new DelegateCommand(BrowseCommandExecuted);
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);

                this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(this._isBusy);
            }
        }

        public string Log
        {
            get { return _log; }
            set
            {
                _log = value;
                RaisePropertyChanged(() => Log);
            }
        }

        public InteractionRequest<SaveFileDialogNotification> FileSaveDialogRequest
        {
            get { return this._fileSaveDialogRequest; }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
                this._runCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand RunCommand
        {
            get { return _runCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public DelegateCommand BrowseCommand
        {
            get { return _browseCommand; }
        }

        protected abstract void RunScript();

        private void BrowseCommandExecuted()
        {
            SaveFileDialogNotification notification = new SaveFileDialogNotification();

            notification.Filter =
                "Txt files (*.txt)|*.txt|All files (*.*)|*.*";

            this._fileSaveDialogRequest.Raise(notification, r =>
            {
                if (r.IsOK)
                {
                    this.Path = r.FileName;
                }
            });
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool RunCommandCanExecute()
        {
            return IsPathValid();
        }

        private void RunCommandExecuted()
        {
            if (!RunCommand.CanExecute()) return;

            this.Log = String.Empty;
            this.IsBusy = true;
			Task.Factory.StartNew(Run).LogTaskFactoryExceptions("RunCommandExecuted");
        }

        private void Run()
        {
            try
            {
                RunScript();
            }
            catch(Exception exc)
            {
                _logger.ErrorException("Run", exc);
            }
            finally
            {
                Utils.RunOnUI(() => IsBusy = false);
            }
        }

        #region Implementation of IDataErrorInfo

        public string this[string propertyName]
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_path))
                    if (!IsPathValid())
                        return Localization.Resources.Validation_Not_Valid_Path;
                return string.Empty;
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }

        #endregion

        private bool IsPathValid()
        {
            if (String.IsNullOrEmpty(_path))
                return false;

            FileInfo fi = new FileInfo(_path);
            if (fi.DirectoryName != null)
                if (!Directory.Exists(fi.DirectoryName))
                    return false;

            return true;
        }
    }
}