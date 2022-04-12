using System;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Services.Ini;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;


namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class InventorStatusDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly DelegateCommand _showInventorCommand;
        private readonly DelegateCommand _changeStatusCommand;
        private readonly DelegateCommand _setCurrentAsProcessCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IStatusInventorRepository _statusInventorRepository;
        private readonly IUnityContainer _unityContainer;
		private readonly IServiceLocator _serviceLocator;

        private readonly IInventorRepository _inventorRepository;

        private string _status;

        public InventorStatusDashboardPartViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IUnityContainer unityContainer,
			IServiceLocator serviceLocator,
            IInventorRepository inventorRepository,
            IStatusInventorRepository statusInventorRepository)
            : base(contextCBIRepository)
        {
            _statusInventorRepository = statusInventorRepository;
            _inventorRepository = inventorRepository;
            this._unityContainer = unityContainer;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
			this._serviceLocator = serviceLocator;
			

            this._showInventorCommand = new DelegateCommand(ShowInventorCommandExecuted, ShowInventorCommandCanExecute);
            this._changeStatusCommand = new DelegateCommand(ChangeStatusCommandExecuted);
            this._setCurrentAsProcessCommand = new DelegateCommand(SetCurrentAsProcessExecuted, SetCurrentAsProcessCanExecute);

        }

        public DelegateCommand SetCurrentAsProcessCommand
        {
            get { return this._setCurrentAsProcessCommand; }
        }

        public DelegateCommand ShowInventorCommand
        {
            get { return this._showInventorCommand; }
        }

        public DelegateCommand ChangeStatusCommand
        {
            get { return this._changeStatusCommand; }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        private bool ShowInventorCommandCanExecute()
        {
            return Utils.IsThereCurrentConfigInHistoryContext(base.ContextCBIRepository);
        }

        private void ShowInventorCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, CBIContext.History);
            Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(query, base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History));

            using (new CursorWait())
            {
                IIturAnalyzesRepository iturAnalyzesRepository = _serviceLocator.GetInstance<IIturAnalyzesRepository>();
                iturAnalyzesRepository.ClearProductDictionary();
            }

            UtilsNavigate.IturListDetailsOpen(this._regionManager, query);
        }

        private void ChangeStatusCommandExecuted()
        {
            InventorStatusChangeEventPayload payload = new InventorStatusChangeEventPayload();
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;

            using (new CursorWait())
            {
                IIturAnalyzesRepository iturAnalyzesRepository = _serviceLocator.GetInstance<IIturAnalyzesRepository>();
                iturAnalyzesRepository.ClearProductDictionary();
            }

            this._eventAggregator.GetEvent<InventorStatusChangeEvent>().Publish(payload);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<InventorStatusChangedEvent>().Subscribe(InventorStatusChanged);

            BuildInventorStatus();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Clear();
        }

        private void InventorStatusChanged(Inventor inventor)
        {
            BuildInventorStatus();
        }

        public void Refresh()
        {
            BuildInventorStatus();
        }

        public void Clear()
        {
            this._eventAggregator.GetEvent<InventorStatusChangedEvent>().Unsubscribe(InventorStatusChanged);
        }

        private bool SetCurrentAsProcessCanExecute()
        {
            var proc = base.ContextCBIRepository.GetProcessCBIConfig(CBIContext.History);
            var cur = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History);

            if (cur == null) return false;
            if (proc == null) return true;

            AuditConfig process = new AuditConfig(proc);
            AuditConfig current = new AuditConfig(cur);

            return process.Code != current.Code;
        }

        private void SetCurrentAsProcessExecuted()
        {
            using (new CursorWait())
            {
                AuditConfig config = new AuditConfig(base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History, true));
                base.ContextCBIRepository.SetProcessCBIConfig(CBIContext.History, config);
				ExportPdaModuleBaseViewModel exportPda = _serviceLocator.GetInstance<ExportPdaModuleBaseViewModel>(ExportPdaAdapterName.ExportPdaMISAdapter);
                using (new CursorWait())
                {
                    exportPda.ClearFolders(base.State);
                    IIturAnalyzesRepository iturAnalyzesRepository = _serviceLocator.GetInstance<IIturAnalyzesRepository>();
                    iturAnalyzesRepository.ClearProductDictionary();
                }

                this._setCurrentAsProcessCommand.RaiseCanExecuteChanged();

                Utils.InventorChangedGlobalAction(this._unityContainer, CBIContext.History, base.GetDbPath);

                this._eventAggregator.GetEvent<ProcessInventorChangedEvent>().Publish(null);
            }
        }

        private void BuildInventorStatus()
        {
            AuditConfig config = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
            if (config == null) return;
            Inventor inventor = _inventorRepository.GetInventorByCode(config.InventorCode);
            if (inventor == null) return;
            
            if (!String.IsNullOrWhiteSpace(inventor.StatusCode))
            {
                StatusInventor status = _statusInventorRepository.GetStatusByCode(inventor.StatusCode);
                if (status != null)
                {
                    Status = status.Name;
                }
            }
        }
    }
}