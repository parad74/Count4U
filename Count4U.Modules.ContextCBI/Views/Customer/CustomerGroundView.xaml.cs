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
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Modules.ContextCBI.Events;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Customer
{
    /// <summary>
    /// Interaction logic for CustomerGroundView.xaml
    /// </summary>
    public partial class CustomerGroundView : UserControl, INavigationAware, IInnerContent
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        public CustomerGroundView(IRegionManager regionManager, IEventAggregator eventAggregator)
        {            
            InitializeComponent();

            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

           RegionManager.SetRegionName(this.content, Common.RegionNames.CustomerGround);
        }       

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            string viewName = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.CBIViewName).Value;

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            _regionManager.RequestNavigate(Common.RegionNames.CustomerGround, new Uri(viewName + query, UriKind.Relative));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _regionManager.Regions.Remove(Common.RegionNames.CustomerGround);

            FrameworkElement fe = this.content.Content as FrameworkElement;
            if (fe != null)
            {
                INavigationAware viewModel = fe.DataContext as INavigationAware;
                if(viewModel != null)
                    viewModel.OnNavigatedFrom(navigationContext);

                INavigationAware view = fe as INavigationAware;
                if (view != null)
                    view.OnNavigatedFrom(navigationContext);
            }
            this._eventAggregator.GetEvent<InventorInventorsRefreshEvent>().Publish(null);       //2021
        }

        public ContentControl InnerContent
        {
            get { return this.content; }
        }
    }
}
