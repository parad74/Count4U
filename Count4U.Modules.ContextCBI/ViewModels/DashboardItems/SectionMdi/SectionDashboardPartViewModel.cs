using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Linq;
using Count4U.Common.Extensions;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U;
using Count4U.Common.UserSettings;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Section
{
    public class SectionDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly DelegateCommand _importCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly DelegateCommand _viewCommand;
        private readonly IRegionManager _regionManager;
        private readonly ISectionRepository _sectionRepository;
        private readonly UICommandRepository _commandRepository;
		private readonly IUserSettingsManager _userSettingsManager;

        private readonly ObservableCollection<Count4U.Model.Count4U.Section> _items;

        public SectionDashboardPartViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            ILocationRepository locationRepository,
            ISectionRepository sectionRepository,
            IRegionManager regionManager,
            UICommandRepository commandRepository,
			 IUserSettingsManager userSettingsManager)
            : base(contextCBIRepository)
        {
			this._commandRepository = commandRepository;
			this._userSettingsManager = userSettingsManager;
            this._sectionRepository = sectionRepository;
            this._regionManager = regionManager;

            this._eventAggregator = eventAggregator;
            this._importCommand = _commandRepository.Build(enUICommand.Import,  ImportCommandExecuted);
            this._viewCommand = _commandRepository.Build(enUICommand.More, ViewCommandExecuted);
            _items = new ObservableCollection<Model.Count4U.Section>();
        }

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

        public DelegateCommand ViewCommand
        {
            get { return this._viewCommand; }
        }

        public ObservableCollection<Model.Count4U.Section> Items
        {
            get { return _items; }
        }

        private void ViewCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            UtilsNavigate.SectionAddEditDeleteOpen(this._regionManager, query);
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeSection);
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("OnNavigatedTo");
        }


        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Clear();
        }

        private void Build()
        {
            //var sections = this._sectionRepository.GetSections(base.GetDbPath).Take(10).ToList();

			SelectParams sp = new SelectParams();
			sp.IsEnablePaging = true;
			sp.CountOfRecordsOnPage = _userSettingsManager.PortionSectionsGet();
			sp.CurrentPage = 1;
			Sections sections = _sectionRepository.GetSections(sp, base.GetDbPath);


            Utils.RunOnUI(() =>
                {
                    this._items.Clear();
                    foreach (var section in sections)
                    {
                        this._items.Add(section);
                    }
                });
        }

        public void Refresh()
        {
			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("Refresh");
        }

        public void Clear()
        {

        }
    }
}