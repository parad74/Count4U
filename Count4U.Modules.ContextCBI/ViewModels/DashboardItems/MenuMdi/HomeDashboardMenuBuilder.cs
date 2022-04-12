using System;
using System.Collections.Generic;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.ContextCBI.Events;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Media;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Menu
{
    public class HomeDashboardMenuBuilder
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
		private readonly IContextCBIRepository _contextCBIRepository;
        private readonly UICommandRepository _commandRepository;

        public HomeDashboardMenuBuilder(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCBIRepository,
            UICommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
            this._contextCBIRepository = contextCBIRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
        }

        private AuditConfig GetHistoryAuditConfig()
        {
            return this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
        }

        private AuditConfig GetMainAuditConfig()
        {
            return this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
        }

        private AuditConfig GetCreateAuditConfig()
        {
            return this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
        }

        private MenuDashboardCommandViewModel BuildItem()
        {
            MenuDashboardCommandViewModel result = new MenuDashboardCommandViewModel();
            result.BackgroundColor = Color.FromRgb(100, 193, 255);// Color.FromRgb(102, 204, 255); //Colors.Transparent;
            result.DashboardName = Common.Constants.DashboardNames.Home;
            result.IsVisible = true;
            result.PartName = Common.Constants.MdiMenus.Commands;
            result.Image = "photo";
            //result.BackgroundColor = Colors.Tomato;

            return result;
        }

        public List<MenuDashboardCommandViewModel> CommandsBuild()
        {
            List<MenuDashboardCommandViewModel> result = new List<MenuDashboardCommandViewModel>();

            UriQuery uriQueryInventor = new UriQuery();
            uriQueryInventor.Add(NavigationSettings.CBIContextInventor, String.Empty);

            UriQuery uriQueryMain = new UriQuery();
            uriQueryMain.Add(NavigationSettings.CBIContextMain, String.Empty);

            UriQuery uriQueryHistory = new UriQuery();
            uriQueryHistory.Add(NavigationSettings.CBIContextHistory, String.Empty);           

            //////////////////////////////////////////////////
            MenuDashboardCommandViewModel addCustomer = BuildItem();
			addCustomer.Image = "command_customerAdd";
			addCustomer.Name = enUICommand.AddCustomer.ToString();
            addCustomer.SortIndex = 0;
            addCustomer.Command = _commandRepository.Build(enUICommand.AddCustomer,  () =>
            {
                var config = this.GetMainAuditConfig();
                if (config != null)
                    config.Clear();
                this._eventAggregator.GetEvent<CustomerAddEvent>().Publish(new CustomerAddEventPayload()
                {
                    Context = CBIContext.Main
                });
            });
            result.Add(addCustomer);

            //////////////////////////////////////////////////
            MenuDashboardCommandViewModel addBranch = BuildItem();
            addBranch.Name = enUICommand.AddBranch.ToString();
			addBranch.Image = "command_branchAdd";
            addBranch.SortIndex = 1;
            addBranch.Command = _commandRepository.Build(enUICommand.AddBranch, () =>
            {
                var config = this.GetMainAuditConfig();
                if (config != null)
                    config.Clear();
                this._eventAggregator.GetEvent<BranchAddEvent>().Publish(new BranchAddEventPayload()
                {
                    IsCustomerComboVisible = true,
                    Context = CBIContext.Main
                });
            });            
            result.Add(addBranch);

            //////////////////////////////////////////////////
            MenuDashboardCommandViewModel addInventor = BuildItem();
			addInventor.Image = "command_invrntorAdd";
            addInventor.Name = enUICommand.AddInventor.ToString();
            addInventor.SortIndex = 2;
            addInventor.Command = _commandRepository.Build(enUICommand.AddInventor, () =>
            {
                var config = this.GetCreateAuditConfig();
                if (config != null)
                    config.Clear();

                this._eventAggregator.GetEvent<InventorAddEvent>().Publish(new InventorAddEventPayload()
                {
                    Context = CBIContext.CreateInventor,
                    IsCustomerComboVisible = true,
                    IsBranchComboVisible = true	,
					WithoutNavigate = false
                });
            });            
            result.Add(addInventor);

            //////////////////////////////////////////////////
            MenuDashboardCommandViewModel searchCustomer = BuildItem();
			searchCustomer.Image = "command_customerGrid";
            searchCustomer.Name = enUICommand.SearchCustomer.ToString();
            searchCustomer.SortIndex = 3;
            searchCustomer.Command = _commandRepository.Build(enUICommand.SearchCustomer, () =>
            {
                var config = this.GetMainAuditConfig();
                if (config != null)
                    config.Clear();

                UriQuery query = new UriQuery();                
                Utils.AddAuditConfigToQuery(query, config);
                UtilsNavigate.CustomerChooseOpen(CBIContext.Main, this._regionManager, query);
            });            
            result.Add(searchCustomer);

            //////////////////////////////////////////////////
            MenuDashboardCommandViewModel searchBranch = BuildItem();
			searchBranch.Image = "command_branchGrid";
            searchBranch.Name = enUICommand.SearchBranch.ToString();
            searchBranch.SortIndex = 4;
            searchBranch.Command = _commandRepository.Build(enUICommand.SearchBranch, () =>
            {
                var config = this.GetMainAuditConfig();
                if (config != null)
                    config.Clear();

                UriQuery query = new UriQuery();
				Utils.AddAuditConfigToQuery(query, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));                
                UtilsNavigate.BranchChooseOpen(CBIContext.Main, this._regionManager, query);
            });            
            result.Add(searchBranch);

            //////////////////////////////////////////////////
            MenuDashboardCommandViewModel searchInventor = BuildItem();
			searchInventor.Image = "command_invrntorGrid";
            searchInventor.Name = enUICommand.SearchInventor.ToString();
            searchInventor.SortIndex = 5;
            searchInventor.Command = _commandRepository.Build(enUICommand.SearchInventor, () =>
            {
                var config = this.GetMainAuditConfig();
                if (config != null)
                    config.Clear();

                UriQuery query = new UriQuery();                
				Utils.AddAuditConfigToQuery(query, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
                UtilsNavigate.InventorChooseOpen(CBIContext.Main, this._regionManager, query);
            });            
            result.Add(searchInventor);

            MenuDashboardCommandViewModel reportFavorites = BuildItem();
			reportFavorites.Image = "command_reportFavorites";
			reportFavorites.Name = enUICommand.ReportFavorites.ToString();
            reportFavorites.SortIndex = 6;
            reportFavorites.Command = _commandRepository.Build(enUICommand.ReportFavorites, ReportFavoritesOpen);            
            result.Add(reportFavorites);

            MenuDashboardCommandViewModel adapterLink = BuildItem();
			adapterLink.Image = "command_adapter";
	        adapterLink.Name = enUICommand.Adapters.ToString();
            adapterLink.SortIndex = 7;
            adapterLink.Command = _commandRepository.Build(enUICommand.Adapters, AdapterLinkOpen);            
            result.Add(adapterLink);

            MenuDashboardCommandViewModel pack = BuildItem();
			pack.Image = "command_pack";
            pack.Name = enUICommand.Pack.ToString();
            pack.Command = _commandRepository.Build(enUICommand.Pack, PackOpen);            
            result.Add(pack);

            MenuDashboardCommandViewModel unpack = BuildItem();
			unpack.Image = "command_unpack";
            unpack.Name = enUICommand.Unpack.ToString();
            unpack.SortIndex = 8;
            unpack.Command = _commandRepository.Build(enUICommand.Unpack, UnpackOpen);
            result.Add(unpack);

            return result;
        }      

        private void ReportFavoritesOpen()
        {
            AuditConfig ac = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            ac.Clear();
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);            
            Utils.AddAuditConfigToQuery(uriQuery, ac);
            UtilsNavigate.ReportFavoritesOpen(this._regionManager, uriQuery);
        }      

        private void AdapterLinkOpen()
        {
            AuditConfig ac = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            ac.Clear();
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddAuditConfigToQuery(uriQuery, ac);
            UtilsNavigate.AdapterLinkOpen(this._regionManager, uriQuery);
        }

        private void PackOpen()
        {
            AuditConfig ac = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            ac.Clear();
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddAuditConfigToQuery(uriQuery, ac);
            UtilsNavigate.PackOpen(this._regionManager, uriQuery);
        }


        private void UnpackOpen()
        {
            AuditConfig ac = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            ac.Clear();
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddAuditConfigToQuery(uriQuery, ac);
            UtilsNavigate.UnpackOpen(this._regionManager, uriQuery);
        }
    }
}