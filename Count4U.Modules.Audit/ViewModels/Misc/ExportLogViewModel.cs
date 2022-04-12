using System;
using System.Diagnostics;
using System.IO;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc
{
    public class ExportLogViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationRepository _navigationRepository;

        private readonly DelegateCommand _closeCommand;
        private readonly DelegateCommand _openCommand;

        private string _log;
        private string _path;

        public ExportLogViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            INavigationRepository navigationRepository
            )
            : base(contextCbiRepository)
        {
            _navigationRepository = navigationRepository;
            _eventAggregator = eventAggregator;
            _closeCommand = new DelegateCommand(CloseCommandExecuted);
            _openCommand = new DelegateCommand(OpenCommandExecuted, OpenCommandCanExecute);
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

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public DelegateCommand OpenCommand
        {
            get { return _openCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            ExportLogViewData data = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, isRemove: true) as ExportLogViewData;
            if (data == null) return;

            if (data.Log.Length > 500000)
                _log = data.Log.Substring(0, 500000);
            else
                _log = data.Log;

            _path = data.Path;
        }

        private void CloseCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OpenCommandExecuted()
        {
            if (!Directory.Exists(_path)) return;

            Utils.OpenFolderInExplorer(_path);
        }

        private bool OpenCommandCanExecute()
        {
            return Directory.Exists(_path);
        }
    }
}