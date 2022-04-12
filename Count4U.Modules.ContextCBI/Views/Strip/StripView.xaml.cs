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
using Count4U.Common.Events;
using Count4U.Modules.ContextCBI.ViewModels.Strip;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Strip
{
    /// <summary>
    /// Interaction logic for StripView.xaml
    /// </summary>
    public partial class StripView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _busy;

        public StripView(StripViewModel viewModel, IEventAggregator eventAggregator)
        {
           
            InitializeComponent();

            this._eventAggregator = eventAggregator;
            this.DataContext = viewModel;

            this.PreviewMouseDown += StripView_PreviewMouseDown;
        }

        void StripView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this._busy)
                e.Handled = true;
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this._eventAggregator.GetEvent<ApplicationBusyEvent>().Subscribe(ApplicationBusy);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this._eventAggregator.GetEvent<ApplicationBusyEvent>().Unsubscribe(ApplicationBusy);
        }

        #endregion

        private void ApplicationBusy(bool isBusy)
        {
            gridMain.IsEnabled = !isBusy;

            this._busy = isBusy;
        }

        #region Implementation of IRegionMemberLifetime

        public bool KeepAlive
        {
            get { return false; }
        }

        #endregion
    }
}
