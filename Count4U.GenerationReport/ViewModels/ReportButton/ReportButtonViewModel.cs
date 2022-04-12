using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Common.View.DragDrop.Utilities;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.GenerationReport;
using Count4U.Localization;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism.Commands;
using NLog;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Extensions;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Count4U.Model.Interface.Main;

namespace Count4U.Report.ViewModels.ReportButton
{
    public class ReportButtonViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IReportRepository _reportRepository;
        private readonly ICustomerReportRepository _customerReportRepository;
        
        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IUserSettingsManager _userSettingsManager;

		private Func<Tuple<SelectParams, Itur, Location, DocumentHeader, Device>> _getSelectParams;

        private ViewDomainContextEnum _viewDomainContextEnum;
        private Action _runReportForm;
        private ContextMenu _contextMenu;
        private Func<IFilterData> _getFilterData;

        Dictionary<string, TabItem> _itemDictionary ;
        Dictionary<string, MenuTagItemViewModel> _tagItemDictionary;
        Dictionary<string, Grid> _gridDictionary;
        Dictionary<string, int> _indexDictionary;
        public Dictionary<ViewDomainContextEnum, Dictionary<string, MenuTagItemViewModel>> _viewDomainContextTagItemDictionary;

        public ReportButtonViewModel(
            IReportRepository reportRepository,
            ICustomerReportRepository customerReportRepository ,
            IContextCBIRepository contextCbiRepository,
            IGenerateReportRepository generateReportRepository,
            IUserSettingsManager userSettingsManager
            )
            : base(contextCbiRepository)
        {
            _userSettingsManager = userSettingsManager;
            _generateReportRepository = generateReportRepository;
            _reportRepository = reportRepository;
            _customerReportRepository = customerReportRepository;
        }

        public void Initialize(Action runReportForm, 
            Func<Tuple<SelectParams, Itur, Location, DocumentHeader, Device>> getSelectParams, 
            ViewDomainContextEnum viewDomainContextEnum,
            Func<IFilterData> getFilterData = null)
        {
            _getFilterData = getFilterData;
            _getSelectParams = getSelectParams;
            _runReportForm = runReportForm;
            _viewDomainContextEnum = viewDomainContextEnum;
        }

        public void BuildMenu(ContextMenu contextMenu)
        {
            _contextMenu = contextMenu;

            FlowDirection direction = _userSettingsManager.LanguageGet() == enLanguage.Hebrew ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            _contextMenu.FlowDirection = direction;

            if (contextMenu == null) return;
            ItemsClear();

			Task.Factory.StartNew(Build).LogTaskFactoryExceptions("BuildMenu");
            //Build();
        }

        private void ItemsClear()
        {
            if (this._itemDictionary != null)
            {
                foreach (var item in this._itemDictionary) 
                {
                    if (item.Value == null) continue;
                    try
                    {
                        item.Value.MouseLeftButtonDown -= TabItem_MouseLeftButtonDown;
                        item.Value.MouseLeftButtonUp -= TabItem_MouseLeftButtonUp;
                    }
                    catch { }
                }

                this._itemDictionary.Clear();
            }
            if (this._gridDictionary != null) this._gridDictionary.Clear();
            if (this._indexDictionary != null) this._indexDictionary.Clear();

            List<object> items = new List<object>();
            foreach (object item in _contextMenu.Items)
            {
                items.Add(item);
            }

            foreach (object item in items)
            {
                _contextMenu.Items.Remove(item);
            }
           

        }

        public void Rebuild(ViewDomainContextEnum viewDomainContextEnum)
        {
            _viewDomainContextEnum = viewDomainContextEnum;
            ItemsClear();
            Build();
            //Task.Factory.StartNew(Build);
        }

        public void RebuildAfterApply()
        {

          //  _viewDomainContextEnum = ViewDomainContextEnum.Iturs;
            ItemsClear();
            Build();
            //Task.Factory.StartNew(Build);
        }

        private void Build()
        {
            try
            {
                Customer customer = base.CurrentCustomer;
                Branch branch = base.CurrentBranch;
                Inventor inventor = base.CurrentInventor;
                
                if (_viewDomainContextTagItemDictionary == null || this._userSettingsManager.RefreshedReport == true)
                {
                    _viewDomainContextTagItemDictionary = new Dictionary<ViewDomainContextEnum, Dictionary<string, MenuTagItemViewModel>>();
                    this._userSettingsManager.RefreshedReport = false;
                }
                //            if (customer == null && branch == null && inventor == null)
                //            {
                //                _logger.Error("ReportButtonViewModel is not initialized");
                //                return;
                //            }

                Reports reports = this._reportRepository.GetAllowedReportTemplate(
                    customer == null ? String.Empty : customer.Code,
                    branch == null ? String.Empty : branch.Code,
                    inventor == null ? String.Empty : inventor.Code,
                    _viewDomainContextEnum,
                    new List<AllowedReportTemplate>() { AllowedReportTemplate.Menu }
                    );

                if (_viewDomainContextTagItemDictionary.ContainsKey(_viewDomainContextEnum) == false)
				{
                    _viewDomainContextTagItemDictionary[_viewDomainContextEnum] = null;
                }
                if (reports == null) return;

				Utils.RunOnUI(() =>
									   {
										   try
				                            {
                                               TabControl tabControl = new TabControl();
                                               tabControl.Margin = new Thickness(0, 0, 0, 0);
                                               tabControl.VerticalAlignment = VerticalAlignment.Stretch;
                                               tabControl.Padding = new Thickness(0, 0, 0, 0);

                                               TabItem itemAll = new TabItem();
                                               itemAll.Background = new SolidColorBrush(ColorArray.ItemArray[0]);
                                               itemAll.Background.Opacity = 0.5;
                                               itemAll.MouseLeftButtonDown += TabItem_MouseLeftButtonDown;
                                               itemAll.MouseLeftButtonUp += TabItem_MouseLeftButtonUp;

                                               StackPanel stackPanelAll = new StackPanel();
                                               stackPanelAll.Orientation = Orientation.Horizontal;
                                               stackPanelAll.Margin = new Thickness(0, 0, 0, 0);
                                               stackPanelAll.Width = 40;
                                               stackPanelAll.HorizontalAlignment = HorizontalAlignment.Center;
                                              
                                               TextBlock textBlockAll = new TextBlock();
                                               textBlockAll.Text = Localization.Resources.View_ReportTemplate_chkAll;
                                               textBlockAll.Width = 40;
                                               textBlockAll.TextAlignment = TextAlignment.Center;
                                               textBlockAll.ToolTip = Localization.Resources.View_ReportTemplate_chkAll;

                                               stackPanelAll.Children.Add(textBlockAll);
                                               itemAll.Header = stackPanelAll;

                                               Grid gridAll = new Grid();

                                               List<string> tags = reports.Select(r => r.Tag).Distinct().OrderBy(x=>x).ToList();
                                               tags.Add(">>");
                                               
                                               if (_viewDomainContextTagItemDictionary[_viewDomainContextEnum] == null)
                                               {
                                                   _viewDomainContextTagItemDictionary[_viewDomainContextEnum] = new Dictionary<string, MenuTagItemViewModel>();
                                                   int tagIndex1 = 1;
                                                   List<CustomerReport> tagIsChecked = this._customerReportRepository.GetCustomerReportListByCodeAndContext("TagIsChecked", _viewDomainContextEnum.ToString());
                                                   foreach (var tag in tags)
                                                   {
                                                       if (tag == "-") continue;
                                                       if (string.IsNullOrWhiteSpace(tag) == true) continue;

                                                       _viewDomainContextTagItemDictionary[_viewDomainContextEnum][tag] =
                                                       new MenuTagItemViewModel() { Text = tag, IsChecked = false };
                                                       if (tagIsChecked.Count == 0)  //состояние в    БД не сохранено
                                                       {
                                                           if (tagIndex1 < 9)     //10
                                                           {
                                                               _viewDomainContextTagItemDictionary[_viewDomainContextEnum][tag].IsChecked = true;
                                                               tagIndex1++;
                                                           }
                                                       }
                                                   }


                                                   if (tagIsChecked.Count > 0)       //состояние в    БД сохранено
                                                   {
                                                       foreach (var t in tagIsChecked)
                                                       {
                                                           if (_viewDomainContextTagItemDictionary[_viewDomainContextEnum].ContainsKey(t.Name) == true)
                                                           {
                                                               _viewDomainContextTagItemDictionary[_viewDomainContextEnum][t.Name].IsChecked = true;
                                                           }
                                                       }
                                                   }

                                               }
                                               else
                                               {
                                                   // int tagIndex1 = 1;
                                                   foreach (var tag in tags)
                                                   {
                                                       if (tag == "-") continue;
                                                       if (string.IsNullOrWhiteSpace(tag) == true) continue;
                                                       if (_viewDomainContextTagItemDictionary[_viewDomainContextEnum].ContainsKey(tag) == true)
                                                       {
                                                           //  tagIndex1++;
                                                           continue;
                                                       }
                                                       else
                                                       {
                                                           _viewDomainContextTagItemDictionary[_viewDomainContextEnum][tag] =
                                                           new MenuTagItemViewModel() { Text = tag, IsChecked = false };
                                                           //if (tagIndex1 < 9)     //10
                                                           //{
                                                           //    _tagItemDictionary[tag].IsChecked = true;
                                                           //    tagIndex1++;
                                                           //}
                                                       }
                                                   }
                                               }
                                             

                                                this._itemDictionary = new Dictionary<string, TabItem>();
                                               this._gridDictionary = new Dictionary<string, Grid>();
                                               this._indexDictionary = new Dictionary<string, int>();
											   int colorindex = 1;

                                               foreach (string tag in tags)
                                               {
                                                   if (tag == "-") continue;
                                                   int i = colorindex % 13;
                                                
                                                   TabItem item = new TabItem();
                                                   Color color = ColorArray.ItemArray[i];
                                                   item.Background = new SolidColorBrush(color);
                                                   item.Background.Opacity = 0.5;
                                                   item.MouseLeftButtonDown += TabItem_MouseLeftButtonDown;
                                                   item.MouseLeftButtonUp += TabItem_MouseLeftButtonUp;

                                                   StackPanel stackPanel = new StackPanel();
                                                   stackPanel.Background = Brushes.Transparent;
                                                   stackPanel.Orientation = Orientation.Horizontal;
                                                   stackPanel.Width = 40;
                                                   
                                                   TextBlock textBlock = new TextBlock();
                                                   textBlock.Text = tag;
                                                   textBlock.Width = 40;
                                                   textBlock.ToolTip = tag;
                                                   textBlock.TextAlignment = TextAlignment.Center;
                                                   // < Image Source = "{x:Static uiCommandService:UICommandIconRepository.Report}" Width = "24" Height = "24" />
                                                   
                                                   stackPanel.Children.Add(textBlock);
                                                  
                                                   //Image image = new Image();
                                                   //image.Source =  new BitmapImage(new Uri("pack://application:,,,/Count4U.Media;component/Icons/delete_very_small.png"));
                                                   //stackPanel.Children.Add(image);

                                                   item.Header = stackPanel;
                                                   //  delete_very_small.png
                                                   //add_very_small.png

                                                   Grid grid = new Grid();
                                                   this._gridDictionary[tag] = grid;
                                                   this._itemDictionary[tag] = item;
                                                   this._indexDictionary[tag] = 0;

                                                   colorindex++;
                                               }

                                               int index = 0;
                                               foreach (Count4U.GenerationReport.Report report in reports.OrderBy(r => r.NN))
                                               {
                                                   if (String.IsNullOrWhiteSpace(report.Code))
                                                   {
                                                       continue;
                                                   }


                                                   FrameworkElement menuItem = BuildMenuItem(report, _reportRepository.IsSearchView(_viewDomainContextEnum));
                                                   FrameworkElement menuItem1 = BuildMenuItem(report, _reportRepository.IsSearchView(_viewDomainContextEnum));

                                                   gridAll.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                                   Grid.SetRow(menuItem, index);
                                                   gridAll.Children.Add(menuItem);
                                                   index++;

                                                   if (string.IsNullOrWhiteSpace(report.Tag) == false)
                                                   {
                                                       if (report.Tag != "-")
                                                       {      
                                                           this._gridDictionary[report.Tag].RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                                           Grid.SetRow(menuItem1, this._indexDictionary[report.Tag]);
                                                           this._gridDictionary[report.Tag].Children.Add(menuItem1);
                                                           this._indexDictionary[report.Tag]++;
                                                       }
                                                   }

                                                   //OLD
                                                   //////FrameworkElement menuItem = BuildMenuItem(report, _reportRepository.IsSearchView(_viewDomainContextEnum));
                                                   //////this._contextMenu.Items.Add(menuItem);
                                               }
											   itemAll.Content = gridAll;
											   tabControl.Items.Add(itemAll);

                                               CustomerReport  customerReport =_customerReportRepository.GetCustomerReportByCodeAndContext("Tag", _viewDomainContextEnum.ToString());
                                               _customerReportRepository.DeleteAllCodeAndContext("TagIsChecked", _viewDomainContextEnum.ToString());
                                               //==================== tabs

                                               int tagIndex = 1;
                                               int selectTagIndex = 0;
                                               foreach (string tag in tags)
                                               {
                                                   if (tag == "-") continue;
                                                   if (tag == ">>") continue;
                                                   if (tagIndex < 9)     //10
                                                   {
                                                       if (this._gridDictionary[tag].RowDefinitions.Count > 0)
                                                       {
                                                           if (_viewDomainContextTagItemDictionary[_viewDomainContextEnum][tag].IsChecked == true)
                                                           {
                                                               //save in db
                                                               try
                                                               {
                                                                   CustomerReport newCustomerReport = new CustomerReport
                                                                   {
                                                                       ReportCode = "TagIsChecked",
                                                                       Name = tag,
                                                                       CustomerCode = _viewDomainContextEnum.ToString()
                                                                   };
                                                                   _customerReportRepository.Insert(newCustomerReport);
                                                               }
                                                               catch { }

                                                               this._itemDictionary[tag].Content = this._gridDictionary[tag];
                                                               tabControl.Items.Add(this._itemDictionary[tag]);
                                                               //_tagItemDictionary[tag].IsChecked = true;
                                                               if (customerReport != null) 
                                                               {
                                                                   if (tag == customerReport.Name) selectTagIndex = tagIndex;
                                                               }
                                                               tagIndex++;
                                                           }
                                                           
                                                       }
                                                   }
                                               }
                                               
                                               tabControl.SelectedIndex = selectTagIndex;

                                               //==================== Tag filter tab
                                               foreach (string tag in tags)
                                               {

                                                   if (tag != "-")
                                                   {
                                                       if (tag != ">>")
                                                       {
                                                           if (string.IsNullOrWhiteSpace(tag) == false)
                                                           {
                                                               try
                                                               {
                                                                   FrameworkElement tagMenuItem = BuildTagItem(_viewDomainContextTagItemDictionary[_viewDomainContextEnum][tag]);
                                                                   this._gridDictionary[">>"].RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                                                   Grid.SetRow(tagMenuItem, this._indexDictionary[">>"]);
                                                                   this._gridDictionary[">>"].Children.Add(tagMenuItem);
                                                                   this._indexDictionary[">>"]++;
                                                               }
                                                               catch (Exception ex) 
                                                               { 
                                                               }
                                                           }
                                                       }
                                                   }
                                                   
                                               }

                                               if (this._indexDictionary[">>"] > 0)
                                               {
                                                   FrameworkElement applayMenuItem = BuildApplyButtonItem();
                                                   this._gridDictionary[">>"].RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                                   Grid.SetRow(applayMenuItem, this._indexDictionary[">>"]);
                                                   this._gridDictionary[">>"].Children.Add(applayMenuItem);
                                                   this._indexDictionary[">>"]++;

                                                   try
                                                   {
                                                       if (this._gridDictionary[">>"].RowDefinitions.Count > 0)
                                                       {
                                                           this._itemDictionary[">>"].Content = this._gridDictionary[">>"];

                                                           tabControl.Items.Add(this._itemDictionary[">>"]);
                                                       }

                                                   }
                                                   catch { }
                                               }

                                               this._contextMenu.Width = 620;
                                               this._contextMenu.MaxHeight = 600;
                                               this._contextMenu.Margin = new Thickness(-32, -4, -44, -7);      //-20,0,-40,0
                                               this._contextMenu.Padding = new Thickness(0, 0, 0, 0);

                                               this._contextMenu.Background = tabControl.Background;
                                               this._contextMenu.Items.Add(tabControl);
                                               this._contextMenu.VerticalOffset = 24;

                                           }
                                           catch (Exception exc)
                                           {
                                               _logger.ErrorException("Build report menu async", exc);
                                           }
               });    //  Utils.RunOnUI
        }
            catch (Exception exc)
            {
                _logger.ErrorException("Build report menu", exc);
            }
        }


        //void AddContextMenu(ContextMenu tabContextMenu)
        //{
        //    MenuItem newQuery = new MenuItem();
        //    newQuery.Header = "Delete Tab";

        //    newQuery.Click += DeleteTab_Click;

        //    tabContextMenu.Items.Add(newQuery);
        //}

        //void DeleteTab_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mi = sender as MenuItem;
        //    if (mi != null)
        //    {
        //        ContextMenu cm = mi.Parent as ContextMenu;
        //        if (cm != null)
        //        {
                 
        //        }
        //    }
        //}

        void TabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tabItem = sender as TabItem;
            string tag = tabItem.Header.ToString();
            e.Handled = true;
        }

        void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
            try
            {
                var tabItem = sender as TabItem;
                var stackPanel = tabItem.Header as StackPanel;
                var textBlock = stackPanel.Children[0] as TextBlock;
                string tag = textBlock.Text;
                CustomerReport customerReport = new CustomerReport(){ 
                    ReportCode = "Tag", 
                    CustomerCode = _viewDomainContextEnum.ToString(),
                    Name = tag
                } ;                                                                                      
                _customerReportRepository.UpdateCustomerReportByCodeAndContext(customerReport);
            }
            catch { }
            e.Handled = true;
        }

   
        private void MenuCommandExecuted(GenerationReport.Report report)
        {
            SelectParams sp = null;
            Itur itur = null;
            Location location = null;
            DocumentHeader dh = null;
			Device device = null;
            IFilterData filterData = null;

            if (this._getSelectParams != null)
            {
                Tuple<SelectParams, Itur, Location, DocumentHeader, Device> p = this._getSelectParams();
                sp = p.Item1;
				if (sp != null)
				{
					sp.IsEnablePaging = false;
					if (sp.FilterParams.ContainsKey("ResultCode") == true)		//"XXX"
					{
						sp.FilterParams.Remove("ResultCode");
					}
				}
                itur = p.Item2;
                location = p.Item3;
                dh = p.Item4;
				device = p.Item5;

            }

            if (this._getFilterData != null)
            {
                filterData = _getFilterData();
            }

            GenerateReportArgs args = new GenerateReportArgs
                                          {
                                              Report = report,
                                              ViewDomainContextType = this._viewDomainContextEnum,
                                              Customer = base.CurrentCustomer,
                                              Branch = base.CurrentBranch,
                                              Inventor = base.CurrentInventor,
                                              DbPath = base.GetDbPath,
                                              SelectParams = sp,
                                              Itur = itur,
                                              Location = location,
                                              Doc = dh,
											  Device = device,
                                              FilterData = filterData,
                                          };

            this._generateReportRepository.GenerateReport(args);
            //this._generateReportRepository.RunPrintReport(args);
        }


        private void MenuTagSelectCommandExecuted(MenuTagItemViewModel tag)
        {
            if (tag == null) return;
            tag.IsChecked = !tag.IsChecked;
            
        }
        private void MenuPrintCommandExecuted(GenerationReport.Report report/*, bool clearIturAnalysisAfterPrint = true*/)
        {
			// report.Print - use like clearIturAnalysisAfterPrint
            SelectParams sp = null;
            Itur itur = null;
            Location location = null;
            DocumentHeader dh = null;
			Device device = null;
            IFilterData filterData = null;

            if (this._getSelectParams != null)
            {
				Tuple<SelectParams, Itur, Location, DocumentHeader, Device> p = this._getSelectParams();
                sp = p.Item1;
				if (sp != null)
				{
					sp.IsEnablePaging = false;
					if (sp.FilterParams.ContainsKey("ResultCode") == true)		//"XXX"
					{
						sp.FilterParams.Remove("ResultCode");
					}
				}
                itur = p.Item2;
                location = p.Item3;
                dh = p.Item4;
				device = p.Item5;
            }

            if (this._getFilterData != null)
            {
                filterData = _getFilterData();
            }

            GenerateReportArgs args = new GenerateReportArgs
            {
                Report = report,
                ViewDomainContextType = this._viewDomainContextEnum,
                Customer = base.CurrentCustomer,
                Branch = base.CurrentBranch,
                Inventor = base.CurrentInventor,
                DbPath = base.GetDbPath,
                SelectParams = sp,
                Itur = itur,
                Location = location,
                Doc = dh,
				Device = device,
                FilterData = filterData,
            };

            //this._generateReportRepository.GenerateReport(args);
			this._generateReportRepository.RunPrintReport(args, report.Print); /*(args, clearIturAnalysisAfterPrint);*///false - для сеч принта и true - для топ меню);
        }

        public string ReportCaption(Count4U.GenerationReport.Report report)
        {
            string localizedName = report.MenuCaption;
            if (string.IsNullOrWhiteSpace(localizedName) == true)
            {
                localizedName = _generateReportRepository.GetLocalizedReportName(report);
            }

//            return report.CodeReport + " " + localizedName;
            return report.CodeReport.Replace("Rep-", String.Empty) + " " + localizedName;
        }

        private FrameworkElement BuildMenuItem(Count4U.GenerationReport.Report report, bool isSearchView)
        {
            FrameworkElement result = null;
            if (report.FileName == "-")
            {
                result = new Separator();
            }
            else
            {
                FlowDirection direction = _userSettingsManager.LanguageGet() == enLanguage.Hebrew ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

                MenuItem menuItem = new MenuItem();
                menuItem.FlowDirection = direction;
                menuItem.Style = Application.Current.TryFindResource("ChromeMenuItemStyle") as Style;

                Grid grid = new Grid();
                grid.Background = Brushes.Transparent;
                menuItem.Header = grid;

				//grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

				//grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });
				//grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60) });
				//grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60) });

				string reportText = ReportCaption(report);

                TextBlock tb = new TextBlock();
                Grid.SetColumn(tb, 0);
                tb.Text = reportText;
                tb.Margin = new Thickness(0, 0, 15, 0);
                tb.VerticalAlignment = VerticalAlignment.Center;

                grid.Children.Add(tb);

				report.Print = true; 
                Button btnView = BuildButton(Localization.Resources.Command_View, new DelegateCommand<GenerationReport.Report>(MenuCommandExecuted), report);
                Grid.SetColumn(btnView, 1);
                grid.Children.Add(btnView);

				if (isSearchView == true)
				{
					report.Print = false; 		// for param clearIturAnalysisAfterPrint
				}
				Button btnPrint = BuildButton(Localization.Resources.Command_Print, new DelegateCommand<GenerationReport.Report>(MenuPrintCommandExecuted), report); //false
				Grid.SetColumn(btnPrint, 2);
				grid.Children.Add(btnPrint);

                grid.PreviewMouseUp += MenuItem_PreviewMouseUp;

                menuItem.Command = new DelegateCommand<GenerationReport.Report>(MenuCommandExecuted);
                menuItem.CommandParameter = report;

                result = menuItem;
            }

            return result;
        }

        private Button BuildButton(string header, DelegateCommand<GenerationReport.Report> command, object commandParameter)
        {
            Button result = new Button();

            result.Width = 50;
            result.Height = 23;
            result.Content = header;
            result.Margin = new Thickness(0, 0, 0, 0);
            result.Style = Application.Current.TryFindResource("ButtonChromeMenuItemStyle") as Style;
            result.Command = command;
            result.CommandParameter = commandParameter;

            return result;
        }

    

        private FrameworkElement BuildTagItem(MenuTagItemViewModel tag)
        {
            FrameworkElement result = null;

            FlowDirection direction = _userSettingsManager.LanguageGet() == enLanguage.Hebrew ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            MenuItem menuItem = new MenuItem();
            menuItem.FlowDirection = direction;
            menuItem.Style = Application.Current.TryFindResource("ChromeMenuItemStyle") as Style;

            Grid grid = new Grid();
            grid.Background = Brushes.Transparent;
            menuItem.Header = grid;

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            CheckBox checkBox = new CheckBox();
            Grid.SetColumn(checkBox, 0);
            checkBox.Content = tag.Text;
            checkBox.IsChecked = tag.IsChecked;
            checkBox.Margin = new Thickness(0, 0, 15, 0);
            checkBox.VerticalAlignment = VerticalAlignment.Center;
            checkBox.Checked += CheckBoxChanged;
            checkBox.Unchecked += CheckBoxChanged;
            grid.Children.Add(checkBox);

            //TextBlock tb = new TextBlock();
            //    Grid.SetColumn(tb, 0);
            //    tb.Text = tag;
            //    tb.Margin = new Thickness(0, 0, 15, 0);
            //    tb.VerticalAlignment = VerticalAlignment.Center;

            //    grid.Children.Add(tb);


            //< CheckBox Content = "{x:Static Localization:Resources.View_ImportAdapter_tbWithQuantityERP}"
            //    Height = "16" HorizontalAlignment = "Left"
            //    IsChecked = "{Binding Path=WithQuantityErp}"
            //    Margin = "10,13,0,0" VerticalAlignment = "Top" Width = "170" Grid.Row = "2" Checked = "CheckBox_Checked" />

            //Button btnView = BuildButton(Localization.Resources.Command_View, new DelegateCommand<GenerationReport.Report>(MenuCommandExecuted), report);
            //Grid.SetColumn(btnView, 1);
            //grid.Children.Add(btnView);

            //Button btnPrint = BuildButton(Localization.Resources.Command_Print, new DelegateCommand<GenerationReport.Report>(MenuPrintCommandExecuted), report); //false
            //Grid.SetColumn(btnPrint, 2);
            //grid.Children.Add(btnPrint);


            grid.PreviewMouseUp += MenuItemTag_PreviewMouseUp;


            //menuItem.Command = new DelegateCommand<MenuTagItemViewModel>(MenuTagSelectCommandExecuted);
            //menuItem.CommandParameter = tag;

            result = menuItem;


            return result;
        }

        private FrameworkElement BuildApplyButtonItem()
        {

            FrameworkElement result = null;

            FlowDirection direction = _userSettingsManager.LanguageGet() == enLanguage.Hebrew ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            MenuItem menuItem = new MenuItem();
            menuItem.FlowDirection = direction;
            menuItem.Style = Application.Current.TryFindResource("ButtonStyleV7") as Style;

            Grid grid = new Grid();
            grid.Background = Brushes.Transparent;
            menuItem.Header = grid;

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(400) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.PreviewMouseUp += MenuItemTag_PreviewMouseUp;

            Button btnApplay = BuildApplyButton(Localization.Resources.Command_Apply);
            Grid.SetColumn(btnApplay, 0);
            grid.Children.Add(btnApplay);
            result = menuItem;

            return result;

        }

        private Button BuildApplyButton(string header/*, object commandParameter*/)
        {
            Button result = new Button();

            result.Width = 70;
            result.Height = 23;
            result.Content = header;
            result.Background = new SolidColorBrush(Colors.YellowGreen);
            result.Background.Opacity = 0.7;

            result.VerticalAlignment = VerticalAlignment.Center;
            result.HorizontalAlignment = HorizontalAlignment.Left;
            result.Margin = new Thickness(10, 20, 0, 0);
               //< Grid.Background >
               //                     < LinearGradientBrush EndPoint = "0.5,1" StartPoint = "0.5,0" >
               //                            < GradientStop Color = "#FFD2FA91" Offset = "0" />
               //                               < GradientStop Color = "#FFAFEA4B" Offset = "1" />
               //                              </ LinearGradientBrush >
               //                          </ Grid.Background >


                     result.Style = Application.Current.TryFindResource("ButtonStyleV7") as Style;
            result.Command = new DelegateCommand(RebuildAfterApply);
            //result.CommandParameter = commandParameter;

            return result;
        }
        
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            var tagItemDictionary = this._viewDomainContextTagItemDictionary[_viewDomainContextEnum];
            string Tag = checkBox.Content.ToString();
            var itemsCheck = tagItemDictionary.Values.Where(k => k.IsChecked == true).Select(k => k).ToList();
            bool? isChecked = checkBox.IsChecked == null ? false : checkBox.IsChecked;

            if (itemsCheck.Count >= 8)
			{
                checkBox.IsChecked = false;
                isChecked = false;
            }
           
            tagItemDictionary[Tag].IsChecked = Convert.ToBoolean(isChecked);
        }
        void MenuItem_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                ContextMenu cm = VisualTreeExtensions.GetVisualAncestor<ContextMenu>(fe);
                if (cm != null)
                {
                    cm.IsOpen = false;
                }
            }
        }

        
         void MenuItemTag_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //FrameworkElement fe = sender as FrameworkElement;
            //if (fe != null)
            //{
            //    //ContextMenu cm = VisualTreeExtensions.GetVisualAncestor<ContextMenu>(fe);
            //    //if (cm != null)
            //    //{
            //    //    cm.IsOpen = false;
            //    //}
            //}
        }
    }
}

  //<Grid>
  //          <TabControl>
  //              <TabItem>
  //                  <TabItem.Header>
  //                      <StackPanel Orientation="Horizontal">
  //                          <Image Source="/WpfTutorialSamples;component/Images/bullet_blue.png" />
  //                          <TextBlock Text="Blue" Foreground="Blue" />
  //                      </StackPanel>
  //                  </TabItem.Header>
  //                  <Label Content="Content goes here..." />
  //              </TabItem>
  //              <TabItem>
  //                  <TabItem.Header>
  //                      <StackPanel Orientation="Horizontal">
  //                          <Image Source="/WpfTutorialSamples;component/Images/bullet_red.png" />
  //                          <TextBlock Text="Red" Foreground="Red" />
  //                      </StackPanel>
  //                  </TabItem.Header>
  //              </TabItem>
  //              <TabItem>
  //                  <TabItem.Header>
  //                      <StackPanel Orientation="Horizontal">
  //                          <Image Source="/WpfTutorialSamples;component/Images/bullet_green.png" />
  //                          <TextBlock Text="Green" Foreground="Green" />
  //                      </StackPanel>
  //                  </TabItem.Header>
  //              </TabItem>
  //          </TabControl>
  //      </Grid>