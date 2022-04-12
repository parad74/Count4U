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
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc.Adapters
{
    /// <summary>
    /// Interaction logic for AdapterLinkView.xaml
    /// </summary>
    public partial class AdapterLinkView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly AdapterLinkViewModel _viewModel;
        private readonly ModalWindowLauncher _modalWindowLauncher;

        public AdapterLinkView(
            AdapterLinkViewModel viewModel,
            IRegionManager regionManager,
            IContextCBIRepository contextCbiRepository,
            ModalWindowLauncher modalWindowLauncher)
        {            
            InitializeComponent();

            this._viewModel = viewModel;
            this._regionManager = regionManager;
            this.DataContext = this._viewModel;
            this._contextCbiRepository = contextCbiRepository;
            this._modalWindowLauncher = modalWindowLauncher;
        }

        public bool KeepAlive { get { return false; } }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.AdapterLinkBackForward);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.AdapterLinkBackForward);
            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);

            this._viewModel.ModalWindowRequest += ViewModel_ModalWindowRequest;
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

            this._regionManager.Regions.Remove(Common.RegionNames.AdapterLinkBackForward);

            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;
        }

        void ViewModel_ModalWindowRequest(object sender, ModalWindowRequestPayload e)
        {
            object result = null;

            if (e.ViewName == Common.ViewNames.AdapterLinkScriptSaveView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.AdapterLinkScriptSaveView, e.WindowTitle, 370, 350,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 370, minHeight: 180);
            }

            if (e.ViewName == Common.ViewNames.AdapterLinkScriptOpenView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.AdapterLinkScriptOpenView, e.WindowTitle, 370, 350,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 370, minHeight: 180);
            }

            if (e.Callback != null)
                e.Callback(result);
        }
    }
}
