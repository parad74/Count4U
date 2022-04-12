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
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.ErpExpected;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.ErpExpected
{
    /// <summary>
    /// Interaction logic for ErpExpectedStep1View.xaml
    /// </summary>
    public partial class ErpExpectedStep1View : UserControl, INavigationAware,IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;

        public ErpExpectedStep1View(ErpExpectedStep1ViewModel viewModel, 
            IRegionManager regionManager, 
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository)
        {
            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            InitializeComponent();

            this._regionManager = regionManager;

            this.DataContext = viewModel;
        }

        public bool KeepAlive { get { return false; } }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            Utils.MainWindowTitleSet(WindowTitles.ErpExpectedStep1, this._eventAggregator);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
