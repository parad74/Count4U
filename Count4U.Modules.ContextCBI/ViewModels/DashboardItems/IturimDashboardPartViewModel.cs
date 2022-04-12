using System;
using System.Collections.Generic;
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
using System.Linq;
using Microsoft.Practices.Prism.Regions;
using System.Collections.ObjectModel;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class IturimDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _importCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly IIturRepository _iturRepository;
        private readonly IRegionManager _regionManager;
        private readonly IStatusIturGroupRepository _statusIturGroupRepository;
        private readonly UICommandRepository _commandRepository;


        private readonly ObservableCollection<string> _items;
        private string _totalIturimText;


        public IturimDashboardPartViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IStatusIturGroupRepository statusIturGroupRepository,
            IRegionManager regionManager,
            IIturRepository iturRepository,
            UICommandRepository commandRepository
            )
            : base(contextCBIRepository)
        {
            _commandRepository = commandRepository;
            this._statusIturGroupRepository = statusIturGroupRepository;
            this._regionManager = regionManager;
            this._iturRepository = iturRepository;
            this._eventAggregator = eventAggregator;
            this._addCommand = _commandRepository.Build(enUICommand.Add,  AddCommandExecuted);
            this._editCommand = _commandRepository.Build(enUICommand.More, this.EditCommandExecuted);
            this._importCommand = _commandRepository.Build(enUICommand.Import, this.ImportCommandExecuted);

            this._items = new ObservableCollection<string>();
        }

        public DelegateCommand EditCommand
        {
            get { return this._editCommand; }
        }

        public string TotalIturimText
        {
            get
            {
                return _totalIturimText;
            }
        }

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

        public DelegateCommand AddCommand
        {
            get { return _addCommand; }
        }

        public ObservableCollection<string> Items
        {
            get { return _items; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<ItursAddedEvent>().Subscribe(ItursAdded);

			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);            

            Clear();
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeItur);
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void EditCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.IturimAddEditDeleteOpen(this._regionManager, query);
        }

        public void Refresh()
        {
            Build();
        }

        public void Clear()
        {
            this._eventAggregator.GetEvent<ItursAddedEvent>().Unsubscribe(ItursAdded);
        }

        private void Build()
        {
            Utils.RunOnUI(() => _items.Clear());

            Dictionary<int, int> dbStatuses = this._iturRepository.GetIturTotalGroupByStatuses(base.GetDbPath);
            int totalIturim = this._iturRepository.GetItursTotal(base.GetDbPath);        
            Dictionary<string, StatusIturGroup> codes = this._statusIturGroupRepository.CodeStatusIturGroupDictionary;            

            Utils.RunOnUI(() =>
                {
                    _totalIturimText = String.Format(Localization.Resources.ViewModel_IturimDashboardPart_total, totalIturim);
                    RaisePropertyChanged(() => TotalIturimText);

                    foreach (KeyValuePair<string, StatusIturGroup> group in codes)
                    {
                        int groupBit = @group.Value.Bit;
                        int bitCaculated = dbStatuses.ContainsKey(groupBit) ? dbStatuses[groupBit] : 0;
                        string groupName = UtilsMisc.LocalizationFromLocalizationKey(group.Value.NameLocalizationCode);
                        string item = String.Format("{0}: {1}", groupName, bitCaculated);
                        //string item = UtilsConvert.LTRMark + group.Value.Name + ": " + bitCaculated + UtilsConvert.LTRMark;
                        _items.Add(item);
                    }
                });
        }

        private void AddCommandExecuted()
        {
            IturAddEventPayload payload = new IturAddEventPayload { Itur = null, Context = base.Context, DbContext = base.CBIDbContext };

            this._eventAggregator.GetEvent<IturAddEvent>().Publish(payload);
        }

        private void ItursAdded(ItursAddedEventPayload itursAddedEventPayload)
        {
            Build();
        }
    }
}