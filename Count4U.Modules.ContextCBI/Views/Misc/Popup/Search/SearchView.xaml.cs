using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchView(SearchViewModel viewModel, IRegionManager regionManager)
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.View = this;
            viewModel.ReportContextMenu = btnReport.ContextMenu;
            _regionManager = regionManager;

            Loaded += SearchView_Loaded;
            //http://stackoverflow.com/questions/5807734/wpf-popup-doesnt-close-automatically-when-datagrid-inside-popup-captures-the-mo                                                          

            RegionManager.SetRegionName(searchField, Common.RegionNames.SearchFieldGround);
            RegionManager.SetRegionName(searchGrid, Common.RegionNames.SearchGridGround);
            RegionManager.SetRegionName(searchFieldTemplate, Common.RegionNames.SearchFieldTemplate);

            this.PreviewKeyDown += SearchView_PreviewKeyDown;
        }

		void SearchView_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SearchViewModel viewModel = this.DataContext as SearchViewModel;

				if (viewModel != null)
				{
					if (viewModel.ObjectTypeSelectedItem.Value == "InventProduct")
						return;

					if (viewModel.SearchCommand.CanExecute())
						viewModel.SearchCommand.Execute();

					e.Handled = true;
				}
			}
			//if (e.Key == Key.Escape)
			//{

			//	SearchViewModel viewModel = this.DataContext as SearchViewModel;

			//	if (viewModel != null)
			//	{
			//		if (viewModel.ObjectTypeSelectedItem.Value == "InventProduct")
			//		e.Handled = true;
			//	}

			//}
		}

        void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            cmbObjectType.Focus();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionManager.RequestNavigate(Common.RegionNames.SearchFieldTemplate, new Uri(Common.ViewNames.FilterTemplateView, UriKind.Relative));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _regionManager.Regions.Remove(Common.RegionNames.SearchFieldGround);
            _regionManager.Regions.Remove(Common.RegionNames.SearchGridGround);
            _regionManager.Regions.Remove(Common.RegionNames.SearchFieldTemplate);

            INavigationAware field = searchField.Content as INavigationAware;
            if (field != null)
            {
                field.OnNavigatedFrom(navigationContext);
            }

            INavigationAware grid = searchGrid.Content as INavigationAware;
            if (grid != null)
            {
                grid.OnNavigatedFrom(navigationContext);
            }

            INavigationAware filterTemplates = searchFieldTemplate.Content as INavigationAware;
            if (filterTemplates != null)
            {
                filterTemplates.OnNavigatedFrom(navigationContext);
            }
        }
    }
}
