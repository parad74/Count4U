using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Count4U.Common.Behaviours;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.ViewModels.Pack;
using Count4U.Modules.ContextCBI.ViewModels.Pack.Unpack;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Pack
{
    /// <summary>
    /// Interaction logic for UnpackView.xaml
    /// </summary>
    public partial class UnpackView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;

        public UnpackView(
            UnpackViewModel viewModel,
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager)
        {            
            InitializeComponent();

            this.DataContext = viewModel;

            _contextCbiRepository = contextCbiRepository;
            _regionManager = regionManager;

            System.Windows.Interactivity.Interaction.GetBehaviors(txtInputArchive).Add(new TextChangedDelayedBehavior{IsTimerEnabled = true});

           
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.UnpackBackForward);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.UnpackBackForward);
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

            this._regionManager.Regions.Remove(Common.RegionNames.UnpackBackForward);
        }

        public bool KeepAlive { get { return false; } }
    }
}
