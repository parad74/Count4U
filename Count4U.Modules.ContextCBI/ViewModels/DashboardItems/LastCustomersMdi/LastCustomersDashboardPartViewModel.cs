using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.ContextCBI.Interfaces;
using Count4U.Modules.ContextCBI.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Main;
using System.Linq;
using Count4U.Common.Extensions;
using Count4U.Common.UserSettings;
using Count4U.Common;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems
{
    public class LastCustomersDashboardPartViewModel : CBIContextBaseViewModel, IMdiChild
    {
        public enum LastCustomersDashboardPartViewModelMode
        {
            LastInInventory,
            LastBuild
        }

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICustomerRepository _customerRepository;
		private readonly IBranchRepository _branchRepository;

        private readonly DelegateCommand<LastCustomersListItem> _customerNavigateCommand;
        private readonly DelegateCommand _moreCustomersCommand;
		private readonly DelegateCommand _packSelectedCustomersCommand;
		private readonly DelegateCommand _deleteSelectedCustomerObjectsCommand;
		private readonly DelegateCommand _addPackSelectedCustomersCommand;
		private readonly DelegateCommand _clearCodeCustomerListCommand;
		
	//	private readonly DelegateCommand<LastCustomersListItem> _editSelectedCommand;
        private readonly UICommandRepository _commandRepository;
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly UICommandRepository<LastCustomersListItem> _commandRepositoryObject;

        private string _totalCustomers;
        private readonly ObservableCollection<LastCustomersListItem> _items;

        public LastCustomersDashboardPartViewModelMode _mode;

        private string _find;
		private bool _useCustomerFilter;
		private string _customerCodeList;
		private string _customerCodeListCount;
		protected bool _isChecked;

        public LastCustomersDashboardPartViewModel(
            IContextCBIRepository contextCBIRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            ICustomerRepository customerRepository,
			  IBranchRepository branchRepository,
            UICommandRepository commandRepository,
			UICommandRepository<LastCustomersListItem> commandRepositoryObject,
			IUserSettingsManager userSettingsManager
			)
            : base(contextCBIRepository)
        {
			this._commandRepository = commandRepository;
            this._customerRepository = customerRepository;
			this._branchRepository = branchRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
			this._userSettingsManager = userSettingsManager;
			this._commandRepositoryObject = commandRepositoryObject;

            this._customerNavigateCommand = new DelegateCommand<LastCustomersListItem>(this.CustomerNavigateCommandExecuted);

			
            this._moreCustomersCommand = _commandRepository.Build(enUICommand.More, MoreCustomersCommandExecuted);
			this._packSelectedCustomersCommand = _commandRepository.Build(enUICommand.Pack, this.PackSelectedCustomersCommandExecuted);
			this._deleteSelectedCustomerObjectsCommand = _commandRepository.Build(enUICommand.DeleteObjectsFromMainDB, this.DeleteSelectedCustomerObjectsCommandExecuted, DeleteSelectedCustomerObjectsCommandCanExecute);
			this._addPackSelectedCustomersCommand = _commandRepository.Build(enUICommand.AddPack, this.AddPackSelectedCustomersCommandExecuted);
			this._clearCodeCustomerListCommand = _commandRepository.Build(enUICommand.ClearList, this.ClearCodeCustomerListCommandExecuted);


		//	this._editSelectedCommand = _commandRepositoryObject.Build(enUICommand.Edit, this.EditSelectedCommandExecuted);
			this._customerCodeListCount = "0";
            this._items = new ObservableCollection<LastCustomersListItem>();
        }



		private bool DeleteSelectedCustomerObjectsCommandCanExecute()
		{
			return _customerCodeListCount != "0";
		}


		public DelegateCommand DeleteSelectedCustomerObjectsCommand
		{
			get { return this._deleteSelectedCustomerObjectsCommand; }
		}
	

	
		//public DelegateCommand<LastCustomersListItem> EditSelectedCommand
		//{
		//	get { return this._editSelectedCommand; }
		//}

		//private void EditCommandExecuted()
		//{
		//	if (_current != null)
		//	{
		//		_editSelectedCommand.Execute(_current);
		//	}
		//}

		//private void EditSelectedCommandExecuted(LastCustomersListItem customer)
		//{
		//	//this._eventAggregator.GetEvent<CustomerEditEvent>().Publish(
		//	//	new CustomerEditEventPayload() { Customer = customer.Customer, Context = this.Context });
		//}

        public DelegateCommand<LastCustomersListItem> CustomerNavigateCommand
        {
            get { return this._customerNavigateCommand; }
        }

        public DelegateCommand MoreCustomersCommand
        {
            get { return this._moreCustomersCommand; }
        }

		public DelegateCommand PackSelectedCustomersCommand
        {
			get { return this._packSelectedCustomersCommand; }
        }

		
		

		public DelegateCommand AddPackSelectedCustomersCommand
		{
			get { return this._addPackSelectedCustomersCommand; }
		}

		public DelegateCommand ClearCodeCustomerListCommand
		{
			get { return this._clearCodeCustomerListCommand; }
		}

		public bool IsChecked
		{
			get { return this._isChecked; }
			set
			{
				this._isChecked = value;
				RaisePropertyChanged(() => IsChecked);
				foreach (LastCustomersListItem item in this._items)
				{
					item.CheckSelect = this._isChecked;
					RaisePropertyChanged(() => item.CheckSelect);
				}
			}
		}
		

        public string TotalCustomers
        {
            get { return _totalCustomers; }
            set
            {
                _totalCustomers = value;
                RaisePropertyChanged(() => TotalCustomers);
            }
        }

        public ObservableCollection<LastCustomersListItem> Items
        {
            get { return _items; }
        }

		public List<Customer> Customers
		{
			get
			{
				List<Customer> customers = _items.Where(x=>x.CheckSelect == true).Select(x => x.Customer).ToList();
				return customers;
			}
		}

        public string Find
        {
            get { return _find; }
            set
            {
                if (_find != value)
                {
                    _find = value;
                    RaisePropertyChanged(() => Find);

                    ItemsBuild();
                }
            }
        }

		public string CustomerCodeList
        {
			get { return _customerCodeList; }
            set
            {
				if (_customerCodeList != value)
                {
					_customerCodeList = value;
					RaisePropertyChanged(() => CustomerCodeList);
                }
            }
        }


		public string CustomerCodeListCount
        {
			get { return _customerCodeListCount; }
            set
            {
				if (_customerCodeListCount != value)
                {
					_customerCodeListCount = value;
					RaisePropertyChanged(() => CustomerCodeListCount);
                }
            }
        }
		
	

		public bool UseCustomerFilter
		{
		//	get { return _useCustomerFilter; }
			get
			{
				return this._userSettingsManager.UseCustomerFilterGet();
			}
			//set
			//{
			//	_useCustomerFilter = value;
			//	RaisePropertyChanged(() => UseCustomerFilter);

		//		_userSettingsManager.UseCustomerFilterSet(_useCustomerFilter);
			//}
		}

        public bool ShowFindTextBox
        {
            get { return _mode == LastCustomersDashboardPartViewModelMode.LastBuild; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._eventAggregator.GetEvent<CustomerAddedEvent>().Subscribe(CustomerAdded);
            this._eventAggregator.GetEvent<CustomerEditedEvent>().Subscribe(CustomerEdited);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.LastCustomers))
            {
                string value = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.LastCustomers).Value;
                this._mode = value == Common.NavigationSettings.LastCustomersInInventory ?
                                 LastCustomersDashboardPartViewModelMode.LastInInventory :
                                 LastCustomersDashboardPartViewModelMode.LastBuild;
            }

			Task.Factory.StartNew(ItemsBuild).LogTaskFactoryExceptions("OnNavigatedTo");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            Clear();
        }

        private void CustomerNavigateCommandExecuted(LastCustomersListItem item)
        {
            AuditConfig config = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History);
            if (config != null)
            {
                base.ContextCBIRepository.SetCurrentCustomer(item.Customer, config);
                base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.History).ClearBranch();
            }

            AuditConfig newMainAuditConfig = base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            if (newMainAuditConfig != null)
            {
                base.ContextCBIRepository.SetCurrentCustomer(item.Customer, newMainAuditConfig);
                base.ContextCBIRepository.GetCurrentCBIConfig(CBIContext.Main).ClearBranch();
            }

            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetMainAuditConfig());
            UtilsNavigate.CustomerDashboardOpen(CBIContext.Main, this._regionManager, query);
        }


		private void PackSelectedCustomersCommandExecuted()
		{
				//	    private void PackOpen()
    
            AuditConfig ac = this._contextCBIRepository.GetCurrentCBIConfig(CBIContext.Main);
            ac.Clear();
            UriQuery uriQuery = new UriQuery();
			string countInCodeList = "";
			string codes = CheckCustomer(CustomerCodeList, out countInCodeList);
			CustomerCodeList = codes;
			CustomerCodeListCount = countInCodeList;
			RaisePropertyChanged(() => CustomerCodeListCount);
			RaisePropertyChanged(() => CustomerCodeList);
			this.DeleteSelectedCustomerObjectsCommand.RaiseCanExecuteChanged();

			//uriQuery.Add(NavigationSettings.CustomerCodes, Customers.Select(r => r.Code).Aggregate(String.Empty, (r, z) => r += String.Format("{0},", z)));
			uriQuery.Add(NavigationSettings.CustomerCodes, codes);
            Utils.AddContextToQuery(uriQuery, CBIContext.Main);
            Utils.AddAuditConfigToQuery(uriQuery, ac);
  
            UtilsNavigate.PackOpen(this._regionManager, uriQuery);
		}


		private void DeleteSelectedCustomerObjectsCommandExecuted()
		{
			List<string> custCode = _items.Where(x => x.CheckSelect == true).Select(x => x.Code).ToList();

			Branches branches = _branchRepository.GetBranches();

			UtilsMisc.ShowMessageBox(Localization.Resources.ViewModel_msgDeleteCustomerWithBranch, MessageBoxButton.OK, MessageBoxImage.Warning, _userSettingsManager);
 		
			using (new CursorWait())
			{
				this._customerRepository.DeleteDomainObjectOnly(custCode);
				foreach (string code in custCode)
				{
					List<string> branchesCode = branches.Where(x => x.CustomerCode == code).Select(x => x.Code).ToList();
					this._branchRepository.DeleteDomainObjectOnly(branchesCode);
				}

				ItemsBuild();
			}
			//string countInCodeList = "0";
			//string codes = CheckCustomer(CustomerCodeList, out countInCodeList);
			//CustomerCodeList = codes;
			CustomerCodeListCount = "0";
			CustomerCodeList = "";
			RaisePropertyChanged(() => CustomerCodeListCount);
			RaisePropertyChanged(() => CustomerCodeList);
			this.DeleteSelectedCustomerObjectsCommand.RaiseCanExecuteChanged();
			//uriQuery.Add(NavigationSettings.CustomerCodes, codes);
			//Utils.AddContextToQuery(uriQuery, CBIContext.Main);
			//Utils.AddAuditConfigToQuery(uriQuery, ac);
  
            //UtilsNavigate.PackOpen(this._regionManager, uriQuery);
		}

	
		private void AddPackSelectedCustomersCommandExecuted()
		{
			string[] custCode =  _items.Where(x => x.CheckSelect == true).Select(x => x.Code).ToArray();
			string CustomerCodeStringIn = string.Join(",", custCode);
			string countInCodeList = "";
			string CustomerCodeStringChecked = CheckCustomer(CustomerCodeStringIn, out countInCodeList);

			if (string.IsNullOrWhiteSpace(CustomerCodeList) == true)
			{
				CustomerCodeList = CustomerCodeStringChecked.Trim(',');
			}
			else
			{
				CustomerCodeList = CustomerCodeList + "," + CustomerCodeStringChecked.Trim(',');
			}


			countInCodeList = "";
			string codes = CheckCustomer(CustomerCodeList, out countInCodeList);
			CustomerCodeList = codes;
			CustomerCodeListCount = countInCodeList;

			RaisePropertyChanged(() => CustomerCodeListCount);
			RaisePropertyChanged(() => CustomerCodeList);
			this.DeleteSelectedCustomerObjectsCommand.RaiseCanExecuteChanged();
		}

		private string CheckCustomer(string codeList, out string countInCodeList)
		{
			string ret = "";
			countInCodeList = "0";
			if (string.IsNullOrWhiteSpace(codeList) == true) return ret;
			List<string> retList = new List<string>();
			List<string> customerCodesDb = this._customerRepository.GetCodeList();
			string[] customerCodesInput = codeList.Split(',');
			foreach (string code in customerCodesInput)
			{
				if (string.IsNullOrWhiteSpace(code) == true) continue;
				if (customerCodesDb.Contains(code) == true)
				{
					if (retList.Contains(code) == false)
					{
						retList.Add(code);
					}
				}
			}
			countInCodeList = retList.Count.ToString(); 
			if (retList.Count == 0) return ret;
			ret = string.Join(",", retList);
			return ret.Trim(',');
		}

		private void ClearCodeCustomerListCommandExecuted()
		{
			CustomerCodeList = "";
			CustomerCodeListCount = "0";
			RaisePropertyChanged(() => CustomerCodeList);
			RaisePropertyChanged(() => CustomerCodeListCount);
			this.DeleteSelectedCustomerObjectsCommand.RaiseCanExecuteChanged();
		}

        private void MoreCustomersCommandExecuted()
        {
            UriQuery query = new UriQuery();
            Utils.AddAuditConfigToQuery(query, GetAuditConfigByCurrentContext());
            UtilsNavigate.CustomerChooseOpen(CBIContext.Main, this._regionManager, query);
        }

        private void ItemsBuild()
        {
            List<LastCustomersListItem> items = new List<LastCustomersListItem>();
            int total = 0;

            List<Customer> customers = new List<Customer>();
            Customers all = this._customerRepository.GetCustomers(new SelectParams());
            total = all.Count;
			Dictionary<string, Customer> dictionary = all.Select(x => x).Distinct().ToDictionary(x => x.Code); //из Maindb

            switch (_mode)
            {
                case LastCustomersDashboardPartViewModelMode.LastInInventory:
                    Customers custs = base.ContextCBIRepository.GetHistoryCustomerLast();	//из AuditDB
					if (custs != null)
					{
						total = custs.Count;
						// customers = custs.Take(100).ToList();
						customers = custs.ToList();
						int k = 0;
						foreach (Customer customer in customers)	//из AuditDB
						{
							if (customer != null)
							{
								if (dictionary.ContainsKey(customer.Code) == true)		 //из Maindb
								{
									if (k <= 30)
									{
										customer.ModifyDate = dictionary[customer.Code].ModifyDate;
										LastCustomersListItem item = new LastCustomersListItem(customer);
										items.Add(item);
										k++;
									}
								}
							}
						}
					}
                    break;
                case LastCustomersDashboardPartViewModelMode.LastBuild:
                  
					if (UseCustomerFilter == true)
					{
						//ShowFindTextBox = false;
						SelectParams selectCodeParams = new SelectParams();
						selectCodeParams.SortParams = "Code ASC";
						string customerFilterCode = this._userSettingsManager.CustomerFilterCodeGet();
						if (string.IsNullOrWhiteSpace(customerFilterCode) == false)
						{
							customers = new List<Customer>();

							List<string> customerCodeList = customerFilterCode.Split(',').Distinct().ToList();
							selectCodeParams.FilterStringListParams.Add("Code", new FilterStringListParam()
							{
								Values = customerCodeList
							});
							List<Customer> customersByCode = this._customerRepository.GetCustomers(selectCodeParams).ToList();
							foreach (Customer customer in customersByCode)
							{
								customers.Add(customer);
							}

						}

	
						SelectParams selectNameParams = new SelectParams();
						selectNameParams.SortParams = "Code ASC";
						string customerFilterName = this._userSettingsManager.CustomerFilterNameGet();
						if (string.IsNullOrWhiteSpace(customerFilterName) == false)
						{
							List<string> customerNameList = customerFilterName.Split(',').Distinct().ToList();
							selectNameParams.FilterStringListParams.Add("Name", new FilterStringListParam()
							{
								Values = customerNameList
							});
							List<Customer> customersByName = this._customerRepository.GetCustomers(selectNameParams).ToList();
							foreach (Customer customer in customersByName)
							{
								customers.Add(customer);
							}
						}

						foreach (Customer customer in customers)
						{
							LastCustomersListItem item = new LastCustomersListItem(customer);
							items.Add(item);
						}
					}
					else
					{//все
						//SelectParams selectParams = new SelectParams();
						//selectParams.SortParams = "ModifyDate DESC";
						//customers = this._customerRepository.GetCustomers(selectParams).ToList();
						customers = all.Select(x => x).OrderByDescending(x => x.ModifyDate).ToList();
						if (customers != null)
						{
							if (String.IsNullOrWhiteSpace(_find) == false)		//что-то есть
							{
								string _findToLower = _find.ToLower();

								foreach (Customer customer in customers)
								{
									if (customer.Code.ToLower().Contains(_findToLower) == false && customer.Name.ToLower().Contains(_findToLower) == false)
									continue;
						
									LastCustomersListItem item = new LastCustomersListItem(customer);
									items.Add(item);

								}
							}
							else			 //нечего нет
							{
								int k = 0;
								foreach (Customer customer in customers)
								{
									if (k <= 30)
									{
										LastCustomersListItem item = new LastCustomersListItem(customer);
										items.Add(item);
										k++;
									}
								}
							}
						}
					}
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }



			if (UseCustomerFilter == true && String.IsNullOrWhiteSpace(_find) == false)
			{
				//#1489
				//string _findToLower = _find.ToLower();
				//foreach (Customer customer in all)
				//{
				//	if (customer.Code.ToLower().Contains(_findToLower) == false && customer.Name.ToLower().Contains(_findToLower) == false)
				//			continue;
				//	LastCustomersListItem item = new LastCustomersListItem(customer);
				//	items.Add(item);
				//}
			}

			
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
             {
                 _items.Clear();
                 items.ForEach(r => _items.Add(r));
                 TotalCustomers = String.Format(Localization.Resources.ViewModel_LastCustomersDashboardPart_TotalCustomers, total);
             }));
        }

        private void CustomerEdited(Customer customer)
        {
			if (customer == null) return;
			Task.Factory.StartNew(ItemsBuild).LogTaskFactoryExceptions("CustomerEdited");
        }

        private void CustomerAdded(Customer customer)
        {
			if (customer == null) return;
			Task.Factory.StartNew(ItemsBuild).LogTaskFactoryExceptions("CustomerAdded");
        }

        public void Refresh()
        {

        }

        public void Clear()
        {
            this._eventAggregator.GetEvent<CustomerAddedEvent>().Unsubscribe(CustomerAdded);
            this._eventAggregator.GetEvent<CustomerEditedEvent>().Unsubscribe(CustomerEdited);
        }
    }
}
