using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Count4U.Common.Events;
using Count4U.Common.UserSettings;
using Count4U.Common.View;
using Count4U.Model;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Common.Helpers
{
    public enum PopupMode
    {
        Search,
        Filter,
        SpeedLink
    }

    public class UtilsPopup
    {
		public static bool PikUnPik = false;
        public static Window BuildPopup(Button button, IUnityContainer container, String viewName, String regionName, UriQuery uriQuery, PopupMode popupMode, double width = 400, double height = 0)
        {
            IUserSettingsManager userSettingsManager = container.Resolve<IUserSettingsManager>();
			PikUnPik = userSettingsManager.SearchDialogIsModalGet(); 
            IRegionManager regionManager = container.Resolve<IRegionManager>();
			//IEventAggregator _eventAggregator = container.Resolve<IEventAggregator>();

            PopupPosition position = CalculatePosition(button, userSettingsManager, width, height);

            PopupView popupView = new PopupView();
            popupView.SetOffset(position.OffsetForArrow);

            switch (popupMode)
            {
                case PopupMode.Search:
                    popupView.tbTitle.Text = Localization.Resources.Msg_SearchWindow;
                    popupView.pathApply.Visibility = Visibility.Collapsed;
                    popupView.pathReset.Visibility = Visibility.Collapsed;
					
                    break;
                case PopupMode.Filter:
                    popupView.tbTitle.Text = Localization.Resources.Msg_FilterWindow;
                    popupView.pathApply.Visibility = Visibility.Visible;
                    popupView.pathReset.Visibility = Visibility.Visible;
					
                    break;
                case PopupMode.SpeedLink:
					popupView.tbTitle.Text = Localization.Resources.View_IturListDetails_btnSpeedLink;
                    popupView.pathApply.Visibility = Visibility.Collapsed;
                    popupView.pathReset.Visibility = Visibility.Collapsed;
					
                    break;
                default:
                    throw new ArgumentOutOfRangeException("popupMode");
            }           

            Window window = new Window();
            window.Height = position.Height;
            window.Width = width;
            window.Left = position.Horizontal;
            window.Top = position.Vertical;
            window.Background = Brushes.Transparent;
            window.FlowDirection = userSettingsManager.LanguageGet() == enLanguage.Hebrew ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowInTaskbar = false;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.WindowStyle = WindowStyle.None;
            window.AllowsTransparency = true;
            window.Owner = Application.Current.MainWindow;
            window.Content = popupView;
			

            popupView.Window = window;

            RegionManager.SetRegionManager(window, regionManager);
            RegionManager.UpdateRegions();
            RegionManager.SetRegionName(popupView.content, regionName);

            regionManager.RequestNavigate(regionName, new Uri(viewName + uriQuery, UriKind.Relative));

            return window;
        }

        public static void NavigateFrom(Window popup, IRegionManager regionManager, String regionName)
        {
            if (popup == null)
                return;

            PopupView popupFilterView = popup.Content as PopupView;
            if (popupFilterView != null)
            {
                INavigationAware view = popupFilterView.content.Content as INavigationAware;
                if (view != null)
                {
                    NavigationContext navigationContext = null;
                    view.OnNavigatedFrom(navigationContext);

                    FrameworkElement fe = view as FrameworkElement;
                    if (fe != null)
                    {
                        INavigationAware viewModel = fe.DataContext as INavigationAware;
                        if (viewModel != null)
                        {
                            viewModel.OnNavigatedFrom(navigationContext);
                        }
                    }
                }
            }

            regionManager.Regions.Remove(regionName);
        }

        public static void Close(FrameworkElement fe, IEventAggregator _eventAggregator = null)
        {
            if (fe == null)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                          return;
#endif
            }

           

            PopupView popupView = VisualTreeHelpers.FindParent<PopupView>(fe);
            if (popupView != null)
            {
                //if (popupView.tbTitle.Text == Localization.Resources.Msg_SearchWindow)
                //{
                //    if (_eventAggregator != null)
                //    {
                //        using (new CursorWait())
                //        {
                //            _eventAggregator.GetEvent<IturQuantityEditChangedEvent>().Publish(
                //                new IturQuantityEditChangedEventPayload()
                //                );
                //        }
                //    }
                //}
                popupView.Close();
            }
        }

        private class PopupPosition
        {
            public double Horizontal { get; set; }
            public double Vertical { get; set; }
            public double OffsetForArrow { get; set; }
            public double Height { get; set; }
        }

        private static PopupPosition CalculatePosition(Button button, IUserSettingsManager settingsManager, double width, double height)
        {
            double horizontal = 0;
            double vertical = 0;
            double heightCalculated = 0;
            double offsetForArrow = 0;
            const double offsetFromBorder = 15;

            double top = Application.Current.MainWindow.WindowState == WindowState.Maximized ? 0 : Application.Current.MainWindow.Top;
            double left = Application.Current.MainWindow.WindowState == WindowState.Maximized ? 0 : Application.Current.MainWindow.Left;

            Point p = button.TransformToVisual(Application.Current.MainWindow).Transform(new Point(0, 0));

            double relative = p.X - width;

            if (relative < 0)
            {
                horizontal = offsetFromBorder;
            }
            else
            {
                horizontal = (relative < offsetFromBorder ? offsetFromBorder : relative) + button.ActualWidth + 30;
            }


            if (settingsManager.LanguageGet() == enLanguage.Hebrew)
            {
                offsetForArrow = horizontal + width - p.X + button.ActualWidth / 2;
            }
            else
            {
                offsetForArrow = p.X - horizontal + button.ActualWidth / 2;
            }

            horizontal = horizontal + left;

            vertical = p.Y + button.ActualHeight + 5;
            heightCalculated = height == 0 ? Application.Current.MainWindow.ActualHeight - vertical - 115 : height;

            vertical = vertical + top;

            return new PopupPosition() { Horizontal = horizontal, Vertical = vertical, OffsetForArrow = offsetForArrow, Height = heightCalculated };
        }
    }
}