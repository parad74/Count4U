using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class LocationDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _addCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILocationRepository _locationRepository;
        private readonly DelegateCommand _viewCommand;
        private bool _isPartEnabled;
        private readonly IRegionManager _regionManager;
        private readonly UICommandRepository _commandRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Locations _locations;

        public LocationDashboardPartViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            ILocationRepository locationRepository,
            IRegionManager regionManager,
            UICommandRepository commandRepository)
            : base(contextCBIRepository)
        {
            _commandRepository = commandRepository;
            this._regionManager = regionManager;
            this._locationRepository = locationRepository;
            this._eventAggregator = eventAggregator;
            this._importCommand = _commandRepository.Build(enUICommand.Import,  ImportCommandExecuted);
            this._addCommand = _commandRepository.Build(enUICommand.Add, AddCommandExecuted);
            this._viewCommand = _commandRepository.Build(enUICommand.More, ViewCommandExecuted);            
        }        

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

        public Locations Locations
        {
            get { return this._isPartEnabled ? _locations : null; }
        }

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand ViewCommand
        {
            get { return this._viewCommand; }
        }

        public bool IsPartEnabled
        {
            get { return this._isPartEnabled; }
        }

        private void AddCommandExecuted()
        {
            LocationAddedEventPayLoad payload = new LocationAddedEventPayLoad();
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;
            this._eventAggregator.GetEvent<LocationAddEvent>().Publish(payload);
        }

        private void ViewCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            UtilsNavigate.LocationAddEditDeleteOpen(this._regionManager, query);
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeLocation);
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void LocationAdded(Location location)
        {
            BuildLocations();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<LocationAddedEvent>().Subscribe(LocationAdded);

            this._isPartEnabled = true;

			Task.Factory.StartNew(BuildLocations).LogTaskFactoryExceptions("OnNavigatedTo");
        }


        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
			base.OnNavigatedFrom(navigationContext);

            Clear();            
        }

        public void Refresh()
        {
            RaisePropertyChanged(() => Locations);
        }

        public void Clear()
        {
            this._eventAggregator.GetEvent<LocationAddedEvent>().Unsubscribe(LocationAdded);            
        }

        private void BuildLocations()
        {
			string dbPath = "";
			try
			{ // я добавила 05.05.2013
				dbPath = base.GetDbPath;
				var locations = this._locationRepository.GetLocations(15, dbPath);

				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
																		  {
																			  this._locations = locations;
																			  RaisePropertyChanged(() => Locations);
																		  }));
			}
			catch (Exception exc)
			{
				_logger.ErrorException("BuildLocations from DBPath " +  dbPath, exc);
				throw;
			}
        }
    }
}