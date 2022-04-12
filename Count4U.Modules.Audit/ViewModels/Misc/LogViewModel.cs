using System;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Misc
{
    public class LogViewModel : CBIContextBaseViewModel
    {
        private readonly DelegateCommand _closeCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationRepository _navigationRepository;

        private string _log;

        public LogViewModel(
            IContextCBIRepository repository,
            IEventAggregator eventAggregator,
            INavigationRepository navigationRepository)
            : base(repository)
        {
            _navigationRepository = navigationRepository;
            _eventAggregator = eventAggregator;
            _closeCommand = new DelegateCommand(CloseCommandExecuted);
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

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            string raw = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, isRemove: true) as String;
            if (String.IsNullOrEmpty(raw))
                return;

            if (raw.Length > 500000)
                _log = raw.Substring(0, 500000);
            else
                _log = raw;
        }

        private void CloseCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }
    }
}