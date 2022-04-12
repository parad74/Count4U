using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Count4U.Common.UserSettings;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface;

namespace Count4U.Common.ViewModel
{
    public class BottomViewModel : NotificationObject, INavigationAware, IRegionMemberLifetime
    {
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
		private readonly IServiceLocator _serviceLocator;
		private readonly IDBSettings _dbSettings;

        private string _code;
        private string _path;
		private string _verDB;
		private string _processCode;

        private bool _isExpanded;

        private bool _isNavigated;

        public BottomViewModel(
            IContextCBIRepository contextCbiRepository,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
			IServiceLocator serviceLocator,
			IDBSettings dbSettings)
        {
            this._userSettingsManager = userSettingsManager;
			this._regionManager = regionManager;
			this._contextCbiRepository = contextCbiRepository;
			 this._serviceLocator = serviceLocator;
			 this._dbSettings = dbSettings;
        }

        public bool KeepAlive
        {
            get { return true; }
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;

                RaisePropertyChanged(() => Code);
            }
        }

		public string ProcessCode
		{
			get { return _processCode; }
			set
			{
				_processCode = value;

				RaisePropertyChanged(() => ProcessCode);
			}
		}

		public string VerDB
		{
			get { return _verDB; }
			set
			{
				_verDB = value;

				RaisePropertyChanged(() => VerDB);
			}
		}

		public string OfficeOrLaptop
		{
			get { return FileSystem.IsAppRedactionOffice() ? "OFFICE" : "LAPTOP";  }
			//get { return "LAPTOP";  }
		}


        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;

                RaisePropertyChanged(() => Path);
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(() => IsExpanded);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!_isNavigated)
            {
                _isNavigated = true;
                this._isExpanded = _userSettingsManager.IsExpandedBottomGet();
                Application.Current.MainWindow.Closing += MainWindow_Closing;
            }
            Build();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
			Application.Current.MainWindow.Closing -= MainWindow_Closing;
		}

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _userSettingsManager.IsExpandedBottomSet(_isExpanded);
        }


        private void Build()
        {
			ProcessCode = _dbSettings.SettingsRepository.ProcessCode;
            Code = String.Empty;
            Path = String.Empty;
			VerDB = String.Empty;

            if (!_regionManager.Regions.ContainsRegionWithName(Common.RegionNames.ApplicationWindow))
                return;

            IRegion region = _regionManager.Regions[Common.RegionNames.ApplicationWindow];

            object view = region.ActiveViews.FirstOrDefault();
            if (view == null)
                return;

            FrameworkElement fe = view as FrameworkElement;
            if (fe == null)
                return;

			IAlterADOProvider alterAdoProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();

            CBIContextBaseViewModel viewModel = fe.DataContext as CBIContextBaseViewModel;
            if (viewModel == null)
                return;

            string code = String.Empty;
			//string path1 = String.Empty;
            object domainObject = viewModel.GetCurrentDomainObject();

            Customer customer = domainObject as Customer;
            if (customer != null)
			{
				code = customer.Code;
				//path1 = this._contextCbiRepository.GetDBPath(customer);
			}

            Branch branch = domainObject as Branch;
            if (branch != null)
			{
                code = branch.Code;
				//path1 = this._contextCbiRepository.GetDBPath(branch);
			}

            Inventor inventor = domainObject as Inventor;
            if (inventor != null)
			{
                code = inventor.Code;
				//path1 = this._contextCbiRepository.GetDBPath(inventor);
			}

            if (!String.IsNullOrEmpty(code))
                Code = code;

			if (String.IsNullOrEmpty(ProcessCode) == true)
				ProcessCode = Code;

            Path = viewModel.GetDbPath;
			if (string.IsNullOrWhiteSpace(Path) == false)
			{
				VerDB = alterAdoProvider.GetVerCount4UDB(Path).ToString();
			}
			else
			{
				VerDB = "";
			}


	
			
			//VerDB = alterAdoProvider.GetVerCount4UDB(path1).ToString();
        }
    }
}