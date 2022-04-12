using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Count4U.Common;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.Import;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.Audit.Views.Import
{
    /// <summary>
    /// Interaction logic for ImportWithModulesView.xaml
    /// </summary>
    public partial class ImportWithModulesView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly ImportWithModulesViewModel _viewModel;

        public ImportWithModulesView(
            ImportWithModulesViewModel viewModel,             
            IRegionManager regionManager,
            IContextCBIRepository contextCbiRepository,
            ModalWindowLauncher modalWindowLauncher)
        {         
            InitializeComponent();

            this._modalWindowLauncher = modalWindowLauncher;
            this._contextCbiRepository = contextCbiRepository;
            this._regionManager = regionManager;

            this._viewModel = viewModel;
            this.DataContext = _viewModel;

            this.Loaded += ImportWithModulesView_Loaded;
        }

        void ImportWithModulesView_Loaded(object sender, RoutedEventArgs e)
        {
            ImportWithModulesViewModel viewModel = this.DataContext as ImportWithModulesViewModel;
            if (viewModel != null)
            {
                if (viewModel.Mode == ImportDomainEnum.ImportBranch)
                {
                    tbEncoding.Margin = new Thickness(556, 2, 0, 0);
                    cmbEncoding.Margin = new Thickness(556, 19, 0, 0);
                    chkInvertLetters.Margin = new Thickness(556, 48, 0, 0);
                    chkInvertWords.Margin = new Thickness(656, 48, 0, 0);
                }
            }
        }

        public bool KeepAlive { get { return false; } }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.ImportBackForward);
            RegionManager.SetRegionName(contentModule, Common.RegionNames.ImportByModule);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            
            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ImportBackForward);

            _viewModel.ModalWindowRequest += ViewModel_ModalWindowRequest;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;

          
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        backForward,     
                                                        contentModule
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.ImportByModule);
            this._regionManager.Regions.Remove(Common.RegionNames.ImportBackForward);
        }

        void ViewModel_ModalWindowRequest(object sender, Common.Interfaces.ModalWindowRequestPayload e)
        {
			//e.ViewName	  = Common.ViewNames.LogView
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

		private void btnImport_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void btnClear_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
    }
}
