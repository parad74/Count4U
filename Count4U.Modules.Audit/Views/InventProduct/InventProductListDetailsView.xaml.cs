using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Count4U.Common.Behaviours;
using Count4U.Common.Constants;
using Count4U.Common.Interfaces;
using Count4U.Common.Misc.PopupExt;
using Count4U.Common.Services.Navigation.Data;
using Count4U.Common.Services.Navigation.Data.SearchData;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Count4U.Common.Helpers;
using Microsoft.Practices.Unity;
using Count4U.Configuration.Dynamic;
using Count4U.Common.UserSettings;

namespace Count4U.Modules.Audit.Views
{
    /// <summary>
    /// Interaction logic for Itur2.xaml
    /// </summary>
    public partial class InventProductListDetailsView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        private static readonly string PopupSearchRegion = Common.RegionNames.PopupSearchListDetailsInventProduct;

        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IUnityContainer _unityContainer;
        private readonly INavigationRepository _navigationRepository;
		private readonly IUserSettingsManager _userSettingsManager;

        private bool _escKeyByQuantityEditTextBox;
        private readonly PopupExtSearch _popupExtSearch;

        public InventProductListDetailsView(
            InventProductListDetailsViewModel viewModel,
            IRegionManager regionManager,
            IContextCBIRepository contextCbiRepository,
            IUnityContainer unityContainer,
            INavigationRepository navigationRepository,
            PopupExtSearch popupExtSearch,
			IUserSettingsManager userSettingsManager)
        {
            this.InitializeComponent();

            this._popupExtSearch = popupExtSearch;
            this._navigationRepository = navigationRepository;
            this._unityContainer = unityContainer;
            this._contextCbiRepository = contextCbiRepository;
            this._regionManager = regionManager;
			this._userSettingsManager = userSettingsManager;

            this.DataContext = viewModel;
            viewModel.InventProductDataGrid = dataGridInventProduct;

            this.PreviewKeyUp += InventProductListDetailsView_PreviewKeyUp;
            this.PreviewKeyDown += InventProductListDetailsView_PreviewKeyDown;
            this.Loaded += InventProductListDetailsView_Loaded;
            this.dataGridInventProduct.PreviewKeyDown += dataGridInventProduct_KeyDown;
			this.dataGridInventProduct.SelectionChanged += listView_SelectionChanged;
			this.editorTemplateCombobox.SelectionChanged += template_SelectionChanged;

            InventProductSearchData navigationData = new InventProductSearchData();
            if (!String.IsNullOrEmpty(viewModel.SearchExpression))
            {
                if (viewModel.SearchItem == ComboValues.FindInventProduct.FilterMakat)
                    navigationData.Makat = viewModel.SearchExpression;

                if (viewModel.SearchItem == ComboValues.FindInventProduct.FilterBarcode)
                    navigationData.Barcode = viewModel.SearchExpression;

                if (viewModel.SearchItem == ComboValues.FindInventProduct.FilterProduct)
                    navigationData.ProductName = viewModel.SearchExpression;
            }
            _popupExtSearch.Button = btnSearch;
            _popupExtSearch.NavigationData = navigationData;
            _popupExtSearch.Region = PopupSearchRegion;
            _popupExtSearch.ViewModel = viewModel;
            _popupExtSearch.Init();

            Interaction.GetBehaviors(dataGridInventProduct).Add(new GridCancelEditBehavior());
            Interaction.GetBehaviors(dataGridInventProduct).Add(new DataGridBehavior());

			Interaction.GetBehaviors(txtErpIturCode).Add(new TextBoxPropertyBehavior());

            System.Windows.Interactivity.Interaction.GetBehaviors(txtIturCode).Add(new TextBoxPropertyBehavior());
        }       

        public bool KeepAlive { get { return false; } }

        void InventProductListDetailsView_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => txtIpSearch.Focus()), DispatcherPriority.Background);

            InventProductListDetailsViewModel viewModel = this.DataContext as InventProductListDetailsViewModel;
            if (viewModel == null) return;

            viewModel.ReportButtonItur.BuildMenu(btnReportItur.ContextMenu);
            viewModel.ReportButtonDocument.BuildMenu(btnReportDocument.ContextMenu);
            viewModel.ReportButtonInventProduct.BuildMenu(btnReportInventProduct.ContextMenu);
        }

        void InventProductListDetailsView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && e.OriginalSource is TextBox)
            {
                if (e.OriginalSource != txtIpSearch)
                    _escKeyByQuantityEditTextBox = true;
            }
        }

        void InventProductListDetailsView_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_escKeyByQuantityEditTextBox)
                {
                    _escKeyByQuantityEditTextBox = false;
                    return;
                }

                if (UtilsNavigate.CanGoBack(this._regionManager))
                    UtilsNavigate.GoBack(this._regionManager);
            }
        }

        void dataGridInventProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                InventProductListDetailsViewModel viewModel = this.DataContext as InventProductListDetailsViewModel;
                if (viewModel != null)
                {
                    if (viewModel.DeleteInventProductCommand.CanExecute())
                    {
                        viewModel.DeleteInventProductCommandExecute();
                    }
                }
            }
        }

		void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{																													//detailDataGrid
			InventProductListDetailsViewModel viewModel = this.DataContext as InventProductListDetailsViewModel;
			photoPanel.DataContext = null;
			if (viewModel != null)
			{
				viewModel.SelectedItemSet(viewModel.InventProductSelectedItem);

				if (viewModel.InventProductSelectedItem != null)
				{
					dataGridInventProduct.ScrollIntoView(viewModel.InventProductSelectedItem);
					photoPanel.DataContext = viewModel.InventProductSelectedItem;
				//	photoImage.Source = viewModel.InventProductSelectedItem.ToolTipPaint;
				//photoTextBlock.Text = viewModel.InventProductSelectedItem.ToolTip;
				}

			}
			//NativPhoto.Visibility = System.Windows.Visibility.Hidden;
		}


		void template_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string templateName = _userSettingsManager.EditorTemplateSelectedItemGet();
			EditorTemplate currentEditorTemplate = editorTemplateCombobox.SelectedValue as EditorTemplate;
			if (currentEditorTemplate.Code == templateName)
			{
				NativPhoto.Visibility = System.Windows.Visibility.Visible;
				photoPanel.Visibility = System.Windows.Visibility.Visible;
				//photoImage.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				NativPhoto.Visibility = System.Windows.Visibility.Hidden;
				photoPanel.Visibility = System.Windows.Visibility.Hidden;
				//photoImage.Visibility = System.Windows.Visibility.Hidden;
			}

		
		}
		

        private void QuantityEditTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            //            TextBox textBox = sender as TextBox;
            //            if (textBox == null) return;
            //
            //            textBox.SelectAll();
        }

        private void QuantityEditTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //            if (e.Key == Key.Escape)
            //                _escKeyByQuantityEditTextBox = true;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.InventProductListDetailsBackForward);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.InventProductListDetailsBackForward);

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);

            InventProductListDetailsViewModel viewModel = this.DataContext as InventProductListDetailsViewModel;
            if (viewModel != null)
                viewModel.NeedScrollIntoView += ViewModel_NeedScrollIntoView;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        backForward                                         
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.InventProductListDetailsBackForward);

            InventProductListDetailsViewModel viewModel = this.DataContext as InventProductListDetailsViewModel;
            if (viewModel != null)
                viewModel.NeedScrollIntoView -= ViewModel_NeedScrollIntoView;
        }

        void ViewModel_NeedScrollIntoView(object sender, EventArgs e)
        {
            Utils.RunOnUI(() =>
                              {
                                  if (dataGridInventProduct.SelectedItem != null)
                                  {
                                      dataGridInventProduct.UpdateLayout();
                                      dataGridInventProduct.Focus();
                                      dataGridInventProduct.ScrollIntoView(dataGridInventProduct.SelectedItem, dataGridInventProduct.CurrentColumn);
                                  }
                              }, DispatcherPriority.Background);
        }

    }
}
