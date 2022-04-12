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
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels.Strip
{
    public class StripCustomerViewModel : StripBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Customer _currentCustomer;
        private readonly DelegateCommand _customerDashboardCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public StripCustomerViewModel(IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
            : base(contextCBIRepository)
        {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._customerDashboardCommand = new DelegateCommand(CustomerDashboardCommandExecuted);
        }

        public new Customer CurrentCustomer
        {
            get
            {
                if (this._currentCustomer == null)
                {
                    try
                    {
                        switch (this.Context)
                        {
                            case CBIContext.CreateInventor:
                                this._currentCustomer = base.ContextCBIRepository.GetCurrentCustomer(this.GetCreateAuditConfig());
                                break;
                            case CBIContext.History:
                                this._currentCustomer = base.ContextCBIRepository.GetCurrentCustomer(this.GetHistoryAuditConfig());
                                break;
                            case CBIContext.Main:
                                this._currentCustomer = base.ContextCBIRepository.GetCurrentCustomer(this.GetMainAuditConfig());
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    catch (Exception exc)
                    {
                        _logger.ErrorException("CurrentCustomer", exc);
                    }
                }

                return this._currentCustomer;
            }
        }

        public DelegateCommand CustomerDashboardCommand
        {
            get { return this._customerDashboardCommand; }
        }

        private void CustomerDashboardCommandExecuted()
        {
            var history = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
            if (history != null)
                history.ClearBranch();

            var main = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            if (main != null)
                main.ClearBranch();

            var createInventor = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
            if (createInventor != null)
                createInventor.ClearBranch();

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.CustomerDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        public void Refresh()
        {
            this._currentCustomer = null;
            RaisePropertyChanged(() => CurrentCustomer);
        }
    }
}