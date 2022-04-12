using System;
using System.Reflection;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Audit;
using System.Windows.Media;
using System.IO;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Media.Imaging;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class HomeDashboardViewModel : NavigationAwareViewModel, IRegionMemberLifetime
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCBIRepository;
		private readonly IServiceLocator _serviceLocator;
		private readonly IDBSettings _dbSettings;
		//private string _backgroundFilePath;
		//private ImageBrush _localBackground;

	     private bool _isMdiAll;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly BackgroundViewModel _backgroundViewModel;

        public HomeDashboardViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCBIRepository,
			IServiceLocator serviceLocator,
			IDBSettings dbSettings,
            IUserSettingsManager userSettingsManager ,
            BackgroundViewModel backgroundViewModel)
        {
            _backgroundViewModel = backgroundViewModel;
            this._userSettingsManager = userSettingsManager;
            this._contextCBIRepository = contextCBIRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
			this._serviceLocator = serviceLocator;
			this._dbSettings = dbSettings;		
        }

        private void StartInventoryCommandExecuted()
        {
           
        }
      
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _eventAggregator.GetEvent<BackgroundChangedEvent>().Subscribe(BackgroundChanged);

            SetBackground();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _eventAggregator.GetEvent<BackgroundChangedEvent>().Unsubscribe(BackgroundChanged);
        }        

        public bool IsMdiAll
        {
            get { return _isMdiAll; }
            set
            {
                _isMdiAll = value;
                RaisePropertyChanged(() => IsMdiAll);
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public bool KeepAlive
        {
            get { return false; }
        }

        public BackgroundViewModel BackgroundViewModel
        {
            get { return _backgroundViewModel; }
        }

        private void SetBackground()
        {
            this._backgroundViewModel.OpacityBackground = _userSettingsManager.DashboardHomeBackgroundOpacityGet();
            this._backgroundViewModel.BackgroundFilePath = _userSettingsManager.DashboardHomeBackgroundGet();           
        }

        private void BackgroundChanged(object o)
        {
            SetBackground();
        }
    }
}