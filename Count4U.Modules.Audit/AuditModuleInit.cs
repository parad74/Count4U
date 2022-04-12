using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Modules.Audit.Controllers;
using Count4U.Modules.Audit.ViewModels;
using Count4U.Modules.Audit.Views;
using Count4U.Modules.Audit.Views.Catalog;
using Count4U.Modules.Audit.Views.DocumentHeader;
using Count4U.Modules.Audit.Views.ErpExpected;
using Count4U.Modules.Audit.Views.Export;
using Count4U.Modules.Audit.Views.Import;
using Count4U.Modules.Audit.Views.InventProduct;
using Count4U.Modules.Audit.Views.Itur;
using Count4U.Modules.Audit.Views.Misc;
using Count4U.Modules.Audit.Views.Misc.Popup.Search;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.InventProductControl;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.InventProductControl.Advanced;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.InventProductControl.Advanced.Grid;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.IturControl;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.LocationControl;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.ProductControl;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.SectionControl;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.SupplierControl;
using Count4U.Modules.Audit.Views.Section;
using Count4U.Modules.Audit.Views.Supplier;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
//using Count4U.Modules.Audit.Controllers;
using Count4U.Model.Audit;
using Count4U.Model.Main;
//using Count4U.Modules.Audit.ViewModels;
using Microsoft.Practices.Prism.Modularity;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model;
using NLog;
using Count4U.Modules.Audit.Views.Location;
using Count4U.Modules.Audit.Views.Misc.Popup.Search.FamilyControl;
using Count4U.Modules.Audit.Views.Family;
using Count4U.Modules.Audit.Views.Device;
using Count4U.Modules.Audit.Views.Tag;
using Count4U.Modules.Audit.ViewModels.Import;
using Count4U.Modules.Audit.ViewModels.Export;
using Count4U.Modules.ContextCBI.View;

namespace Count4U.Modules.Audit
{

    public class AuditModuleInit : IModule
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region IModule Members
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private AuditController _auditController;
        // private ContextCBIController this._contextCBIConroller;

        public AuditModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
			this._container = container;
			this._regionManager = regionManager;
        }

        public void Initialize()
        {
            _logger.Info("Initialize AuditModule");

            try
            {                              
                this._container.RegisterType<object, IturListDetailsView>(Common.ViewNames.IturListDetailsView);
                this._container.RegisterType<object, ReportsView>(Common.ViewNames.ReportsView);
                this._container.RegisterType<object, LocationAddEditView>(Common.ViewNames.LocationAddEditView);
				this._container.RegisterType<object, ProcessAddEditView>(Common.ViewNames.ProcessAddEditView);
				this._container.RegisterType<object, LocationMultiAddView>(Common.ViewNames.LocationMultiAddView);
                this._container.RegisterType<object, IturStatusChangeView>(Common.ViewNames.IturStatusChangeView);
                this._container.RegisterType<object, InventProductListDetailsView>(Common.ViewNames.InventProductListDetailsView);
                this._container.RegisterType<object, InventProductAddEditView>(Common.ViewNames.InventProductAddEditView);
                this._container.RegisterType<object, InventProductCloneView>(Common.ViewNames.InventProductCloneView);
                this._container.RegisterType<object, InventProductView>(Common.ViewNames.InventProductView);
                this._container.RegisterType<object, InventProductListSimpleView>(Common.ViewNames.InventProductListSimpleView);
                this._container.RegisterType<object, InventProductListSumView>(Common.ViewNames.InventProductListSumView);
				this._container.RegisterType<object, DocumentHeaderCloneView>(Common.ViewNames.DocumentHeaderCloneView);
				
                this._container.RegisterType<object, IturimAddEditDeleteView>(Common.ViewNames.IturimAddEditDeleteView);
                this._container.RegisterType<object, IturLocationChangeView>(Common.ViewNames.IturLocationChangeView);
				this._container.RegisterType<object, LocationTagChangeView>(Common.ViewNames.LocationTagChangeView);
				this._container.RegisterType<object, SectionTagChangeView>(Common.ViewNames.SectionTagChangeView);
				
				this._container.RegisterType<object, IturTagChangeView>(Common.ViewNames.IturTagChangeView);
				this._container.RegisterType<object, IturPrefixChangeView>(Common.ViewNames.IturPrefixChangeView);
				this._container.RegisterType<object, ShowShelfView>(Common.ViewNames.ShowShelfView);
				this._container.RegisterType<object, IturNameChangeView>(Common.ViewNames.IturNameChangeView);
                this._container.RegisterType<object, IturAddView>(Common.ViewNames.IturAddView);
				this._container.RegisterType<object, IturDeleteView>(Common.ViewNames.IturDeleteView);
                this._container.RegisterType<object, IturEditView>(Common.ViewNames.IturEditView);
                this._container.RegisterType<object, IturStateChangeView>(Common.ViewNames.IturStateChangeView);
                this._container.RegisterType<object, IturSelectView>(Common.ViewNames.IturSelectView);
				this._container.RegisterType<object, IturSelectDissableView>(Common.ViewNames.IturSelectDissableView);
				
				this._container.RegisterType<object, LocationCodeSelectView>(Common.ViewNames.LocationCodeSelectView);
				this._container.RegisterType<object, TagSelectView>(Common.ViewNames.TagSelectView);       
                
                this._container.RegisterType<object, CatalogFormView>(Common.ViewNames.CatalogFormView);

				this._container.RegisterType<object, DeviceFormView>(Common.ViewNames.DeviceFormView);
                this._container.RegisterType<object, DeviceWorkerFormView>(Common.ViewNames.DeviceWorkerFormView);
                

                this._container.RegisterType<object, ImportFromPdaView>(Common.ViewNames.ImportFromPdaView);
                this._container.RegisterType<object, ErpExpectedStep1View>(Common.ViewNames.ErpExpectedStep1View);
                this._container.RegisterType<object, ImportWithModulesView>(Common.ViewNames.ImportWithModulesView);
				this._container.RegisterType<object, ComplexOperationView>(Common.ViewNames.ComplexOperationView);


				

                this._container.RegisterType<object, LocationAddEditDeleteView>(Common.ViewNames.LocationAddEditDeleteView);
				this._container.RegisterType<object, LocationMultiAddView>(Common.ViewNames.LocationMultiAddView);
                this._container.RegisterType<object, SectionAddEditDeleteView>(Common.ViewNames.SectionAddEditDeleteView);
                this._container.RegisterType<object, SectionAddEditView>(Common.ViewNames.SectionAddEditView);

                this._container.RegisterType<object, SupplierAddEditDeleteView>(Common.ViewNames.SupplierAddEditDeleteView);
                this._container.RegisterType<object, SupplierAddEditView>(Common.ViewNames.SupplierAddEditView);

				this._container.RegisterType<object, FamilyAddEditDeleteView>(Common.ViewNames.FamilyAddEditDeleteView);
				this._container.RegisterType<object, FamilyAddEditView>(Common.ViewNames.FamilyAddEditView);

                this._container.RegisterType<object, ProductAddEditView>(Common.ViewNames.ProductAddEditView);
                this._container.RegisterType<object, DocumentHeaderAddEditView>(Common.ViewNames.DocumentHeaderAddEditView);

				this._container.RegisterType<object, DeviceAddEditView>(Common.ViewNames.DeviceAddEditView);

                this._container.RegisterType<object, ExportPdaWithModulesView>(Common.ViewNames.ExportPdaWithModulesView);
                this._container.RegisterType<object, ExportErpWithModulesView>(Common.ViewNames.ExportErpWithModulesView);                

                this._container.RegisterType<object, LogView>(Common.ViewNames.LogView);
                this._container.RegisterType<object, ExportLogView>(Common.ViewNames.ExportLogView);
				this._container.RegisterType<object, ConfigEditAndSaveView>(Common.ViewNames.ConfigEditAndSaveView);
				

                this._container.RegisterType<object, SearchIturView>(Common.ViewNames.SearchIturView);
                this._container.RegisterType<object, SearchIturAdvancedView>(Common.ViewNames.SearchIturAdvancedView);
                this._container.RegisterType<object, SearchIturFieldView>(Common.ViewNames.SearchIturFieldView);
                this._container.RegisterType<object, SearchIturAdvancedFieldView>(Common.ViewNames.SearchIturAdvancedFieldView);
                this._container.RegisterType<object, SearchLocationView>(Common.ViewNames.SearchLocationView);
                this._container.RegisterType<object, SearchLocationFieldView>(Common.ViewNames.SearchLocationFieldView);
                this._container.RegisterType<object, SearchSectionView>(Common.ViewNames.SearchSectionView);
                this._container.RegisterType<object, SearchSectionFieldView>(Common.ViewNames.SearchSectionFieldView);
                this._container.RegisterType<object, SearchSupplierView>(Common.ViewNames.SearchSupplierView);
				this._container.RegisterType<object, SearchSupplierFieldView>(Common.ViewNames.SearchSupplierFieldView);
				this._container.RegisterType<object, SearchFamilyView>(Common.ViewNames.SearchFamilyView);
				this._container.RegisterType<object, SearchFamilyFieldView>(Common.ViewNames.SearchFamilyFieldView);

                this._container.RegisterType<object, SearchInventProductFieldView>(Common.ViewNames.SearchInventProductFieldView);
                this._container.RegisterType<object, SearchInventProductView>(Common.ViewNames.SearchInventProductView);
                this._container.RegisterType<object, SearchInventProductAdvancedView>(Common.ViewNames.SearchInventProductAdvancedView);
				this._container.RegisterType<object, SearchInventProductAdvancedAggregateView>(Common.ViewNames.SearchInventProductAdvancedAggregateView);
				
                this._container.RegisterType<object, SearchInventProductAdvancedFieldView>(Common.ViewNames.SearchInventProductAdvancedFieldView);
				this._container.RegisterType<object, SearchInventProductAdvancedAggregateFieldView>(Common.ViewNames.SearchInventProductAdvancedAggregateFieldView);
                this._container.RegisterType<object, SearchInventProductAdvancedFieldSumView>(Common.ViewNames.SearchInventProductAdvancedFieldSumView);
                this._container.RegisterType<object, SearchInventProductAdvancedFieldSimpleView>(Common.ViewNames.SearchInventProductAdvancedFieldSimpleView);
                this._container.RegisterType<object, SearchInventProductAdvancedGridSimpleView>(Common.ViewNames.SearchInventProductAdvancedGridSimpleView);
                this._container.RegisterType<object, SearchInventProductAdvancedGridSumView>(Common.ViewNames.SearchInventProductAdvancedGridSumView);
                this._container.RegisterType<object, SearchProductFieldView>(Common.ViewNames.SearchProductFieldView);
                this._container.RegisterType<object, SearchProductView>(Common.ViewNames.SearchProductView);

                this._container.RegisterType<object, UploadToPdaView>(Common.ViewNames.UploadToPdaView);
				this._container.RegisterType<object, DownloadFromPDAView>(Common.ViewNames.DownloadFromPDAView);
                this._container.RegisterType<object, LocationItursChangeView>(Common.ViewNames.LocationItursChangeView);
                
                //this._container.RegisterType<object, FromFtpView>(Common.ViewNames.FromFtpView);
                //this._container.RegisterType<object, ToFtpView>(Common.ViewNames.ToFtpView);

                //this._container.RegisterType<ToFtpViewModel>(new TransientLifetimeManager());
                //this._container.RegisterType<FromFtpViewModel>(new TransientLifetimeManager());
                //FromFtpViewModel fromFtpViewModel = serviceLocator.GetInstance<FromFtpViewModel>();
                //this._container.RegisterType(typeof(ToFtpViewModel), typeof(ToFtpViewModel),Common.ViewNames.ToFtpViewModel);
                //this._container.RegisterType(typeof(FromFtpViewModel), typeof(FromFtpViewModel), Common.ViewNames.FromFtpViewModel);



                this._container.RegisterType<AuditController>(new ContainerControlledLifetimeManager());

                this._container.RegisterType<InventProductViewModel>(new TransientLifetimeManager());

				this._container.RegisterType<ImportFromPdaViewModel>(new TransientLifetimeManager());


                //controllers
                this._auditController = this._container.Resolve<AuditController>();

            }
            catch (Exception exc)
            {
                _logger.DebugException("Initialize AuditModule error", exc);
            }
            _logger.Info("Initialize AuditModule OK");
        }

        #endregion
    }
}
