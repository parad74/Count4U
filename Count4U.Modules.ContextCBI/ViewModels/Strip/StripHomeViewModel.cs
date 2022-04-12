using System;
using Count4U.Common.Helpers;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism;

namespace Count4U.Modules.ContextCBI.ViewModels.Strip
{
    public class StripHomeViewModel : StripBaseViewModel
    {
        private readonly DelegateCommand _homeCommand;
        private readonly IRegionManager _regionManager;

        public StripHomeViewModel(IContextCBIRepository contextCBIRepository,
            IRegionManager regionManager)
            : base(contextCBIRepository)
        {
            this._regionManager = regionManager;
            this._homeCommand = new DelegateCommand(HomeCommandExecuted);
        }

        private void HomeCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.HomeDashboardOpen(CBIContext.Main, this._regionManager, query);
        }

        public DelegateCommand HomeCommand
        {
            get { return this._homeCommand; }
        }
    }
}