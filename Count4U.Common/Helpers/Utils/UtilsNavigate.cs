using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Count4U.Common.Events;
using Count4U.Common.State;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;

namespace Count4U.Common.Helpers
{
    public static class UtilsNavigate
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();        

        static  UtilsNavigate()
        {
        }

        public static void IturListDetailsOpen(IRegionManager regionManager, UriQuery uriQuery, Action<NavigationResult> navigationCallback = null, bool gcollect = true)
        {
            using (new CursorWait("IturListDetailsOpen"))
            {
                GlobalState.BACK = false;

                if (navigationCallback != null)
                    regionManager.RequestNavigate(Common.RegionNames.ApplicationWindow, new Uri(ViewNames.IturListDetailsView + uriQuery, UriKind.Relative), navigationCallback);
                else
                    regionManager.RequestNavigate(Common.RegionNames.ApplicationWindow, new Uri(ViewNames.IturListDetailsView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("IturListDetailsOpen");
            }
			if (gcollect == true)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				ScreenNavigationOccured("IturListDetailsOpen after GC.Collect");
			}
		}

        public static void HomeDashboardOpen(CBIContext context, IRegionManager regionManager, UriQuery query, Action<NavigationResult> navigationCallback = null)
        {
            using (new CursorWait("HomeDashboardOpen"))
            {
                Utils.AddContextToQuery(query, context);

                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                if (navigationCallback != null)
                    region.RequestNavigate(new Uri(Common.ViewNames.HomeDashboardView + query, UriKind.Relative), navigationCallback);
                else
                    region.RequestNavigate(new Uri(Common.ViewNames.HomeDashboardView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("HomeDashboardOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("HomeDashboardOpen after GC.Collect");
		}

        public static void CustomerDashboardOpen(CBIContext context, IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("CustomerDashboardOpen"))
            {
                Utils.AddContextToQuery(query, CBIContext.Main);
                Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextCustomer);
                regionManager.RequestNavigate(RegionNames.ApplicationWindow, new Uri(ViewNames.CustomerDashboardFullView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("CustomerDashboardOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("CustomerDashboardOpen after GC.Collect");
		}

        public static void BranchDashboardOpen(CBIContext context, IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("BranchDashboardOpen"))
            {
                Utils.AddContextToQuery(query, CBIContext.Main);
                Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextBranch);
                regionManager.RequestNavigate(RegionNames.ApplicationWindow, new Uri(ViewNames.BranchDashboardFullView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("BranchDashboardOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("BranchDashboardOpen after GC.Collect");
		}


        public static void InventorDashboardOpen(CBIContext context, IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("InventorDashboardOpen"))
            {
                Utils.AddContextToQuery(query, CBIContext.History);
                Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
                regionManager.RequestNavigate(RegionNames.ApplicationWindow, new Uri(ViewNames.InventorDashboardFullView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("InventorDashboardOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("InventorDashboardOpen after GC.Collect");
		}

        public static void CustomerChooseOpen(CBIContext context, IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("CustomerChooseOpen"))
            {
                Utils.AddContextToQuery(query, context); //add current settings

                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.CustomerChooseView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("CustomerChooseOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("CustomerChooseOpen after GC.Collect");
		}

        public static void BranchChooseOpen(CBIContext context, IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("BranchChooseOpen"))
            {
                Utils.AddContextToQuery(query, context); //add current settings
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.BranchChooseView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("BranchChooseOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("BranchChooseOpen after GC.Collect");
		}

        public static void InventorChooseOpen(CBIContext context, IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("InventorChooseOpen"))
            {
                Utils.AddContextToQuery(query, context); //add current settings

                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.InventorChooseView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("InventorChooseOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("InventorChooseOpen after GC.Collect");
		}

        public static void InventProductDetailsOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("InventProductDetailsOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.InventProductListDetailsView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("InventProductDetailsOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("InventProductDetailsOpen after GC.Collect");
		}

        public static void InventProductListOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("InventProductListOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.InventProductView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("InventProductListOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("InventProductListOpen after GC.Collect");
		}

        public static void InventProductListSimpleOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("InventProductListSimpleOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.InventProductListSimpleView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("InventProductListSimpleOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("InventProductListSimpleOpen after GC.Collect");
		}

        public static void InventProductListSumOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("InventProductListSumOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.InventProductListSumView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("InventProductListSumOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("InventProductListSumOpen after GC.Collect");
		}

        public static void ImportFromPdaOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("ImportFromPdaOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.ImportFromPdaView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("ImportFromPdaOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ImportFromPdaOpen after GC.Collect");
		}

        public static void ErpExpectedStep1Open(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("ErpExpectedStep1Open"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.ErpExpectedStep1View + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("ErpExpectedStep1Open");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ErpExpectedStep1Open after GC.Collect");
		}

        public static void ImportWithModulesOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("ImportWithModulesOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.ImportWithModulesView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("ImportWithModulesOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ImportWithModulesOpen after GC.Collect");
		}

		public static void ComplexOperationViewOpen(IRegionManager regionManager, UriQuery uriQuery)
		{
			using (new CursorWait("ComplexOperationViewOpen"))
			{
				Utils.AddContextToQuery(uriQuery, CBIContext.History);
				Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
				IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
				region.RequestNavigate(new Uri(Common.ViewNames.ComplexOperationView + uriQuery, UriKind.Relative));

				NavigateBottomView(regionManager);
				ScreenNavigationOccured("ComplexOperationViewOpen");
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ComplexOperationViewOpen after GC.Collect");
		}

		public static void ComplexOperationBranchViewOpen(IRegionManager regionManager, UriQuery uriQuery)
		{
			using (new CursorWait("ComplexOperationViewOpen"))
			{
				Utils.AddContextToQuery(uriQuery, CBIContext.Main);
				Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextBranch);

				IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
				region.RequestNavigate(new Uri(Common.ViewNames.ComplexOperationView + uriQuery, UriKind.Relative));

				NavigateBottomView(regionManager);
				ScreenNavigationOccured("ComplexOperationViewOpen");
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ComplexOperationViewOpen after GC.Collect");
		}

		//public static void BranchDashboardOpen(CBIContext context, IRegionManager regionManager, UriQuery query)
		//{
		//	using (new CursorWait("BranchDashboardOpen"))
		//	{
		//		Utils.AddContextToQuery(query, CBIContext.Main);
		//		Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextBranch);
		//		regionManager.RequestNavigate(RegionNames.ApplicationWindow, new Uri(ViewNames.BranchDashboardFullView + query, UriKind.Relative));

		//		NavigateBottomView(regionManager);
		//		ScreenNavigationOccured();
		//	}
		//}

        public static void ExportPdaWithModulesOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("ExportPdaWithModulesOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.ExportPdaWithModulesView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("ExportPdaWithModulesOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ExportPdaWithModulesOpen after GC.Collect");
		}

        public static void ExportErpWithModulesOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("ExportErpWithModulesOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.ExportErpWithModulesView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("ExportErpWithModulesOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ExportErpWithModulesOpen after GC.Collect");
		}

        public static void IturimAddEditDeleteOpen(IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("IturimAddEditDeleteOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.IturimAddEditDeleteView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("IturimAddEditDeleteOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("IturimAddEditDeleteOpen after GC.Collect");
		}

        public static void LocationAddEditDeleteOpen(IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("LocationAddEditDeleteOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.LocationAddEditDeleteView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("LocationAddEditDeleteOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("LocationAddEditDeleteOpen after GC.Collect");
		}

        public static void CatalogOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("CatalogOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.CatalogFormView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("CatalogOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("CatalogOpen after GC.Collect");
		}

		public static void DevicePDAOpen(IRegionManager regionManager, UriQuery uriQuery)
		{
			using (new CursorWait("DevicePDAOpen"))
			{
				IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
				region.RequestNavigate(new Uri(Common.ViewNames.DeviceFormView + uriQuery, UriKind.Relative));

				NavigateBottomView(regionManager);
				ScreenNavigationOccured("DevicePDAOpen");
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("DevicePDAOpen after GC.Collect");
		}

        public static void DeviceWorkerPDAOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("DeviceWorkerPDAOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.DeviceWorkerFormView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("DeviceWorkerPDAOpen");
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            ScreenNavigationOccured("DeviceWorkerPDAOpen after GC.Collect");
        }

        public static void MaskListOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("MaskListOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.MaskListView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("MaskListOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("MaskListOpen after GC.Collect");
		}

        public static void ReportTemplateOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("ReportTemplateOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.ReportTemplateView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("ReportTemplateOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ReportTemplateOpen after GC.Collect");
		}

        public static void ReportFavoritesOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("ReportFavoritesOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.ReportFavoritesView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("ReportFavoritesOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("ReportFavoritesOpen after GC.Collect");
		}

        public static void AdapterLinkOpen(IRegionManager regionManager, UriQuery uriQuery)
        {
            using (new CursorWait("AdapterLinkOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.AdapterLinkView + uriQuery, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("AdapterLinkOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("AdapterLinkOpen after GC.Collect");
		}

        public static void SectionAddEditDeleteOpen(IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("SectionAddEditDeleteOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.SectionAddEditDeleteView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("SectionAddEditDeleteOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("SectionAddEditDeleteOpen after GC.Collect");
		}

        public static void SupplierAddEditDeleteOpen(IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("SupplierAddEditDeleteOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.SupplierAddEditDeleteView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("SupplierAddEditDeleteOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("SupplierAddEditDeleteOpen after GC.Collect");
		}

		public static void FamilyAddEditDeleteOpen(IRegionManager regionManager, UriQuery query)
		{
			using (new CursorWait("FamilyAddEditDeleteOpen"))
			{
				IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
				region.RequestNavigate(new Uri(Common.ViewNames.FamilyAddEditDeleteView + query, UriKind.Relative));

				NavigateBottomView(regionManager);
				ScreenNavigationOccured("FamilyAddEditDeleteOpen");
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("FamilyAddEditDeleteOpen after GC.Collect");
		}

        public static void PackOpen(IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("PackOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.PackView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("PackOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("PackOpen after GC.Collect");
		}

		

        public static void UnpackOpen(IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("UnpackOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.UnpackView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("UnpackOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("UnpackOpen after GC.Collect");
		}

   	     public static void PlanogramOpen(IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("PlanogramOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.PlanBasementView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("PlanogramOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("PlanogramOpen after GC.Collect");
		}

        public static void PlanogramAddEditDeleteOpen(IRegionManager regionManager, UriQuery query)
        {
            using (new CursorWait("PlanogramAddEditDeleteOpen"))
            {
                IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
                region.RequestNavigate(new Uri(Common.ViewNames.PlanAddEditDeleteView + query, UriKind.Relative));

                NavigateBottomView(regionManager);
                ScreenNavigationOccured("PlanogramAddEditDeleteOpen");
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			ScreenNavigationOccured("PlanogramAddEditDeleteOpen after GC.Collect");
		}

        //////////////////////////////////////////////////////////////////////////////

        public static void ApplicationStripNavigate(IRegionManager regionManager, UriQuery uriQuery, string mode)
        {
            if (uriQuery == null) return;
            if (String.IsNullOrEmpty(mode)) return;
            uriQuery.Add(Common.NavigationSettings.StripMode, mode);
            regionManager.RequestNavigate(Common.RegionNames.ApplicationStrip, new Uri(ViewNames.StripView + uriQuery, UriKind.Relative));
        }

        public static void ApplicationStripNavigateFromNavigationContext(NavigationContext navigationContext, IRegionManager regionManager)
        {
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            string cbiDbContext = Utils.CBIDbContextFromNavigationParameters(navigationContext);
            switch (cbiDbContext)
            {
                case Common.NavigationSettings.CBIDbContextCustomer:
                    UtilsNavigate.ApplicationStripNavigate(regionManager, query, Common.NavigationSettings.StripModeCustomer);
                    break;
                case Common.NavigationSettings.CBIDbContextBranch:
                    UtilsNavigate.ApplicationStripNavigate(regionManager, query, Common.NavigationSettings.StripModeCustomerBranch);
                    break;
                case Common.NavigationSettings.CBIDbContextInventor:
                    UtilsNavigate.ApplicationStripNavigate(regionManager, query, Common.NavigationSettings.StripModeCustomerBranchInventor);
                    break;
            }
        }

        public static void BackForwardNavigate(IRegionManager regionManager, string regionName)
        {
            regionManager.RequestNavigate(regionName, new Uri(ViewNames.BackForwardView, UriKind.Relative));
        }

        public static void SearchFilterNavigate(IRegionManager regionManager, string regionName)
        {
            regionManager.RequestNavigate(regionName, new Uri(ViewNames.SearchFilterView, UriKind.Relative));
        }

        public static void GoBack(IRegionManager regionManager)
        {
            if (regionManager.Regions[Common.RegionNames.ApplicationWindow].NavigationService.Journal.CanGoBack)
            {
                GlobalState.BACK = true;
                regionManager.Regions[Common.RegionNames.ApplicationWindow].NavigationService.Journal.GoBack();
            }

            NavigateBottomView(regionManager);
        }

        public static void GoForward(IRegionManager regionManager)
        {
            if (regionManager.Regions[Common.RegionNames.ApplicationWindow].NavigationService.Journal.CanGoForward)
                regionManager.Regions[Common.RegionNames.ApplicationWindow].NavigationService.Journal.GoForward();
        }

        public static bool CanGoBack(IRegionManager regionManager)
        {
            return regionManager.Regions[Common.RegionNames.ApplicationWindow].NavigationService.Journal.CanGoBack;
        }

        public static bool CanGoForward(IRegionManager regionManager)
        {
            return regionManager.Regions[Common.RegionNames.ApplicationWindow].NavigationService.Journal.CanGoForward;
        }

        public static bool OpenProcessInventoryCanExecute(IContextCBIRepository contextCbiRepository)
        {
            bool result = Utils.IsThereProcessConfigInHistoryContext(contextCbiRepository);
            return result;
        }

        public static void OpenProcessInventoryExecute(IContextCBIRepository contextCbiRepository, IRegionManager regionManager)
        {
            AuditConfig config = new AuditConfig(contextCbiRepository.GetProcessCBIConfig(CBIContext.History));
            contextCbiRepository.SetCurrentCBIConfig(CBIContext.History, config);

            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, CBIContext.History);
            Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(query, config);

            UtilsNavigate.IturListDetailsOpen(regionManager, query);
        }

        public static bool DataFileMissed(NavigationContext navigationContext, IContextCBIRepository cbiRepository, IDBSettings dbSettings, Window owner, IUserSettingsManager userSettingsManager)
        {
            CBIContext? rawCbiContext = Utils.CBIContextFromNavigationParameters(navigationContext);
            if (rawCbiContext == null)
            {
                _logger.Error("UtilsNavigate.CheckDataFile: cbiContext is null");
                return false;
            }

            CBIContext cbiContext = rawCbiContext.Value;

            string cbiDbContext = Utils.CBIDbContextFromNavigationParameters(navigationContext);

            AuditConfig auditConfig = cbiRepository.GetCurrentCBIConfig(cbiContext);

            if (auditConfig == null)
            {
                _logger.Error("UtilsNavigate.CheckDataFile: auditConfig is null");
                return false;
            }

            switch (cbiDbContext)
            {
                case NavigationSettings.CBIDbContextCustomer:
                    Customer customer = cbiRepository.GetCustomerByCode(auditConfig.CustomerCode);
                    return DataFileMissed(customer, cbiRepository, dbSettings, owner, userSettingsManager);

                case NavigationSettings.CBIDbContextBranch:
                    Branch branch = cbiRepository.GetBranchByCode(auditConfig.BranchCode);
                    return DataFileMissed(branch, cbiRepository, dbSettings, owner, userSettingsManager);
                    break;

				case NavigationSettings.CBIDbContextInventor:
					Inventor inventor = cbiRepository.GetInventorByCode(auditConfig.InventorCode);
					return DataFileMissed(inventor, cbiRepository, dbSettings, owner, userSettingsManager);
					break;
                default:
                    _logger.Error(String.Format("CheckDataFile for unhandled CbiDbContext: {0}", cbiDbContext));
                    break;
            }

            return false;
        }

        public static bool DataFileMissed(object domainObject, IContextCBIRepository cbiRepository, IDBSettings dbSettings, Window owner, IUserSettingsManager userSettingsManager)
        {
            Customer customer = domainObject as Customer;
            if (customer != null)
            {
                if (!cbiRepository.CheckDbPath(customer)) //если отсутствует путь или одна из БД
                {
					string fileName = "";
					if (cbiRepository.CheckCount4UDbPath(customer) == false)
					{
						fileName = fileName + dbSettings.Count4UDBFile;
						
						if (cbiRepository.CheckAnalitic4UDbPath(customer) == false)
						fileName = fileName + "  " + dbSettings.AnalyticDBFile;

						string message = String.Format(Localization.Resources.Msg_Customer_Sdf_File_Missed, fileName);

						UtilsMisc.ShowMessageBox(message, MessageBoxButton.OK, MessageBoxImage.Warning, userSettingsManager);
						_logger.Info(String.Format("UtilsNavigate.CheckDataFile: .sdf file for [{0}] customer is missed. Attempt to create database file", customer.Code));
					}

                    cbiRepository.AddMissedCount4UDbFile(customer);
                    return true;
                }
            }

            Branch branch = domainObject as Branch;
            if (branch != null)
            {
				if (!cbiRepository.CheckDbPath(branch))		  //если отсутствует путь или одна из БД
                {

					string fileName = "";
					if (cbiRepository.CheckCount4UDbPath(branch) == false)	  // нет 	Count4UDb
					{
						fileName = fileName + dbSettings.Count4UDBFile;
						if (cbiRepository.CheckAnalitic4UDbPath(branch) == false) fileName = fileName + "  " + dbSettings.AnalyticDBFile;

						string message = String.Format(Localization.Resources.Msg_Branch_Sdf_File_Missed, fileName);
						MessageBoxResult result = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Warning, userSettingsManager);

						_logger.Info(String.Format("UtilsNavigate.CheckDataFile: .sdf file for [{0}] branch is missed. Attempt to create database file", branch.Code));
						cbiRepository.AddMissedCount4UDbFile(branch, result == MessageBoxResult.Yes);
					}
					else		   // нет AnalitikDb 
					{
						cbiRepository.AddMissedCount4UDbFile(branch, false);
					}
                  
                    return true;
                }
            }


			Inventor inventor = domainObject as Inventor;
			if (inventor != null)
			{
				if (!cbiRepository.CheckDbPath(inventor))		  //если отсутствует путь или одна из БД	Count4U не допустимо ее отсутсвие
				{
					cbiRepository.AddMissedCount4UDbFile(inventor, false);
					return true;
				}
			}
            return false;
        }

        private static void NavigateBottomView(IRegionManager regionManager)
        {
            regionManager.RequestNavigate(Common.RegionNames.ApplicationBottom, new Uri(Common.ViewNames.BottomView, UriKind.Relative));
        }

        private static void ScreenNavigationOccured(string contextName = "")
        {
            GlobalState.BACK = false;

            Process process = Process.GetCurrentProcess();

            _logger.Info("Memory usage in {0} : {1} MB", contextName, (int)process.PrivateMemorySize64 / 1024 / 1024);            
        }
    }
}