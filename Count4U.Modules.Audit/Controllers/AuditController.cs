using System;
using System.Collections.Generic;
using System.Windows;
using Count4U.Common;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Count4U.Modules.Audit.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.Linq;

namespace Count4U.Modules.Audit.Controllers
{
    public class AuditController
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ModalWindowLauncher _modalWindowLauncher;

        public AuditController(IUnityContainer container,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ModalWindowLauncher modalWindowLauncher
            )
        {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._container = container;
            this._modalWindowLauncher = modalWindowLauncher;

			this._eventAggregator.GetEvent<LocationMultiAddEvent>().Subscribe(LocationMultiAdd); // Marina
			this._eventAggregator.GetEvent<LocationAddEvent>().Subscribe(LocationAdd);
			this._eventAggregator.GetEvent<ProcessAddEvent>().Subscribe(ProcessAdd);
            this._eventAggregator.GetEvent<IturLocationChangeEvent>().Subscribe(IturLocationChange);
			this._eventAggregator.GetEvent<IturPrefixChangeEvent>().Subscribe(IturPrefixChange);
			this._eventAggregator.GetEvent<ShowShelfEvent>().Subscribe(ShowShelf);
			this._eventAggregator.GetEvent<IturNameChangeEvent>().Subscribe(IturNameChange);
            this._eventAggregator.GetEvent<IturStatusChangeEvent>().Subscribe(IturStatusChange);
            this._eventAggregator.GetEvent<IturStateChangeEvent>().Subscribe(IturStateChange);
            this._eventAggregator.GetEvent<IturAddEvent>().Subscribe(IturAdd);
			this._eventAggregator.GetEvent<IturClearWithItemsEvent>().Subscribe(IturDeleteWithItems);
            this._eventAggregator.GetEvent<ProductAddEditEvent>().Subscribe(ProductAddEdit);
            this._eventAggregator.GetEvent<IturEditEvent>().Subscribe(IturEdit);
            this._eventAggregator.GetEvent<DocumentHeaderAddEditEvent>().Subscribe(DocumentHeaderAddEdit);
			this._eventAggregator.GetEvent<DeviceAddEditEvent>().Subscribe(DeviceAddEdit);
            this._eventAggregator.GetEvent<InventProductAddEvent>().Subscribe(InventProductAdd);
            this._eventAggregator.GetEvent<InventProductEditEvent>().Subscribe(InventProductEdit);
			this._eventAggregator.GetEvent<LocationTagChangeEvent>().Subscribe(LocationTagChange);
			this._eventAggregator.GetEvent<SectionTagChangeEvent>().Subscribe(SectionChangeTag);
			
			this._eventAggregator.GetEvent<IturTagChangeEvent>().Subscribe(IturTagChange);

        }

      
        private void LocationAdd(LocationAddedEventPayLoad payload)
        {
            string title = WindowTitles.LocationAdd;
            Dictionary<string, string> settings = new Dictionary<string, string>();
            if (payload != null)
            {
                if (payload.Location != null)
                {
                    settings.Add(Common.NavigationSettings.LocationCode, payload.Location.Code);
                    title = WindowTitles.LocationEdit;
                }
                if (payload.AddUnknownLocation)
                {
                    settings.Add(Common.NavigationSettings.AddUnknownLocation, String.Empty);
                    title = WindowTitles.LocationAdd;
                }

                Utils.AddContextToDictionary(settings, payload.Context);
                Utils.AddDbContextToDictionary(settings, payload.DbContext);
            }
            StartModalWindow(Common.ViewNames.LocationAddEditView, title, 360, 360, ResizeMode.NoResize, settings);
        }

		private void ProcessAdd(ProcessAddedEventPayLoad payload)
		{
			string title = WindowTitles.ProcessAdd;
			Dictionary<string, string> settings = new Dictionary<string, string>();
			if (payload != null)
			{
				if (payload.Process != null)
				{
					settings.Add(Common.NavigationSettings.ProcessCode, payload.Process.ProcessCode);
					title = WindowTitles.ProcessEdit;
				}

				if (payload.AddUnknownProcess)
				{
					settings.Add(Common.NavigationSettings.AddUnknownProcess, String.Empty);
					title = WindowTitles.ProcessAdd;
				}
			

				//Utils.AddContextToDictionary(settings, payload.Context);
				//Utils.AddDbContextToDictionary(settings, payload.DbContext);
			}
			StartModalWindow(Common.ViewNames.ProcessAddEditView, title, 450, 600, ResizeMode.NoResize, settings);
		}

		private void LocationMultiAdd(LocationMultiAddedEventPayLoad payload)
		{
			string title = WindowTitles.LocationMultiAdd;
			Dictionary<string, string> settings = new Dictionary<string, string>();
			if (payload != null)
			{
				//if (payload.Location != null)
				//{
				//	settings.Add(Common.NavigationSettings.LocationCode, payload.Location.Code);
				//	title = WindowTitles.LocationMultiAdd;
				//}
				//if (payload.AddUnknownLocation)
				//{
				//	settings.Add(Common.NavigationSettings.AddUnknownLocation, String.Empty);
				//	title = WindowTitles.LocationMultiAdd;
				//}

				Utils.AddContextToDictionary(settings, payload.Context);
				Utils.AddDbContextToDictionary(settings, payload.DbContext);
			}
			StartModalWindow(Common.ViewNames.LocationMultiAddView, title, 330, 60, ResizeMode.NoResize, settings);
		}
        private void InventProductAdd(InventProductAddEventPayload payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add(NavigationSettings.DocumentCode, payload.DocumentCode);

            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

            StartModalWindow(Common.ViewNames.InventProductAddEditView, WindowTitles.InventProductAdd, 330, 280, ResizeMode.NoResize, settings);
        }

        private void InventProductEdit(InventProductEditEventPayLoad payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add(NavigationSettings.InventProductId, payload.InventProduct.ID.ToString());

            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

            StartModalWindow(Common.ViewNames.InventProductAddEditView, WindowTitles.InventProductEdit, 330, 280, ResizeMode.NoResize, settings);
        }

		private void IturPrefixChange(IturPrefixChangeEventPayload payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add(NavigationSettings.IturCodes, payload.Iturs.Select(r => r.IturCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));

            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.IturPrefixChangeView, WindowTitles.IturChangePrefix, 280, 160, ResizeMode.NoResize, settings);
        }


		private void ShowShelf(ShowShelfEventPayload payload)
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings.Add(NavigationSettings.IturCodes, payload.Iturs.Select(r => r.IturCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));
			Utils.AddContextToDictionary(settings, payload.Context);
			Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.ShowShelfView, WindowTitles.ShowShelf, 1100, 700, ResizeMode.NoResize, settings);
		}

		private void IturLocationChange(IturLocationChangeEventPayload payload)
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings.Add(NavigationSettings.IturCodes, payload.Iturs.Select(r => r.IturCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));

			Utils.AddContextToDictionary(settings, payload.Context);
			Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.IturLocationChangeView, WindowTitles.IturChangeLocation, 600, 400, ResizeMode.NoResize, settings);
		}

		private void LocationTagChange(LocationTagChangeEventPayload payload)
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings.Add(NavigationSettings.LocationCodes, payload.Locations.Select(r => r.Code).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));

			Utils.AddContextToDictionary(settings, payload.Context);
			Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.LocationTagChangeView, WindowTitles.LocationChangeTag, 354, 245, ResizeMode.NoResize, settings);
		}

		private void SectionChangeTag(SectionTagChangeEventPayload payload)
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings.Add(NavigationSettings.SectionCodes, payload.Sections.Select(r => r.SectionCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));

			Utils.AddContextToDictionary(settings, payload.Context);
			Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.SectionTagChangeView, WindowTitles.SectionChangeTag, 354, 245, ResizeMode.NoResize, settings);
		}

		private void IturTagChange(IturTagChangeEventPayload payload)
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings.Add(NavigationSettings.IturCodes, payload.Iturs.Select(r => r.IturCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));

			Utils.AddContextToDictionary(settings, payload.Context);
			Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.IturTagChangeView, WindowTitles.IturChangeTag, 354, 245, ResizeMode.NoResize, settings);
		}

		private void IturNameChange(IturNameChangeEventPayload payload)
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings.Add(NavigationSettings.IturCodes, payload.Iturs.Select(r => r.IturCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));

			Utils.AddContextToDictionary(settings, payload.Context);
			Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.IturNameChangeView, WindowTitles.IturChangeName, 280, 160, ResizeMode.NoResize, settings);
		}

        private void IturStatusChange(IturStatusChangeEventPayload payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add(NavigationSettings.IturCodes, payload.Iturs.Select(r => r.IturCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));

            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

            StartModalWindow(ViewNames.IturStatusChangeView, WindowTitles.IturChangeState, 280, 200, ResizeMode.NoResize, settings);
        }

        private void IturStateChange(ItursStateChangeEventPayload payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add(NavigationSettings.IturCodes, payload.Iturs.Select(r => r.IturCode).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));

            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

            StartModalWindow(ViewNames.IturStateChangeView, WindowTitles.IturChangeState, 280, 200, ResizeMode.NoResize, settings);
        }

        private void IturAdd(IturAddEventPayload payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

            StartModalWindow(ViewNames.IturAddView, WindowTitles.AddItur, 360, 360, ResizeMode.NoResize, settings);
        }


		private void IturDeleteWithItems(IturClearWithItemsEventPayload payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.IturDeleteView, WindowTitles.ClearIturs, 380, 380, ResizeMode.NoResize, settings);
        }

        private void IturEdit(IturEditEventPayload payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add(Common.NavigationSettings.IturCode, payload.Itur.IturCode);
            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);
            StartModalWindow(ViewNames.IturEditView, WindowTitles.EditItur, 360, 360, ResizeMode.NoResize, settings);
        }

        private void ProductAddEdit(ProductAddEditEventPayload payload)
        {            
            Dictionary<string, string> settings = new Dictionary<string, string>();
            string title = WindowTitles.ProductAdd;
            if (payload.Product != null)
            {
                settings.Add(Common.NavigationSettings.ProductMakat, payload.Product.Makat ?? String.Empty);
                title = WindowTitles.ProductEdit;
            }

            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

            StartModalWindow(ViewNames.ProductAddEditView, title, 360, 360, ResizeMode.NoResize, settings);
        }

        private void DocumentHeaderAddEdit(DocumentHeaderAddEditEventPayload payload)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            if (payload.DocumentHeader != null)
            {
                settings.Add(Common.NavigationSettings.DocumentCode, payload.DocumentHeader.DocumentCode);
            }
            settings.Add(Common.NavigationSettings.IturCode, payload.IturCode);

            Utils.AddContextToDictionary(settings, payload.Context);
            Utils.AddDbContextToDictionary(settings, payload.DbContext);

            StartModalWindow(ViewNames.DocumentHeaderAddEditView, WindowTitles.DocumentHeaderAddEdit, 300, 270, ResizeMode.NoResize, settings);
        }


		private void DeviceAddEdit(DeviceAddEditEventPayload payload)
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
			if (payload.Device != null)
			{
				settings.Add(Common.NavigationSettings.DeviceCode, payload.Device.DeviceCode);
			}
			//settings.Add(Common.NavigationSettings.IturCode, payload.IturCode);

			//settings.Add(Common.NavigationSettings.PeriodFromInventorDate, payload.PeriodFromInventorDate);
			settings.Add(Common.NavigationSettings.PeriodFromStartDate, payload.PeriodFromStartDate);
			settings.Add(Common.NavigationSettings.QuentetyEdit, payload.QuentetyEdit.ToString());
			Utils.AddContextToDictionary(settings, payload.Context);
			Utils.AddDbContextToDictionary(settings, payload.DbContext);

			StartModalWindow(ViewNames.DeviceAddEditView, WindowTitles.DeviceAddEdit, 300, 270, ResizeMode.NoResize, settings);
		}

        private void StartModalWindow(string viewName, string windowTitle, int width = 800, int height = 600, ResizeMode resizeMode = ResizeMode.CanResize, Dictionary<string, string> settings = null)
        {
            this._modalWindowLauncher.StartModalWindow(viewName, windowTitle, width, height, resizeMode, settings);
        }
    }
}