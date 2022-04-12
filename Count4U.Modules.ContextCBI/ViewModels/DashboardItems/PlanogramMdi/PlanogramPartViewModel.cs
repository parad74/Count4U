using System;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.PlanogramMdi
{
    public class PlanogramPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly DelegateCommand _moreCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _exportCommand;
        private readonly DelegateCommand _viewCommand;
        private readonly DelegateCommand _statisticCommand;
        private readonly UICommandRepository _commandRepository;
        private readonly IRegionManager _regionManager;

        public PlanogramPartViewModel(
            IContextCBIRepository contextCbiRepository,
            UICommandRepository commandRepository,
            IRegionManager regionManager)
            : base(contextCbiRepository)
        {
            _regionManager = regionManager;
            _commandRepository = commandRepository;

            _moreCommand = _commandRepository.Build(enUICommand.More, MoreCommandExecuted);
            _editCommand = _commandRepository.Build(enUICommand.Edit, EditCommandExecuted);
            _importCommand = _commandRepository.Build(enUICommand.Import, ImportCommandExecuted);
            _exportCommand = _commandRepository.Build(enUICommand.Export, ExportCommandExecuted);
            _viewCommand = _commandRepository.Build(enUICommand.View, ViewCommandExecuted);
            _statisticCommand = _commandRepository.Build(enUICommand.PlanogramStatistic, StatisticCommandExecuted);
        }        

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand ImportCommand
        {
            get { return _importCommand; }
        }

        public DelegateCommand ExportCommand
        {
            get { return _exportCommand; }
        }

        public DelegateCommand ViewCommand
        {
            get { return _viewCommand; }
        }

        public DelegateCommand StatisticCommand
        {
            get { return _statisticCommand; }
        }

        public void Refresh()
        {
            
        }

        public void Clear()
        {
            
        }

        private void MoreCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.PlanogramAddEditDeleteOpen(this._regionManager, uriQuery);
        }

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeUnitPlan);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void EditCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.PlanogramOpen(this._regionManager, uriQuery);
        }

        private void StatisticCommandExecuted()
        {

        }

        private void ViewCommandExecuted()
        {

        }

        private void ExportCommandExecuted()
        {

        }
    }
}