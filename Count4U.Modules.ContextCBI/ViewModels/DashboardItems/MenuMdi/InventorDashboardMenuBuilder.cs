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
using Count4U.Modules.ContextCBI.Events.Misc;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Media;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Menu
{
    public class InventorDashboardMenuBuilder
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCBIRepository;
        private readonly UICommandRepository _commandRepository;

        public InventorDashboardMenuBuilder(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCBIRepository,
            UICommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
            this._contextCBIRepository = contextCBIRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
        }

        private MenuDashboardCommandViewModel BuildItem()
        {
            MenuDashboardCommandViewModel result = new MenuDashboardCommandViewModel();
            result.BackgroundColor = Color.FromRgb(100, 193, 255);// Color.FromRgb(153, 153, 153); //Colors.Transparent;
            result.DashboardName = Common.Constants.DashboardNames.Inventor;
            result.IsVisible = true;
            result.PartName = Common.Constants.MdiMenus.Commands;
            result.Image = "photo";

            return result;
        }

        public List<MenuDashboardCommandViewModel> CommandsBuild()
        {
            List<MenuDashboardCommandViewModel> result = new List<MenuDashboardCommandViewModel>();
            UriQuery uriQueryMain = new UriQuery();
            uriQueryMain.Add(NavigationSettings.CBIContextMain, String.Empty);
//
//            MenuDashboardCommandViewModel changeCatalog = BuildItem();
//            changeCatalog.Name = enUICommand.ChangeCatalog.ToString();
//            changeCatalog.SortIndex = 0;
//            changeCatalog.Command = _commandRepository.Build(enUICommand.ChangeCatalog, ImportCatalogOpen);
//            result.Add(changeCatalog);

            MenuDashboardCommandViewModel getFromPDA = BuildItem();
			getFromPDA.Image = "command_ImportFromPda";
			//getFromPDA.BackgroundColor = Color.FromRgb(11, 211, 0);
            getFromPDA.Name = enUICommand.ImportFromPda.ToString();
            getFromPDA.SortIndex = 1;
            getFromPDA.Command = _commandRepository.Build(enUICommand.ImportFromPda, ImportFromPdaOpen);
            result.Add(getFromPDA);

//            MenuDashboardCommandViewModel catalog = BuildItem();
//            catalog.Name = enUICommand.Catalog.ToString();
//            catalog.SortIndex = 2;
//            catalog.Command = _commandRepository.Build(enUICommand.Catalog, CatalogOpen);            
//            result.Add(catalog);

            MenuDashboardCommandViewModel reportFavorites = BuildItem();
			reportFavorites.Image = "command_reportFavorites";
			//reportFavorites.BackgroundColor = Color.FromRgb(11, 211, 211);
            reportFavorites.Name = enUICommand.ReportFavorites.ToString();
            reportFavorites.SortIndex = 3;
            reportFavorites.Command = _commandRepository.Build(enUICommand.ReportFavorites, ReportFavoritesOpen);
            result.Add(reportFavorites);

            MenuDashboardCommandViewModel exportPDA = BuildItem();
			exportPDA.Image = "command_exportPDA";
			//exportPDA.BackgroundColor = Color.FromRgb(11, 211, 211);
            exportPDA.Name = enUICommand.ExportPDA.ToString();
            exportPDA.SortIndex = 4;
            exportPDA.Command = _commandRepository.Build(enUICommand.ExportPDA, ExportOpen);            
            result.Add(exportPDA);

            MenuDashboardCommandViewModel exportErp = BuildItem();
			exportErp.Image = "command_exportErp";
			//exportErp.BackgroundColor = Color.FromRgb(11, 211, 211);
            exportErp.Name = enUICommand.ExportERP.ToString();
            exportErp.SortIndex = 5;
            exportErp.Command = _commandRepository.Build(enUICommand.ExportERP, ExportErpOpen);
            result.Add(exportErp);

			//MenuDashboardCommandViewModel editInventor = BuildItem();
			//editInventor.Image = "command_editInventor";
			//editInventor.BackgroundColor = Color.FromRgb(11, 211, 211);
			//editInventor.Name = enUICommand.EditInventor.ToString();
			//editInventor.SortIndex = 6;
			//editInventor.Command = _commandRepository.Build(enUICommand.EditInventor, EditInventorOpen);
			//result.Add(editInventor);

            MenuDashboardCommandViewModel properties = BuildItem();
			properties.Image = "command_properties";
			//properties.BackgroundColor = Color.FromRgb(11, 211, 211);
            properties.Name = enUICommand.Properties.ToString();
            properties.SortIndex = 7;
            properties.Command = _commandRepository.Build(enUICommand.Properties, PropertiesOpen);
            result.Add(properties);

            MenuDashboardCommandViewModel pack = BuildItem();
			pack.Image = "command_pack";
			//pack.BackgroundColor = Color.FromRgb(11, 211, 211);
            pack.Name = enUICommand.Pack.ToString();
            pack.SortIndex = 8;
            pack.Command = _commandRepository.Build(enUICommand.Pack, PackOpen);
            result.Add(pack);                                                                        
//
//            MenuDashboardCommandViewModel unpack = new MenuDashboardCommandViewModel();
//            unpack.Command = _commandRepository.Build(enUICommand.Unpack, UnpackOpen);
//            yield return unpack;

            //            MenuDashboardCommandViewModel sendZipOffice = new MenuDashboardCommandViewModel();
            //            sendZipOffice.Command = _commandRepository.Build(enUICommand.SendZipOffice, SendZipOfficeOpen);
            //            yield return sendZipOffice;

            return result;
        }

        private void ImportCatalogOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeCatalog);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void IturimAddEditDeleteOpen()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, CBIContext.History);
            Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(query, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.IturimAddEditDeleteOpen(this._regionManager, query);
        }

        private void ErpExpectedOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.ErpExpectedStep1Open(this._regionManager, uriQuery);
        }

        private void ImportFromPdaOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.ImportFromPdaOpen(this._regionManager, uriQuery);
        }

        private void CatalogOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.CatalogOpen(this._regionManager, uriQuery);
        }

        private void ReportFavoritesOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.ReportFavoritesOpen(this._regionManager, uriQuery);
        }

        private void ExportOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.ExportPdaWithModulesOpen(this._regionManager, uriQuery);
        }

        private void ExportErpOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.ExportErpWithModulesOpen(this._regionManager, uriQuery);
        }

        private void EditInventorOpen()
        {
            Inventor inventor = this._contextCBIRepository.GetCurrentInventor(this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));

            if (inventor != null)
            {
                this._eventAggregator.GetEvent<InventorEditEvent>().Publish(new InventorEditEventPayload() { Inventor = inventor, Context = CBIContext.History });
            }
        }

        private void PropertiesOpen()
        {
            ObjectPropertiesViewEventPayload payload = new ObjectPropertiesViewEventPayload();
            payload.Context = CBIContext.History;
            payload.DbContext = Common.NavigationSettings.CBIDbContextInventor;

            this._eventAggregator.GetEvent<ObjectPropertiesViewEvent>().Publish(payload);
        }

        private void SendZipOfficeOpen()
        {
            InventorStatusChangeEventPayload payload = new InventorStatusChangeEventPayload();
            payload.Context = CBIContext.History;
            payload.DbContext = Common.NavigationSettings.CBIDbContextInventor;

            this._eventAggregator.GetEvent<InventorStatusChangeEvent>().Publish(payload);
        }

        private void PackOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.PackOpen(this._regionManager, uriQuery);
        }

        private void UnpackOpen()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, CBIContext.History);
            Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
            Utils.AddAuditConfigToQuery(uriQuery, this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.History));
            UtilsNavigate.UnpackOpen(this._regionManager, uriQuery);
        }
    }
}