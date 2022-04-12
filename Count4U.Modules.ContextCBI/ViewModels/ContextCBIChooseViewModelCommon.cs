using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Events.InterCommData;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Script;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public abstract class ContextCBIChooseViewModelCommon : CBIContextBaseViewModel
    {
        protected const string All = "All";
        protected static readonly string AllName = Localization.Resources.ViewModel_ContextCBIChoose_All;

        protected readonly IEventAggregator _eventAggregator;
        protected readonly IRegionManager _regionManager;
        protected readonly INavigationRepository _navigationRepository;
        protected readonly UICommandRepository _commandRepository;

        private readonly DelegateCommand _scriptOpenCommand;
        private readonly DelegateCommand _scriptSaveCommand;

        protected readonly ObservableCollection<string> _filteringAlphabet;

        public ContextCBIChooseViewModelCommon(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            UICommandRepository commandRepository)
            : base(contextCBIRepository)
        {
            this._commandRepository = commandRepository;
			this._navigationRepository = navigationRepository;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

            this._filteringAlphabet = new ObservableCollection<string>();
            this.BuildFilteringAlphabet();

            this._scriptOpenCommand = _commandRepository.Build(enUICommand.OpenScript, ScriptOpenCommandExecuted);
            this._scriptSaveCommand = _commandRepository.Build(enUICommand.SaveScript, ScriptSaveCommandExecuted);
        }

        #region properties


        public DelegateCommand ScriptOpenCommand
        {
            get { return _scriptOpenCommand; }
        }

        public DelegateCommand ScriptSaveCommand
        {
            get { return _scriptSaveCommand; }
        }

        public ObservableCollection<string> FilteringAlphabet
        {
            get { return this._filteringAlphabet; }
        }

        protected abstract enCBIScriptMode CBIScriptMode { get; }
        protected abstract SelectParams SelectParams { get; }

        #endregion

        private void BuildFilteringAlphabet()
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            this._filteringAlphabet.Add(All);
            foreach (char c in alpha)
            {
                this._filteringAlphabet.Add(c.ToString());
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);


        }

        private void ScriptSaveCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.CBIScriptSaveView;
            payload.WindowTitle = WindowTitles.ScriptSave;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            UtilsConvert.AddEnumToDictionary(payload.Settings, typeof(enCBIScriptMode), this.CBIScriptMode);
            UtilsConvert.AddObjectToDictionary(payload.Settings, this._navigationRepository, this.SelectParams, NavigationObjects.SelectParams);

            OnModalWindowRequest(payload);
        }

        private void ScriptOpenCommandExecuted()
        {
            ModalWindowRequestPayload payload = new ModalWindowRequestPayload();
            payload.Settings = new Dictionary<string, string>();
            payload.ViewName = Common.ViewNames.CBIScriptOpenView;
            payload.WindowTitle = WindowTitles.ScriptOpen;
            Utils.AddContextToDictionary(payload.Settings, base.Context);
            Utils.AddDbContextToDictionary(payload.Settings, base.CBIDbContext);
            UtilsConvert.AddEnumToDictionary(payload.Settings, typeof(enCBIScriptMode), this.CBIScriptMode);

            payload.Callback = r =>
            {
                ScriptOpenData data = r as ScriptOpenData;
                if (data == null) return;
                if (data.IsOk)
                {

                }
            };

            OnModalWindowRequest(payload);
        }
    }
}