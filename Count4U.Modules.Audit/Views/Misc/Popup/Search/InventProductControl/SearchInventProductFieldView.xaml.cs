using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl;
using Microsoft.Practices.Prism.Regions;
using System.Windows;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.InventProductControl
{
    public partial class SearchInventProductFieldView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchInventProductFieldView(
            SearchInventProductFieldViewModel viewModel,
            IRegionManager regionManager)
        {
            InitializeComponent();

            _regionManager = regionManager;

            this.DataContext = viewModel;

            this.Loaded += SearchInventProductFieldView_Loaded;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(sortControl, Common.RegionNames.Sort);

            _regionManager.RequestNavigate(Common.RegionNames.Sort, new Uri(Common.ViewNames.SortView, UriKind.Relative));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        sortControl,                                                      
                                                  }, navigationContext);

            _regionManager.Regions.Remove(Common.RegionNames.Sort);
        }

        void SearchInventProductFieldView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();

                    SearchInventProductFieldViewModel viewModel = this.DataContext as SearchInventProductFieldViewModel;
                    if (viewModel != null)
                    {
                        switch (viewModel.FocusedField)
                        {
                            case FocusValues.InventProductSearch.Makat:
                                txtMakat.Focus();
                                break;
							case FocusValues.InventProductSearch.Code:
								txtCode.Focus();
                                break;
                            case FocusValues.InventProductSearch.CodeInputFromPDA:
                                txtCodeInputFromPda.Focus();
                                break;
							case FocusValues.InventProductSearch.SerialNumber:
								txtSerialNumber.Focus();
								break;
                            case FocusValues.InventProductSearch.ProductName:
                                txtProductName.Focus();
                                break;
                            case FocusValues.InventProductSearch.IturCode:
                                txtIturCode.Focus();
                                break;
							case FocusValues.InventProductSearch.PropertyStr:
								txtProperty.Focus();
								break;
							case FocusValues.InventProductSearch.PropertyNumber:
								txtPropertyNumber.Focus();
								break;

                        }
                    }
                }), DispatcherPriority.Background);
        }
    }
}
