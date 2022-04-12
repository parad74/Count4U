using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.FamilyMdi
{
    public class FamilyDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IRegionManager _regionManager;
		private readonly IFamilyRepository _familyRepository;
        private readonly UICommandRepository _commandRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly DelegateCommand _viewCommand;
        private readonly DelegateCommand _importCommand;

		private string _totalFamilyText;

		private readonly ObservableCollection<Family> _items;

		public FamilyDashboardPartViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
			IFamilyRepository familyRepository,
            IRegionManager regionManager,
            UICommandRepository commandRepository,
            IUserSettingsManager userSettingsManager)
            : base(contextCBIRepository)
        {
            _userSettingsManager = userSettingsManager;
            this._commandRepository = commandRepository;
			this._familyRepository = familyRepository;
            this._regionManager = regionManager;

            this._importCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted);
            this._viewCommand = _commandRepository.Build(enUICommand.More, ViewCommandExecuted);
			_items = new ObservableCollection<Family>();
        }

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

        public DelegateCommand ViewCommand
        {
            get { return this._viewCommand; }
        }

		public string TotalFamilyText
        {
			get { return _totalFamilyText; }
            set
            {
				_totalFamilyText = value;
				RaisePropertyChanged(() => TotalFamilyText);
            }
        }

		public ObservableCollection<Family> Items
        {
            get { return _items; }
        }

        private void ViewCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

			UtilsNavigate.FamilyAddEditDeleteOpen(this._regionManager, query);
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeFamily);
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
            Utils.RunOnUI(()=>Items.Clear());

            SelectParams sp = new SelectParams();
            sp.IsEnablePaging = true;
			sp.CountOfRecordsOnPage = _userSettingsManager.PortionFamilysGet();
            sp.CurrentPage = 1;
			Familys familys = _familyRepository.GetFamilys(sp, base.GetDbPath);

			long total = familys.TotalCount;

            Utils.RunOnUI(() =>
                {
					foreach (Family family in familys)
                    {
						Items.Add(family);
                    }

					TotalFamilyText = String.Format(Localization.Resources.ViewModel_FamilesDashboardPart, total);
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