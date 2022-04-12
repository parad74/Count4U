using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Common.View.DragDrop;
using Count4U.Model.Interface;
using Count4U.Modules.ContextCBI.ViewModels.Zip;
using Count4U.Modules.ContextCBI.Views;
using Count4U.Modules.ContextCBI.Views.Branch;
using Count4U.Modules.ContextCBI.Views.Customer;
using Count4U.Modules.ContextCBI.Views.DashboardItems;
using Count4U.Modules.ContextCBI.Views.DashboardItems.Branch;
using Count4U.Modules.ContextCBI.Views.DashboardItems.DomainObject;
using Count4U.Modules.ContextCBI.Views.DashboardItems.InventProductMdi;
using Count4U.Modules.ContextCBI.Views.DashboardItems.PlanogramMdi;
using Count4U.Modules.ContextCBI.Views.DashboardItems.Section;
using Count4U.Modules.ContextCBI.Views.DashboardItems.SectionMdi;
using Count4U.Modules.ContextCBI.Views.DashboardItems.StatisticMdi;
using Count4U.Modules.ContextCBI.Views.Inventor;
using Count4U.Modules.ContextCBI.Views.Misc;
using Count4U.Modules.ContextCBI.Views.Misc.Adapters;
using Count4U.Modules.ContextCBI.Views.Misc.CBITab;
using Count4U.Modules.ContextCBI.Views.Misc.Popup;
using Count4U.Modules.ContextCBI.Views.Misc.Popup.Search;
using Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.BranchControl;
using Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.CustomerControl;
using Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.InventorControl;
using Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.PackControl;
using Count4U.Modules.ContextCBI.Views.Misc.Popup.SpeedLink;
using Count4U.Modules.ContextCBI.Views.Misc.Script;
using Count4U.Modules.ContextCBI.Views.Pack;
using Count4U.Modules.ContextCBI.Views.ParsingMask;
using Count4U.Modules.ContextCBI.Views.ParsingMask.Script;
using Count4U.Modules.ContextCBI.Views.Report;
using Count4U.Modules.ContextCBI.Views.Settings;
using Count4U.Modules.ContextCBI.Views.Strip;
using Count4U.Modules.ContextCBI.Views.Zip;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Modules.ContextCBI.Controllers;
using Count4U.Model.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U;
using Count4U.Model;
using NLog;
using Count4U.Modules.ContextCBI.Views.DashboardItems.FamilyMdi;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Common.Web;

namespace Count4U.Modules.ContextCBI
{
  
	public class ContextCBIModuleInit : IModule
	{
		#region IModule Members
        private readonly IUnityContainer _container;
		private readonly IServiceLocator _serviceLocator;
        private readonly IRegionManager _regionManager;
        private ContextCBIController _contextCbiController;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ContextCBIModuleInit(IUnityContainer container, 
			IServiceLocator serviceLocator, 
			IRegionManager regionManager)
		{
			this._container = container;
			this._regionManager = regionManager;
			this._serviceLocator = serviceLocator;
		}

        public void Initialize()
        {
            _logger.Info("Initialize ContextCBIModule");

            try
            {
                DropTargetAdorners dropTargetAdorners = new DropTargetAdorners(); //fake invoke to mark drag dll as used

                this._container.RegisterType(typeof(ZipExclusionRules), typeof(ZipExclusionRules), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ProcessExclusionRules), typeof(ProcessExclusionRules), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(FtpFolderProFile), typeof(FtpFolderProFile), new ContainerControlledLifetimeManager());
				this._container.RegisterType<ToFtpViewModel>(new TransientLifetimeManager());
				this._container.RegisterType<FromFtpViewModel>(new TransientLifetimeManager());
				this._container.RegisterType<object, FromFtpView>(Common.ViewNames.FromFtpView);
				this._container.RegisterType<object, ToFtpView>(Common.ViewNames.ToFtpView);


                this._container.RegisterType<object, StripView>(Common.ViewNames.StripView);

                this._container.RegisterType<object, CustomerChooseView>(Common.ViewNames.CustomerChooseView);
                this._container.RegisterType<object, BranchChooseView>(Common.ViewNames.BranchChooseView);
                this._container.RegisterType<object, InventorChooseView>(Common.ViewNames.InventorChooseView);                

                this._container.RegisterType<object, CustomerAddView>(Common.ViewNames.CustomerAddView);
                this._container.RegisterType<object, BranchAddView>(Common.ViewNames.BranchAddView);
                this._container.RegisterType<object, InventorAddView>(Common.ViewNames.InventorAddView);
                this._container.RegisterType<object, CustomerGroundView>(Common.ViewNames.CustomerGroundView);
                this._container.RegisterType<object, CustomerPostView>(Common.ViewNames.CustomerPostView);
                this._container.RegisterType<object, BranchPostView>(Common.ViewNames.BranchPostView);
                this._container.RegisterType<object, InventorPostView>(Common.ViewNames.InventorPostView); 

                this._container.RegisterType<object, CustomerDashboardFullView>(Common.ViewNames.CustomerDashboardFullView);
                this._container.RegisterType<object, BranchDashboardFullView>(Common.ViewNames.BranchDashboardFullView);
                this._container.RegisterType<object, InventorDashboardFullView>(Common.ViewNames.InventorDashboardFullView);

                this._container.RegisterType<object, CustomerEditView>(Common.ViewNames.CustomerEditView);
                this._container.RegisterType<object, BranchEditView>(Common.ViewNames.BranchEditView);
                this._container.RegisterType<object, InventorEditView>(Common.ViewNames.InventorEditView);
				this._container.RegisterType<object, InventorEditOptionsView>(Common.ViewNames.InventorEditOptionsView);
				this._container.RegisterType<object, CustomerEditOptionsView>(Common.ViewNames.CustomerEditOptionsView);
				

                this._container.RegisterType<object, MenuDashboardPartView>(Common.ViewNames.MenuDashboardPartView);
                this._container.RegisterType<object, CatalogInfoForCBIDashboardPartView>(Common.ViewNames.CatalogInfoForCBIDashboardPartView);
                this._container.RegisterType<object, ReportsDashboardPartView>(Common.ViewNames.ReportsDashboardPartView);
                this._container.RegisterType<object, LastBranchesDashboardPartView>(Common.ViewNames.LastBranchesDashboardPartView);
                this._container.RegisterType<object, BranchesDashboardPartView>(Common.ViewNames.BranchesDashboardPartView);
                this._container.RegisterType<object, LastInventorsDashboardPartView>(Common.ViewNames.LastInventorsDashboardPartView);

                this._container.RegisterType<object, LocationDashboardPartView>(Common.ViewNames.LocationDashboardPartView);
                this._container.RegisterType<object, PdaDashboardPartView>(Common.ViewNames.PdaDashboardPartView);
                this._container.RegisterType<object, FromPdaDashboardPartView>(Common.ViewNames.FromPdaDashboardPartView);
                this._container.RegisterType<object, InventorStatusDashboardPartView>(Common.ViewNames.InventorStatusDashboardPartView);
                this._container.RegisterType<object, IturimDashboardPartView>(Common.ViewNames.IturimDashboardPartView);
                this._container.RegisterType<object, LastCustomersDashboardPartView>(Common.ViewNames.LastCustomersDashboardPartView);                
                this._container.RegisterType<object, SectionDashboardPartView>(Common.ViewNames.SectionDashboardPartView);
                this._container.RegisterType<object, SupplierDashboardPartView>(Common.ViewNames.SupplierDashboardPartView);
				this._container.RegisterType<object, FamilyDashboardPartView>(Common.ViewNames.FamilyDashboardPartView);
                this._container.RegisterType<object, InventProductPartView>(Common.ViewNames.InventProductPartView);
                this._container.RegisterType<object, CustomerDashboardPartView>(Common.ViewNames.CustomerDashboardPartView);
                this._container.RegisterType<object, BranchDashboardPartView>(Common.ViewNames.BranchDashboardPartView);
                this._container.RegisterType<object, InventorDashboardPartView>(Common.ViewNames.InventorDashboardPartView);
                this._container.RegisterType<object, HomeDashboardPartView>(Common.ViewNames.HomeDashboardPartView);
                this._container.RegisterType<object, InventProductSimplePartView>(Common.ViewNames.InventProductSimplePartView);
                this._container.RegisterType<object, InventProductSumPartView>(Common.ViewNames.InventProductSumPartView);
                this._container.RegisterType<object, PlanogramPartView>(Common.ViewNames.PlanogramPartView);
                this._container.RegisterType<object, StatisticDashboardPartView>(Common.ViewNames.StatisticDashboardPartView);

                this._container.RegisterType<object, HomeDashboardView>(Common.ViewNames.HomeDashboardView);
                
                this._container.RegisterType<object, InventorChangeStatusView>(Common.ViewNames.InventorChangeStatusView);

                this._container.RegisterType<object, UserSettingsView>(Common.ViewNames.UserSettingsView);
                this._container.RegisterType<object, PathSettingsView>(Common.ViewNames.PathSettingsView);
                this._container.RegisterType<object, ConfigurationSetAddView>(Common.ViewNames.ConfigurationSetAddView);              

                this._container.RegisterType<object, BackForwardView>(Common.ViewNames.BackForwardView);

                this._container.RegisterType<object, ZipExportView>(Common.ViewNames.ZipExportView);
                this._container.RegisterType<object, ZipImportView>(Common.ViewNames.ZipImportView);
				this._container.RegisterType<object, ProcessZipView>(Common.ViewNames.ProcessZipView);
				this._container.RegisterType<object, ProcessAddEditGridView>(Common.ViewNames.ProcessAddEditGridView);
				
				

                this._container.RegisterType<object, PackView>(Common.ViewNames.PackView);
                this._container.RegisterType<object, UnpackView>(Common.ViewNames.UnpackView);

                this._container.RegisterType<object, MaskTemplateAddEditView>(Common.ViewNames.MaskTemplateAddEditView);
                this._container.RegisterType<object, MaskListView>(Common.ViewNames.MaskListView);
                this._container.RegisterType<object, MaskAddEditView>(Common.ViewNames.MaskAddEditView);
                this._container.RegisterType<object, MaskSelectView>(Common.ViewNames.MaskSelectView);
                this._container.RegisterType<object, MaskScriptOpenView>(Common.ViewNames.MaskScriptOpenView);
                this._container.RegisterType<object, MaskScriptSaveView>(Common.ViewNames.MaskScriptSaveView);

                this._container.RegisterType<object, ExportPdaSettingsView>(Common.ViewNames.ExportPdaSettingsView);
                this._container.RegisterType<object, ExportErpSettingsView>(Common.ViewNames.ExportErpSettingsView);              
                this._container.RegisterType<object, ImportFoldersView>(Common.ViewNames.ImportFoldersView);
				this._container.RegisterType<object, ConfigAdapterSettingView>(Common.ViewNames.ConfigAdapterSettingView);
				
                this._container.RegisterType<object, DynamicColumnSettingsView>(Common.ViewNames.DynamicColumnSettingsView);
				this._container.RegisterType<object, AdditionalSettingsSettingsView>(Common.ViewNames.AdditionalSettingsSettingsView);
				this._container.RegisterType<object, AutoGenerateResultSettingsView>(Common.ViewNames.AutoGenerateResultSettingsView);

				

                this._container.RegisterType<object, CBIObjectPropertiesView>(Common.ViewNames.CBIObjectPropertiesView);

                this._container.RegisterType<object, AdapterLinkView>(Common.ViewNames.AdapterLinkView);
                this._container.RegisterType<object, AdapterLinkScriptSaveView>(Common.ViewNames.AdapterLinkScriptSaveView);
                this._container.RegisterType<object, AdapterLinkScriptOpenView>(Common.ViewNames.AdapterLinkScriptOpenView);

                this._container.RegisterType<object, CBIScriptSaveView>(Common.ViewNames.CBIScriptSaveView);
                this._container.RegisterType<object, CBIScriptOpenView>(Common.ViewNames.CBIScriptOpenView);

                this._container.RegisterType<object, MdiFilterView>(Common.ViewNames.MdiFilterView);

                this._container.RegisterType<object, SearchView>(Common.ViewNames.SearchView);
                this._container.RegisterType<object, SearchCustomerFieldView>(Common.ViewNames.SearchCustomerFieldView);
                this._container.RegisterType<object, SearchBranchFieldView>(Common.ViewNames.SearchBranchFieldView);
                this._container.RegisterType<object, SearchInventorFieldView>(Common.ViewNames.SearchInventorFieldView);
                this._container.RegisterType<object, SearchCustomerView>(Common.ViewNames.SearchCustomerView);
                this._container.RegisterType<object, SearchBranchView>(Common.ViewNames.SearchBranchView);
                this._container.RegisterType<object, SearchInventorView>(Common.ViewNames.SearchInventorView);
                this._container.RegisterType<object, SearchPackFieldView>(Common.ViewNames.SearchPackFieldView);

                this._container.RegisterType<object, SpeedLinkView>(Common.ViewNames.SpeedLinkView);

                this._container.RegisterType<object, UpdateAdaptersView>(Common.ViewNames.UpdateAdaptersView);
				this._container.RegisterType<object, InventorChangeStatusViewModel>(Common.ViewModelNames.InventorChangeStatusViewModel);


				//ImportModuleInfo complexOperationSetting = new ImportModuleInfo();
				//complexOperationSetting.Name = Common.Constants.ComplexAdapterName.ComplexOperationSettingAdapter;
				//complexOperationSetting.Title = Common.Constants.CompleхAdapterTitle.ComplexOperationSettingAdapter;
				//complexOperationSetting.UserControlType = typeof(ComplexOperationSettingAdapterView);
				//complexOperationSetting.ImportDomainEnum = ImportDomainEnum.ComplexOperation;
				//complexOperationSetting.IsDefault = false;
				//complexOperationSetting.Description = "";
				//this._container.RegisterInstance(typeof(IImportModuleInfo), complexOperationSetting.Name, complexOperationSetting, new ContainerControlledLifetimeManager());

                
                //controllers
                this._contextCbiController = this._container.Resolve<ContextCBIController>();

                _logger.Info("Initialize ContextCBIModule OK");
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Initialize AuditModule failed", exc);
            }
        }

		#endregion
	}

}
