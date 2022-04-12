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
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Modules.Audit.ViewModels.Import;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Modules.Audit.Views.Import
{
    /// <summary>
    /// форма Import from PDA Adapter с комбобоксом 
    /// </summary>
    public partial class ImportFromPdaView : UserControl, INavigationAware,IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly ImportFromPdaViewModel _viewModel;
        private readonly ModalWindowLauncher _modalWindowLauncher;
		private readonly IServiceLocator _serviceLocator;
		private string _adapterName;
		private int _height ;

        public ImportFromPdaView(
            ImportFromPdaViewModel viewModel,
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
			this._height = 158;

        }

        public bool KeepAlive { get { return false; } }

		public int AdapterHeight
		{
			get { return this._height; }
			set { 		_height = value; }
		}

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
			string inventorCode = "";
			string branchCode = "";
			string customerCode = "";

            RegionManager.SetRegionName(backForward, Common.RegionNames.ImportPdaBackForward);
            RegionManager.SetRegionName(contentModule, Common.RegionNames.ImportByModule);
            RegionManager.SetRegionName(extraSettings, Common.RegionNames.ExportPdaExtraSettings);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

			string dbContext = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.CBIDbContext).Value;
			//payload.DbContext = Common.NavigationSettings.CBIDbContextCustomer;
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);

			//if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AuditConfigInventor))
			//{
			//	//inventorCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AuditConfigInventor).Value;
			//	UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranchInventor);
			//}

			//else if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AuditConfigBranch))
			//{
				
			//	//branchCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AuditConfigBranch).Value;
			//	UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranch);
			//}

			//else if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AuditConfigCustomer))
			//{
			//	//customerCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AuditConfigCustomer).Value;
			//	UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomer);
			//}

         
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.ImportPdaBackForward);
			UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
			
           this._regionManager.RequestNavigate(Common.RegionNames.ExportPdaExtraSettings, new Uri(Common.ViewNames.ExportPdaExtraSettingsView + query, UriKind.Relative));
		   //extraSettings.Visibility = System.Windows.Visibility.Hidden;
			

		   //if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AdapterName))
		   //{
		   //   this._adapterName = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AdapterName).Value;
		   //}

		   //this._adapterName = GetSelectedAdapterName(customerCode, branchCode, inventorCode);
		   this._adapterName = "";
			
		   if (	  this._adapterName == ImportAdapterName.ImportPdaDB3Adapter
				//|| this._adapterName == ImportAdapterName.ImportPdaMISAdapter
				//|| this._adapterName == ImportAdapterName.ImportPdaMISAndDefaultAdapter
			    || this._adapterName == ImportAdapterName.ImportPdaMerkavaDB3Adapter
				|| this._adapterName == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaClalitSqliteAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaNativSqliteAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaYesXlsxAdapter
			   || this._adapterName == ImportAdapterName.ImportPdaCount4UdbSdfAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaAddCount4UdbSdfAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaMergeCount4UdbSdfAdapter
			   || this._adapterName == ImportAdapterName.ImportPdaCompareCount4UdbSdfAdapter
			   || this._adapterName == ImportAdapterName.ImportPdaContinueAfterCompareCount4UdbSdfAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaAddSumCount4UdbSdfAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaCloneCount4UdbSdfAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaMinusByMakatCount4UdbSdfAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaUpdate2SumByIturBarcodeCount4UdbSdfAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaUpdate2SumByIturDocMakatCount4UdbSdfAdapter
			   || this._adapterName == ImportAdapterName.ImportPdaMerkavaUpdateDbAdapter
			   || this._adapterName == ImportAdapterName.ImportPdaUpdateBarcodeDbAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaMultiCsvAdapter
				|| this._adapterName == ImportAdapterName.ImportPdaMISSqliteAdapter
			   )
		   {
			   this._viewModel.IsExtraSettingsVisibility = false;
				AdapterHeight = 363;
			   // extraSettings.Visibility = System.Windows.Visibility.Hidden;
		   }
		   else
		   {
			   this._viewModel.IsExtraSettingsVisibility = true;
				AdapterHeight = 158;
		   }

            this._viewModel.ModalWindowRequest += ViewModel_ModalWindowRequest;

            Utils.MainWindowTitleSet(WindowTitles.ImportFromPda, this._eventAggregator);
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
                                                        contentModule,
                                                        extraSettings
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.ImportByModule);
            this._regionManager.Regions.Remove(Common.RegionNames.ImportPdaBackForward);
            this._regionManager.Regions.Remove(Common.RegionNames.ExportPdaExtraSettings);

            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;
        }

		private string GetSelectedAdapterName(string customerCode, string branchCode, string inventorCode)
		{
			//switch (context)
			//{
				//case Common.NavigationSettings.CBIDbContextCustomer:
				//	return CurrentCustomer == null ? String.Empty : base.CurrentCustomer.ImportPDAProviderCode;
				//case Common.NavigationSettings.CBIDbContextBranch:
				//	return base.CurrentBranch == null ? String.Empty : base.CurrentBranch.ImportPDAProviderCode;
				//case Common.NavigationSettings.CBIDbContextInventor:
			if (string.IsNullOrWhiteSpace(inventorCode) == false)
			{
				IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
				Inventor inventor = inventorRepository.GetInventorByCode(inventorCode);
				return inventor == null ? String.Empty : inventor.ImportPDAProviderCode;
			}
			else if (string.IsNullOrWhiteSpace(branchCode) == false)
			{
				IBranchRepository branchRepository = this._serviceLocator.GetInstance<IBranchRepository>();
				Branch branch = branchRepository.GetBranchByCode(branchCode);
				return branch == null ? String.Empty : branch.ImportPDAProviderCode;
			}
			else if (string.IsNullOrWhiteSpace(customerCode) == false)
			{
				ICustomerRepository customeRepository = this._serviceLocator.GetInstance<ICustomerRepository>();
				Customer customer = customeRepository.GetCustomerByCode(customerCode);
				return customer == null ? String.Empty : customer.ImportPDAProviderCode;
			}
			
			//}
			return String.Empty;
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

		private void btnGetFromFtp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}


    }
}
