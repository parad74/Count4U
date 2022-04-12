using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Model.Audit;
using Count4U.Model.ExportImport.Items;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.Import;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Modules.Audit.Views.Import
{
    /// <summary>
	/// форма Import  Complex Adapter с комбобоксом 
    /// </summary>
	public partial class ComplexOperationView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly ComplexOperationViewModel _viewModel;
        private readonly ModalWindowLauncher _modalWindowLauncher;
		private readonly IServiceLocator _serviceLocator;
		private string _adapterName;

		public ComplexOperationView(
            ComplexOperationViewModel viewModel,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository,
            ModalWindowLauncher modalWindowLauncher,
			IServiceLocator serviceLocator)
        {
            InitializeComponent();

            this._modalWindowLauncher = modalWindowLauncher;
            this._viewModel = viewModel;
            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
			this._serviceLocator = serviceLocator;

            this.DataContext = _viewModel;
			this.Loaded += ComplexOperationView_Loaded;
			 
        }

		void ComplexOperationView_Loaded(object sender, RoutedEventArgs e)
		{
			//ImportWithModulesViewModel viewModel = this.DataContext as ImportWithModulesViewModel;
			//if (viewModel != null)
			//{
				//if (viewModel.Mode == ImportDomainEnum.ImportBranch)
				//{
				//	tbEncoding.Margin = new Thickness(556, 2, 0, 0);
				//	cmbEncoding.Margin = new Thickness(556, 19, 0, 0);
				//	chkInvertLetters.Margin = new Thickness(556, 48, 0, 0);
				//	chkInvertWords.Margin = new Thickness(656, 48, 0, 0);
				//}
			//}
		}

        public bool KeepAlive { get { return false; } }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
			//RegionManager.SetRegionName(backForward, Common.RegionNames.ImportComplexForward);
			//RegionManager.SetRegionName(backBranchForward, Common.RegionNames.ImportBranchComplexForward);
			//RegionManager.SetRegionName(backCustomerForward, Common.RegionNames.ImportCustomerComplexForward);

			//backForward.Visibility = Visibility.Hidden;
			//backBranchForward.Visibility = Visibility.Hidden;
			//backCustomerForward.Visibility = Visibility.Hidden;
           // RegionManager.SetRegionName(contentModule, Common.RegionNames.ImportByModule);
           // RegionManager.SetRegionName(extraSettings, Common.RegionNames.ExportPdaExtraSettings);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);

            //UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranchInventor);

			string cbiDbContext = Utils.CBIDbContextFromNavigationParameters(navigationContext);
			switch (cbiDbContext)
			{
				case Common.NavigationSettings.CBIDbContextCustomer:
					{
					//	UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeEmpty);
	   
						UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomer);
						RegionManager.SetRegionName(backForward, Common.RegionNames.ImportComplexForward);
						RegionManager.SetRegionName(contentModule, Common.RegionNames.ImportByModule);
						UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ImportComplexForward);
					//	backCustomerForward.Visibility = Visibility.Visible;
					}
					break;
				case Common.NavigationSettings.CBIDbContextBranch:
					{
						UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranch);
						RegionManager.SetRegionName(backForward, Common.RegionNames.ImportComplexForward);
						RegionManager.SetRegionName(contentModule, Common.RegionNames.ImportByModule);
						UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ImportComplexForward);
						//backBranchForward.Visibility = Visibility.Visible;
					}
					break;
				case Common.NavigationSettings.CBIDbContextInventor:
					{
						UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranchInventor);
						RegionManager.SetRegionName(backForward, Common.RegionNames.ImportComplexForward);
						RegionManager.SetRegionName(contentModule, Common.RegionNames.ImportByModule);
						UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ImportComplexForward);
						//backForward.Visibility = Visibility.Visible;
					}
					break;
			}

			//UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ImportComplexForward);
       //    this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaExtraSettings, new Uri(Common.ViewNames.ExportPdaExtraSettingsView + query, UriKind.Relative));

		
            this._viewModel.ModalWindowRequest += ViewModel_ModalWindowRequest;

			Utils.MainWindowTitleSet(WindowTitles.Complex, this._eventAggregator);


			//#if DEBUG
			//			deleteInv.Visibility = System.Windows.Visibility.Visible;
			//#else
			//			deleteInv.Visibility = System.Windows.Visibility.Hidden; 
			//#endif
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
														//backBranchForward,
														//backCustomerForward,
                                                        contentModule,
                                                        //,extraSettings
                                                  }, navigationContext);

			if (this._regionManager.Regions.ContainsRegionWithName(Common.RegionNames.ImportByModule) == true)
			{
				try { this._regionManager.Regions.Remove(Common.RegionNames.ImportByModule); }
				catch { }
			}
			if (this._regionManager.Regions.ContainsRegionWithName(Common.RegionNames.ImportComplexForward) == true)
			{
				try { this._regionManager.Regions.Remove(Common.RegionNames.ImportComplexForward);}
				catch { }
			}
			//if (this._regionManager.Regions.ContainsRegionWithName(Common.RegionNames.ImportBranchComplexForward) == true)
			//{
			//	try { this._regionManager.Regions.Remove(Common.RegionNames.ImportBranchComplexForward);}
			//	catch { }
			//}
			//if (this._regionManager.Regions.ContainsRegionWithName(Common.RegionNames.ImportCustomerComplexForward) == true)
			//{
			//	try { this._regionManager.Regions.Remove(Common.RegionNames.ImportCustomerComplexForward); }
			//	catch { }
			//}
           // this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaExtraSettings);

            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;
        }

		private string GetSelectedAdapterName(string inventorCode)
		{
			//switch (context)
			//{
				//case Common.NavigationSettings.CBIDbContextCustomer:
				//	return CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportPDAProviderCode;
				//case Common.NavigationSettings.CBIDbContextBranch:
				//	return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportPDAProviderCode;
				//case Common.NavigationSettings.CBIDbContextInventor:
				//IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
				//Inventor inventor = inventorRepository.GetInventorByCode(inventorCode);
				//return inventor == null ? String.Empty : inventor.ImportPDAProviderCode;
			//}
			return String.Empty;
		}

        void ViewModel_ModalWindowRequest(object sender, Common.Interfaces.ModalWindowRequestPayload e)
        {
			//e.ViewName	  = Common.ViewNames.LogView
			object result = this._modalWindowLauncher.StartModalWindow(e.ViewName, e.WindowTitle, 540, 500, ResizeMode.CanResize, e.Settings, Window.GetWindow(this));
            if (e.Callback != null)
                e.Callback(result);
        }


		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ComplexOperationViewModel viewModel = this.DataContext as ComplexOperationViewModel;
			if (viewModel == null) return;

			DependencyObject depObj = e.OriginalSource as DependencyObject;
			if (depObj == null) return;
			DataGridRow row = VisualTreeHelpers.FindParent<DataGridRow>(depObj);

			if (row != null)
			{
				FileItemViewModel inventorFile = row.DataContext as FileItemViewModel;
				if (inventorFile != null)
				{
					#if DEBUG
							viewModel.AuditNavigateCommandExecuted(inventorFile);
					#else
					
					#endif
					
				}
			}

		}

    }
}
