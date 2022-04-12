using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Behaviours;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Configuration.Dynamic;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels;
using Count4U.Modules.Audit.ViewModels.Catalog;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.InventProduct
{
    /// <summary>
    /// Interaction logic for InventProductView.xaml
    /// </summary>
    public partial class InventProductView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IEventAggregator _eventAggregator;
		private readonly IUserSettingsManager _userSettingsManager;
        private readonly Guid _guid;

        private readonly string backForwardRegionName;
        private readonly string searchFilterRegionName;

        public InventProductView(
            InventProductViewModel viewModel,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository,
			IUserSettingsManager userSettingsManager)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this._contextCbiRepository = contextCbiRepository;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
			this._userSettingsManager = userSettingsManager;

            this.Loaded += InventProductView_Loaded;
			this.masterDataGrid.SelectionChanged += listView_SelectionChanged;
			viewModel.DataGrid = masterDataGrid;
			this.detailDataGrid.SelectionChanged += propertyView_SelectionChanged;
			editorTemplateCombobox.SelectionChanged += template_SelectionChanged;

			Interaction.GetBehaviors(masterDataGrid).Add(new GridCancelEditBehavior());
			Interaction.GetBehaviors(masterDataGrid).Add(new DataGridBehavior());
			Interaction.GetBehaviors(detailDataGrid).Add(new GridCancelEditBehavior());
			Interaction.GetBehaviors(detailDataGrid).Add(new DataGridBehavior());
			
            Interaction.GetBehaviors(btnReports).Add(new ContextMenuLeftButtonBehavior());

            _guid = Guid.NewGuid();

            viewModel.SearchFilterRegionKey = _guid.ToString();

            backForwardRegionName = Common.RegionNames.InventProductBackForward + _guid.ToString();
            searchFilterRegionName = Common.RegionNames.InventProductSearchFilter + _guid.ToString();
        }

		void propertyView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{																													//detailDataGrid
			InventProductViewModel viewModel = this.DataContext as InventProductViewModel;
			//if (viewModel != null)
			//{
			//	viewModel.SelectedPropertySet(viewModel.DetailSelectedItem);
			//}
		}

		void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{																													//detailDataGrid
			InventProductViewModel viewModel = this.DataContext as InventProductViewModel;
			if (viewModel != null)
			{
				viewModel.SelectedItemSet(viewModel.SelectedItem);
				if (viewModel.SelectedItem != null)
				{
					masterDataGrid.ScrollIntoView(viewModel.SelectedItem);
				}

				//var selectedItems = masterDataGrid.SelectedItems.Cast<InventProductItemViewModel>().ToList();
				//viewModel.SelectedItemSet(selectedItems.FirstOrDefault());
					
				//if (viewModel.Items != null)
				//{
				//	InventProductItemViewModel selectedItem = viewModel.SelectedItem;
				//	InventProductItemViewModel firstItem = viewModel.ItemsFirstOrDefault();

					//if (selectedItem != null)
					//{
					//	if (viewModel.SelectedItem != null)
					//	{
					//		if (selectedItem.Num != viewModel.SelectedItem.Num
					//			&& selectedItem.Makat != viewModel.SelectedItem.Makat)
					//		{
					//			viewModel.SelectedItem = selectedItem;
					//		}
					//	}
					//	else
					//	{
					//		viewModel.SelectedItem = selectedItem;
					//	}
					//}
					//else if (firstItem != null)
					//{
					//	viewModel.SelectedItem = firstItem;
					//}
				//}
			}
		}

		void template_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string templateName = _userSettingsManager.EditorTemplateSelectedItemGet();
			EditorTemplate currentEditorTemplate = editorTemplateCombobox.SelectedValue as EditorTemplate;
			if (currentEditorTemplate.Code == templateName)
			{
				NativPhoto.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				NativPhoto.Visibility = System.Windows.Visibility.Hidden;
			}

			if (currentEditorTemplate.Code == "Compare")
			{
				FlagColumn.Visibility = System.Windows.Visibility.Visible;
				In1Column.Visibility = System.Windows.Visibility.Visible;
				In2Column.Visibility = System.Windows.Visibility.Visible;
				IPValueStr1Column.Visibility = System.Windows.Visibility.Visible;
				IPValueStr2Column.Visibility = System.Windows.Visibility.Visible;
				//QuantityEditCompareColumn.Visibility = System.Windows.Visibility.Visible;
				MarkColumn.Visibility = System.Windows.Visibility.Hidden;
				//editorTemplateCombobox.Visibility = System.Windows.Visibility.Hidden;
				//AcceptQuantityColumn.Visibility = System.Windows.Visibility.Hidden;
				//Accept1Column.Visibility = System.Windows.Visibility.Hidden;
				//Accept2Column.Visibility = System.Windows.Visibility.Hidden;
				//RsulteColumn.Visibility = System.Windows.Visibility.Hidden;

				StatusBitColumn.Visibility = System.Windows.Visibility.Hidden;
				//BarCodeColumn.Visibility = System.Windows.Visibility.Hidden;
				SerialNumberColumn.Visibility = System.Windows.Visibility.Visible;
				PropertyStr10Column.Visibility = System.Windows.Visibility.Visible;
				NumColumn.Visibility = System.Windows.Visibility.Hidden;
				InputTypeColumn.Visibility = System.Windows.Visibility.Hidden;
				CreateDateColumn.Visibility = System.Windows.Visibility.Hidden;
				//QuantityEditColumn.Visibility = System.Windows.Visibility.Hidden;
			}
			else
			{
				MarkColumn.Visibility = System.Windows.Visibility.Hidden;
				FlagColumn.Visibility = System.Windows.Visibility.Hidden;
				
				In1Column.Visibility = System.Windows.Visibility.Hidden;
				In2Column.Visibility = System.Windows.Visibility.Hidden;
				IPValueStr1Column.Visibility = System.Windows.Visibility.Hidden;
				IPValueStr2Column.Visibility = System.Windows.Visibility.Hidden;
				//editorTemplateCombobox.Visibility = System.Windows.Visibility.Visible;
				//QuantityEditCompareColumn.Visibility = System.Windows.Visibility.Hidden;
				//AcceptQuantityColumn.Visibility = System.Windows.Visibility.Hidden;
				//Accept1Column.Visibility = System.Windows.Visibility.Hidden;
				//Accept2Column.Visibility = System.Windows.Visibility.Hidden;
				//RsulteColumn.Visibility = System.Windows.Visibility.Hidden;

				StatusBitColumn.Visibility = System.Windows.Visibility.Visible;
				//BarCodeColumn.Visibility = System.Windows.Visibility.Visible;
				SerialNumberColumn.Visibility = System.Windows.Visibility.Visible;
				PropertyStr10Column.Visibility = System.Windows.Visibility.Hidden;
				NumColumn.Visibility = System.Windows.Visibility.Visible;
				InputTypeColumn.Visibility = System.Windows.Visibility.Visible;
				CreateDateColumn.Visibility = System.Windows.Visibility.Visible;
				//QuantityEditColumn.Visibility = System.Windows.Visibility.Visible;
			}
		}

        public bool KeepAlive { get { return false; } }

        void InventProductView_Loaded(object sender, RoutedEventArgs e)
        {
            InventProductViewModel viewModel = this.DataContext as InventProductViewModel;
            if (viewModel != null && btnReports.ContextMenu != null)
                viewModel.ReportButtonViewModel.BuildMenu(btnReports.ContextMenu);
			//InventProductItemViewModel selectedItem = viewModel.Items.FirstOrDefault();
			//viewModel.SelectedItem = selectedItem;
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, backForwardRegionName);
            RegionManager.SetRegionName(searchFilter, searchFilterRegionName);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            Utils.MainWindowTitleSet(WindowTitles.InventProduct, this._eventAggregator);

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, backForwardRegionName);
            UtilsNavigate.SearchFilterNavigate(this._regionManager, searchFilterRegionName);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                      backForward,
                                                      searchFilter,
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(backForwardRegionName);
            this._regionManager.Regions.Remove(searchFilterRegionName);
        }

        #endregion

        private void QuantityEditTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            textBox.SelectAll();
        }
    }
}


	 //  <DataGridTemplateColumn x:Name ="AcceptQuantityColumn" Header="{x:Static Localization:Resources.View_CommonInventProduct_columnAcceptQuantity}"
	 //									 Width="90" IsReadOnly="True" >
	 //				   <DataGridTemplateColumn.CellTemplate>
	 //					   <DataTemplate>
	 //						   <Grid >
	 //							   <Grid.ColumnDefinitions>
	 //								   <ColumnDefinition Width="*" />
	 //							   </Grid.ColumnDefinitions>

	 //							   <ImageButtonCursor:ImageButtonCursor Grid.Column="0" 
	 //											   HorizontalAlignment="Center" 
	 //											   ToolTip="{Binding DataContext.AcceptQuantityCommand.Title, RelativeSource={RelativeSource AncestorType={x:Type DataGrid}}}"                                                                                     
	 //											   Command="{Binding DataContext.AcceptQuantityCommand, RelativeSource={RelativeSource AncestorType={x:Type DataGrid}}}"
	 //											   CommandParameter="{Binding .}" 
	 //											   ImageSource="{Binding ShowAccept, Mode=OneWay, Converter={StaticResource stringToImage}}"
	 //											   ImageWidth="12" ImageHeight="12"/>

	 //						   </Grid>
	 //					   </DataTemplate>
	 //				   </DataGridTemplateColumn.CellTemplate>
	 //			   </DataGridTemplateColumn>




	 //			   <DataGridTemplateColumn x:Name ="RsulteColumn" Header="{x:Static Localization:Resources.InventProductListDetailsView_Compare_Column_PropertyFloat4}"
	 //								   Width="60" IsReadOnly="True" >
	 //				   <DataGridTemplateColumn.CellTemplate>
	 //					   <DataTemplate>
	 //						   <TextBlock Text="{Binding Path=IPValueInt4, Mode=OneWay}" HorizontalAlignment="Left" FontWeight="Bold" />
	 //					   </DataTemplate>
	 //				   </DataGridTemplateColumn.CellTemplate>
	 //			   </DataGridTemplateColumn>


	 //   <DataGridTemplateColumn x:Name ="Accept1Column" Header="{x:Static Localization:Resources.View_CommonInventProduct_columnAccept1}"
	 //									   Width="60" IsReadOnly="True" >
	 //				   <DataGridTemplateColumn.CellTemplate>
	 //					   <DataTemplate>
	 //						   <Grid >
	 //							   <Grid.ColumnDefinitions>
	 //								   <ColumnDefinition Width="*" />
	 //							   </Grid.ColumnDefinitions>

	 //							   <ImageButtonCursor:ImageButtonCursor Grid.Column="0" 
	 //											   HorizontalAlignment="Center" 
	 //											   ToolTip="{Binding DataContext.Accept1Command.Title, RelativeSource={RelativeSource AncestorType={x:Type DataGrid}}}"                                                                                     
	 //											   Command="{Binding DataContext.Accept1Command, RelativeSource={RelativeSource AncestorType={x:Type DataGrid}}}"
	 //											   CommandParameter="{Binding .}" 
	 //											   ImageSource="{Binding ShowAccept, Mode=OneWay, Converter={StaticResource stringToImage}}"
	 //											   ImageWidth="12" ImageHeight="12"/>


	 //						   </Grid>
	 //					   </DataTemplate>
	 //				   </DataGridTemplateColumn.CellTemplate>
	 //			   </DataGridTemplateColumn>

	 //<DataGridTemplateColumn x:Name ="Accept2Column" Header="{x:Static Localization:Resources.View_CommonInventProduct_columnAccept2}"
	 //									 Width="60" IsReadOnly="True" >
	 //				   <DataGridTemplateColumn.CellTemplate>
	 //					   <DataTemplate>
	 //						   <Grid >
	 //							   <Grid.ColumnDefinitions>
	 //								   <ColumnDefinition Width="*" />
	 //							   </Grid.ColumnDefinitions>

	 //							   <ImageButtonCursor:ImageButtonCursor Grid.Column="0" 
	 //											   HorizontalAlignment="Center" 
	 //											   ToolTip="{Binding DataContext.Accept2Command.Title, RelativeSource={RelativeSource AncestorType={x:Type DataGrid}}}"                                                                                     
	 //											   Command="{Binding DataContext.Accept2Command, RelativeSource={RelativeSource AncestorType={x:Type DataGrid}}}"
	 //											   CommandParameter="{Binding .}" 
	 //											   ImageSource="{Binding ShowAccept, Mode=OneWay, Converter={StaticResource stringToImage}}"
	 //											   ImageWidth="12" ImageHeight="12"/>

	 //						   </Grid>
	 //					   </DataTemplate>
	 //				   </DataGridTemplateColumn.CellTemplate>
	 //			   </DataGridTemplateColumn>