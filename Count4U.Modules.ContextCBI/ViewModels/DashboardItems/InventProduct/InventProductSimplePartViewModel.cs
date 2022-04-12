using System;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class InventProductSimplePartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly UICommandRepository _commandRepository;

        private readonly DelegateCommand _moreCommand;        

        public InventProductSimplePartViewModel(IContextCBIRepository contextCBIRepository,
         IEventAggregator eventAggregator,
         IRegionManager regionManager,            
         UICommandRepository commandRepository)
            :base(contextCBIRepository)
        {
            _commandRepository = commandRepository;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _moreCommand = _commandRepository.Build(enUICommand.More, MoreCommandExecuted);
        }        

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        public void Refresh()
        {
            
        }

        public void Clear()
        {
            
        }

        private void MoreCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());                       

            UtilsNavigate.InventProductListSimpleOpen(this._regionManager, query);
        }
    }
}