using System;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Strip
{
    public class StripBranchViewModel : StripBaseViewModel
    {
        private Branch _currentBranch;
        private readonly IEventAggregator _eventAggregator;
        private readonly DelegateCommand _branchDashboardCommand;
        private readonly IRegionManager _regionManager;

        public StripBranchViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
            : base(contextCBIRepository)
        {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._branchDashboardCommand = new DelegateCommand(BranchDashboardCommandExecuted);
        }

        public new Branch CurrentBranch
        {
            get
            {
                if (this._currentBranch == null)
                {
                    switch (this.Context)
                    {
                        case CBIContext.CreateInventor:
                            this._currentBranch = base.ContextCBIRepository.GetCurrentBranch(this.GetCreateAuditConfig());
                            break;
                        case CBIContext.History:
                            this._currentBranch = base.ContextCBIRepository.GetCurrentBranch(this.GetHistoryAuditConfig());
                            break;
                        case CBIContext.Main:
                            this._currentBranch = base.ContextCBIRepository.GetCurrentBranch(this.GetMainAuditConfig());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return this._currentBranch;
            }
        }

        public DelegateCommand BranchDashboardCommand
        {
            get { return this._branchDashboardCommand; }
        }

        private void BranchDashboardCommandExecuted()
        {
            var history = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
            if (history != null)
                history.ClearInventor();

            var main = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            if (main != null)
                main.ClearInventor();

            var createInventor = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
            if (createInventor != null)
                createInventor.ClearInventor();

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
           
            UtilsNavigate.BranchDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        public void Refresh()
        {
            this._currentBranch = null;
            RaisePropertyChanged(()=>CurrentBranch);
        }
    }
}