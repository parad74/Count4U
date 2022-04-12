using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.ViewModels.Catalog;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.SelectionParams;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.ProductControl
{
    public class SearchProductViewModel : CBIContextBaseViewModel, ISearchGridViewModel
    {
        private readonly IProductRepository _productRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly INavigationRepository _navigationRepository;

        private readonly ObservableCollection<ProductItemViewModel> _items;
        private ProductItemViewModel _current;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private readonly DelegateCommand _moreCommand;
        private ISearchFieldViewModel _searchFieldViewModel;

        public SearchProductViewModel(IContextCBIRepository contextCbiRepository,
            IProductRepository productRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            INavigationRepository navigationRepository
            )
            : base(contextCbiRepository)
        {
            _navigationRepository = navigationRepository;
            _userSettingsManager = userSettingsManager;
            _regionManager = regionManager;
            _productRepository = productRepository;

            _items = new ObservableCollection<ProductItemViewModel>();
            _moreCommand = new DelegateCommand(MoreCommandExecuted);
        }        

        public FrameworkElement View { get; set; }

        public ObservableCollection<ProductItemViewModel> Items
        {
            get { return _items; }
        }

        public ProductItemViewModel Current
        {
            get { return _current; }
            set
            {
                _current = value;
                RaisePropertyChanged(() => Current);
            }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                RaisePropertyChanged(() => PageSize);
            }
        }

        public int PageCurrent
        {
            get { return _pageCurrent; }
            set
            {
                _pageCurrent = value;
                RaisePropertyChanged(() => PageCurrent);

                if (IsBusy != null)
                    IsBusy(true);
                Task.Factory.StartNew(() =>
                {
                    Build();

                    Utils.RunOnUI(() =>
                    {
                        if (IsBusy != null)
                            IsBusy(false);
                    });
				}).LogTaskFactoryExceptions("PageCurrent");
            }
        }

        public int ItemsTotal
        {
            get { return _itemsTotal; }
            set
            {
                _itemsTotal = value;
                RaisePropertyChanged(() => ItemsTotal);

                RaisePropertyChanged(() => TotalString);
            }
        }

        public string TotalString
        {
            get { return String.Format(Localization.Resources.ViewModel_SearchProductTotal, _itemsTotal); }
        }

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _pageCurrent = 1;
            _pageSize = this._userSettingsManager.PortionProductsGet();
        }

        private void Build()
        {
            Utils.RunOnUI(() => _items.Clear());
            SelectParams sp = SelectParamsBuild();

            Products products = this._productRepository.GetProducts(sp, base.GetDbPath);

            List<ProductItemViewModel> toAdd = new List<ProductItemViewModel>();
            foreach (Product product in products)
            {
                ProductItemViewModel viewModel = new ProductItemViewModel(product);
                toAdd.Add(viewModel);
            }

            Utils.RunOnUI(() =>
            {
                toAdd.ForEach(r => _items.Add(r));
                ItemsTotal = (int)products.TotalCount;
                if ((products.TotalCount > 0) && (products.Count == 0)) //do not show empty space - move on previous page           
                {
                    PageCurrent = _pageCurrent - 1;
                }
            });
        }

        private SelectParams SelectParamsBuild()
        {
            SelectParams result = new SelectParams()
            {
                IsEnablePaging = true,
                CountOfRecordsOnPage = _pageSize,
                CurrentPage = _pageCurrent,
            };

            if (_searchFieldViewModel != null)
            {
                ProductFilterData filterData = _searchFieldViewModel.BuildFilterData() as ProductFilterData;
                if (filterData != null)
                    filterData.ApplyToSelectParams(result, _productRepository, base.GetDbPath);
            }

            return result;
        }

        private void MoreCommandExecuted()
        {
            UtilsPopup.Close(View);

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

            if (_searchFieldViewModel != null)
            {
                ProductFilterData filterData = _searchFieldViewModel.BuildFilterData() as ProductFilterData;
                if (filterData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
            }

            UtilsNavigate.CatalogOpen(this._regionManager, query);
        }

        public void Search()
        {
            _pageCurrent = 1;
            Utils.RunOnUI(() => RaisePropertyChanged(() => PageCurrent));
            Build();
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
            return SelectParamsBuild();
        }
    }
}