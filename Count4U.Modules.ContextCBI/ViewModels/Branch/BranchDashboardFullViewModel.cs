using System;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Interface;
using System.IO;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BranchDashboardFullViewModel : CBIContextBaseViewModel
    {
        private readonly IRegionManager _regionManager;        
        private readonly IEventAggregator _eventAggregator;
		private readonly IDBSettings _dbSettings;
        private bool _isMdiAll;
        private readonly BackgroundViewModel _backgroundViewModel;
        private readonly IUserSettingsManager _userSettingsManager;

        public BranchDashboardFullViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator, 
			IContextCBIRepository contextCBIRepository,
			IDBSettings dbSettings,
             BackgroundViewModel backgroundViewModel,
            IUserSettingsManager userSettingsManager)
            : base(contextCBIRepository)
        {
            _userSettingsManager = userSettingsManager;
            _backgroundViewModel = backgroundViewModel;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
			this._dbSettings = dbSettings;
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

        public BackgroundViewModel BackgroundViewModel
        {
            get { return _backgroundViewModel; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._backgroundViewModel.OpacityBackground = _userSettingsManager.DashboardBranchBackgroundOpacityGet();
            this._backgroundViewModel.BackgroundFilePath = _userSettingsManager.DashboardBranchBackgroundGet();           
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void RaisePropertyChanged()
        {
           
        }
    }
}