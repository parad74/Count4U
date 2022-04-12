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
using Count4U.Common.Misc.PopupExt;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.ViewModels.Pack;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Pack
{
    /// <summary>
    /// Interaction logic for PackView.xaml
    /// </summary>
    public partial class PackView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IRegionManager _regionManager;
        private readonly PopupExtFilter _popupExtFilter;
        private readonly INavigationRepository _navigationRepository;

        public PackView(
            PackViewModel viewModel,
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            PopupExtFilter popupExtFilter,
            INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
            _popupExtFilter = popupExtFilter;
            InitializeComponent();

            _regionManager = regionManager;
            _contextCbiRepository = contextCbiRepository;

            this.DataContext = viewModel;

            this.Loaded += PackView_Loaded;

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = Common.RegionNames.PopupFilterPack;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.FilterView;
            _popupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, viewModel.Filter, Common.NavigationObjects.Filter);
            _popupExtFilter.Init();
        }

        void PackView_Loaded(object sender, RoutedEventArgs e)
        {
            tree.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(CheckBox_CheckedUnchecked));
            tree.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(CheckBox_CheckedUnchecked));
        }

        void CheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as PackViewModel).SomeNodeCheckedUnchecked();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.PackBackForward);            

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);            

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.PackBackForward);            
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
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.PackBackForward);            
        }

        public bool KeepAlive { get { return false; } }
    }
}
