using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Configuration.Dynamic;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Count4U.Common.Extensions;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl
{
    public class SearchInventProductViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private readonly IInventProductRepository _inventProductRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IRegionManager _regionManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly DynamicRepository _dynamicRepository;

		private readonly DispatcherTimer _timer;

        private readonly ObservableCollection<InventProductItemViewModel> _inventProductList;
        private InventProductItemViewModel _inventProductChooseCurrent;
		private InventProductItemViewModel _selectedItem;

		private readonly DelegateCommand<InventProductItemViewModel> _editGridCommand;
		private readonly DelegateCommand<InventProductItemViewModel> _cancelGridCommand;
		private readonly DelegateCommand<InventProductItemViewModel> _commitGridCommand;

        private int _inventProductPageSize;
        private int _inventProductPageCurrent;
        private int _inventProductItemsTotal;
        private double _inventProductItemsSum;
        private long _totalIturs;

        private readonly DelegateCommand _inventProductMoreCommand;
		private readonly DelegateCommand _inventProductSelectCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchInventProductViewModel(IContextCBIRepository contextCbiRepository,
            IInventProductRepository inventProductRepository,
            IUserSettingsManager userSettingsManager,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            DynamicRepository dynamicRepository)
            : base(contextCbiRepository)
        {
            _dynamicRepository = dynamicRepository;
            _navigationRepository = navigationRepository;
            _regionManager = regionManager;
            _userSettingsManager = userSettingsManager;
            _inventProductRepository = inventProductRepository;
            _inventProductList = new ObservableCollection<InventProductItemViewModel>();
            _inventProductMoreCommand = new DelegateCommand(InventProdutMoreCommandExecuted);
			_inventProductSelectCommand = new DelegateCommand(InventProdutSelectCommandExecuted, InventProdutSelectCommandCanExecuted);

			_editGridCommand = new DelegateCommand<InventProductItemViewModel>(EditGridCommandExecuted);
			_cancelGridCommand = new DelegateCommand<InventProductItemViewModel>(CancelGridCommandExecuted);
			_commitGridCommand = new DelegateCommand<InventProductItemViewModel>(CommitGridCommandExecuted);

			//this._timer = new DispatcherTimer();
			//this._timer.Interval = TimeSpan.FromMilliseconds(this._userSettingsManager.DelayGet());
			//this._timer.Tick += Timer_Tick;
        }

		//void Timer_Tick(object sender, EventArgs e)
		//{
		//		using (new CursorWait())
		//		{
		//			this._timer.Stop();
		//			//this.BuildDetailsItems();
		//		}
		//}

		public DataGrid DataGrid { get; set; }
        public FrameworkElement View { get; set; }

		public DelegateCommand<InventProductItemViewModel> EditGridCommand
		{
			get { return _editGridCommand; }
		}

		public DelegateCommand<InventProductItemViewModel> CancelGridCommand
		{
			get { return _cancelGridCommand; }
		}

		public DelegateCommand<InventProductItemViewModel> CommitGridCommand
		{
			get { return _commitGridCommand; }
		}

		private bool _isEditing;

		private void CommitGridCommandExecuted(InventProductItemViewModel item)
		{
			if (_isEditing)
			{
				SaveInventProductViewModel(item);

				item.UpdateUIAfterDbSave();
				_isEditing = false;
			}
		}

		private void CancelGridCommandExecuted(InventProductItemViewModel item)
		{
			_isEditing = false;
		}

		private void EditGridCommandExecuted(InventProductItemViewModel item)
		{
			_isEditing = true;
		}
		private void SaveInventProductViewModel(InventProductItemViewModel viewModel)
		{
			using (new CursorWait())
			{
				this._inventProductRepository.Update(viewModel.InventProduct, base.GetDbPath);
			}
		}

        public ObservableCollection<InventProductItemViewModel> InventProductList
        {
            get { return _inventProductList; }
        }

        public InventProductItemViewModel InventProductChooseCurrent
        {
            get { return _inventProductChooseCurrent; }
            set
            {
                _inventProductChooseCurrent = value;
                RaisePropertyChanged(() => InventProductChooseCurrent);
				this._inventProductSelectCommand.RaiseCanExecuteChanged();
            }
        }

        public int InventProductPageSize
        {
            get { return _inventProductPageSize; }
            set
            {
                _inventProductPageSize = value;
                RaisePropertyChanged(() => InventProductPageSize);
            }
        }

        public int InventProductPageCurrent
        {
            get { return _inventProductPageCurrent; }
            set
            {
                _inventProductPageCurrent = value;
                RaisePropertyChanged(() => InventProductPageCurrent);

                if (IsBusy != null)
                    IsBusy(true);
                Task.Factory.StartNew(() =>
                {
                    BuildInventProduct();

                    Utils.RunOnUI(() =>
                        {
                            if (IsBusy != null)
                                IsBusy(false);
                        });
				}).LogTaskFactoryExceptions("InventProductPageCurrent");
            }
        }

        public int InventProductItemsTotal
        {
            get { return _inventProductItemsTotal; }
            set
            {
                _inventProductItemsTotal = value;
                RaisePropertyChanged(() => InventProductItemsTotal);

                RaisePropertyChanged(() => InventProductTotalString);                
            }
        }

        public string InventProductTotalString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchInventProductTotal2, _inventProductItemsTotal); }
        }

        public string InventProductSumString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchInventProductSum, _inventProductItemsSum); }
        }

        public string IturTotalString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchInventProduct_TotalIturs, _totalIturs); }
        }

        public DelegateCommand InventProductMoreCommand
        {
            get { return _inventProductMoreCommand; }
        }

		  public DelegateCommand InventProductSelectCommand
        {
			get { return _inventProductSelectCommand; }
        }
		

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _inventProductPageCurrent = 1;
            _inventProductPageSize = this._userSettingsManager.PortionInventProductsGet();
        }

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);
			this.DataGrid = null;
		}
		

        private void BuildInventProduct()
        {
            Utils.RunOnUI(() => _inventProductList.Clear());
            SelectParams sp = SelectParamsInventProduct();


            InventProducts inventProducts = this._inventProductRepository.GetInventProducts(sp, base.GetDbPath);       

            List<InventProductItemViewModel> toAdd = new List<InventProductItemViewModel>();
            foreach (InventProduct inventProduct in inventProducts)
            {                
                InventProductItemViewModel viewModel = new InventProductItemViewModel(inventProduct, null);                
                toAdd.Add(viewModel);
            }

            Utils.RunOnUI(() =>
            {
                toAdd.ForEach(r => _inventProductList.Add(r));
                
                InventProductItemsTotal = (int)inventProducts.TotalCount;              

                if ((inventProducts.TotalCount > 0) && (inventProducts.Count == 0)) //do not show empty space - move on previous page           
                {
                    InventProductPageCurrent = _inventProductPageCurrent - 1;
                }
            });
        }       

        private SelectParams SelectParamsInventProduct()
        {
            SelectParams result = new SelectParams()
            {
                IsEnablePaging = true,
                CountOfRecordsOnPage = _inventProductPageSize,
                CurrentPage = _inventProductPageCurrent,
            };

            if (_searchFieldViewModel != null)
            {
                InventProductFilterData filterData = _searchFieldViewModel.BuildFilterData() as InventProductFilterData;
                if (filterData != null)
                    filterData.ApplyToSelectParams(result);
            }

            return result;
        }

        public void InventProductNavigate(InventProductItemViewModel viewModel)
        {
            UtilsPopup.Close(View);

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            SelectParams selectParams = new SelectParams();
            selectParams.IsEnablePaging = false;
            selectParams.FilterParams.Add("IturCode", new FilterParam() { Operator = FilterOperator.Contains, Value = viewModel.InventProduct.IturCode });

            InventProductDetailsData navigationData = new InventProductDetailsData();
            navigationData.IturSelectParams = selectParams;
            navigationData.IturCode = viewModel.InventProduct.IturCode;
            navigationData.DocumentCode = viewModel.InventProduct.DocumentCode;
            navigationData.InventProductId = viewModel.InventProduct.ID;
            navigationData.SearchItem = String.Empty;
            navigationData.SearchExpression = String.Empty;

            UtilsConvert.AddObjectToQuery(query, this._navigationRepository, navigationData, NavigationObjects.InventProductDetails);

            UtilsNavigate.InventProductDetailsOpen(this._regionManager, query);
        }

        private void InventProdutMoreCommandExecuted()
        {
            UtilsPopup.Close(View);

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            if (_searchFieldViewModel != null)
            {
                InventProductFilterData filterData = _searchFieldViewModel.BuildFilterData() as InventProductFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.InventProductListOpen(this._regionManager, query);
        }

		private bool InventProdutSelectCommandCanExecuted()
		{
			return this._inventProductChooseCurrent != null;
		}

		private void InventProdutSelectCommandExecuted()
		{
			UtilsPopup.Close(View);
			if (InventProductChooseCurrent == null) return;

			long ID = InventProductChooseCurrent.InventProduct.ID;
			UriQuery query = new UriQuery();
			Utils.AddContextToQuery(query, base.Context);
			Utils.AddDbContextToQuery(query, base.CBIDbContext);
			Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

			//if (_searchFieldViewModel != null)
			//{
			//	InventProductFilterData filterData = _searchFieldViewModel.BuildFilterData() as InventProductFilterData;
			//	if (filterData != null)
			//		UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
			//}

			if (_searchFieldViewModel != null)
			{
				InventProductFilterData filterData = _searchFieldViewModel.BuildFilterSelectData(ID.ToString()) as InventProductFilterData;
				if (filterData != null)
					UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
			}


			UtilsNavigate.InventProductListOpen(this._regionManager, query);
		}

        public void Search()
        {
            _inventProductPageCurrent = 1;
            Utils.RunOnUI(() => RaisePropertyChanged(() => InventProductPageCurrent));
            BuildInventProduct();

            SelectParams sp = SelectParamsInventProduct();
            InventProducts inventProductTotal = this._inventProductRepository.GetInventProductTotal(base.GetDbPath, sp);

            _inventProductItemsSum = inventProductTotal.SumQuantityEdit;
            RaisePropertyChanged(() => InventProductSumString);

            _totalIturs = inventProductTotal.TotalItur;
            RaisePropertyChanged(() => IturTotalString);
        }

        public Action<bool> IsBusy { get; set; }

        public ISearchFieldViewModel SearchFieldViewModel
        {
            set { _searchFieldViewModel = value; }
        }

        public void CanSearch(bool isCanSearch)
        {

        }

        public SelectParams BuildSelectParams()
        {
            return SelectParamsInventProduct();
        }
    }
}