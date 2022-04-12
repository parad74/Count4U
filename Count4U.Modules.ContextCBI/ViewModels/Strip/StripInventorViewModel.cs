using System;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Strip
{
    public class StripInventorViewModel : StripBaseViewModel
    {
        private readonly DelegateCommand _inventorDashboardCommand;
        private Inventor _currentInventor;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;

        public StripInventorViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager)
            : base(contextCBIRepository)
        {
            _userSettingsManager = userSettingsManager;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._inventorDashboardCommand = new DelegateCommand(InventorDashboardCommandExecuted);
        }

        public new Inventor CurrentInventor
        {
            get
            {
                if (this._currentInventor == null)
                    switch (this.Context)
                    {
                        case CBIContext.CreateInventor:
                            this._currentInventor = base.ContextCBIRepository.GetCurrentInventor(this.GetCreateAuditConfig());
                            break;
                        case CBIContext.History:
                            this._currentInventor = base.ContextCBIRepository.GetCurrentInventor(this.GetHistoryAuditConfig());
                            break;
                        case CBIContext.Main:
                            this._currentInventor = base.ContextCBIRepository.GetCurrentInventor(this.GetMainAuditConfig());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                return this._currentInventor;
            }
        }

        public DelegateCommand InventorDashboardCommand
        {
            get { return this._inventorDashboardCommand; }
        }

        public string InventorDate
        {
            get
            {
                if (CurrentInventor == null)
                    return String.Empty;

                enLanguage language = _userSettingsManager.LanguageGet();

                return UtilsConvert.DateHebrewConvert(CurrentInventor.InventorDate, language);
            }
        }

		public string InventorDescription
		{
			get
			{
				if (CurrentInventor == null)
					return String.Empty;

				//enLanguage language = _userSettingsManager.LanguageGet();

				return CurrentInventor.Description;
			}
		}

        private void InventorDashboardCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
        }

        public void Refresh()
        {
            this._currentInventor = null;
            RaisePropertyChanged(() => CurrentInventor);
            RaisePropertyChanged(() => InventorDate);
        }
    }
}