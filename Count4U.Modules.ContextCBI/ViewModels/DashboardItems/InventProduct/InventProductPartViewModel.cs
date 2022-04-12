using System;
using System.Threading.Tasks;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.ContextCBI.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Interface.Audit;
using Count4U.Common.Extensions;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Filter.Sorting;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class InventProductPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IInventProductRepository _inventProductRepository;
        private readonly UICommandRepository _commandRepository;
		private readonly INavigationRepository _navigationRepository;


        private readonly DelegateCommand _moreCommand;
		private readonly DelegateCommand _compareCommand;
		private readonly IUserSettingsManager _userSettingsManager;
		

        private string _insertFromBarcodeText;
        private string _insertManuallyText;

        public InventProductPartViewModel(
         IContextCBIRepository contextCBIRepository,
         IEventAggregator eventAggregator,
         IRegionManager regionManager,
            IInventProductRepository inventProductRepository,
            UICommandRepository commandRepository ,
			 INavigationRepository navigationRepository	 ,
			IUserSettingsManager userSettingsManager
         )
            : base(contextCBIRepository)
        {
            _commandRepository = commandRepository;
            _inventProductRepository = inventProductRepository;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
			_navigationRepository = navigationRepository;
			_userSettingsManager = userSettingsManager;

            _moreCommand = _commandRepository.Build(enUICommand.More,  MoreCommandExecuted);
			_compareCommand = _commandRepository.Build(enUICommand.Compare, CompareCommandExecuted);
        }       

        public DelegateCommand MoreCommand
        {
            get { return _moreCommand; }
        }

		public DelegateCommand CompareCommand
        {
			get { return _compareCommand; }
        }
		

        public string InsertFromBarcodeText
        {
            get { return _insertFromBarcodeText; }
            set
            {
                _insertFromBarcodeText = value;
                RaisePropertyChanged(() => InsertFromBarcodeText);
            }
        }

        public string InsertManuallyText
        {
            get { return _insertManuallyText; }
            set
            {
                _insertManuallyText = value;
                RaisePropertyChanged(() => InsertManuallyText);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void Build()
        {
            string fromBarcode =  _inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountItems_InsertFromBarcode, base.GetDbPath);
            string manually = _inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountItems_InsertManually, base.GetDbPath);

            Utils.RunOnUI(() =>
                {
                    InsertFromBarcodeText = String.Format(Localization.Resources.ViewModel_InventProductPart_InsertFromBarcode, fromBarcode);
                    InsertManuallyText = String.Format(Localization.Resources.ViewModel_InventProductPart_InsertManually, manually);
                });
        }

        public void Refresh()
        {
            Build();
        }

        public void Clear()
        {

        }

        private void MoreCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            //query.Add(Common.NavigationSettings.AdapterName, "");

            UtilsNavigate.InventProductListOpen(this._regionManager, query);
        }



		private void CompareCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
			query.Add(Common.NavigationSettings.AdapterName, "Compare");
			//Utils.AddSelectParamsToQuery(
			InventProductFilterData filterData = new InventProductFilterData();
			this._userSettingsManager.InventProductPropertyFilterSelectedNumberItemSet("IPValueInt3");

			filterData.PropertyNumberName = "IPValueInt3";
			filterData.PropertyNumber = "0";
			filterData.PropertyNumberIsAbsolute = true;
			filterData.PropertyNumberEquality = Common.Constants.ComboValues.Equality.Greater;
			filterData.SortField = "ERPIturCode";
			filterData.SortDirection = enSortDirection.ASC;
			//	InventProductFilterData filterData = _searchFieldViewModel.BuildFilterData() as InventProductFilterData;
			if (filterData != null)	UtilsConvert.AddObjectToQuery(query, _navigationRepository, filterData, Common.NavigationObjects.Filter);
		

            UtilsNavigate.InventProductListOpen(this._regionManager, query);
        }
    }
}