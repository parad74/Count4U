using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Controls;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Events.Misc;
using Count4U.Modules.ContextCBI.Events.ParsingMask;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Unity;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Modules.ContextCBI.Views;
using Count4U.Model.Main;
using Count4U.Model.Interface.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Events;
using Count4U.Modules.ContextCBI.Events;
using NLog;

namespace Count4U.Modules.ContextCBI.Controllers
{
    public class ContextCBIController
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

		private readonly IContextCBIRepository _contextCBIRepository;
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly IDBSettings _dbSettings;

        public ContextCBIController(
                                    IRegionManager regionManager,
                                    IEventAggregator eventAggregator,
                                    IContextCBIRepository contextCBIRepository,
                                    ModalWindowLauncher modalWindowLauncher,
                                    IDBSettings dbSettings
            )
        {
            this._dbSettings = dbSettings;
            this._modalWindowLauncher = modalWindowLauncher;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._contextCBIRepository = contextCBIRepository;

            this._eventAggregator.GetEvent<CustomerAddEvent>().Subscribe(this.CustomerAdd, true);
            this._eventAggregator.GetEvent<CustomerEditEvent>().Subscribe(this.CustomerEdit, true);
            this._eventAggregator.GetEvent<CustomerViewEvent>().Subscribe(this.CustomerView, true);
			this._eventAggregator.GetEvent<CustomerEditSettingsEvent>().Subscribe(this.CustomerEditSettingsEvent, true);
            this._eventAggregator.GetEvent<BranchAddEvent>().Subscribe(this.BranchAdd, true);
            this._eventAggregator.GetEvent<BranchEditEvent>().Subscribe(this.BranchEdit, true);
            this._eventAggregator.GetEvent<BranchViewEvent>().Subscribe(this.BranchView, true);
            this._eventAggregator.GetEvent<InventorAddEvent>().Subscribe(this.InventorAdd, true);
            this._eventAggregator.GetEvent<InventorEditEvent>().Subscribe(this.InventorEditEvent, true);
			this._eventAggregator.GetEvent<InventorEditSettingsEvent>().Subscribe(this.InventorEditSettingsEvent, true);
            this._eventAggregator.GetEvent<InventorViewEvent>().Subscribe(this.InventorView, true);            
            this._eventAggregator.GetEvent<InventorStatusChangeEvent>().Subscribe(InventorChangeStatus, true);
			this._eventAggregator.GetEvent<MaskTemplateAddEditEvent>().Subscribe(MaskTemplateAddEdit, true);
            this._eventAggregator.GetEvent<MaskAddEditEvent>().Subscribe(MaskAddEdit, true);
            this._eventAggregator.GetEvent<ObjectPropertiesViewEvent>().Subscribe(ObjectPropertiesView);
        }     

        private void CustomerAdd(CustomerAddEventPayload customer)
        {
            var args = new Dictionary<string, string>();
            args.Add(Common.NavigationSettings.CBIViewName, Common.ViewNames.CustomerAddView);
            if (customer != null)
                Utils.AddContextToDictionary(args, customer.Context);
            
            this.StartModalWindow(Common.ViewNames.CustomerGroundView, Localization.Resources.Window_Title_NewCustomer, settings: args, resizeMode: ResizeMode.NoResize, width: 750, height: 650);
        }

        private void CustomerEdit(CustomerEditEventPayload customer)
        {
            var args = new Dictionary<string, string>();
            if (customer != null)
                Utils.AddContextToDictionary(args, customer.Context);
            this.StartModalWindow(Common.ViewNames.CustomerEditView, Localization.Resources.Window_Title_EditCustomer, settings: args, resizeMode: ResizeMode.NoResize, width: 750, height: 690);
        }

        private void BranchAdd(BranchAddEventPayload branch)
        {
            var args = new Dictionary<string, string>();
            args.Add(Common.NavigationSettings.CBIViewName, Common.ViewNames.BranchAddView);
            if (branch != null)
            {
                if (branch.IsCustomerComboVisible)
                    args.Add(NavigationSettings.IsCustomerComboVisible, String.Empty);
			
                Utils.AddContextToDictionary(args, branch.Context);
            }
            this.StartModalWindow(Common.ViewNames.CustomerGroundView, Localization.Resources.Window_Title_NewBranch, 750, 640, settings: args, resizeMode: ResizeMode.NoResize);
        }

        private void BranchEdit(BranchEditEventPayload branch)
        {
            var args = new Dictionary<string, string>();
            if (branch != null)
                Utils.AddContextToDictionary(args, branch.Context);
            this.StartModalWindow(Common.ViewNames.BranchEditView, Localization.Resources.Window_Title_EditBranch, 750, 640, settings: args, resizeMode: ResizeMode.NoResize);
        }

        private void BranchView(BranchViewEventPayload payload)
        {
            var args = new Dictionary<string, string>();
            args.Add(NavigationSettings.ViewOnly, String.Empty);
            Utils.AddContextToDictionary(args, payload.Context);
            this.StartModalWindow(Common.ViewNames.BranchEditView, Localization.Resources.Window_Title_ViewBranch, 750, 640, settings: args, resizeMode: ResizeMode.NoResize);
        }

        private void InventorAdd(InventorAddEventPayload inventor)
        {
            var args = new Dictionary<string, string>();
            args.Add(Common.NavigationSettings.CBIViewName, Common.ViewNames.InventorAddView);
            if (inventor != null)
            {
                Utils.AddContextToDictionary(args, inventor.Context);
                if (inventor.IsCustomerComboVisible)
                    args.Add(NavigationSettings.IsCustomerComboVisible, String.Empty);
                if (inventor.IsBranchComboVisible)
                    args.Add(NavigationSettings.IsBranchComboVisible, String.Empty);
				if (inventor.WithoutNavigate)
					args.Add(NavigationSettings.WithoutNavigate, String.Empty);
            }
            this.StartModalWindow(Common.ViewNames.CustomerGroundView, Localization.Resources.Window_Title_NewInventor, 750, 600, settings: args, resizeMode: ResizeMode.NoResize);
        }

        private void InventorEditEvent(InventorEditEventPayload inventor)
        {
            var args = new Dictionary<string, string>();
            if (inventor != null)
                Utils.AddContextToDictionary(args, inventor.Context);
            this.StartModalWindow(Common.ViewNames.InventorEditView, Localization.Resources.Window_Title_EditInventor, 700, 600, settings: args, resizeMode: ResizeMode.NoResize);
        }


		private void InventorEditSettingsEvent(InventorEditSettingsEventPayload inventor)
        {
            var args = new Dictionary<string, string>();
            if (inventor != null)
                Utils.AddContextToDictionary(args, inventor.Context);
			this.StartModalWindow(Common.ViewNames.InventorEditOptionsView, Localization.Resources.Window_Title_EditInventor, 700, 600, settings: args, resizeMode: ResizeMode.NoResize);
        }


		private void CustomerEditSettingsEvent(CustomerEditSettingsEventPayload customer)
        {
            var args = new Dictionary<string, string>();
			if (customer != null)
				Utils.AddContextToDictionary(args, customer.Context);
			this.StartModalWindow(Common.ViewNames.CustomerEditOptionsView, Localization.Resources.Window_Title_CustomerSetting, 1200, 600, settings: args, resizeMode: ResizeMode.NoResize);
        }


		
        private void CustomerView(CustomerViewEventPayload payload)
        {
            var args = new Dictionary<string, string>();            
            args.Add(NavigationSettings.ViewOnly, String.Empty);
            Utils.AddContextToDictionary(args, payload.Context);
            this.StartModalWindow(Common.ViewNames.CustomerEditView, Localization.Resources.Window_Title_ViewCustomer, settings: args, resizeMode: ResizeMode.NoResize, width:750, height: 620);
        }

        private void InventorView(InventorViewEventPayload payload)
        {
            var args = new Dictionary<string, string>();
            args.Add(NavigationSettings.ViewOnly, String.Empty);
            Utils.AddContextToDictionary(args, payload.Context);
            this.StartModalWindow(Common.ViewNames.InventorEditView, Localization.Resources.Window_Title_ViewInventor, 700, 600, settings: args, resizeMode: ResizeMode.NoResize);
        }

        private void InventorChangeStatus(InventorStatusChangeEventPayload payload)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            Utils.AddContextToDictionary(args, payload.Context);
            Utils.AddDbContextToDictionary(args, payload.DbContext);

            StartModalWindow(Common.ViewNames.InventorChangeStatusView, Localization.Resources.Window_Title_InventorStatusChange, 650, 600, ResizeMode.CanResize, args);
        }

        private void MaskAddEdit(MaskAddEditEventPayload payload)
        {
            string title = payload.Mask == null ? Localization.Resources.Window_Title_AddMask : Localization.Resources.Window_Title_EditMask;

            var args = new Dictionary<string, string>();
            if (payload.Mask != null)
            {
                args.Add(NavigationSettings.MaskId, payload.Mask.ID.ToString());
            }        

            Utils.AddContextToDictionary(args, payload.CDBContext);
            Utils.AddDbContextToDictionary(args, payload.CBIDbContext);

            StartModalWindow(Common.ViewNames.MaskAddEditView, title, 370, 200, ResizeMode.NoResize, args);
        }

		private void MaskTemplateAddEdit(MaskTemplate maskTemplate)
        {
            string title = maskTemplate == null ? Localization.Resources.Window_Title_AddMaskTemplate : Localization.Resources.Window_Title_EditMaskTemplate;

            var args = new Dictionary<string, string>();
            if (maskTemplate != null)
            {                
                args.Add(NavigationSettings.MaskTemplateCode, maskTemplate.Code);
            }

		    StartModalWindow(Common.ViewNames.MaskTemplateAddEditView, title, 400, 200, ResizeMode.NoResize, args);
        }

        private void ObjectPropertiesView(ObjectPropertiesViewEventPayload payload)
        {
            var args = new Dictionary<string, string>();
            Utils.AddContextToDictionary(args, payload.Context);
            Utils.AddDbContextToDictionary(args, payload.DbContext);

            StartModalWindow(Common.ViewNames.CBIObjectPropertiesView, WindowTitles.CBIObjectProperties, 375, 300, ResizeMode.CanResize, args);
        }

        private void StartModalWindow(string viewName, string windowTitle, int width = 800, int height = 600, ResizeMode resizeMode = ResizeMode.CanResize, Dictionary<string, string> settings = null)
        {
            this._modalWindowLauncher.StartModalWindow(viewName, windowTitle, width, height, resizeMode, settings);
        }



    }

}


