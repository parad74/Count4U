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
using Count4U.Planogram.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Planogram.View
{
    /// <summary>
    /// Interaction logic for PlanAddEditDeleteView.xaml
    /// </summary>
    public partial class PlanAddEditDeleteView : UserControl, INavigationAware
    {
        private readonly string backForwardRegionName;
        private readonly string searchFilterRegionName;

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly Guid _guid;

        public PlanAddEditDeleteView(
            PlanAddEditDeleteViewModel viewModel,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository)
        {
            InitializeComponent();            

            _contextCbiRepository = contextCbiRepository;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _guid = Guid.NewGuid();
            backForwardRegionName = Common.RegionNames.PlanogramAddEditDeleteBackForward + _guid.ToString();
            searchFilterRegionName = Common.RegionNames.PlanogramAddEditDeleteSearchFilter + _guid.ToString();

            this.DataContext = viewModel;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, backForwardRegionName);
            RegionManager.SetRegionName(searchFilter, searchFilterRegionName);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            Utils.MainWindowTitleSet(WindowTitles.PlanogramAddEditDelete, this._eventAggregator);

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, backForwardRegionName);
            UtilsNavigate.SearchFilterNavigate(this._regionManager, searchFilterRegionName);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                      backForward,
                                                      searchFilter
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(backForwardRegionName);
            this._regionManager.Regions.Remove(searchFilterRegionName);
        }
    }
}
