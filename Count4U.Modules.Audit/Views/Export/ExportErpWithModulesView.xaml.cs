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
using Count4U.Modules.Audit.ViewModels.Export;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Export
{
    /// <summary>
    /// Interaction logic for ExportErpWithModulesView.xaml
    /// </summary>
    public partial class ExportErpWithModulesView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IRegionManager _regionManager;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly ExportErpWithModulesViewModel _viewModel;

        public ExportErpWithModulesView(
            ExportErpWithModulesViewModel viewModel,
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            ModalWindowLauncher modalWindowLauncher)
        {            
            InitializeComponent();

            this._viewModel = viewModel;
            this.DataContext = _viewModel;
            this._regionManager = regionManager;
            this._contextCbiRepository = contextCbiRepository;
            this._modalWindowLauncher = modalWindowLauncher;
        }

        public bool KeepAlive { get { return false; } }
	

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.OpenAsModalWindow) == false)
            {
                RegionManager.SetRegionName(backForward, Common.RegionNames.ExportErpBackForward);

                Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
                UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ExportErpBackForward);
                UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            }

            RegionManager.SetRegionName(exportErpAdapter, Common.RegionNames.ExportErpByModule);

            _viewModel.ModalWindowRequest += ViewModel_ModalWindowRequest;
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

            this._regionManager.Regions.Remove(Common.RegionNames.ExportErpBackForward);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportErpByModule);

            _viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;
        }

        #endregion

        void ViewModel_ModalWindowRequest(object sender, ModalWindowRequestPayload e)
        {
			e.Width = 540;
			e.Height = 500;
			if (e.ViewName == Common.ViewNames.ConfigEditAndSaveView)
			{
				e.Width = 900;
				e.Height = 700;
			}
			object result = this._modalWindowLauncher.StartModalWindow(e.ViewName, e.WindowTitle, e.Width, e.Height, ResizeMode.CanResize, e.Settings, Window.GetWindow(this));
            if (e.Callback != null)
                e.Callback(result);
        }

        private void ChkIsIturEnabled_OnChecked(object sender, RoutedEventArgs e)
        {
            txtIturs.Focus();
        }

		private void btnExport_PreviewMouseDoubleClick_1(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void btnClear_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
    }
}
