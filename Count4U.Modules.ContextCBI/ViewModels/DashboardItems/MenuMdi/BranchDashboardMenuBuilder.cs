using System;
using System.Collections.Generic;
using System.Windows.Media;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.Events.Misc;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Menu
{
    public class BranchDashboardMenuBuilder
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCBIRepository;
        private readonly UICommandRepository _commandRepository;

        private readonly AuditConfig _createInventorAuditConfig;
        private readonly AuditConfig _historyAuditConfig;
        private readonly AuditConfig _mainAuditConfig;


        public BranchDashboardMenuBuilder(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCBIRepository,
            UICommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
            this._contextCBIRepository = contextCBIRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this._createInventorAuditConfig = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
            this._historyAuditConfig = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
            this._mainAuditConfig = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
        }

        private MenuDashboardCommandViewModel BuildItem()
        {
            MenuDashboardCommandViewModel result = new MenuDashboardCommandViewModel();
            result.BackgroundColor = Color.FromRgb(100, 193, 255); //Color.FromRgb(153, 204, 0); //Colors.Transparent;
            result.DashboardName = Common.Constants.DashboardNames.Branch;
            result.IsVisible = true;
            result.PartName = Common.Constants.MdiMenus.Commands;
            result.Image = "photo";

            return result;
        }

        public List<MenuDashboardCommandViewModel> CommandsBuild()
        {
            List<MenuDashboardCommandViewModel>  result = new List<MenuDashboardCommandViewModel>();

            UriQuery uriQueryMain = new UriQuery();
            uriQueryMain.Add(NavigationSettings.CBIContextMain, String.Empty);         

            MenuDashboardCommandViewModel addInventor = AddInventorCommand();
			addInventor.Image = "command_invrntorAdd";
            result.Add(addInventor);

            MenuDashboardCommandViewModel searchInventor = SearchInventorCommand();
			searchInventor.Image = "command_invrntorGrid";
            searchInventor.SortIndex = 0;
            result.Add(searchInventor);

            MenuDashboardCommandViewModel reportFavorites = BuildItem();
			reportFavorites.Image = "command_reportFavorites";
            reportFavorites.Name = enUICommand.ReportFavorites.ToString();
            reportFavorites.Command = _commandRepository.Build(enUICommand.ReportFavorites, ReportFavoritesOpen);
            reportFavorites.SortIndex = 1;
            result.Add(reportFavorites);

			//MenuDashboardCommandViewModel editBranch = BuildItem();
			//editBranch.Name = enUICommand.EditBranch.ToString();
			//editBranch.Command = _commandRepository.Build(enUICommand.EditBranch, EditBranchOpen);
			//editBranch.SortIndex = 2;
			//result.Add(editBranch);

            MenuDashboardCommandViewModel properties = BuildItem();
			properties.Image = "command_properties";
            properties.Name = enUICommand.Properties.ToString();
            properties.Command = _commandRepository.Build(enUICommand.Properties, PropertiesOpen);
            properties.SortIndex = 3;
            result.Add(properties);

            MenuDashboardCommandViewModel pack = BuildItem();
			pack.Image = "command_pack";
            pack.Name = enUICommand.Pack.ToString();
            pack.Command = _commandRepository.Build(enUICommand.Pack, PackOpen);
            pack.SortIndex = 4;
            result.Add(pack);            

//             MenuDashboardCommandViewModel unpack = new MenuDashboardCommandViewModel();
//            unpack.Command = _commandRepository.Build(enUICommand.Unpack, UnpackOpen);
//            yield return unpack;

            return result;
        }

        private MenuDashboardCommandViewModel SearchInventorCommand()
        {
            MenuDashboardCommandViewModel result = BuildItem();
            result.Name = enUICommand.SearchInventor.ToString();
            result.Command = _commandRepository.Build(enUICommand.SearchInventor, () =>
            {
                UriQuery query = new UriQuery();
                Utils.AddAuditConfigToQuery(query, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
                UtilsNavigate.InventorChooseOpen(CBIContext.Main, this._regionManager, query);
            });

            return result;
        }

        private MenuDashboardCommandViewModel AddInventorCommand()
        {
            AuditConfig mainAuditConfig = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            Customer customer = this._contextCBIRepository.GetCurrentCustomer(mainAuditConfig);
            Branch branch = this._contextCBIRepository.GetCurrentBranch(mainAuditConfig);

            AuditConfig createInventorConfig = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.CreateInventor);
            this._contextCBIRepository.SetCurrentCustomer(customer, createInventorConfig);
            this._contextCBIRepository.SetCurrentBranch(branch, createInventorConfig);

            MenuDashboardCommandViewModel result = BuildItem();
            result.Name = enUICommand.AddInventor.ToString();
            result.Command = _commandRepository.Build(enUICommand.AddInventor, () => this._eventAggregator.GetEvent<InventorAddEvent>().Publish
                                                               (new InventorAddEventPayload()
                                                               {
                                                                   Context = CBIContext.CreateInventor,
                                                                   IsCustomerComboVisible = false,
                                                                   IsBranchComboVisible = false	,
																   WithoutNavigate = false
                                                               }));
          
            return result;
        }

        private void ReportFavoritesOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextBranch);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
            UtilsNavigate.ReportFavoritesOpen(this._regionManager, uriQuery);
        }

        private void ExportOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextBranch);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
            UtilsNavigate.ExportPdaWithModulesOpen(this._regionManager, uriQuery);
        }

        private void EditBranchOpen()
        {
            Branch branch = this._contextCBIRepository.GetCurrentBranch(this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));

            this._eventAggregator.GetEvent<BranchEditEvent>().Publish(new BranchEditEventPayload() { Branch = branch, Context = CBIContext.Main });
        }

        private void PropertiesOpen()
        {
            ObjectPropertiesViewEventPayload payload = new ObjectPropertiesViewEventPayload();
            payload.Context = CBIContext.Main;
            payload.DbContext = Common.NavigationSettings.CBIDbContextBranch;

            this._eventAggregator.GetEvent<ObjectPropertiesViewEvent>().Publish(payload);
        }

        private void PackOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextBranch);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
            UtilsNavigate.PackOpen(this._regionManager, uriQuery);
        }

        private void UnpackOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextBranch);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main));
            UtilsNavigate.UnpackOpen(this._regionManager, uriQuery);
        }
    }
}