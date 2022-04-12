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
using Count4U.Modules.ContextCBI.ViewModels.ParsingMask;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.ParsingMask
{
    /// <summary>
    /// Interaction logic for MaskListView.xaml
    /// </summary>
    public partial class MaskListView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly MaskListViewModel _viewModel;

        public MaskListView(
            MaskListViewModel viewModel,
            IRegionManager regionManager,             
            IContextCBIRepository contextCbiRepository,
            ModalWindowLauncher modalWindowLauncher)
        {
           
            InitializeComponent();

            this._viewModel = viewModel;
            this.DataContext = this._viewModel;
            this._regionManager = regionManager;
            this._contextCbiRepository = contextCbiRepository;            
            this._modalWindowLauncher = modalWindowLauncher;
        }

        public bool KeepAlive { get { return false; } }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.MaskListBackForward);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.MaskListBackForward);
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
                                                        backForward                                         
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.MaskListBackForward);

            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;
        }

        #endregion

        void ViewModel_ModalWindowRequest(object sender, ModalWindowRequestPayload e)
        {
            object result = null;

            if (e.ViewName == Common.ViewNames.MaskScriptSaveView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.MaskScriptSaveView, e.WindowTitle, 370, 350,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 370, minHeight: 180);
            }

            if (e.ViewName == Common.ViewNames.MaskScriptOpenView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.MaskScriptOpenView, e.WindowTitle, 370, 350,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 370, minHeight: 180);
            }

            if (e.Callback != null)
                e.Callback(result);
        }
    }
}
