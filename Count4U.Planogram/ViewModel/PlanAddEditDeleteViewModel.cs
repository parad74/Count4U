using System.Collections.ObjectModel;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.SelectionParams;
using Count4U.Planogram.Lib;
using Count4U.Planogram.Lib.Enums;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using NLog;
using System;

namespace Count4U.Planogram.ViewModel
{
    public class PlanAddEditDeleteViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IUnitPlanRepository _unitPlanRepository;
        private readonly UICommandRepository _uiCommandRepository;
        private readonly IRegionManager _regionManager;

        private readonly UICommand _planogramCommand;
        private readonly UICommand _addCommand;
        private readonly UICommand _editCommand;
        private readonly UICommand _deleteCommand;
        private readonly UICommand _importCommand;

        private readonly ObservableCollection<UnitPlanItemViewModel> _items;
        private UnitPlanItemViewModel _selected;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        public PlanAddEditDeleteViewModel(
            IContextCBIRepository contextCbiRepository,
            IUnitPlanRepository unitPlanRepository,
            UICommandRepository uiCommandRepository,
            IRegionManager regionManager)
            : base(contextCbiRepository)
        {
            _regionManager = regionManager;
            _uiCommandRepository = uiCommandRepository;
            _unitPlanRepository = unitPlanRepository;

            this._planogramCommand = _uiCommandRepository.Build(enUICommand.Planogram, PlanogramCommandExecuted);
            this._addCommand = _uiCommandRepository.Build(enUICommand.Add, this.AddCommandExecuted);
            this._deleteCommand = _uiCommandRepository.Build(enUICommand.Delete, this.DeleteCommandExecuted, this.DeleteCommandCanExecute);
            this._editCommand = _uiCommandRepository.Build(enUICommand.Edit, this.EditCommandExecuted, this.EditCommandCanExecute);
            this._importCommand = _uiCommandRepository.Build(enUICommand.Import, this.ImportCommandExecuted);

            _items = new ObservableCollection<UnitPlanItemViewModel>();
        }

        public ObservableCollection<UnitPlanItemViewModel> Items
        {
            get { return _items; }
        }

        public UnitPlanItemViewModel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RaisePropertyChanged(()=>Selected);

                _editCommand.RaiseCanExecuteChanged();
                _deleteCommand.RaiseCanExecuteChanged();
            }
        }

        public int PageSize
        {
            get { return this._pageSize; }
            set
            {
                this._pageSize = value;
                this.RaisePropertyChanged(() => this.PageSize);
            }
        }

        public int PageCurrent
        {
            get { return this._pageCurrent; }
            set
            {
                this._pageCurrent = value;
                this.RaisePropertyChanged(() => this.PageCurrent);

                Build();
            }
        }

        public int ItemsTotal
        {
            get { return this._itemsTotal; }
            set
            {
                this._itemsTotal = value;
                this.RaisePropertyChanged(() => this.ItemsTotal);
            }
        }

        public UICommand PlanogramCommand
        {
            get { return _planogramCommand; }
        }

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand DeleteCommand
        {
            get { return this._deleteCommand; }
        }

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return this._editCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._pageSize = 50;
            this._pageCurrent = 1;

            Build();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void Build()
        {
            _items.Clear();

            SelectParams selectParams = BuildSelectParams();

            try
            {
                UnitPlans unitPlans = _unitPlanRepository.GetUnitPlans(selectParams, base.GetDbPath);

                foreach (UnitPlan plan in unitPlans)
                {
                    if (plan.ObjectCode == enPlanObjectType.Planogram.ToString())
                        continue;

                    _items.Add(new UnitPlanItemViewModel(plan));
                }

                this.ItemsTotal = (int)unitPlans.TotalCount;

                if ((unitPlans.TotalCount > 0)
                    && (this._items.Count == 0))	//do not show empty space - move on previous page
                {
                    this.PageCurrent = this._pageCurrent - 1;
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
                _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                _logger.Error("SelectParams: {0}", selectParams.ToString());
                throw;
            }

        }

        private SelectParams BuildSelectParams()
        {
            SelectParams result = new SelectParams();
           // result.SortParams = "Code";
            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = this._pageSize;
            result.CurrentPage = this._pageCurrent;            

            return result;
        }

        private void PlanogramCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.PlanogramOpen(this._regionManager, uriQuery);
        }

        private void AddCommandExecuted()
        {
         
        }
        

        private bool DeleteCommandCanExecute()
        {
            return _selected != null;
        }

        private void DeleteCommandExecuted()
        {
//            MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
//            notification.Title = String.Empty;
//            notification.Settings = this._userSettingsManager;
//            string locationNames = this._selectedItems.Select(r => r.Name).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z));
//            locationNames = locationNames.Remove(locationNames.Length - 1, 1);
//            string message = String.Format(Localization.Resources.Msg_Delete_Location, locationNames);
//
//            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Warning, _userSettingsManager);
//
//            if (messageBoxResult == MessageBoxResult.Yes)
//            {
//                foreach (LocationItemViewModel location in this._selectedItems)
//                {
//                    this._locationRepository.Delete(location.Location, base.GetDbPath);
//                }
//
//                Build();
//            }
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

        private bool EditCommandCanExecute()
        {
            return _selected != null;
        }

        private void EditCommandExecuted()
        {
         
        }      
    }
}