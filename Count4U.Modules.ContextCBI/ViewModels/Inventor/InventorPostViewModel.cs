using System;
using System.Text;
using System.Windows.Input;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Events;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class InventorPostViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly INavigationRepository _navigationRepository;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IInventorRepository _inventorRepository;
        private readonly UICommandRepository _uiCommandRepository;

        //        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _mainDashboardCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _closeCommand;
        //        private readonly DelegateCommand _homeCommand;
        private readonly DelegateCommand _inventorDashboardCommand;
        private readonly DelegateCommand _homeDashboardCommand;

        private string _title;
        private string _log;
        private Inventor _inventor;

        public InventorPostViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IInventorRepository inventorRepository,
            UICommandRepository uiCommandRepository)
            : base(contextCbiRepository)
        {
            _uiCommandRepository = uiCommandRepository;
            _inventorRepository = inventorRepository;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _navigationRepository = navigationRepository;

            //            _addCommand = new DelegateCommand(AddCommandExecuted);
            _editCommand = _uiCommandRepository.Build(enUICommand.InventorPostEditInventor, EditCommandExecuted, AddCommandCanExecute);
            _closeCommand = new DelegateCommand(CloseCommandExecuted);
            //            _homeCommand = new DelegateCommand(HomeCommandExecuted);
            _inventorDashboardCommand = _uiCommandRepository.Build(enUICommand.InventorPostInventorDashboard, InventorDashboardCommandExecuted, AddCommandCanExecute);
            _homeDashboardCommand = _uiCommandRepository.Build(enUICommand.InventorPostHomeDashboard, HomeDashboardCommandExecuted, AddCommandCanExecute);
            _mainDashboardCommand = _uiCommandRepository.Build(enUICommand.InventorPostMainDashboard, MainDashboardCommandExecuted, MainDashboardCommandCanExecute);
        }

        private bool AddCommandCanExecute()
        {
            return _inventor != null;
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string Log
        {
            get { return _log; }
            set
            {
                _log = value;
                RaisePropertyChanged(() => Log);
            }
        }

        //        public DelegateCommand AddCommand
        //        {
        //            get { return _addCommand; }
        //        }

        public DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public DelegateCommand InventorDashboardCommand
        {
            get { return _inventorDashboardCommand; }
        }

        public DelegateCommand MainDashboardCommand
        {
            get { return _mainDashboardCommand; }
        }

        public DelegateCommand HomeDashboardCommand
        {
            get { return _homeDashboardCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            InventorPostData postData = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, Common.NavigationObjects.InventorPost, true) as InventorPostData;
            if (postData != null)
            {
                if (postData.IsNew)
                {
                    _inventor = _inventorRepository.GetInventorByCode(postData.InventorCode);
                    if (_inventor == null)
                    {
                        Mouse.OverrideCursor = null;
                        StringBuilder log1 = new StringBuilder();
                        log1.Append(Localization.Resources.ViewModel_InventorDontCreate);
                        log1.Append(Environment.NewLine);
                        if (postData.ftpCommandResult != null)
                        {
                            log1.Append(postData.ftpCommandResult.ToString());
                        }
                        Log = log1.ToString();
                        return;
                    }
                    Title = Localization.Resources.ViewModel_InventorPost_tbCreated;

                    StringBuilder log = new StringBuilder();
                    log.Append(String.Format(Localization.Resources.ViewModel_InventorPost_Code, _inventor.Code, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_InventorPost_Description, _inventor.Description, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_InventorPost_CreateDate, UtilsConvert.DateToStringLong(_inventor.CreateDate), Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_InventorPost_DbPath, _inventor.DBPath, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_InventorPost_ImportCatalog, _inventor.ImportCatalogAdapterCode, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_InventorPost_ImportItur, _inventor.ImportIturAdapterCode, Environment.NewLine));
                    log.Append(String.Format(Localization.Resources.ViewModel_InventorPost_ImportLocation, _inventor.ImportLocationAdapterCode, Environment.NewLine));

                    Log = log.ToString();


                    Mouse.OverrideCursor = null;
                }
                else //is edit
                {

                }
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        //        private void AddCommandExecuted()
        //        {
        //            UriQuery query = new UriQuery();
        //
        //            query.Add(NavigationSettings.IsCustomerComboVisible, String.Empty);
        //            query.Add(NavigationSettings.IsBranchComboVisible, String.Empty);
        //
        //            Utils.AddContextToQuery(query, CBIContext.CreateInventor);
        //
        //            Utils.AddContextToQuery(query, base.Context);
        //            Utils.AddDbContextToQuery(query, base.CBIDbContext);
        //
        //            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.InventorAddView + query, UriKind.Relative));
        //        }

        private void EditCommandExecuted()
        {
            using (new CursorWait())
            {
                _eventAggregator.GetEvent<ModalWindowChangeTitleEvent>().Publish(new ModalWindowChangeTitleEventPayload() {Title = Localization.Resources.Window_Title_EditInventor, ViewModel = this});

                UriQuery query = new UriQuery();
                Utils.AddContextToQuery(query, base.Context);
                Utils.AddDbContextToQuery(query, base.CBIDbContext);

                _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(Common.ViewNames.InventorEditView + query, UriKind.Relative));
            }
        }

        private void CloseCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void HomeDashboardCommandExecuted()
        {
            try
            {
                _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

                UriQuery query = new UriQuery();
                Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
                UtilsNavigate.HomeDashboardOpen(CBIContext.Main, this._regionManager, query);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("HomeDashboardCommandExecuted", exc);
            }
        }

        private void InventorDashboardCommandExecuted()
        {
            try
            {
                using (new CursorWait())
                {
                    _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

                    UriQuery query = new UriQuery();
                    Utils.AddAuditConfigToQuery(query, GetHistoryAuditConfig());
                    UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("InventorDashboardCommandExecuted", exc);
            }
        }

        //        private void HomeCommandExecuted()
        //        {
        //            try
        //            {
        //                using (new CursorWait())
        //                {
        //                    _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        //
        //                    UriQuery query = new UriQuery();
        //                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
        //                    UtilsNavigate.MainDashboardOpen(CBIContext.Main, this._regionManager, query);
        //                }
        //            }
        //            catch (Exception exc)
        //            {
        //                _logger.ErrorException("HomeCommandExecuted", exc);
        //            }
        //        }

        private bool MainDashboardCommandCanExecute()
        {
            return _inventor != null;
            //return true;
        }

        private void MainDashboardCommandExecuted()
        {
            try
            {
                using (new CursorWait())
                {
                    _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

                    UriQuery query = new UriQuery();
                    Utils.AddContextToQuery(query, CBIContext.History);
                    Utils.AddDbContextToQuery(query, NavigationSettings.CBIDbContextInventor);
                    Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

                    UtilsNavigate.IturListDetailsOpen(this._regionManager, query);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("HomeDashboardCommandExecuted", exc);
            }
        }

    }
}