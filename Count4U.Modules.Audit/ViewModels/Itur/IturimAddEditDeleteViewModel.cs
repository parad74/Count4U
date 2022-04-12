using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Events.Filter;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.Events;
using Count4U.Report.ViewModels.ReportButton;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using NLog;
using Microsoft.Practices.ServiceLocation;
using Count4U.Common.Extensions;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common;
using Count4U.Localization;
using Count4U.Model.Common;

namespace Count4U.Modules.Audit.ViewModels
{
    public class IturimAddEditDeleteViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceLocator _serviceLocator;

        private Stopwatch _stopwatch;

        private readonly IEventAggregator _eventAggregator;
        private readonly IIturRepository _iturRepository;
		private readonly IInventProductRepository _inventProductRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IStatusIturRepository _statusIturRepository;
        private readonly INavigationRepository _navigationRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IRegionManager _regionManager;
        private readonly UICommandRepository _commandRepository;
		private readonly IDocumentHeaderRepository _documentHeaderRepository;
		private readonly ITemporaryInventoryRepository _temporaryInventoryRepository;

        private readonly DelegateCommand _addCommand;
        private readonly DelegateCommand _changeLocationCommand;
		private readonly DelegateCommand _changeIturPrefixCommand;
		private readonly DelegateCommand _showShelfCommand;
		private readonly DelegateCommand _changeNameCommand;
        private readonly DelegateCommand _deleteCommand;
		private readonly DelegateCommand _clearWithItemsCommand;
		private readonly DelegateCommand _clearWithItemsDialogCommand;
		
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _editCommand;
        private readonly DelegateCommand _changeStatusCommand;
        private readonly DelegateCommand _changeStateCommand;
        private readonly DelegateCommand _reportCommand;
        private readonly DelegateCommand _repairCommand;
        private readonly DelegateCommand _searchCommand;
		private readonly DelegateCommand _changeIturTagCommand;
		private readonly DelegateCommand<IturItemViewModel> _openDetailsCommand;

        private int _pageSize;
        private int _pageCurrent;
        private int _itemsTotal;

        private List<IturItemViewModel> _selectedItems;
        private readonly ObservableCollection<IturItemViewModel> _items;

        private readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
        private readonly InteractionRequest<MessageBoxNotification> _messageBoxRequest;

        private string _totalFoundText;
        private string _warningText;
        private bool _warningTextIsVisible;

        private bool? _isDisabled;

        private bool _isGeneratedByCode;

        private readonly ReportButtonViewModel _reportButtonViewModel;

        private IturFilterData _filter;

        public IturimAddEditDeleteViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IIturRepository iturRepository,
            ILocationRepository locationRepository,
			IInventProductRepository inventProductRepository,
            IStatusIturRepository statusIturRepository,
            IUserSettingsManager userSettingsManager,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            ReportButtonViewModel reportButtonViewModel,
            UICommandRepository commandRepository,
			IDocumentHeaderRepository documentHeaderRepository ,
			ITemporaryInventoryRepository temporaryInventoryRepository
            )
            : base(contextCBIRepository)
        {
            this._commandRepository = commandRepository;
            this._reportButtonViewModel = reportButtonViewModel;
            this._navigationRepository = navigationRepository;
            this._regionManager = regionManager;
            this._userSettingsManager = userSettingsManager;
            this._statusIturRepository = statusIturRepository;
            this._locationRepository = locationRepository;
            this._iturRepository = iturRepository;
            this._eventAggregator = eventAggregator;
            this._serviceLocator = serviceLocator;
			this._inventProductRepository = inventProductRepository;
			this._documentHeaderRepository = documentHeaderRepository;
			this._temporaryInventoryRepository = temporaryInventoryRepository;

            this._addCommand = _commandRepository.Build(enUICommand.Add, this.AddCommandExecuted);
            this._changeLocationCommand = _commandRepository.Build(enUICommand.ChangeLocation, this.ChangeLocationCommandExecuted, this.ChangeLocationCommandCanExecute);
			this._changeIturPrefixCommand = _commandRepository.Build(enUICommand.ChangeIturPrefix, this.ChangeIturPrefixCommandExecuted, this.ChangeIturPrefixCommandCanExecute);
			this._showShelfCommand = _commandRepository.Build(enUICommand.ShowShelf, this.ShowShelfCommandExecuted, this.ShowShelfCommandCanExecute);
			this._changeNameCommand = _commandRepository.Build(enUICommand.ChangeIturName, this.ChangeNameCommandExecuted, this.ChangeNameCommandCanExecute);
            //this._changeStatusCommand = _commandRepository.Build(enUICommand.ChangeState, this.ChangeStatusCommandExecuted, this.ChangeStatusCommandCanExecute);
            this._changeStateCommand = _commandRepository.Build(enUICommand.ChangeState, this.ChangeStateCommandExecuted, ChangeStateCommandCanExecute);
            this._deleteCommand = _commandRepository.Build(enUICommand.Delete, this.DeleteCommandExecuted, this.DeleteCommandCanExecute);
			this._clearWithItemsCommand = _commandRepository.Build(enUICommand.ClearIturs, this.ClearWithItemsCommandExecuted, this.ClearWithItemsCommandCanExecute);
			this._clearWithItemsDialogCommand = _commandRepository.Build(enUICommand.ClearIturs, this.ClearWithItemsDialogCommandExecuted, this.ClearWithItemsDialogCommandCanExecute);
			this._openDetailsCommand = new DelegateCommand<IturItemViewModel>(OpenDetailsCommandExecuted);

			this._importCommand = _commandRepository.Build(enUICommand.Import, this.ImportCommandExecuted);
            this._editCommand = _commandRepository.Build(enUICommand.Edit, this.EditCommandExecuted, this.EditCommandCanExecute);
            this._reportCommand = _commandRepository.Build(enUICommand.Report, (ReportCommandExecuted));
			this._changeIturTagCommand = _commandRepository.Build(enUICommand.ChangeIturTag, this.ChangeIturTagCommandExecuted, this.ChangeIturTagCommandCanExecute);


            this._yesNoRequest = new InteractionRequest<MessageBoxYesNoNotification>();
            this._messageBoxRequest = new InteractionRequest<MessageBoxNotification>();
            this._repairCommand = _commandRepository.Build(enUICommand.RepairFromDb, RepairCommandExecuted);

            this._items = new ObservableCollection<IturItemViewModel>();
            this._searchCommand = _commandRepository.Build(enUICommand.Search, delegate { });
        }

		public DelegateCommand<IturItemViewModel> OpenDetailsCommand
		{
			get { return _openDetailsCommand; }
		}

		private void OpenDetailsCommandExecuted(IturItemViewModel item)
		{
			UriQuery query = new UriQuery();
			Utils.AddContextToQuery(query, base.Context);
			Utils.AddDbContextToQuery(query, base.CBIDbContext);
			Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());

			SelectParams selectParams = new SelectParams();
			selectParams.IsEnablePaging = false;
			selectParams.FilterParams.Add("IturCode", new FilterParam() { Operator = FilterOperator.Contains, Value = item.Code });

			InventProductDetailsData navigationData = new InventProductDetailsData();
			navigationData.IturSelectParams = selectParams;
			navigationData.IturCode = item.Code;
			navigationData.DocumentCode = "";
			navigationData.InventProductId = 0;
			//navigationData.SearchItem = this._searchItem;
			//navigationData.SearchExpression = this._searchExpression;

			UtilsConvert.AddObjectToQuery(query, this._navigationRepository, navigationData, NavigationObjects.InventProductDetails);

			UtilsNavigate.InventProductDetailsOpen(this._regionManager, query);
		}

		private void DeleteDocumentHeader(string locationCode, string iturCode, string documentHeaderCode, string adapterCode)
		{
			MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
			notification.Title = String.Empty;
			notification.Settings = this._userSettingsManager;
			notification.Content = String.Format(Localization.Resources.Msg_Delete_Document_Header, documentHeaderCode);
			this._yesNoRequest.Raise(notification, r =>
			{
				if (r.IsYes == true)
				{
					using (new CursorWait())
					{

						if (adapterCode == ImportAdapterName.ImportPdaMerkavaDB3Adapter
							|| adapterCode == ImportAdapterName.ImportPdaMerkavaXlsxAdapter
							|| adapterCode == ImportAdapterName.ImportPdaClalitSqliteAdapter
							|| adapterCode == ImportAdapterName.ImportPdaNativSqliteAdapter
							|| adapterCode == ImportAdapterName.ImportPdaYesXlsxAdapter
							|| adapterCode == ImportAdapterName.ImportPdaNativPlusSqliteAdapter
							|| adapterCode == ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter)
						{
							InventProducts inventProducts = this._inventProductRepository.GetInventProductsByDocumentCode(
								documentHeaderCode, base.GetDbPath);
							//Записать в темпоральную таблицу
							foreach (var inventProduct in inventProducts)
							{
								TemporaryInventory temporaryInventory = new TemporaryInventory();
								temporaryInventory.OldUid = inventProduct.Barcode.CutLength(249);
								string dateModified = (DateTime.Now).ConvertDateTimeToAndroid();
								temporaryInventory.DateModified = dateModified;
								temporaryInventory.OldSerialNumber = inventProduct.SerialNumber.CutLength(249);
								temporaryInventory.OldItemCode = inventProduct.Makat.CutLength(249);
								temporaryInventory.OldKey = iturCode;
								temporaryInventory.OldLocationCode = locationCode;
								temporaryInventory.Description = "Delete InventProduct with DocHeader in Count4U :" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
								temporaryInventory.Operation = "DELETE";
								temporaryInventory.Domain = "InventProduct";
								this._temporaryInventoryRepository.Insert(temporaryInventory, base.GetDbPath);
							}
						}

						this._documentHeaderRepository.Delete(documentHeaderCode, base.GetDbPath);
						try
						{
							this._iturRepository.RefillApproveStatusBitByIturCode(iturCode, base.GetDbPath);
						}
						catch (Exception exp)
						{
							_logger.ErrorException("DeleteDocumentHeader : RefillApproveStatusBitByIturCode :  ", exp);
						}
					}
				}
			});
		}

		public DelegateCommand ChangeIturTagCommand
		{
			get { return this._changeIturTagCommand; }
		}

		private bool ChangeIturTagCommandCanExecute()
		{
			//return true;
			return this._selectedItems != null && this._selectedItems.Count > 0;
		}

		private void ChangeIturTagCommandExecuted()
		{
			Iturs iturs = new Iturs();
			if (this._selectedItems != null)
			{
				iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));
			}

			IturTagChangeEventPayload payload = new IturTagChangeEventPayload();
			payload.Iturs = iturs;
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;

			this._eventAggregator.GetEvent<IturTagChangeEvent>().Publish(payload);
			BuildItems();
		}

        public string TotalFoundText
        {
            get { return this._totalFoundText; }
            set
            {
                this._totalFoundText = value;
                this.RaisePropertyChanged(() => this.TotalFoundText);
            }
        }

        public int PageSize
        {
            get { return this._pageSize; }
            set
            {
                this._pageSize = value;
                this.RaisePropertyChanged(() => this.PageSize);
            }
        }

        public int PageCurrent
        {
            get { return this._pageCurrent; }
            set
            {
                this._pageCurrent = value;
                this.RaisePropertyChanged(() => this.PageCurrent);

                using (new CursorWait())
                {
                    BuildItems();
                }
            }
        }

        public int ItemsTotal
        {
            get { return this._itemsTotal; }
            set
            {
                this._itemsTotal = value;
                this.RaisePropertyChanged(() => this.ItemsTotal);
            }
        }       

        public DelegateCommand AddCommand
        {
            get { return this._addCommand; }
        }

        public DelegateCommand ChangeLocationCommand
        {
            get { return this._changeLocationCommand; }
        }

		public DelegateCommand ChangeIturPrefixCommand
        {
			get { return this._changeIturPrefixCommand; }
        }


		public DelegateCommand ShowShelfCommand
        {
			get { return this._showShelfCommand; }
        }
		

		public DelegateCommand ChangeNameCommand
		{
			get { return this._changeNameCommand; }
		}

        public DelegateCommand DeleteCommand
        {
            get { return this._deleteCommand; }
        }


		public DelegateCommand ClearWithItemsCommand
        {
			get { return this._clearWithItemsCommand; }
        }


		public DelegateCommand ClearWithItemsDialogCommand
        {
			get { return this._clearWithItemsDialogCommand; }
        }

        public ObservableCollection<IturItemViewModel> Items
        {
            get { return _items; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<IturLocationChangedEvent>().Subscribe(this.ItursLocatonChanged);
			this._eventAggregator.GetEvent<IturPrefixChangedEvent>().Subscribe(this.ItursPrefixChanged);
			this._eventAggregator.GetEvent<IturNameChangedEvent>().Subscribe(this.ItursNameChanged);
            this._eventAggregator.GetEvent<IturStatusChangedEvent>().Subscribe(this.ItursStatusChanged);
            this._eventAggregator.GetEvent<ItursAddedEvent>().Subscribe(this.ItursAdded);
            this._eventAggregator.GetEvent<IturEditedEvent>().Subscribe(this.IturEdited);
			this._eventAggregator.GetEvent<IturDeletedEvent>().Subscribe(this.IturDeleted);
            this._eventAggregator.GetEvent<IturStateChangedEvent>().Subscribe(IturStateChanged);
            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(FilterData);
			this._eventAggregator.GetEvent<IturTagChangedEvent>().Subscribe(this.IturTagChanged);


            this._pageCurrent = 1;
            this._pageSize = this._userSettingsManager.PortionItursListGet();

            _filter = UtilsConvert.GetObjectFromNavigation(navigationContext, this._navigationRepository, Common.NavigationObjects.Filter, true) as IturFilterData;
            if (_filter == null)
                _filter = new IturFilterData();

            this.ReportButtonViewModel.OnNavigatedTo(navigationContext);
            this.ReportButtonViewModel.Initialize(this.ReportCommandExecuted, () =>
            {
                Locations locations = this._locationRepository.GetLocations(base.GetDbPath);
                //Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;
                SelectParams sp = BuildSelectParams(locations/*, statuses*/);

				return new Tuple<SelectParams, Itur, Location, DocumentHeader, Device>(sp, null, null, null, null);
            }, ViewDomainContextEnum.Itur);

            Utils.MainWindowTitleSet(WindowTitles.IturimAddEdit, this._eventAggregator);

			Task.Factory.StartNew(BuildItems).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<ItursAddedEvent>().Unsubscribe(this.ItursAdded);
            this._eventAggregator.GetEvent<IturLocationChangedEvent>().Unsubscribe(this.ItursLocatonChanged);
			this._eventAggregator.GetEvent<IturPrefixChangedEvent>().Unsubscribe(this.ItursPrefixChanged);
			
			this._eventAggregator.GetEvent<IturNameChangedEvent>().Unsubscribe(this.ItursNameChanged);
		
            this._eventAggregator.GetEvent<IturEditedEvent>().Unsubscribe(this.IturEdited);
			this._eventAggregator.GetEvent<IturDeletedEvent>().Unsubscribe(this.IturDeleted);
            this._eventAggregator.GetEvent<IturStatusChangedEvent>().Unsubscribe(this.ItursStatusChanged);
            this._eventAggregator.GetEvent<IturStateChangedEvent>().Unsubscribe(IturStateChanged);
            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Unsubscribe(FilterData);
			this._eventAggregator.GetEvent<IturTagChangedEvent>().Unsubscribe(this.IturTagChanged);

			foreach (IturItemViewModel iturViewModel in this._items) {
				iturViewModel.PropertyChanged -= IturViewModel_PropertyrChanged;
			}
   			Utils.RunOnUI(() => _items.Clear());

			this.ReportButtonViewModel.OnNavigatedFrom(navigationContext);
        }

        private SelectParams BuildSelectParams(Locations locations/*, Dictionary<string, StatusItur> statuses*/)
        {
            SelectParams result = new SelectParams();
            result.SortParams = "Number ASC";

            result.IsEnablePaging = true;
            result.CountOfRecordsOnPage = this._pageSize;
            result.CurrentPage = this._pageCurrent;

            if (_filter != null)
            {
                _filter.ApplyToSelectParams(result, locations/*, statuses*/);
            }

            return result;
        }

        private void BuildItems()
        {
            foreach (IturItemViewModel iturViewModel in this._items)
            {
                iturViewModel.PropertyChanged -= IturViewModel_PropertyrChanged;
            }

            SelectParams selectParams = null;

            Utils.RunOnUI(() => _items.Clear());

            try
            {
                Locations locations = this._locationRepository.GetLocations(base.GetDbPath);
                Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;

                selectParams = BuildSelectParams(locations/*, statuses*/);

                if (selectParams == null)
                {
                    Utils.RunOnUI(() => ItemsTotal = 0);
                }
                else
                {
                    Iturs iturs = this._iturRepository.GetIturs(selectParams, base.GetDbPath);
                    List<IturItemViewModel> uiItems = new List<IturItemViewModel>();
                    foreach (Itur itur in iturs)
                    {
                        IturItemViewModel viewModel = new IturItemViewModel(itur,
                                                                            locations.FirstOrDefault(r => r.Code == itur.LocationCode),
                                                                            statuses.Values.FirstOrDefault(r => r.Bit == itur.StatusIturBit));
                        uiItems.Add(viewModel);
                    }

                    Utils.RunOnUI(() =>
                                      {
                                          uiItems.ForEach(r => _items.Add(r));
                                          ItemsTotal = (int)iturs.TotalCount;
                                      });


                    if ((iturs.TotalCount > 0) && (iturs.Count == 0))	//do not show empty space - move on previous page               
                    {
                        Utils.RunOnUI(() => this.PageCurrent = this._pageCurrent - 1);
                    }
                }

                Utils.RunOnUI(() => TotalFoundText = String.Format("Total found: {0}", ItemsTotal));
            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildItems", exc);
                _logger.Error("ItemsTotal: {0}, PageCurrent: {1}, PageSize: {2}", this._itemsTotal, this._pageCurrent, this._pageSize);
                if (selectParams != null)
                    _logger.Error("SelectParams: {0}", selectParams.ToString());
                throw;
            }

            foreach (IturItemViewModel iturViewModel in this._items)
            {
                iturViewModel.PropertyChanged += IturViewModel_PropertyrChanged;
            }
        }

        void IturViewModel_PropertyrChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this._isGeneratedByCode) return;

            if (e.PropertyName == "IsDisabled")
            {
                using (new CursorWait())
                {
                    IturItemViewModel iturViewModel = sender as IturItemViewModel;
                    if (iturViewModel == null) return;

                    Itur itur = iturViewModel.Itur;
                    if (itur == null) return;

                    itur.Disabled = iturViewModel.IsDisabled;
                    this._iturRepository.Update(itur, base.GetDbPath);
					try
					{
						this._iturRepository.RefillApproveStatusBitByIturCode(itur.IturCode, base.GetDbPath);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("IturViewModel_PropertyrChanged : RefillApproveStatusBitByIturCode :  ", exp);
					}

                    this.IturUpdate(itur);

                    RecalculateIsDisabled();
                    RaisePropertyChanged(() => IsDisabled);
                }
            }
        }        

        public string this[string propertyName]
        {
            get
            {
               
                return null;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
        {
            get { return this._yesNoRequest; }
        }

        public DelegateCommand ImportCommand
        {
            get { return this._importCommand; }
        }

        public DelegateCommand EditCommand
        {
            get { return this._editCommand; }
        }

        //        public DelegateCommand ChangeStatusCommand
        //        {
        //            get { return this._changeStatusCommand; }
        //        }

        public string WarningText
        {
            get { return this._warningText; }
            set
            {
                this._warningText = value;
                this.RaisePropertyChanged(() => this.WarningText);
            }
        }

        public bool WarningTextIsVisible
        {
            get { return this._warningTextIsVisible; }
            set
            {
                this._warningTextIsVisible = value;
                this.RaisePropertyChanged(() => this.WarningTextIsVisible);
            }
        }

        public DelegateCommand ReportCommand
        {
            get { return _reportCommand; }
        }

        public bool? IsDisabled
        {
            get { return _isDisabled; }
            set
            {
                this._stopwatch = Stopwatch.StartNew();
                //this._stopwatch.Stop();
                //string timeSpent = String.Format("Start time: {0}", this._stopwatch.ElapsedTicks);

                this._isDisabled = value;

                RaisePropertyChanged(() => IsDisabled);

                using (new CursorWait())
                {
                    Iturs iturs = new Iturs();

                    foreach (var iturViewModel in this._selectedItems)
                    {
                        Itur itur = iturViewModel.Itur;
                        iturs.Add(itur);
                    }

                    bool disabled = (this._isDisabled == null) ? false : Convert.ToBoolean(this._isDisabled);

                    this._stopwatch.Stop();
                    System.Diagnostics.Debug.Print(String.Format("GUI time: {0}", this._stopwatch.ElapsedTicks.ToString()));

                    this._stopwatch = Stopwatch.StartNew();
                    this._iturRepository.SetDisabledStatusBitByIturCode(iturs, disabled, base.GetDbPath);
                    this._stopwatch.Stop();
                    System.Diagnostics.Debug.Print(String.Format("SetDisabled time: {0}", this._stopwatch.ElapsedTicks.ToString()));

                    this._stopwatch = Stopwatch.StartNew();

                    this.IturUpdate(iturs);
                    this._stopwatch.Stop();
                    System.Diagnostics.Debug.Print(String.Format("IturUpdate time: {0}", this._stopwatch.ElapsedTicks.ToString()));

                }
            }
        }

		private void IturTagChanged(IturTagChangedEventPayload payload)
		{
			Iturs iturs = new Iturs();
			foreach (Itur itur in payload.Iturs)
			{
				itur.Tag = payload.Tag;
				//location.Location = payload.Location.Name;
				iturs.Add(itur);
			}
			this._iturRepository.Update(iturs, base.GetDbPath);
		}

        public DelegateCommand ChangeStateCommand
        {
            get { return _changeStateCommand; }
        }

        public ReportButtonViewModel ReportButtonViewModel
        {
            get { return _reportButtonViewModel; }
        }

        public void SelectedItemsSet(List<IturItemViewModel> items)
        {
            this._selectedItems = items;

            this._changeLocationCommand.RaiseCanExecuteChanged();
			this._changeIturPrefixCommand.RaiseCanExecuteChanged();
			this._showShelfCommand.RaiseCanExecuteChanged();
            this._deleteCommand.RaiseCanExecuteChanged();
            this._editCommand.RaiseCanExecuteChanged();
            //this._changeStatusCommand.RaiseCanExecuteChanged();
            this._changeStateCommand.RaiseCanExecuteChanged();
			this._changeIturTagCommand.RaiseCanExecuteChanged();

            RecalculateIsDisabled();

            RaisePropertyChanged(() => IsDisabled);
            RaisePropertyChanged(() => IsAnyItemSelected);
        }

        private void RecalculateIsDisabled()
        {
            if (_selectedItems.All(r => r.IsDisabled))
                _isDisabled = true;
            else if (_selectedItems.All(r => !r.IsDisabled))
                _isDisabled = false;
            else
                _isDisabled = null;
        }

        public bool IsAnyItemSelected
        {
            get { return this._selectedItems != null && this._selectedItems.Count > 0; }
        }

        public InteractionRequest<MessageBoxNotification> MessageBoxRequest
        {
            get { return _messageBoxRequest; }
        }

        public DelegateCommand RepairCommand
        {
            get { return _repairCommand; }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public IturFilterData Filter
        {
            get { return _filter; }
        }

        public bool IsFilterAnyField
        {
            get { return _filter == null ? false : _filter.IsAnyField(); }
        }

        private void AddCommandExecuted()
        {
            IturAddEventPayload payload = new IturAddEventPayload { Itur = null, Context = base.Context, DbContext = base.CBIDbContext };

            this._eventAggregator.GetEvent<IturAddEvent>().Publish(payload);
        }


		private void ClearWithItemsDialogCommandExecuted()
        {
			IturClearWithItemsEventPayload payload = new IturClearWithItemsEventPayload { Itur = null, Context = base.Context, DbContext = base.CBIDbContext };

			this._eventAggregator.GetEvent<IturClearWithItemsEvent>().Publish(payload);
        }



		private bool ClearWithItemsCommandCanExecute()
        {
			#if DEBUG
			#else
				if (base.CurrentCustomer.Code != "610") return false;
			#endif
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }


		private bool ClearWithItemsDialogCommandCanExecute()
        {
			#if DEBUG
			#else
				if (base.CurrentCustomer.Code != "610") return false;
			#endif
            return true;
        }

		private void ClearWithItemsCommandExecuted()
        {
			MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();
		
                notification.Title = String.Empty;
                notification.Settings = this._userSettingsManager;
                string itursNumbers = CommaDashStringParser.Reverse(this._selectedItems.Select(r => Int32.Parse(r.Number)).ToList());
				notification.Content = String.Format(Localization.Resources.Msg_Delete_IturHierarchical, itursNumbers);
                this._yesNoRequest.Raise(notification, r =>
                    {
                        if (r.IsYes)
                        {
                            using (new CursorWait())
                            {
                                foreach (Itur itur in this._selectedItems.Select(z => z.Itur))
                                {
									this._iturRepository.ClearIturHierarchical(itur, base.GetDbPath);
                                }

                                BuildItems();
                            }
                        }
                    }
                    );
        }

		private bool DeleteCommandCanExecute()
        {
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }

		private void DeleteCommandExecuted()
		{
			MessageBoxYesNoNotification notification = new MessageBoxYesNoNotification();

			IturItemViewModel emptyItur = _selectedItems.FirstOrDefault(r => r.Itur.StatusIturGroupBit == (int)IturStatusGroupEnum.Empty);
			if (emptyItur == null)
			{
				notification.Title = String.Empty;
				notification.Settings = this._userSettingsManager;

				string message = Localization.Resources.Msg_IturNotEmpty;
				notification.Content = message;
				_yesNoRequest.Raise(notification, r =>
				{
					if (r.IsYes)
					{
						using (new CursorWait())
						{
							var iturs1 = this._selectedItems.Select(z => z.Itur);
							Iturs iturs = Iturs.FromEnumerable(iturs1);
							this._iturRepository.DeleteOnlyEmpty(iturs, base.GetDbPath);
							BuildItems();
						}

					}
				});
			}
			else
			{
				notification.Title = String.Empty;
				notification.Settings = this._userSettingsManager;
				string itursNumbers = CommaDashStringParser.Reverse(this._selectedItems.Select(r => Int32.Parse(r.Number)).ToList());
				notification.Content = String.Format(Localization.Resources.Msg_Delete_Itur, itursNumbers);
				this._yesNoRequest.Raise(notification, r =>
				{
					if (r.IsYes)
					{
						using (new CursorWait())
						{
							foreach (Itur itur in this._selectedItems.Select(z => z.Itur))
							{
								this._iturRepository.Delete(itur, base.GetDbPath);
							}

							BuildItems();
						}
					}
				}
					);
			}
		}

        private void ImportCommandExecuted()
        {
            UriQuery uriQuery = new UriQuery();
            Utils.AddContextToQuery(uriQuery, base.Context);
            Utils.AddDbContextToQuery(uriQuery, base.CBIDbContext);
            uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ImportModeItur);
            Utils.AddAuditConfigToQuery(uriQuery, GetAuditConfigByCurrentContext());
            UtilsNavigate.ImportWithModulesOpen(this._regionManager, uriQuery);
        }

        private bool ChangeLocationCommandCanExecute()
        {
			//return true;
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }

		private bool ChangeIturPrefixCommandCanExecute()
        {
			//if (this._inventProductRepository.IsAnyInventProductInDb(base.GetDbPath) == true) return false; вернуть
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }

		private bool ShowShelfCommandCanExecute()
        {
			//if (this._inventProductRepository.IsAnyInventProductInDb(base.GetDbPath) == true) return false; вернуть
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }
		

		private bool ChangeNameCommandCanExecute()
		{
			return this._selectedItems != null && this._selectedItems.Count > 0;
		}

        private bool ChangeStatusCommandCanExecute()
        {
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }

        private void ChangeStatusCommandExecuted()
        {
            Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

            IturStatusChangeEventPayload payload = new IturStatusChangeEventPayload();
            payload.Iturs = iturs;
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;

            this._eventAggregator.GetEvent<IturStatusChangeEvent>().Publish(payload);
        }

        private void ChangeLocationCommandExecuted()
        {
			Iturs iturs = new Iturs();
			if (this._selectedItems != null)
			{
				iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));
			}

            IturLocationChangeEventPayload payload = new IturLocationChangeEventPayload();
            payload.Iturs = iturs;
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;

            this._eventAggregator.GetEvent<IturLocationChangeEvent>().Publish(payload);
        }

		private void ChangeIturPrefixCommandExecuted()
		{
			Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

			IturPrefixChangeEventPayload payload = new IturPrefixChangeEventPayload();
			payload.Iturs = iturs;
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;

			this._eventAggregator.GetEvent<IturPrefixChangeEvent>().Publish(payload);
		}

		private void ShowShelfCommandExecuted()//todo
		{
			Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

			ShowShelfEventPayload payload = new ShowShelfEventPayload();
			payload.Iturs = iturs;
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;

			this._eventAggregator.GetEvent<ShowShelfEvent>().Publish(payload);
		}


		private void ChangeNameCommandExecuted()
		{
			Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

			IturNameChangeEventPayload payload = new IturNameChangeEventPayload();
			payload.Iturs = iturs;
			payload.Context = base.Context;
			payload.DbContext = base.CBIDbContext;

			this._eventAggregator.GetEvent<IturNameChangeEvent>().Publish(payload);
		}

        private void ItursLocatonChanged(IturLocationChangedEventPayload payload)
        {
            this._isGeneratedByCode = true;
            Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;
            Iturs iturs = new Iturs();
            foreach (Itur itur in payload.Iturs)
            {
                itur.LocationCode = payload.Location.Code;
				itur.Name1 = payload.Location.Name;
                iturs.Add(itur);
            }

            this._iturRepository.Update(iturs, base.GetDbPath);

            foreach (Itur itur in payload.Iturs)
            {
                IturItemViewModel itemViewModel = this._items.FirstOrDefault(r => r.Itur.IturCode == itur.IturCode);
                if (itemViewModel != null)
                {
                    itemViewModel.UpdateViewModel(itur, payload.Location, statuses.Values.FirstOrDefault(r => r.Bit == itur.StatusIturBit));
                }
            }
            this._isGeneratedByCode = false;
        }

		private void ItursPrefixChanged(IturPrefixChangedEventPayload payload)
		{
			 
			this._isGeneratedByCode = true;
			Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;
			Iturs iturs = new Iturs();
			string prefixNew = "";
			if (payload.PrefixNew.Length == 4) prefixNew = payload.PrefixNew;
			if (payload.PrefixNew.Length > 4) prefixNew = payload.PrefixNew.Substring(0, 4);
			if (payload.PrefixNew.Length < 0) prefixNew = payload.PrefixNew.PadLeft(4, '0');

			if (payload.AllChange == false)
			{
				foreach (Itur itur in payload.Iturs)
				{
					itur.NumberPrefix = prefixNew;
					iturs.Add(itur);
				}
				this._iturRepository.Update(iturs, base.GetDbPath);
			}
			else
			{
				this._iturRepository.UpdatePrefix(prefixNew, base.GetDbPath);
			}
			

			foreach (Itur itur in payload.Iturs)
			{
				Locations locations = this._locationRepository.GetLocations(base.GetDbPath);
				//перерисовка
				IturItemViewModel itemViewModel = this._items.FirstOrDefault(r => r.Itur.IturCode == itur.IturCode);
				if (itemViewModel != null)
				{
					itemViewModel.UpdateViewModel(itur, locations.FirstOrDefault(r => r.Code == itur.LocationCode), statuses.Values.FirstOrDefault(r => r.Bit == itur.StatusIturBit));
				}
			}

			if (payload.AllChange == false)
			{
				Iturs iturs1 = new Iturs();
				foreach (Itur itur in payload.Iturs)
				{
					itur.Description = payload.PrefixNew + itur.NumberSufix;
					iturs1.Add(itur);
				}

				this._iturRepository.UpdateIturCode(iturs1, base.GetDbPath);
			}
			else
			{
				this._iturRepository.UpdateIturCode( base.GetDbPath);
			}

			this._isGeneratedByCode = false;

			if (payload.AllChange == true)
			{
				if (UtilsNavigate.CanGoBack(this._regionManager))
					UtilsNavigate.GoBack(this._regionManager);
			}
		}


		private void ItursNameChanged(IturNameChangedEventPayload payload)
		{
			this._isGeneratedByCode = true;
			Iturs iturs = new Iturs();
			if (payload.Iturs.Count > 0)
			{
				Itur itur = payload.Iturs[0];
				itur.Name = payload.Name;
				itur.ERPIturCode = "";
				if (payload.ERPCode != null)	itur.ERPIturCode = payload.ERPCode.CutLength(249);
				iturs.Add(itur);
			}

			//foreach (Itur itur in payload.Iturs)
			//{
			//	itur.Name = payload.Name;
			//	itur.ERPIturCode = payload.ERPCode;
			//	iturs.Add(itur);
			//}
			//this._stopwatch.Stop();
			//System.Diagnostics.Debug.Print(String.Format("GUI time: {0}", this._stopwatch.ElapsedTicks.ToString()));

			//this._stopwatch = Stopwatch.StartNew();
			this._iturRepository.Update(iturs, base.GetDbPath);
			//this._stopwatch.Stop();
			//System.Diagnostics.Debug.Print(String.Format("ItursNameChanged time: {0}", this._stopwatch.ElapsedTicks.ToString()));

			//this._stopwatch = Stopwatch.StartNew();
			this.IturUpdate(iturs);
			//this._stopwatch.Stop();
			//System.Diagnostics.Debug.Print(String.Format("GUI refresh time: {0}", this._stopwatch.ElapsedTicks.ToString()));
			this._isGeneratedByCode = false;
		}

        private void ItursStatusChanged(IturStatusChangedEventPayload payload)
        {
            this._isGeneratedByCode = true;
            Iturs iturs = new Iturs();
            foreach (Itur itur in payload.Iturs)
            {
                //itur.LocationID = payload.Location.ID;
                itur.StatusIturCode = payload.Status.Code;
                itur.StatusIturBit = payload.Status.Bit;
                iturs.Add(itur);
            }

            this._iturRepository.Update(iturs, base.GetDbPath);

            foreach (Itur itur in payload.Iturs)
            {
                Locations locations = this._locationRepository.GetLocations(base.GetDbPath);

                IturItemViewModel itemViewModel = this._items.FirstOrDefault(r => r.Itur.IturCode == itur.IturCode);
                if (itemViewModel != null)
                {
                    itemViewModel.UpdateViewModel(itur, locations.FirstOrDefault(r => r.Code == itur.LocationCode), payload.Status);
                }
            }
            this._isGeneratedByCode = false;
        }

        private void IturStateChanged(ItursStateChangedEventPayload payload)
        {
            Iturs iturs = new Iturs();
            this._stopwatch = Stopwatch.StartNew();

            foreach (Itur itur in payload.Iturs)
            {
                iturs.Add(itur);
            }
            this._stopwatch.Stop();
            System.Diagnostics.Debug.Print(String.Format("GUI time: {0}", this._stopwatch.ElapsedTicks.ToString()));

            this._stopwatch = Stopwatch.StartNew();
            bool disabled = (this._isDisabled == null) ? false : Convert.ToBoolean(payload.Disabled);
            this._iturRepository.SetDisabledStatusBitByIturCode(iturs, disabled, base.GetDbPath);
            this._stopwatch.Stop();
            System.Diagnostics.Debug.Print(String.Format("Disabled time: {0}", this._stopwatch.ElapsedTicks.ToString()));

            this._stopwatch = Stopwatch.StartNew();
            this.IturUpdate(iturs);
            this._stopwatch.Stop();
            System.Diagnostics.Debug.Print(String.Format("GUI refresh time: {0}", this._stopwatch.ElapsedTicks.ToString()));

        }

        private void ItursAdded(ItursAddedEventPayload payload)
        {
            BuildItems();

            if (payload.ItursThatExist.Count > 0)
            {
                string numbers = CommaDashStringParser.Reverse(payload.ItursThatExist);
                WarningText = String.Format(Localization.Resources.ViewModel_IturAddEditDelete_AlreadyExist, numbers);
                WarningTextIsVisible = true;

                Observable
                .Timer(TimeSpan.FromSeconds(5))
                .ObserveOnDispatcher()
                .Subscribe(_ => WarningTextIsVisible = false);
            }
        }

        private void IturEdited(Itur itur)
        {
			try
			{
				this._iturRepository.RefillApproveStatusBitByIturCode(itur.IturCode, base.GetDbPath);
			}
			catch (Exception exp)
			{
				_logger.ErrorException("IturEdited : RefillApproveStatusBitByIturCode :  ", exp);
			}

            IturUpdate(itur);
        }


		private void IturDeleted(bool rebuild)
		{
			this.BuildItems();
		}

        private void IturUpdate(Itur itur)
        {
            IturItemViewModel viewModel = this._items.FirstOrDefault(r => r.Code == itur.IturCode);
            if (viewModel == null) return;

            Itur dbItur = this._iturRepository.GetIturByCode(itur.IturCode, base.GetDbPath);
			if (dbItur == null) return;
            Location location = this._locationRepository.GetLocationByCode(dbItur.LocationCode, base.GetDbPath);
            Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;

            _isGeneratedByCode = true;
            viewModel.UpdateViewModel(dbItur, location, statuses.Values.FirstOrDefault(r => r.Bit == dbItur.StatusIturBit));
            _isGeneratedByCode = false;
        }

        private void IturUpdate(Iturs iturs)
        {
            List<string> searchItur = iturs.Select(r => r.IturCode).Distinct().ToList();
            SelectParams sp = new SelectParams();
            if (searchItur.Count != 0)
            {
                sp.FilterStringListParams.Add("IturCode", new FilterStringListParam()
                {
                    Values = searchItur
                });
            }

            Iturs dbIturs = this._iturRepository.GetIturs(sp, base.GetDbPath);
            Dictionary<string, StatusItur> iturStatusDictionary = this._statusIturRepository.CodeStatusIturDictionary;
            Dictionary<string, Location> locationDictionary = this._locationRepository.GetLocationDictionary(base.GetDbPath, true);

            foreach (Itur dbItur in dbIturs)
            {
                IturItemViewModel viewModel = this._items.FirstOrDefault(r => r.Code == dbItur.IturCode);
                if (viewModel == null) continue;
                Location location = null;
                if (locationDictionary.ContainsKey(dbItur.LocationCode))
                {
                    location = locationDictionary[dbItur.LocationCode];
                }

                this._isGeneratedByCode = true;
                viewModel.UpdateViewModel(dbItur, location, iturStatusDictionary.Values.FirstOrDefault(r => r.Bit == dbItur.StatusIturBit));
                this._isGeneratedByCode = false;
            }
        }


        private bool EditCommandCanExecute()
        {
            return this._selectedItems != null && this._selectedItems.Count == 1;
        }

        private void EditCommandExecuted()
        {
            Itur itur = this._selectedItems.First().Itur;

            IturEditEventPayload payload = new IturEditEventPayload();
            payload.Itur = itur;
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;

            this._eventAggregator.GetEvent<IturEditEvent>().Publish(payload);
        }

        private void ReportCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddContextToQuery(query, base.Context);
            Utils.AddDbContextToQuery(query, base.CBIDbContext);
            Utils.AddAuditConfigToQuery(query, base.GetAuditConfigByCurrentContext());
            UtilsConvert.AddEnumToUriQuery(query, typeof(ViewDomainContextEnum), ViewDomainContextEnum.Iturs);
            UtilsConvert.AddEnumToUriQuery(query, typeof(AllowedReportTemplate), AllowedReportTemplate.Main);

            Locations locations = this._locationRepository.GetLocations(base.GetDbPath);
//            Dictionary<string, StatusItur> statuses = this._statusIturRepository.CodeStatusIturDictionary;
            SelectParams sp = BuildSelectParams(locations/*, statuses*/);

            Utils.AddSelectParamsToQuery(query, sp);
            UtilsNavigate.ReportTemplateOpen(this._regionManager, query);
        }

        private bool ChangeStateCommandCanExecute()
        {
            return this._selectedItems != null && this._selectedItems.Count > 0;
        }

        private void ChangeStateCommandExecuted()
        {
            Iturs iturs = Iturs.FromEnumerable(this._selectedItems.Select(r => r.Itur));

            ItursStateChangeEventPayload payload = new ItursStateChangeEventPayload();
            payload.Iturs = iturs;
            payload.Context = base.Context;
            payload.DbContext = base.CBIDbContext;

            this._eventAggregator.GetEvent<IturStateChangeEvent>().Publish(payload);
        }

        private void RepairCommandExecuted()
        {
            MessageBoxResult messageBoxResult = UtilsMisc.ShowMessageBox(Localization.Resources.Message_Repair, MessageBoxButton.YesNo, MessageBoxImage.Question, _userSettingsManager);

            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            using (new CursorWait())
            {
				string fromDbPath =  "";	  //только по специальному требованию
				//string fromDbPath = @"Inventor\2013\11\24\d831cf8a-694a-4eda-946e-e29cc3518dde"; //из строки path на форме

				//IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
				if (fromDbPath != "")
				{
					try
					{
						//Iturs itursFromDB =  this._iturRepository.GetIturs(fromDbPath);
						//this._iturRepository.Insert(itursFromDB, base.GetDbPath);

						IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
						InventProducts inventProductFromDB = inventProductRepository.GetInventProducts(fromDbPath);
						inventProductRepository.Insert(inventProductFromDB, base.GetDbPath);

						IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
						DocumentHeaders documentHeaderFromDB = documentHeaderRepository.GetDocumentHeaders(fromDbPath);
						documentHeaderRepository.Insert(documentHeaderFromDB, base.GetDbPath);
					}
					catch (Exception exc)
					{
						_logger.ErrorException("BuildItems", exc);
					}
				}

				this._iturRepository.RepairCodeFromDB(base.GetDbPath);

				this._iturRepository.RefillApproveStatusBit(base.GetDbPath);

                UtilsMisc.ShowMessageBox(Localization.Resources.Msg_RestoreDone, MessageBoxButton.OK, MessageBoxImage.Information, _userSettingsManager);
                this.BuildItems();
            }
        }

        private void FilterData(IFilterData filterData)
        {
            this._filter = filterData as IturFilterData;

            RaisePropertyChanged(() => IsFilterAnyField);

            BuildItems();
        }
    }
}