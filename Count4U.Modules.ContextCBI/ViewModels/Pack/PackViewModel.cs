using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common;
using Count4U.Common.Events.Filter;
using Count4U.Common.Extensions;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Localization;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.ViewModels.Zip;
using Ionic.Zip;
using Ionic.Zlib;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Count4U.Modules.ContextCBI.ViewModels.Pack
{
    public class PackViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const string NodeSdfCommon = "NodeSdfCommon";
        private const string NodeMainDbSdf = "NodeMainDbSdf";
        private const string NodeCBICommon = "NodeCBICommon";

        private readonly ObservableCollection<PackItemViewModel> _items;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IInventorRepository _inventorRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IDBSettings _dbSettings;
        private readonly DelegateCommand _packCommand;
        private readonly IZip _zipService;
        private readonly IServiceLocator _serviceLocator;

        private readonly DelegateCommand _busyCancelCommand;
        private bool _isBusy;
        private CancellationTokenSource _cancellationTokenSource;

        private double _progress;
        private string _progressState;
        private string _progressFile;

        private string _busyText;

        private PackFilterData _filter;

        private bool _isExpandTree;
		private bool _selectAllInventor;

        private bool _autoCheckCatalogFiles;
        private bool _autoCheckInDataFiles;
		private bool _autoCheckConfigFiles;
		private bool _autoCheckProfileFiles;
		
	//	bool isPackDataFileCatalog = _userSettingsManager.PackDataFileCatalogGet();

        public PackViewModel(
            IContextCBIRepository contextCbiRepository,
            ICustomerRepository customerRepository,
            IBranchRepository branchRepository,
            IInventorRepository inventorRepository,
            IUserSettingsManager userSettingsManager,
            IServiceLocator serviceLocator,
            IDBSettings dbSettings,
            IZip zipService,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _serviceLocator = serviceLocator;
            _zipService = zipService;
            _dbSettings = dbSettings;
            _userSettingsManager = userSettingsManager;
            _inventorRepository = inventorRepository;
            _branchRepository = branchRepository;
            _customerRepository = customerRepository;
            _items = new ObservableCollection<PackItemViewModel>();
            _packCommand = new DelegateCommand(PackCommandExecuted, PackCommandCanExecute);
            _busyCancelCommand = new DelegateCommand(BusyCancelCommandExecuted);

            _autoCheckCatalogFiles = true;
			try	{ _autoCheckCatalogFiles = _userSettingsManager.PackDataFileCatalogGet();	}catch { }

            _autoCheckInDataFiles = true;
			_autoCheckConfigFiles = true;
			_autoCheckProfileFiles = true;
        }

        public ObservableCollection<PackItemViewModel> Items
        {
            get { return _items; }
        }

		public string[] CutomersCodes { get; set; }
		public string CustomerCodesCommaSeparated { get; set; }

        public DelegateCommand PackCommand
        {
            get { return _packCommand; }
        }

        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {
                this._isBusy = value;
                RaisePropertyChanged(() => IsBusy);

                Progress = 0;
            }
        }

        public DelegateCommand BusyCancelCommand
        {
            get { return this._busyCancelCommand; }
        }

        public double Progress
        {
            get { return this._progress; }
            set
            {
                this._progress = value;
                RaisePropertyChanged(() => Progress);
            }
        }

        public string ProgressState
        {
            get { return this._progressState; }
            set
            {
                this._progressState = value;
                RaisePropertyChanged(() => ProgressState);
            }

        }

        public string ProgressFile
        {
            get { return this._progressFile; }
            set
            {
                this._progressFile = value;
                RaisePropertyChanged(() => ProgressFile);
            }
        }

        public string BusyText
        {
            get { return _busyText; }
            set
            {
                _busyText = value;
                RaisePropertyChanged(() => BusyText);
            }
        }

        public bool IsFilterAnyField
        {
            get { return _filter == null ? false : _filter.IsAnyField(); }
        }

        public IFilterData Filter
        {
            get { return _filter; }
        }

        public bool AutoCheckCatalogFiles
        {
            get { return _autoCheckCatalogFiles; }
            set
            {
                _autoCheckCatalogFiles = value;
		        RaisePropertyChanged(() => AutoCheckCatalogFiles);
				IEnumerable<PackItemViewModel> nodes = this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>());
				foreach (PackItemViewModel node in nodes)
				{
					Inventor inventor = node.DomainObject as Inventor;
					if (inventor != null)
					{
						node.ImportFolderIsChecked = value;
					}
				}		
            }
        }

        public bool AutoCheckInDataFiles
        {
            get { return _autoCheckInDataFiles; }
            set
            {
                _autoCheckInDataFiles = value;
                RaisePropertyChanged(() => AutoCheckInDataFiles);
				IEnumerable<PackItemViewModel> nodes = this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>());
				foreach (PackItemViewModel node in nodes)
				{
					Inventor inventor = node.DomainObject as Inventor;
					if (inventor != null)
					{
						node.ImportFolderInDataIsChecked = value;
					}
				}
            }
        }

		public bool AutoCheckProfileFiles
		{
			get { return _autoCheckProfileFiles; }
			set
			{
				_autoCheckProfileFiles = value;
				RaisePropertyChanged(() => AutoCheckProfileFiles);
				IEnumerable<PackItemViewModel> nodes = this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>());
				foreach (PackItemViewModel node in nodes)
				{
					Inventor inventor = node.DomainObject as Inventor;
					if (inventor != null)
					{
						node.ImportFolderProfileIsChecked = value;
					}
				}
			}
		}

		public bool AutoCheckConfigFiles
        {
			get { return _autoCheckConfigFiles; }
            set
            {
				_autoCheckConfigFiles = value;
				RaisePropertyChanged(() => AutoCheckConfigFiles);
				IEnumerable<PackItemViewModel> nodes = this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>());
				foreach (PackItemViewModel node in nodes)
				{
					Customer customer = node.DomainObject as Customer;
					if (customer != null)
					{
						node.ImportFolderConfigIsChecked = value;
					}
				}
            }
        }


        public bool IsExpandTree
        {
            get { return _isExpandTree; }
            set
            {
                _isExpandTree = value;

                this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>()).ToList().ForEach(r => r.IsExpanded = _isExpandTree);

                if (_isExpandTree == false)
                {
                    foreach (PackItemViewModel item in _items)
                    {
                        item.IsExpanded = true;
                    }
                }
            }
        }

		public bool SelectAllInventor
        {
			get { return this._selectAllInventor; }
            set
            {
				this._selectAllInventor = value;


				IEnumerable<PackItemViewModel> nodes = this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>());
				//if (_selectAllInventor == true)
				//{
					foreach (PackItemViewModel node in nodes)
					{
						Inventor inventor = node.DomainObject as Inventor;
						if (inventor != null)
						{
							node.IsChecked = this._selectAllInventor;
						}
					}
				//}
			
            }
        }

		

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
			CutomersCodes = null;
			CustomerCodesCommaSeparated = navigationContext.Parameters.FirstOrDefault(r => r.Key == NavigationSettings.CustomerCodes).Value;
			//if (string.IsNullOrWhiteSpace(commaSeparatedCustomerCodes) == false)
			//{
			//	string[] split = commaSeparatedCustomerCodes.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();
			//	CutomersCodes = split.ToList();
			//}
		
            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Subscribe(PackFilter);

            _filter = new PackFilterData();

            _isExpandTree = !String.IsNullOrWhiteSpace(base.CBIDbContext); //_isExpandTree = false on for HomeDashboard

            Build();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            this._eventAggregator.GetEvent<FilterEvent<IFilterData>>().Unsubscribe(PackFilter);
        }

        private void SelectInventorAfterBuild()
        {
            if (base.CBIDbContext == Common.NavigationSettings.CBIDbContextInventor)
            {
                string code = base.CurrentInventor.Code;
                IEnumerable<PackItemViewModel> nodes = _items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>());

                foreach (PackItemViewModel node in nodes)
                {
                    Inventor inventor = node.DomainObject as Inventor;
                    if (inventor != null)
                    {
                        if (inventor.Code == code)
                        {
                            node.IsSelected = true;
                            node.IsChecked = true;
                            break;
                        }
                    }
                }
            }            
        }

		private void SelectCustomerAfterBuild()
		{
			IEnumerable<PackItemViewModel> nodes = _items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>());

			foreach (PackItemViewModel node in nodes)
			{
				Customer customer = node.DomainObject as Customer;
				if (customer != null)
				{
						node.IsChecked = true;
						node.ImportFolderInDataIsChecked = false;
						node.ImportFolderIsChecked = false;
						node.ImportFolderConfigIsChecked = false;
						node.ImportFolderProfileIsChecked = false;
				}
			}
		}

        #region Build tree

        private void Build()
        {
            BuildSdf();
            BuildCBI();
        }

        private void BuildSdf()
        {
            bool isRedactionOffice = FileSystem.IsAppRedactionOffice();
            if (isRedactionOffice)
            {
                PackItemViewModel rootSdf = new PackItemViewModel(null);
                rootSdf.DomainObject = NodeSdfCommon;
                rootSdf.Header = Resources.ViewModel_Pack_MainDbFromOffice;
                rootSdf.Image = PackIcons.Folder;
                rootSdf.IsExpanded = true;
                rootSdf.IsCheckboxVisible = false;
                _items.Add(rootSdf);

                PackItemViewModel mainSdf = new PackItemViewModel(null);
                mainSdf.DomainObject = NodeMainDbSdf;
                mainSdf.Header = "MainDB.sdf";
                mainSdf.Image = PackIcons.Database;

                rootSdf.Children.Add(mainSdf);
            }
        }

		private void BuildCBI()
		{
			PackItemViewModel cbiRoot = new PackItemViewModel(null);
			cbiRoot.DomainObject = NodeCBICommon;
			cbiRoot.Header = UtilsConvert.HebrewText(Resources.ViewModel_Pack_CBIDb);
			cbiRoot.Image = PackIcons.Folder;
			cbiRoot.IsCheckboxVisible = false;
			cbiRoot.IsExpanded = true;

			_items.Add(cbiRoot);

			string customerCode = String.Empty;
			string branchCode = String.Empty;
			string inventorCode = String.Empty;

			switch (base.CBIDbContext)
			{
				case NavigationSettings.CBIDbContextCustomer:
					customerCode = base.CurrentCustomer.Code;
					break;
				case NavigationSettings.CBIDbContextBranch:
					customerCode = base.CurrentBranch.CustomerCode;
					branchCode = base.CurrentBranch.Code;
					break;
				case NavigationSettings.CBIDbContextInventor:
					customerCode = base.CurrentInventor.CustomerCode;
					branchCode = base.CurrentInventor.BranchCode;
					inventorCode = base.CurrentInventor.Code;
					break;
			}

			try
			{
				Task.Factory.StartNew(() =>
					{
						try
						{
							List<Customer> customers = _customerRepository.GetCustomers(CBIContext.Main).ToList();

							bool filterFind = false;
							if (string.IsNullOrWhiteSpace(CustomerCodesCommaSeparated) == false)//from find
							{
								string[] customerCodesInput = CustomerCodesCommaSeparated.Split(',');	  //filter by Customer
								if (customerCodesInput.Length > 0)
								{
									List<string> customerCodesDb = this._customerRepository.GetCodeList();	 //All Customer Code from DB (table customer)
									if (customerCodesDb.Count > 0)
									{
										List<string> selectedCodeList = new List<string>();
										List<Customer> customersSelectedOnly = new List<Customer>();

										foreach (string code in customerCodesInput)			 //filter by Customer
										{
											if (string.IsNullOrWhiteSpace(code) == true) continue;
											if (customerCodesDb.Contains(code) == true)
											{
												if (selectedCodeList.Contains(code) == false)
												{
													selectedCodeList.Add(code);
													Customer customer = this._customerRepository.GetCustomerByCode(code);
													if (customer != null)
													{
														customersSelectedOnly.Add(customer);
														filterFind = true;
													}
												}
											}
										}
									
										customers = customersSelectedOnly;
									}
								}
							}

							List<Branch> branches = _branchRepository.GetBranches(CBIContext.Main).ToList();
							List<Inventor> inventors = _inventorRepository.GetInventors(new SelectParams() { IsEnablePaging = false, SortParams = "Code DESC" }).ToList();

							Utils.RunOnUI(() =>
								{
									try
									{
										List<PackItemViewModel> nodeListCustomer = BuildCustomers(cbiRoot, customerCode, customers);
										foreach (PackItemViewModel nodeCustomer in nodeListCustomer)
										{
											AssignAutoCheckFuncToNode(nodeCustomer);

											List<PackItemViewModel> nodeListBranch = BuildBranches(nodeCustomer, branchCode, branches);
											foreach (PackItemViewModel nodeBranch in nodeListBranch)
											{
												AssignAutoCheckFuncToNode(nodeBranch);

												List<PackItemViewModel> nodeListInventor = BuildInventor(nodeBranch, inventorCode, inventors);

												foreach (PackItemViewModel nodeInventor in nodeListInventor)
												{
													AssignAutoCheckFuncToNode(nodeInventor);
												}
											}
										}

										ApplyFilterToTree();

										SelectInventorAfterBuild();
										if (filterFind == true)
										{
											SelectCustomerAfterBuild();
										}
									}
									catch (Exception ex1)
									{
										_logger.ErrorException("BuildCBI in pack (Utils.RunOnUI)", ex1);
									}
								});
						}
						catch (Exception ex2)
						{
							_logger.ErrorException("BuildCBI in pack (Task.Factory.StartNew)", ex2);
						}
					}).LogTaskFactoryExceptions("BuildCBI");
			}
			catch (Exception ex3)
			{
				_logger.ErrorException("BuildCBI in pack", ex3);
			}
		}

        private void AssignAutoCheckFuncToNode(PackItemViewModel node)
        {
            node.AutoCheckCatalogFiles = () => { return _autoCheckCatalogFiles; }; // use for CBI

			if (node.DomainObject is Inventor)
			{
				node.AutoCheckInDataFiles = () => { return _autoCheckInDataFiles; };   //use only for	I
				node.AutoCheckProfileFiles = () => { return _autoCheckProfileFiles; };   //use only for	I
			}
			else
			{
				node.AutoCheckInDataFiles = () => { return false; };
				node.AutoCheckProfileFiles = () => { return false; };   //use only for	I
			}

			if (node.DomainObject is Customer)
			{
				node.AutoCheckConfigFiles = () => { return _autoCheckConfigFiles; };   //use only for	C
			}
			else
			{
				node.AutoCheckConfigFiles = () => { return false; };
			}
		
			
        }

        private List<PackItemViewModel> BuildCustomers(PackItemViewModel rootNode, string customerCode, List<Customer> customers)
        {
            List<PackItemViewModel> nodes = new List<PackItemViewModel>();

            List<Customer> customersLocal = customers;
            if (String.IsNullOrWhiteSpace(customerCode) == false)
            {
                customersLocal = customersLocal.Where(r => r.Code == customerCode).ToList();
            }

            foreach (Customer customer in customersLocal.OrderBy(r => r.Code))
            {
				if (!base.ContextCBIRepository.CheckDbPath(customer)) continue;
				//if (base.ContextCBIRepository.CheckDbPath(customer) == false)
				//{
				//	string message = String.Format(Localization.Resources.Msg_Customer_Sdf_File_Missed, customer.DBPath);

				//	MessageBoxResult result = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Warning, _userSettingsManager);
				//	_logger.Info("Pack BuildCustomers: - CheckDbPath Customer is false ");
				//	_logger.Info(String.Format("Pack BuildCustomers: .sdf file for [{0}] Customer is missed. Attempt to create database file", customer.Code));
				//	_contextCBIRepository.AddMissedCount4UDbFile(customer, result == MessageBoxResult.Yes);
				//}

		                        
                PackItemViewModel viewModel = new PackItemViewModel(rootNode);

                viewModel.DomainObject = customer;
                viewModel.Header = UtilsConvert.CustomerFancyName(customer);
                viewModel.Image = PackIcons.Customer;
                viewModel.IsExpanded = _isExpandTree;
                viewModel.Level = 1;
                viewModel.ImportFolderIsVisible = this.ImportFolderExist(customer);
                viewModel.ImportFolderInDataIsVisible = false; // this.ImportFolderInDataExist(customer);
				viewModel.ImportFolderProfileIsVisible = false;
				viewModel.ImportFolderConfigIsVisible = this.ImportFolderConfigExist(customer); 

                rootNode.Children.Add(viewModel);
                nodes.Add(viewModel);
            }

            return nodes;
        }

        private List<PackItemViewModel> BuildBranches(PackItemViewModel customerNode, string branchCode, List<Branch> branches)
        {
			List<PackItemViewModel> nodes = new List<PackItemViewModel>();
			if (customerNode == null)
			{
				_logger.Info("Pack BuildBranches - customerNode is Null ");
				return nodes;
			}


            Customer customer = customerNode.DomainObject as Customer;
            List<Branch> branchesLocal = branches.Where(r => r.CustomerCode == customer.Code).ToList();

            if (String.IsNullOrWhiteSpace(branchCode)  == false)
            {
                branchesLocal = branchesLocal.Where(r => r.Code == branchCode).ToList();
            }

            foreach (Branch branch in branchesLocal.OrderBy(r => r.Code))
            {
				if (!base.ContextCBIRepository.CheckDbPath(branch)) continue;
				//if (base.ContextCBIRepository.CheckDbPath(branch) == false)
				//{
				//	string message = String.Format(Localization.Resources.Msg_Branch_Sdf_File_Missed, branch.DBPath);
				//	MessageBoxResult result = UtilsMisc.ShowMessageBox(message, MessageBoxButton.YesNo, MessageBoxImage.Warning, _userSettingsManager);
				//	_logger.Info("Pack BuildBranches: - CheckDbPath branch is false ");
				//	_logger.Info(String.Format("Pack BuildBranches: .sdf file for [{0}] branch is missed. Attempt to create database file", branch.Code));
				//	_contextCBIRepository.AddMissedCount4UDbFile(branch, result == MessageBoxResult.Yes);
		
				//}

                PackItemViewModel viewModel = new PackItemViewModel(customerNode);

                viewModel.DomainObject = branch;
                viewModel.Header = UtilsConvert.BranchFancyName(branch);
                viewModel.Image = PackIcons.Branch;
                viewModel.IsExpanded = _isExpandTree; 
                viewModel.Level = 2;
                viewModel.ImportFolderIsVisible = this.ImportFolderExist(branch);
                viewModel.ImportFolderInDataIsVisible = false;// this.ImportFolderInDataExist(branch);
				viewModel.ImportFolderProfileIsVisible = false;
				viewModel.ImportFolderConfigIsVisible = false;

                customerNode.Children.Add(viewModel);
                nodes.Add(viewModel);
            }

            return nodes;
        }

        private List<PackItemViewModel> BuildInventor(PackItemViewModel branchNode, string inventorCode, List<Inventor> inventors)
        {
            List<PackItemViewModel> nodes = new List<PackItemViewModel>();
			if (branchNode == null)
			{
				_logger.Info("Pack BuildInventor - branchNode is Null ");
				return nodes;
			}

            Branch branch = branchNode.DomainObject as Branch;
            List<Inventor> inventorsLocal = inventors.Where(r => r.BranchCode == branch.Code).ToList();

            if (!String.IsNullOrWhiteSpace(inventorCode))
            {
                inventorsLocal = inventorsLocal.Where(r => r.Code == inventorCode).ToList();
            }

            enLanguage language = _userSettingsManager.LanguageGet();

            foreach (Inventor inventor in inventorsLocal.OrderBy(r => r.InventorDate))
            {
				if (base.ContextCBIRepository.CheckDbPath(inventor) == false) 
				{
					_logger.Info("Pack BuildInventor - CheckDbPath inventor is false ");
					continue;
				}
                PackItemViewModel viewModel = new PackItemViewModel(branchNode);

                viewModel.DomainObject = inventor;
                viewModel.Header = UtilsConvert.InventorFancyName(inventor, language);
                viewModel.Image = PackIcons.Inventor;
                viewModel.IsExpanded = false;
                viewModel.Level = 3;
                viewModel.ImportFolderIsVisible = this.ImportFolderExist(inventor);
                viewModel.ImportFolderInDataIsVisible = this.ImportFolderInDataExist(inventor);
				viewModel.ImportFolderProfileIsVisible = this.ImportFolderProfileExist(inventor);
				viewModel.ImportFolderConfigIsVisible = false;

                branchNode.Children.Add(viewModel);

                nodes.Add(viewModel);
            }

            return nodes;
        }

        #endregion

        private bool PackCommandCanExecute()
        {
            bool checkedCBI =  this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>()).Any(r => r.IsChecked);
			bool checkedImportFolder =this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>()).Any(r => r.ImportFolderIsChecked);
			bool checkedImportFolderInData = this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>()).Any(r => r.ImportFolderInDataIsChecked);
			bool checkedImportFolderProfile = this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>()).Any(r => r.ImportFolderProfileIsChecked);
			bool checkedImportFolderConfig =  this._items.FlattenHierarchyNodes(r => r.Children.Cast<PackItemViewModel>()).Any(r => r.ImportFolderConfigIsChecked);
			bool ret = checkedCBI || checkedImportFolder || checkedImportFolderInData || checkedImportFolderConfig || checkedImportFolderProfile;
			return ret;
        }

        private void PackCommandExecuted()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            IsBusy = true;
            BusyText = String.Empty;
            Task.Factory.StartNew(() =>
                {
                    string zipFilePath = Pack();
                    Utils.RunOnUI(() =>
                        {
                            IsBusy = false;

                            if (_cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested == false)
                                Utils.OpenFileInExplorer(zipFilePath);
                        });
				}).LogTaskFactoryExceptions("PackCommandExecuted");
        }

        private string Pack()
        {
            string resultZipFile = String.Empty;

            Utils.RunOnUI(() => ProgressState = Localization.Resources.ViewModel_Pack_Preparing);

            try
            {
                PackItemViewModel rootCbiNode = _items.FirstOrDefault(r => IsNodeSpecial(NodeCBICommon, r));
                List<PackItemViewModel> checkedCbi = rootCbiNode.FlattenHierarchyNode(r => r.Children.Cast<PackItemViewModel>()).Where(r => r.IsChecked).ToList();

                _logger.Info("alterAdoProvider.UpdateCount4UDBViaScript");
                IAlterADOProvider alterAdoProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();

                List<string> relativePathList = new List<string>();
                foreach (PackItemViewModel item in checkedCbi)
                {
                    relativePathList.Add(base.ContextCBIRepository.BuildRelativeDbPath(item.DomainObject));
                }

                alterAdoProvider.UpdateCount4UDBViaScript(relativePathList);
				alterAdoProvider.AlterTableIturAnalyzesCount4UDBViaScript(relativePathList);
				alterAdoProvider.UpdateAnalyticDBViaScript(relativePathList);
				

                _logger.Info("ContextCBIRepository.RefillInventorConfigs");
                foreach (PackItemViewModel item in checkedCbi)
                {
                    base.ContextCBIRepository.RefillInventorConfigs(item.DomainObject);
                }

                Utils.RunOnUI(() =>
                    {
                        ProgressState = String.Empty;
                        BusyText = Localization.Resources.ViewModel_Pack_Packing;
                    });

                resultZipFile = RunZip();
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Pack", exc);
            }

            return resultZipFile;
        }

        private string RunZip()
        {
            string resultFileName = String.Empty;

            using (ZipFile bundle = new ZipFile())
            {
                bundle.CompressionLevel = CompressionLevel.BestSpeed;
                bundle.BufferSize = 1048576;
                bundle.CodecBufferSize = 1048576;
                bundle.ParallelDeflateThreshold = -1;

                _logger.Info("Building zip");
                BuildZip(bundle);

                ZipEntry curEntry = null;
                int total = bundle.Entries.Count;

				int totalFile = 0;
				foreach (var curFileName in bundle.EntryFileNames)
				{
					//Marina
					var curFileNameReverse = curFileName.ToCharArray().Reverse();
					if (curFileNameReverse.ElementAt(0).CompareTo('/') != 0)
					{
						totalFile++;
					}
				}

                int processed = 0;
				int processedFile = 0;
				string processedFileName = "";
				
                bundle.SaveProgress += (s, e) =>
                    {
                        if (_cancellationTokenSource.IsCancellationRequested)
                            e.Cancel = true;

                        if (e.CurrentEntry == null)
                            return;

                        if (curEntry != e.CurrentEntry)
                        {
                            processed++;
                            curEntry = e.CurrentEntry;

							//Marina
							var curName = curEntry.FileName;
							var processedFileNameReverse = curName.ToCharArray().Reverse();
							if(processedFileNameReverse.ElementAt(0).CompareTo('/') != 0 )
							{
								processedFileName = curEntry.FileName;
								processedFile++;
							}

                            _logger.Info(curEntry.FileName);
                        }


                        double v = e.BytesTransferred * 100 / (double)e.TotalBytesToTransfer;

                        //UpdateProgress(v, String.Format("{0}/{1}", processed, total), curEntry.FileName);
						UpdateProgress(v, String.Format("{0}/{1}", processedFile, totalFile), processedFileName);

                        if (_cancellationTokenSource.IsCancellationRequested)
                            e.Cancel = true;
                    };

                string outFolder = UtilsPath.ZipOfficeFolder(_dbSettings);
                string fileName = _zipService.BuildFileName("Pack_");
                resultFileName = Path.Combine(outFolder, fileName);
                _logger.Info("Saving zip");
                bundle.Save(resultFileName);
            }

            return resultFileName;
        }

        private void BuildZip(ZipFile bundle)
        {
            try
            {
                BuildZipRootSdf(bundle);
                BuildZipRootCBI(bundle);

            }
            catch (Exception exc)
            {
                _logger.ErrorException("BuildZip", exc);
            }
        }

        private void BuildZipRootSdf(ZipFile bundle)
        {
            PackItemViewModel rootSdfNode = _items.FirstOrDefault(r => IsNodeSpecial(NodeSdfCommon, r));
            string App_DataFolderName = _dbSettings.FolderApp_Data;

            if (rootSdfNode != null)
            {
                PackItemViewModel mainDbSdfNode = rootSdfNode.Children.Cast<PackItemViewModel>().FirstOrDefault(r => IsNodeSpecial(NodeMainDbSdf, r));

                if (mainDbSdfNode.IsChecked)
                {
                    bundle.AddFile(_dbSettings.MainDbSdfPath(), App_DataFolderName);  //TODO сейчас только для процесса asis

                    string folderLogoPath = _dbSettings.FolderLogoPath();
                    DirectoryInfo di = new DirectoryInfo(folderLogoPath);
                    bundle.AddDirectory(di.FullName, Path.Combine(App_DataFolderName, di.Name));
                }
            }
        }

		private void BuildZipRootCBI(ZipFile bundle)
		{
			PackItemViewModel rootCbiNode = _items.FirstOrDefault(r => IsNodeSpecial(NodeCBICommon, r));
			List<PackItemViewModel> checkedCbi = rootCbiNode.FlattenHierarchyNode(r => r.Children.Cast<PackItemViewModel>()).Where(r => r.IsChecked).ToList();

			string App_DataFolderName = _dbSettings.FolderApp_Data;
			string basePath = _dbSettings.BuildAppDataFolderPath();

			foreach (PackItemViewModel item in checkedCbi)
			{
				Customer customer = item.DomainObject as Customer;
				Branch branch = item.DomainObject as Branch;
				Inventor inventor = item.DomainObject as Inventor;

				if (customer != null || branch != null || inventor != null)
				{
					string path = base.ContextCBIRepository.BuildFullDbPath(item.DomainObject);
					FileInfo fi = new FileInfo(path);
					string relative = fi.Directory.FullName.Substring(basePath.Length).Trim(new[] { '\\' });
					string relativeStartingWithAppData = Path.Combine(App_DataFolderName, relative);
					bundle.AddFile(fi.FullName, relativeStartingWithAppData);

					string folderPath = Path.GetDirectoryName(path);

					string[] files = Directory.GetFiles(folderPath, "*.c4u", SearchOption.TopDirectoryOnly);
					for (int i = 0; i < files.Length; i++)
					{
						string fileNameExtension = Path.GetExtension(files[i]);
						if (fileNameExtension == ".c4u")
						{
							try
							{
								string pathfile = Path.GetFullPath(files[i]);
								FileInfo fiC4u = new FileInfo(pathfile);
								bundle.AddFile(fiC4u.FullName, relativeStartingWithAppData);
							}
							catch { }
						}
					}

					string[] filesAnalytic = Directory.GetFiles(folderPath, "*AnalyticDB.sdf", SearchOption.TopDirectoryOnly);
					for (int i = 0; i < filesAnalytic.Length; i++)
					{
						string fileName = Path.GetFileName(filesAnalytic[i]);
						if (fileName == "AnalyticDB.sdf")
						{
							try
							{
								string pathfile = Path.GetFullPath(filesAnalytic[i]);
								FileInfo fileAnalyticDb = new FileInfo(pathfile);
								bundle.AddFile(fileAnalyticDb.FullName, relativeStartingWithAppData);
							}
							catch { }
						}
					}

					//BuildZipImportData(ref bundle, item);  
				}
			}


			List<PackItemViewModel> checkedImportFolder = rootCbiNode.FlattenHierarchyNode(r => r.Children.Cast<PackItemViewModel>()).
				Where(r => r.ImportFolderIsChecked || r.ImportFolderInDataIsChecked || r.ImportFolderConfigIsChecked || r.ImportFolderProfileIsChecked).ToList();
			foreach (PackItemViewModel item in checkedImportFolder)
			{
				BuildZipImportData(ref bundle, item);
			}

		}

        private void BuildZipImportData(ref ZipFile bundle, PackItemViewModel item)
        {
			if (item.ImportFolderIsChecked == false
				&& item.ImportFolderInDataIsChecked == false
				&& item.ImportFolderConfigIsChecked == false
				&& item.ImportFolderProfileIsChecked == false)
				return;

            string folderImport = _dbSettings.FolderImport;
            string folderImportPath = _dbSettings.ImportFolderPath();

            DirectoryInfo diImportPath = new DirectoryInfo(folderImportPath);

            string folderObjectImportPath = BuildPathToImportFolder(item.DomainObject);
            string folderObjectInDataPath = BuildPathToImportInDataFolder(item.DomainObject);
			string folderObjectProfilePath = BuildPathToImportProfileFolder(item.DomainObject);
			string folderObjectConfigPath = BuildPathConfigFolder(item.DomainObject);

            DirectoryInfo diObjectImportPath = new DirectoryInfo(folderObjectImportPath);
            DirectoryInfo diObjectInDataPath = new DirectoryInfo(folderObjectInDataPath);
			DirectoryInfo diObjectConfigPath = new DirectoryInfo(folderObjectConfigPath);
			DirectoryInfo diObjectProfilePath = new DirectoryInfo(folderObjectProfilePath);

            string relativeImport = diObjectImportPath.FullName.Substring(folderImportPath.Length).Trim(new[] { '\\' });
            string relativeInData = diObjectInDataPath.FullName.Substring(folderImportPath.Length).Trim(new[] { '\\' });
			string relativeConfig = diObjectConfigPath.FullName.Substring(folderImportPath.Length).Trim(new[] { '\\' });
			string relativeProfile = diObjectProfilePath.FullName.Substring(folderImportPath.Length).Trim(new[] { '\\' });

			// 	ImportFolder\inData + ImportFolder		  так может случиться только в Инвенторе сейчас
			if (item.ImportFolderIsChecked == true)
			{

				if (Directory.Exists(diObjectImportPath.FullName))
				{
					foreach (FileInfo file in diObjectImportPath.EnumerateFiles())
					{
						bundle.AddFile(file.FullName, Path.Combine(folderImport, relativeImport));
					}
				}

				if (item.ImportFolderInDataIsChecked == true)
				{
					if (Directory.Exists(diObjectInDataPath.FullName))
					{
						bundle.AddDirectory(diObjectInDataPath.FullName, Path.Combine(folderImport, relativeInData));
					}
				}

				if (item.ImportFolderProfileIsChecked == true)
				{
					if (Directory.Exists(diObjectProfilePath.FullName))
					{
						bundle.AddDirectory(diObjectProfilePath.FullName, Path.Combine(folderImport, relativeProfile));
					}
				}

                // 	ImportFolder\Config 		  так может случиться только в Customer сейчас
                if (item.ImportFolderConfigIsChecked == true)
                {
                    if (Directory.Exists(diObjectConfigPath.FullName))
                    {
                        bundle.AddDirectory(diObjectConfigPath.FullName, Path.Combine(folderImport, relativeConfig));
                    }
                }
            }

            // 	ImportFolder\Config + ImportFolder		  так может случиться только в Customer сейчас
            //if (item.ImportFolderIsChecked == true && item.ImportFolderConfigIsChecked == true)
            //         {
             //             if (Directory.Exists(diObjectConfigPath.FullName))	
			//	{
			//		bundle.AddDirectory(diObjectConfigPath.FullName, Path.Combine(folderImport, relativeImport));
			//	}
			//}
			// все что в папке ImportFolder исключив внутренние папки	CBI
			
            if (item.ImportFolderIsChecked == true && item.ImportFolderInDataIsChecked == false && item.ImportFolderConfigIsChecked == false && item.ImportFolderProfileIsChecked == false)  
            {
                if (Directory.Exists(diObjectImportPath.FullName))
                {
                    foreach (FileInfo file in diObjectImportPath.EnumerateFiles())
                    {
                        bundle.AddFile(file.FullName, Path.Combine(folderImport, relativeImport));
                    }

                    foreach (DirectoryInfo dir in diObjectImportPath.EnumerateDirectories())
                    {
                        if (dir.Name.ToLower() == "indata")
                        {
                            continue;
                        }
						if (dir.Name.ToLower() == "config")
						{
							continue;
						}
						if (dir.Name.ToLower() == "profile")
						{
							continue;
						}
						if (dir.Name.ToLower() == "log")
						{
							continue;
						}

                        bundle.AddDirectory(dir.FullName, Path.Combine(folderImport, relativeImport, dir.Name));
                    }
                }
            }
			//только   InData - пока может быть только в Инвенторе 
			else if (item.ImportFolderIsChecked == false && item.ImportFolderInDataIsChecked == true)
            {
                if (Directory.Exists(diObjectInDataPath.FullName))
                {
                    bundle.AddDirectory(diObjectInDataPath.FullName, Path.Combine(folderImport, relativeInData));
                }
				if (item.ImportFolderProfileIsChecked == true)
				{
					if (Directory.Exists(diObjectProfilePath.FullName))
					{
						bundle.AddDirectory(diObjectProfilePath.FullName, Path.Combine(folderImport, relativeProfile));
					}
				}
				if (item.ImportFolderConfigIsChecked == true)
				{
					if (Directory.Exists(diObjectConfigPath.FullName))
					{
						bundle.AddDirectory(diObjectConfigPath.FullName, Path.Combine(folderImport, relativeConfig));
					}
				}
            }

			else if (item.ImportFolderIsChecked == false && item.ImportFolderProfileIsChecked == true)
            {
				if (Directory.Exists(diObjectProfilePath.FullName))
				{
					bundle.AddDirectory(diObjectProfilePath.FullName, Path.Combine(folderImport, relativeProfile));
				}
            }
				

			//только   Config - пока может быть только в Customer 
			else if (item.ImportFolderIsChecked == false && item.ImportFolderConfigIsChecked == true)
			{
				if (Directory.Exists(diObjectConfigPath.FullName))
				{
					bundle.AddDirectory(diObjectConfigPath.FullName, Path.Combine(folderImport, relativeConfig));
				}
			}

			//TODO  CheckConfigIsChecked
        }

        private bool IsNodeSpecial(string specialMark, PackItemViewModel node)
        {
            return (node.DomainObject is String) && (String)node.DomainObject == specialMark;
        }

        private void UpdateProgress(double progress, string processedTotal, string fileName)
        {
            Utils.RunOnUI(() =>
                {
                    Progress = progress;
                    ProgressFile = fileName;
                    ProgressState = processedTotal;
                });
        }

        private void BusyCancelCommandExecuted()
        {
            this._cancellationTokenSource.Cancel();

            IsBusy = false;
        }

        public void SomeNodeCheckedUnchecked()
        {
            _packCommand.RaiseCanExecuteChanged();
        }

        private void PackFilter(IFilterData filterData)
        {
            _filter = filterData as PackFilterData;
            RaisePropertyChanged(() => IsFilterAnyField);
            ApplyFilterToTree();
        }

        private void ApplyFilterToTree()
        {
            if (_filter == null)
                return;

            PackItemViewModel rootCbiNode = _items.FirstOrDefault(r => IsNodeSpecial(NodeCBICommon, r));
            List<PackItemViewModel> cbiList = rootCbiNode.
                FlattenHierarchyNode(r => r.Children.Cast<PackItemViewModel>()).
                Where(r => !(r.DomainObject is String)).
                OrderBy(r => r.Level).
                ToList();

            foreach (PackItemViewModel item in cbiList)
            {
                item.IsVisible = false;
            }

            foreach (PackItemViewModel item in cbiList)
            {
                if (item.IsVisible)
                    continue;

                bool isVisible = false;

                if (_filter.IsAnyField() == false)
                {
                    isVisible = true;
                }
                else
                {
                    Customer customer = item.DomainObject as Customer;
                    Branch branch = item.DomainObject as Branch;
                    Inventor inventor = item.DomainObject as Inventor;

                    string filterCode = String.IsNullOrWhiteSpace(_filter.Code) ? String.Empty : _filter.Code.ToLower();
                    string filterName = String.IsNullOrWhiteSpace(_filter.Name) ? String.Empty : _filter.Name.ToLower();

                    bool isFilteredByInventorDates = _filter.From != null || _filter.To != null;

                    if (customer != null && isFilteredByInventorDates == false)
                    {
                        if (!String.IsNullOrWhiteSpace(filterCode) && !String.IsNullOrWhiteSpace(customer.Code))
                        {
                            isVisible = customer.Code.ToLower().Contains(filterCode);
                        }
                        else
                        {
                            isVisible = true;
                        }

                        if (!String.IsNullOrWhiteSpace(filterName) && !String.IsNullOrWhiteSpace(customer.Name))
                        {
                            isVisible = isVisible && customer.Name.ToLower().Contains(filterName);
                        }
                    }

                    if (branch != null && isFilteredByInventorDates == false)
                    {
                        if (!String.IsNullOrWhiteSpace(filterCode))
                        {
                            if (!String.IsNullOrWhiteSpace(branch.Code))
                            {
                                isVisible = isVisible || branch.Code.ToLower().Contains(filterCode);
                            }
                            if (!String.IsNullOrWhiteSpace(branch.BranchCodeERP))
                            {
                                isVisible = isVisible || branch.BranchCodeERP.ToLower().Contains(filterCode);
                            }
                            if (!String.IsNullOrWhiteSpace(branch.BranchCodeLocal))
                            {
                                isVisible = isVisible || branch.BranchCodeLocal.ToLower().Contains(filterCode);
                            }
                        }
                        else
                        {
                            isVisible = true;
                        }

                        if (!String.IsNullOrWhiteSpace(filterName) && !String.IsNullOrWhiteSpace(branch.Name))
                        {
                            isVisible = isVisible && branch.Name.ToLower().Contains(filterName);
                        }
                    }

                    if (inventor != null)
                    {
                        if (!String.IsNullOrWhiteSpace(filterCode))
                        {
                            isVisible = inventor.Code.ToLower().Contains(filterCode);
                        }
                        else
                        {
                            isVisible = String.IsNullOrWhiteSpace(filterName);
                        }

                        DateTime? from = _filter.From;
                        DateTime? to = _filter.To;

                        DateTime inventorDate = inventor.InventorDate
                            .Subtract(TimeSpan.FromHours(inventor.InventorDate.Hour))
                            .Subtract(TimeSpan.FromMinutes(inventor.InventorDate.Minute))
                            .Subtract(TimeSpan.FromSeconds(inventor.InventorDate.Second));

                        if (from != null && to != null)
                        {
                            isVisible = isVisible && inventorDate >= from && inventorDate <= to;
                        }

                        if (from != null && to == null)
                        {
                            isVisible = isVisible && inventorDate >= from;
                        }

                        if (from == null && to != null)
                        {
                            isVisible = isVisible && inventorDate <= to;
                        }
                    }
                }

                item.IsVisible = isVisible;
            }

        }

        private bool ImportFolderExist(object domainObject)
        {
            return Directory.Exists(BuildPathToImportFolder(domainObject));
        }

        private bool ImportFolderInDataExist(object domainObject)
        {
            return Directory.Exists(BuildPathToImportInDataFolder(domainObject));
        }

		private bool ImportFolderProfileExist(object domainObject)
        {
			return Directory.Exists(BuildPathToImportProfileFolder(domainObject));
        }

		

		private bool ImportFolderConfigExist(object domainObject)
        {
			string pathConfigFolder = BuildPathConfigFolder(domainObject);
			return Directory.Exists(pathConfigFolder);
        }
		

        private string BuildPathToImportFolder(object domainObject)
        {
            return base.ContextCBIRepository.GetImportFolderPath(domainObject);
        }

        private string BuildPathToImportInDataFolder(object domainObject)
        {
            return Path.Combine(BuildPathToImportFolder(domainObject), FileSystem.inData);
        }


		private string BuildPathToImportProfileFolder(object domainObject)
        {
			return Path.Combine(BuildPathToImportFolder(domainObject), "Profile");
        }

		private string BuildPathConfigFolder(object domainObject)
		{
			return Path.Combine(BuildPathToImportFolder(domainObject), "Config");
		}

		
    }
}