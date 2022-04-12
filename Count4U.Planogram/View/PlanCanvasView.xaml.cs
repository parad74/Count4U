using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Behaviours;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Planogram.Lib;
using Count4U.Planogram.ViewModel;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Planogram.View
{
    /// <summary>
    /// Interaction logic for PlanCanvasView.xaml
    /// </summary>
    public partial class PlanCanvasView : UserControl, INavigationAware
    {
        private readonly PlanCanvasViewModel _viewModel;
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        private DrawingCanvas _canvas;
        private Scrolling _scrolling;
        private readonly IUserSettingsManager _userSettingsManager;

        public PlanCanvasView(
            PlanCanvasViewModel viewModel,
            IRegionManager regionManager,
            IUserSettingsManager userSettingsManager,
            IUnityContainer container)
        {
            _container = container;
            _userSettingsManager = userSettingsManager;
            InitializeComponent();

            _regionManager = regionManager;

            this._viewModel = viewModel;
            this.Background = Brushes.Transparent;
            this.FlowDirection = FlowDirection.LeftToRight;

            this.DataContext = _viewModel;

            Init();

            this.PreviewKeyDown += PlanCanvas_KeyDown;
            this.PreviewKeyUp += PlanCanvas_KeyUp;

            this.Loaded += PlanCanvas_Loaded;

            _viewModel.View = this;

            contextMenu.Opened += ContextMenu_Opened;          

            RegionManager.SetRegionName(contentTree, Common.RegionNames.PlanogramTree);
            RegionManager.SetRegionName(contentProperties, Common.RegionNames.PlanogramProperties);

            if (_userSettingsManager.LanguageGet() == enLanguage.Hebrew)
            {
                contextMenu.FlowDirection = FlowDirection.RightToLeft;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {            
            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            _regionManager.RequestNavigate(Common.RegionNames.PlanogramTree, new Uri(Common.ViewNames.PlanTreeView + query, UriKind.Relative));

            _regionManager.RequestNavigate(Common.RegionNames.PlanogramProperties, new Uri(Common.ViewNames.PlanPropertiesView + query, UriKind.Relative));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {                                                      
                                                      contentTree,
                                                      contentProperties,

                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.PlanogramTree);
            this._regionManager.Regions.Remove(Common.RegionNames.PlanogramProperties);       
        }

        void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            
        }

        void PlanCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void PlanCanvas_KeyUp(object sender, KeyEventArgs e)
        {           
            if (_canvas != null)
            {
                _canvas.DrawingCanvas_KeyUp(this, e);
            }
        }

        void PlanCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is TextBox)
            {
                return;
            }

            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (_viewModel.CopyCommand.CanExecute())
                {
                    _viewModel.CopyCommand.Execute();
                    e.Handled = true;
                    return;
                }
            }

            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (_viewModel.PasteCommand.CanExecute())
                {
                    _viewModel.PasteCommand.Execute();
                    e.Handled = true;
                    return;
                }
            }

            if (e.Key == Key.Insert && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                if (_viewModel.PasteCommand.CanExecute())
                {
                    _viewModel.PasteCommand.Execute();
                    e.Handled = true;
                    return;
                }
            }


            if (_canvas != null)
            {
                _canvas.DrawingCanvas_KeyDown(this, e);
            }
        }

        public DrawingCanvas Canvas
        {
            get { return _canvas; }
        }

        private void Init()
        {
            grid.MinWidth = Helpers.DefaultCanvasWidth + 200;
            grid.MinHeight = Helpers.DefaultCanvasHeight + 200;

            this._scrolling = new Scrolling(scrollViewer, grid, scaleTransform);

            this._canvas = new DrawingCanvas(_scrolling, grid, _container);
            Canvas.Width = Helpers.DefaultCanvasWidth;
            Canvas.Height = Helpers.DefaultCanvasHeight;
            Canvas.HorizontalAlignment = HorizontalAlignment.Center;
            Canvas.VerticalAlignment = VerticalAlignment.Center;

            grid.Children.Add(Canvas);

            PlanCanvasViewModel viewModel = this.DataContext as PlanCanvasViewModel;
            if (viewModel != null)
            {
                viewModel.Init(_canvas);
            }
        }

     
    }
}
