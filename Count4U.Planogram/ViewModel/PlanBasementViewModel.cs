using System;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Planogram.ViewModel
{
    public class PlanBasementViewModel : CBIContextBaseViewModel, IConfirmNavigationRequest
    {
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly UICommandRepository _comandRepository;

        private readonly DelegateCommand _navigateToGridCommand;
        private PlanCanvasViewModel _planCanvasViewModel;

        public PlanBasementViewModel(
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            IEventAggregator eventAggregator,
            UICommandRepository commandRepository
            )
            : base(contextCbiRepository)
        {
            _comandRepository = commandRepository;
            _eventAggregator = eventAggregator;
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;

            _navigateToGridCommand = _comandRepository.Build(enUICommand.FromImportToGrid, NavigateToGridCommandExecuted);
        }

        public DelegateCommand NavigateToGridCommand
        {
            get { return _navigateToGridCommand; }
        }

        public PlanCanvasViewModel PlanCanvasViewModel
        {
            get { return _planCanvasViewModel; }           
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<ApplicationClosingEvent>().Subscribe(ApplicationClosing);

            _planCanvasViewModel = Utils.GetViewModelFromRegion<PlanCanvasViewModel>(Common.RegionNames.PlanogramCanvas, _regionManager);
        }
      
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<ApplicationClosingEvent>().Unsubscribe(ApplicationClosing);
           
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            Save();

            continuationCallback(true);
        }

        private void ApplicationClosing(object o)
        {
            Save();
        }

        private void Save()
        {
            if (_planCanvasViewModel != null && _planCanvasViewModel.DrawingCanvasIsDirty)
            {
                string message = Localization.Resources.ViewModel_PlanBasement_msgContinue;
                MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Information, _userSettingsManager);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    if (_planCanvasViewModel.SaveCommand.CanExecute())
                    {
                        _planCanvasViewModel.SaveCommand.Execute();
                    }
                }
            }
        }

        private void NavigateToGridCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.PlanogramAddEditDeleteOpen(this._regionManager, uriQuery);
        }
    }
}