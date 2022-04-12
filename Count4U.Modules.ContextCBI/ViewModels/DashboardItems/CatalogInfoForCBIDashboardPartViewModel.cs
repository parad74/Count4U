using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Common.Extensions;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class CatalogInfoForCBIDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly DelegateCommand _catalogFormOpenCommand;
        private readonly DelegateCommand _importCatalogCommand;
        private readonly DelegateCommand _updateCatalogCommand;
		private readonly DelegateCommand _complexOperationCommand;
        private readonly DelegateCommand _maskEditCommand;
        private readonly IProductRepository _productRepository;
        private readonly ICatalogConfigRepository _catalogConfigRepository;
        private readonly UICommandRepository _commandRepository;
        private readonly IRegionManager _regionManager;
        private readonly DelegateCommand _exportPdaCommand;

        private bool _isPartEnabled;

        private string _totalItemsText;
        private string _lastChangesText;
        private string _totalMakatsText;

        public CatalogInfoForCBIDashboardPartViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IProductRepository productRepository,
            ICatalogConfigRepository catalogConfigRepository,
             IRegionManager regionManager,
            UICommandRepository commandRepository
            )
            : base(contextCBIRepository)
        {
            _commandRepository = commandRepository;
            this._regionManager = regionManager;
            this._catalogConfigRepository = catalogConfigRepository;
            this._productRepository = productRepository;
            this._eventAggregator = eventAggregator;
            this._catalogFormOpenCommand = _commandRepository.Build(enUICommand.More, CatalogFormOpenCommandExecuted);
            this._importCatalogCommand = _commandRepository.Build(enUICommand.Import, ImportCatalogCommandExecuted);
            this._maskEditCommand = _commandRepository.Build(enUICommand.MaskEdit, MaskEditCommandExecuted);
            this._updateCatalogCommand = _commandRepository.Build(enUICommand.UpdateCatalog, UpdateCatalogCommandExecuted);
			this._complexOperationCommand = _commandRepository.Build(enUICommand.ComplexOperation, ComplexOperationCommandExecuted, ComplexOperationCommandCanExecute);
			
            this._exportPdaCommand = _commandRepository.Build(enUICommand.ExportPDA, ExportPdaCommandExecuted);
        }        

        public string TotalItemsText
        {
            get { return _totalItemsText; }
        }

        public string LastChangesText
        {
            get { return _lastChangesText; }
        }

        public string TotalMakatsText
        {
            get { return _totalMakatsText; }
        }

        public DelegateCommand CatalogFormOpenCommand
        {
            get { return this._catalogFormOpenCommand; }
        }

        public DelegateCommand ImportCatalogCommand
        {
            get { return this._importCatalogCommand; }
        }

        public bool IsPartEnabled
        {
            get { return this._isPartEnabled; }
        }

        public DelegateCommand MaskEditCommand
        {
            get { return this._maskEditCommand; }
        }

        public DelegateCommand UpdateCatalogCommand
        {
            get { return _updateCatalogCommand; }
        }


		public DelegateCommand ImportExportCommand
        {
			get { return _complexOperationCommand; }
        }

        public DelegateCommand ExportPdaCommand
        {
            get { return _exportPdaCommand; }
        }

        private void CatalogFormOpenCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.CatalogOpen(this._regionManager, uriQuery);
        }

        private void ImportCatalogCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeCatalog);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private void UpdateCatalogCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeUpdateCatalog);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }


		private void ComplexOperationCommandExecuted()
		{
			UriQuery uriQuery = new UriQuery();
			//Utils.AddContextToQuery(uriQuery, base.Context);			  //предполагает только инветор 
			//Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);	    //предполагает только инветор 
			uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ComplexModeObject);
			Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
			Utils.AddFromContextToQuery(uriQuery, FromContext.MdiInventorLinkWithoutAction);

			uriQuery = Utils.AddFromContextToQuery(uriQuery, FromContext.SelectAdapterWithoutAction);

			UtilsNavigate.ComplexOperationViewOpen(this._regionManager, uriQuery);
		}

		private bool ComplexOperationCommandCanExecute()
		{
			if (base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor)
			{
				return true;
			}
			else return false;
		}

        private void MaskEditCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, base.GetAuditConfigByCurrentContext());
            UtilsNavigate.MaskListOpen(this._regionManager, uriQuery);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._isPartEnabled = true;

			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        private void Build()
        {
            long total = this._productRepository.CountProduct(base.GetDbPath);
            //DateTime dt = this._productRepository.GetMaxModifyDate(base.GetDbPath);
			//DateTime dt = base.GetModifyDateTimeCurrentDomainObject();
			DateTime dt = base.GeLastUpdateDateTimeCurrentDomainObject();
			//customer.ModifyDate = dictionary[customer.Code].ModifyDate;
            long totalMakat = this._productRepository.CountMakat(base.GetDbPath);

            Utils.RunOnUI(() =>
                {
                    this._totalItemsText = String.Format(Localization.Resources.ViewModel_CatalogInfoForCBIPart_TotalItems, total);
                    string lastDate = dt == DateTime.MinValue ? String.Empty : UtilsConvert.DateToStringLong(dt);
                    string last = String.Format(Localization.Resources.ViewModel_CatalogInfoForCBIPart_LastChanges, lastDate);
                    this._lastChangesText = last;
                    this._totalMakatsText = String.Format(Localization.Resources.ViewModel_CatalogInfoForCBIPart_TotalItemCodes, totalMakat);

                    RaisePropertyChanged(() => TotalItemsText);
                    RaisePropertyChanged(() => LastChangesText);
                    RaisePropertyChanged(() => TotalMakatsText);
                });
        }

        public void Refresh()
        {
            this.RaisePropertyChanged(() => TotalItemsText);
            this.RaisePropertyChanged(() => LastChangesText);
        }

        public void Clear()
        {

        }

        private void ExportPdaCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(uriQuery, base.ContextCBIRepository.GetCurrentCBIConfig(base.Context));
            UtilsNavigate.ExportPdaWithModulesOpen(this._regionManager, uriQuery);
        }
    }
}