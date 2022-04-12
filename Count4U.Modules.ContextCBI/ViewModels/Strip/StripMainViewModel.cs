using System;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Strip
{
    public class StripMainViewModel: StripBaseViewModel
    {
        private readonly DelegateCommand _mainCommand;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        public StripMainViewModel(
            IContextCBIRepository contextCBIRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator)
            : base(contextCBIRepository)
        {
            _eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._mainCommand = new DelegateCommand(MainCommandExecuted, MainCommandCanExecute);
        }

      
        public DelegateCommand MainCommand
        {
            get { return _mainCommand; }
        }

        private bool MainCommandCanExecute()
        {
            return UtilsNavigate.OpenProcessInventoryCanExecute(base.ContextCBIRepository);
        }

        private void MainCommandExecuted()
        {
            UtilsNavigate.OpenProcessInventoryExecute(base.ContextCBIRepository, this._regionManager);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<ProcessInventorChangedEvent>().Subscribe(ProcessInventorChanged);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<ProcessInventorChangedEvent>().Unsubscribe(ProcessInventorChanged);
        }

        private void ProcessInventorChanged(object o)
        {
            this._mainCommand.RaiseCanExecuteChanged();
        }
    }
}