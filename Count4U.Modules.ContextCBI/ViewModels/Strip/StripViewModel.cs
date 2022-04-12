using System;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Strip
{
    public enum Mode
    {
        Empty,
        Customer,
        CustomerBranch,
        CustomerBranchInventor
    }

    public class StripViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ObservableCollection<StripBaseViewModel> _viewModels;
        private readonly StripCustomerViewModel _customerViewModel;
        private readonly StripBranchViewModel _branchViewModel;
        private readonly StripHomeViewModel _homeViewModel;
        private readonly StripInventorViewModel _inventorViewModel;
        private readonly StripEmptyViewModel _emptyViewModel;
        private readonly StripMainViewModel _mainViewModel;

        public StripViewModel(IContextCBIRepository contextCBIRepository,
            StripHomeViewModel homeViewModel,
            StripCustomerViewModel customerViewModel,
            StripBranchViewModel branchViewModel,
            StripInventorViewModel inventorViewModel,
            StripEmptyViewModel emptyViewModel,
            StripMainViewModel mainViewModel,
            IEventAggregator eventAggregator
            )
            : base(contextCBIRepository)
        {
            this._mainViewModel = mainViewModel;
            this._eventAggregator = eventAggregator;
            this._emptyViewModel = emptyViewModel;
            this._inventorViewModel = inventorViewModel;
            this._homeViewModel = homeViewModel;
            this._branchViewModel = branchViewModel;
            this._customerViewModel = customerViewModel;

            this._viewModels = new ObservableCollection<StripBaseViewModel>();
        }


        public ObservableCollection<StripBaseViewModel> ViewModels
        {
            get { return this._viewModels; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<CustomerEditedEvent>().Subscribe(CustomerEdited);
            this._eventAggregator.GetEvent<BranchEditedEvent>().Subscribe(BranchEdited);
            this._eventAggregator.GetEvent<InventorEditedEvent>().Subscribe(InventorEdited);

            this._homeViewModel.OnNavigatedTo(navigationContext);
            this._customerViewModel.OnNavigatedTo(navigationContext);
            this._branchViewModel.OnNavigatedTo(navigationContext);
            this._inventorViewModel.OnNavigatedTo(navigationContext);
            this._mainViewModel.OnNavigatedTo(navigationContext);
//            this._homeViewModel.Context = this.Context;
//            this._customerViewModel.Context = this.Context;
//            this._branchViewModel.Context = this.Context;
//            this._inventorViewModel.Context = this.Context;

            this._homeViewModel.Column = 0;
            this._customerViewModel.Column = 1;
            this._branchViewModel.Column = 2;
            this._inventorViewModel.Column = 3;
            this._mainViewModel.Column = 4;

            this._viewModels.Clear();

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.StripMode))
            {
                string strMode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.StripMode).Value;
                switch (strMode)
                {
                    case Common.NavigationSettings.StripModeEmpty:
                        {
                            //                            this._mode = Mode.Empty;
                            this._viewModels.Add(this._homeViewModel);
                            this._emptyViewModel.Column = 1;
                            this._emptyViewModel.ColumnSpan = 3;
                            this._viewModels.Add(this._emptyViewModel);
                            this._viewModels.Add(this._mainViewModel);
                            break;
                        }
                    case Common.NavigationSettings.StripModeCustomer:
                        {
                            //                            this._mode = Mode.Customer;
                            this._viewModels.Add(this._homeViewModel);
                            this._customerViewModel.ColumnSpan = 3;
                            this._viewModels.Add(this._customerViewModel);
                            this._viewModels.Add(this._mainViewModel);
                            break;
                        }
                    case Common.NavigationSettings.StripModeCustomerBranch:
                        {
                            //                            this._mode = Mode.CustomerBranch;
                            this._viewModels.Add(this._homeViewModel);
                            this._viewModels.Add(this._customerViewModel);
                            this._branchViewModel.ColumnSpan = 2;
                            this._viewModels.Add(this._branchViewModel);
                            this._viewModels.Add(this._mainViewModel);
                            break;
                        }
                    case Common.NavigationSettings.StripModeCustomerBranchInventor:
                        {
                            //                            this._mode = Mode.CustomerBranchInventor;
                            this._viewModels.Add(this._homeViewModel);
                            this._viewModels.Add(this._customerViewModel);
                            this._viewModels.Add(this._branchViewModel);
                            this._viewModels.Add(this._inventorViewModel);
                            this._viewModels.Add(this._mainViewModel);
                            break;
                        }
                    default:
                        throw new InvalidOperationException();
                }
            }


        }
      
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._homeViewModel.OnNavigatedFrom(navigationContext);
            this._customerViewModel.OnNavigatedFrom(navigationContext);
            this._branchViewModel.OnNavigatedFrom(navigationContext);
            this._inventorViewModel.OnNavigatedFrom(navigationContext);
            this._mainViewModel.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<CustomerEditedEvent>().Unsubscribe(CustomerEdited);
            this._eventAggregator.GetEvent<BranchEditedEvent>().Unsubscribe(BranchEdited);
            this._eventAggregator.GetEvent<InventorEditedEvent>().Unsubscribe(InventorEdited);
        }

        private void CustomerEdited(Customer customer)
        {
            if (this._customerViewModel != null)
                this._customerViewModel.Refresh();
        }

        private void BranchEdited(Branch branch)
        {
            if (this._branchViewModel != null)
                this._branchViewModel.Refresh();
        }

        private void InventorEdited(Inventor inventor)
        {
            if (this._inventorViewModel != null)
                this._inventorViewModel.Refresh();
        }

    }
}