using System;
using System.Threading;
using System.Windows.Threading;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Modules.Audit.ViewModels.Export
{
    public abstract class ExportWithModulesBaseViewModel : CBIContextBaseViewModel
    {
        protected readonly UICommandRepository _commandRepository;

        protected readonly DelegateCommand _exportCommand;
        protected readonly DelegateCommand _logCommand;
		protected readonly DelegateCommand _configCommand;
        protected readonly DelegateCommand _clearCommand;

        private readonly IEventAggregator _eventAggregator;

        protected readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        protected bool _isWriteLogToFile;
        protected string _log;

        protected bool _isBusy;
        protected string _progress;
        protected string _progressTime;
        protected readonly DelegateCommand _busyCancelCommand;
        protected CancellationTokenSource _cts;
        protected readonly DispatcherTimer _timer;
        protected DateTime _timerStart;

        public ExportWithModulesBaseViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            UICommandRepository commandRepository)
            : base(contextCbiRepository)
        {
            _commandRepository = commandRepository;
            _eventAggregator = eventAggregator;
            this._exportCommand = _commandRepository.Build(enUICommand.Export, ExportCommandExecuted, ExportCommandCanExecute);
            this._logCommand = _commandRepository.Build(enUICommand.Log, LogCommandExecuted, LogCommandCanExecute);
			this._configCommand = _commandRepository.Build(enUICommand.Config, ConfigCommandExecuted, ConfigCommandCanExecute);
			this._clearCommand = _commandRepository.Build(enUICommand.Clear, ClearCommandExecuted, ClearCommandCanExecute);

            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();

            this._busyCancelCommand = new DelegateCommand(BusyCancelCommandExecuted);
            this._timer = new DispatcherTimer();
            this._timer.Tick += ProgressTimer_Tick;

            this._isWriteLogToFile = true;
        }

        public DelegateCommand ExportCommand
        {
            get { return _exportCommand; }
        }

        public DelegateCommand LogCommand
        {
            get { return _logCommand; }
        
		}

		public DelegateCommand ConfigCommand
        {
			get { return _configCommand; }
		}
		


        public DelegateCommand ClearCommand
        {
            get { return _clearCommand; }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {

                    _isBusy = value;
                    RaisePropertyChanged(() => IsBusy);

                    this._eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(this._isBusy);

                    if (_isBusy)
                    {
                        _timer.Start();
                        _timerStart = DateTime.Now;
                    }
                    else
                    {
                        _timer.Stop();
                    }
                }
            }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return _yesNoRequest; }
        }

        public string Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public DelegateCommand BusyCancelCommand
        {
            get { return _busyCancelCommand; }
        }

        public string ProgressTime
        {
            get { return _progressTime; }
            set
            {
                _progressTime = value;
                RaisePropertyChanged(() => ProgressTime);
            }
        }

        public bool IsWriteLogToFile
        {
            get { return _isWriteLogToFile; }
            set
            {
                _isWriteLogToFile = value;

                RaisePropertyChanged(() => IsWriteLogToFile);
            }
        }      

        protected abstract void ExportCommandExecuted();
        protected abstract bool ExportCommandCanExecute();
		protected abstract bool ClearCommandCanExecute();
        protected abstract bool LogCommandCanExecute();
        protected abstract void LogCommandExecuted();
		protected abstract bool ConfigCommandCanExecute();
		protected abstract  void ConfigCommandExecuted() ;
        protected abstract void ClearCommandExecuted();

        private void BusyCancelCommandExecuted()
        {
            if (_cts != null)
                _cts.Cancel();
        }

        void ProgressTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - this._timerStart;
            ProgressTime = String.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
        }

        protected void ShowLog()
        {
            if (_logCommand.CanExecute())
                _logCommand.Execute();
        }

		protected void ShowConfig()
		{
			if (_configCommand.CanExecute())
				_configCommand.Execute();
		}

		public void ClearRegions()
		{
			//см ImportModuleBaseViewModel
			//foreach (string maskRegion in this._maskRegions.ToList())
			//{
			//	this._regionManager.Regions.Remove(maskRegion);
			//	this._maskRegions.Remove(maskRegion);
			//}
		}
    }
}