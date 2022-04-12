using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.Export;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Export
{  	 // форма Export Pda Adapter с комбобоксом 
    public partial class ExportPdaWithModulesView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly ExportPdaWithModulesViewModel _viewModel;
        private readonly ModalWindowLauncher _modalWindowLauncher;

        public ExportPdaWithModulesView(
            ExportPdaWithModulesViewModel viewModel,
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            ModalWindowLauncher modalWindowLauncher)
        {
            InitializeComponent();

            this._modalWindowLauncher = modalWindowLauncher;
            this._viewModel = viewModel;
            this._contextCbiRepository = contextCbiRepository;
            this._regionManager = regionManager;
            this.DataContext = _viewModel;
        }

        public bool KeepAlive { get { return false; } }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.ExportBackForward);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ExportBackForward);
            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            RegionManager.SetRegionName(exportAdapter, Common.RegionNames.ExportByModule);
			RegionManager.SetRegionName(exportPdaAdapter, Common.RegionNames.ExportPdaAdapter);
            RegionManager.SetRegionName(exportSettings, Common.RegionNames.ExportPdaSettings);
            RegionManager.SetRegionName(programTypeControl, Common.RegionNames.ExportPdaProgramType);

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            query.Add(Common.NavigationSettings.ExportPdaSettingsConrolInExportFormMode, String.Empty);


			_regionManager.RequestNavigate(Common.RegionNames.ExportPdaAdapter, new Uri(Common.ViewNames.ExportPdaMerkavaAdapterView + query, UriKind.Relative));
  			_regionManager.RequestNavigate(Common.RegionNames.ExportPdaSettings, new Uri(Common.ViewNames.ExportPdaSettingsControlView + query, UriKind.Relative));
            _regionManager.RequestNavigate(Common.RegionNames.ExportPdaProgramType, new Uri(Common.ViewNames.ExportPdaProgramTypeView + query, UriKind.Relative));

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
                                                        exportAdapter,
														exportPdaAdapter,
                                                        exportSettings,
                                                        programTypeControl
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.ExportBackForward);
			this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaAdapter);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaSettings);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaProgramType);

            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;
        }

        void ViewModel_ModalWindowRequest(object sender, Common.Interfaces.ModalWindowRequestPayload e)
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

		private void btnDownloadFromPDA_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void btnUploadToPDA_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void btnClear_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void btnExport_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
    }
}
