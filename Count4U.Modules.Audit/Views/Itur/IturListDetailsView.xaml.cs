using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Count4U.Common.Constants;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Common.Services.Navigation.Data.SearchData;
using Count4U.Common.Services.UICommandService;
using Count4U.Common.UserSettings;
using Count4U.GenerationReport;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Modules.Audit.Views
{
	/// <summary>
	/// Interaction logic for LocationDetailView.xaml
	/// </summary>
	public partial class IturListDetailsView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        private static readonly string PopupSearchRegion = Common.RegionNames.PopupSearchItur;
        private static readonly string PopupSpeedRegion = Common.RegionNames.PopupSpeedLink;

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IUnityContainer _unityContainer;
        private readonly INavigationRepository _navigationRepository;
        private readonly UICommandRepository _commandRepository;

        private readonly PopupExtSearch _popupExtSearch;
        private readonly PopupExtFilter _popupExtFilter;

        private Window _windowSpeed;
        private string dbPath;

		private int _iturListPrintCount = 5;
		private int _beforeMenuPrintCount = 4; //!!! увеличить при добавлении в меню

        public IturListDetailsView(
            IturListDetailsViewModel viewModel,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IContextCBIRepository contextCbiRepository,
            IUserSettingsManager userSettingsManager,
            IUnityContainer unityContainer,
            INavigationRepository navigationRepository,
            PopupExtSearch popupExtSearch,
            PopupExtFilter popupExtFilter,
            UICommandRepository commandRepository)
        {
            this.InitializeComponent();

            this._popupExtFilter = popupExtFilter;
            this._navigationRepository = navigationRepository;
            this._unityContainer = unityContainer;
            this._userSettingsManager = userSettingsManager;
            this._contextCbiRepository = contextCbiRepository;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._popupExtSearch = popupExtSearch;
            this._commandRepository = commandRepository;

            this.DataContext = viewModel;

            RegionManager.SetRegionName(MainRegionBackForward, Common.RegionNames.MainRegionBackForward);

            this.Loaded += IturListDetailsView_Loaded;
            btnSpeedLink.Click += btnSpeedLink_Click;

            this.listIturs.SelectionChanged += listIturs_SelectionChanged;

            _popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = new InventProductSearchData();
            _popupExtSearch.Region = PopupSearchRegion;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.Init();

            _popupExtFilter.Button = btnFilter;
            _popupExtFilter.Region = Common.RegionNames.PopupFilterItur;
            _popupExtFilter.ViewModel = viewModel;
            _popupExtFilter.View = Common.ViewNames.FilterView;
            _popupExtFilter.ApplyForQuery = query => UtilsConvert.AddObjectToQuery(query, _navigationRepository, viewModel.Filter, Common.NavigationObjects.Filter);
            _popupExtFilter.Init();

            this.PreviewKeyDown += IturListDetailsView_PreviewKeyDown;
        }

        void IturListDetailsView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
//            if (Keyboard.Modifiers == ModifierKeys.Control)
//            {
//                IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
//                if (viewModel == null) return;
//
//                if (e.Key == Key.PageUp)
//                {
//                    viewModel.UpCommand.Execute();
//                }
//
//                if (e.Key == Key.PageDown)
//                {
//                    viewModel.DownCommand.Execute();
//                }
//            }
        }

        void IturListDetailsView_Loaded(object sender, RoutedEventArgs e)
        {
            IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
            if (viewModel != null && btnReports.ContextMenu != null)
                viewModel.ReportButtonViewModel.BuildMenu(btnReports.ContextMenu);
				Utils.RunOnUI(() => pagination.Focus(), DispatcherPriority.Background);
        }



        void listIturs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
            if (viewModel == null) return;

            List<IturDashboardItemViewModel> items = new List<IturDashboardItemViewModel>();
            foreach (var selectedItem in listIturs.SelectedItems)
            {
                IturDashboardItemViewModel item = selectedItem as IturDashboardItemViewModel;
                if (item != null)
                    items.Add(item);
            }
            viewModel.SelectedItems = items;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            if (menu == null) return;
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;

            if (menu.Items.Count > 1)
            {
                MenuItem mnuIsDisabled = menu.Items[0] as MenuItem;
                if (mnuIsDisabled != null)
                {
                    CheckBox checkBox = mnuIsDisabled.Header as CheckBox;
                    if (checkBox != null)
                    {
                        
                        if (viewModel != null)
                        {
                            checkBox.IsChecked = viewModel.IsDisabled;
                        }
                    }
                }
            }

			if (menu.Items.Count > 4)
			{
				MenuItem mnutExportERP = menu.Items[3] as MenuItem;
				if (mnutExportERP != null)
				{
					if (viewModel != null)
					{
						bool can = viewModel.RunExportErpByConfigCommand.CanExecute();
						mnutExportERP.IsEnabled = can;
					}
				}
			}

			IReportInfoRepository reportInfoRepository = viewModel._serviceLocator.GetInstance<IReportInfoRepository>();
			List<ReportInfo> reportInfoList = reportInfoRepository.BuildContextMenuReportInfoList(viewModel.CurrentInventor.Code, viewModel._reportIniFile);

			for (int i = 0; i < this._iturListPrintCount; i++)
			{
				string reportNumInContextMenu = "Report" + i.ToString();
				int index = this._beforeMenuPrintCount + i;
				if (menu.Items.Count < index) continue;
				var menuItem = menu.Items[index] as MenuItem; // TODO MenuItem
				var retInfoReport = reportInfoList.Where(x => x.ReportNumInContextMenu == reportNumInContextMenu).FirstOrDefault();
				if (retInfoReport != null)
				{
					if (retInfoReport.ShowInContextMenu == true)
					{
						menuItem.Visibility = Visibility.Visible;
						menuItem.Header = Localization.Resources.View_IturAddEditDelete_tpIturListPrint + " " + retInfoReport.ReportCode;
					}
					else
					{
						menuItem.Visibility = Visibility.Collapsed;
					}
				}
			}
			
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;

            IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
            if (viewModel != null)
            {
                viewModel.IsDisabled = checkBox.IsChecked;
            }
        }

        private void mnuChangeLocation_Click(object sender, RoutedEventArgs e)
        {
            IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
            if (viewModel != null)
            {
                viewModel.ChangeLocationCommand.Execute();
            }
        }

		private void mnuChangeName_Click(object sender, RoutedEventArgs e)
		{
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.ChangeIturNameCommand.Execute();
			}
		}

		private void mnuIturListPrint_Click0(object sender, RoutedEventArgs e)
		{
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.IturListPrintByIndexCommand.Execute(ReportIniContextMenuStringIndex.Report0);
			}
		}

		private void mnuIturListPrint_Click1(object sender, RoutedEventArgs e)
		{
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.IturListPrintByIndexCommand.Execute(ReportIniContextMenuStringIndex.Report1);
			}
		}

		private void mnuIturListPrint_Click2(object sender, RoutedEventArgs e)
		{
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.IturListPrintByIndexCommand.Execute(ReportIniContextMenuStringIndex.Report2);
			}
		}

		private void mnuIturListPrint_Click3(object sender, RoutedEventArgs e)
		{
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.IturListPrintByIndexCommand.Execute(ReportIniContextMenuStringIndex.Report3);
			}
		}

		private void mnuIturListPrint_Click4(object sender, RoutedEventArgs e)
		{
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.IturListPrintByIndexCommand.Execute(ReportIniContextMenuStringIndex.Report4);
			}
		}


		private void mnuIturListPrint_Click5(object sender, RoutedEventArgs e)
		{
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.IturListPrintByIndexCommand.Execute(ReportIniContextMenuStringIndex.Report5);
				//viewModel.RunExportErpByConfigCommand.Execute();
			}
		}


		private void mnuIturListExportERPByItur_Click(object sender, RoutedEventArgs e)
		{
			IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.RunExportErpByConfigCommand.Execute();
			}
		}

		//old
		//private void mnuIturListPrint_Click(object sender, RoutedEventArgs e)
		//{
		//	IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
		//	if (viewModel != null)
		//	{
		//		viewModel.IturListPrintCommand.Execute();
		//	}
		//}

		//private void mnuIturListPrintIS0155_Click(object sender, RoutedEventArgs e)
		//{
		//	IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
		//	if (viewModel != null)
		//	{
		//		viewModel.IturListPrintIS0155Command.Execute();
		//	}
		//}

		//private void mnuIturListPrintIS0160_Click(object sender, RoutedEventArgs e)
		//{
		//	IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
		//	if (viewModel != null)
		//	{
		//		viewModel.IturListPrintIS0160Command.Execute();
		//	}
		//}

		

        public void ListBoxItemMouseLeftButtonDown(object sender, MouseButtonEventArgs eventArgs)
        {
            if (eventArgs.ClickCount == 1)
            {

            }
            if (eventArgs.ClickCount == 2)
            {
                ListBoxItem listBoxItem = sender as ListBoxItem;
                if (listBoxItem != null)
                {
                    SelectedItemFromListBoxItem(listBoxItem);
                }
                eventArgs.Handled = true;
            }
        }

        private void ListBoxItemKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListBoxItem listBoxItem = sender as ListBoxItem;
                if (listBoxItem != null)
                {
                    SelectedItemFromListBoxItem(listBoxItem);
                }
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

            Utils.MainWindowTitleSet(WindowTitles.InventoryForm, this._eventAggregator);

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);

            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.MainRegionBackForward);
            UtilsNavigate.ApplicationStripNavigate(this._regionManager, query, Common.NavigationSettings.StripModeCustomerBranchInventor);

            IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;

                if (viewModel.State != null && viewModel.GetDbPath != dbPath)
                {
                    dbPath = viewModel.GetDbPath;
                }
            }

			_eventAggregator.GetEvent<PopupWindowCloseEvent>().Subscribe(PopupWindowClose);		    //!!!  Тест Вернуть
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
            if (viewModel != null)
                viewModel.PropertyChanged -= ViewModel_PropertyChanged;

            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                      MainRegionBackForward,                                                    
                                                  }, navigationContext);

            _eventAggregator.GetEvent<PopupWindowCloseEvent>().Unsubscribe(PopupWindowClose);			 //!!!  Тест Вернуть

        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsGrouping")
            {
                IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
                if (viewModel != null)
                    if (viewModel.IsGrouping)
                    {
                        //listIturs.GroupStyle.Add(new GroupStyle());
                        //listIturs.GroupStyle.First().HeaderTemplate = this.FindResource("groupHeaderStyle") as DataTemplate;
                        listIturs.GroupStyle.Clear();

                        if (viewModel.GroupBySelectedItem == null) return;

                        if (viewModel.GroupBySelectedItem.Value == ComboValues.GroupItur.LocationValue)
                            listIturs.GroupStyle.Add(this.FindResource("groupStyleLocation") as GroupStyle);
						else if (viewModel.GroupBySelectedItem.Value == ComboValues.GroupItur.StatusValue)
                            listIturs.GroupStyle.Add(this.FindResource("groupStyleStatus") as GroupStyle);
						else if (viewModel.GroupBySelectedItem.Value == ComboValues.GroupItur.TagValue)
							listIturs.GroupStyle.Add(this.FindResource("groupStyleTag") as GroupStyle);
                    }
                    else
                    {
                        listIturs.GroupStyle.Clear();
                    }
            }
        }

        void SelectedItemFromListBoxItem(ListBoxItem listBoxItem)
        {
            object item = listIturs.ItemContainerGenerator.ItemFromContainer(listBoxItem);
            IturDashboardItemViewModel itemViewModel = item as IturDashboardItemViewModel;
            if (itemViewModel != null)
            {
                IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
                if (viewModel != null)
                    viewModel.SelectedItem = itemViewModel;
            }
        }

        #region Implementation of IRegionMemberLifetime

        public bool KeepAlive
        {
            get { return true; }
        }

        #endregion

        void btnSpeedLink_Click(object sender, RoutedEventArgs e)
        {
            IturListDetailsViewModel viewModel = this.DataContext as IturListDetailsViewModel;
            if (viewModel == null)
                return;

            if (_windowSpeed != null)
            {
                _windowSpeed.Close();
                _windowSpeed = null;
            }
            else
            {
                UriQuery query = new UriQuery();
                Utils.AddContextToQuery(query, viewModel.Context);
                Utils.AddDbContextToQuery(query, viewModel.CBIDbContext);

                this._windowSpeed = UtilsPopup.BuildPopup(btnSpeedLink, this._unityContainer, Common.ViewNames.SpeedLinkView, PopupSpeedRegion, query, PopupMode.SpeedLink, 270, 255);

                _windowSpeed.Closed += PopupSpeed_Closed;

                _windowSpeed.Show();
            }
        }

        void PopupSpeed_Closed(object sender, EventArgs e)
        {
            if (_windowSpeed != null)
            {
                _windowSpeed.Closed -= PopupSpeed_Closed;
                UtilsPopup.NavigateFrom(_windowSpeed, _regionManager, PopupSpeedRegion);
                _windowSpeed = null;
            }
        }

        private void PopupWindowClose(object o)
        {
            if (_windowSpeed != null)
            {
                _windowSpeed.Close();
            }
        }

			
	}
}
