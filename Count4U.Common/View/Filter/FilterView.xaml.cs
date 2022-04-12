using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Controls;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Filter;
using Microsoft.Practices.Prism.Regions;
using Count4U.Common.View.DragDrop.Utilities;

namespace Count4U.Common.View.Filter
{
    /// <summary>
    /// Interaction logic for FilterView.xaml
    /// </summary>
    public partial class FilterView : UserControl, INavigationAware, IPopupChildControl
    {
        private readonly IRegionManager _regionManager;
        private FrameworkElement _visualRoot;

        public FilterView(
            IRegionManager regionManager,
            FilterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this._regionManager = regionManager;

            viewModel.View = this;

            this.KeyDown += FilterView_KeyDown;

            this.Loaded += FilterView_Loaded;
        }

        void FilterView_Loaded(object sender, RoutedEventArgs e)
        {
           // PopupView popup = this.GetVisualAncestor<PopupView>();

            Window window = Window.GetWindow(this);

            _visualRoot = window;

            if (_visualRoot != null)
            {
                _visualRoot.MouseEnter += Window_MouseEnter;
                _visualRoot.MouseLeave += Window_MouseLeave;
            }
        }

        void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            _visualRoot.Opacity = 1;
        }

        void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            _visualRoot.Opacity = 0.8;
        }

        void FilterView_KeyDown(object sender, KeyEventArgs e)
        {
            FilterViewModel viewModel = this.DataContext as FilterViewModel;
            if (viewModel != null)
            {
                if (e.Key == Key.Enter)
                {
                    viewModel.EnterPressed();
                }
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(ground, Common.RegionNames.FilterFieldGround);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>() { ground }, navigationContext);

            _regionManager.Regions.Remove(Common.RegionNames.FilterFieldGround);
        }

        public void Apply()
        {
            FilterViewModel viewModel = this.DataContext as FilterViewModel;
            if (viewModel != null)
            {
                viewModel.ApplyCommand.Execute();
            }
        }

        public void Reset()
        {
            FilterViewModel viewModel = this.DataContext as FilterViewModel;
            if (viewModel != null)
            {
                viewModel.ResetCommand.Execute();
            }
        }
    }
}
