using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Controls;
using Count4U.Common.Events;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;

namespace Count4U.Common.Helpers
{
    public class ModalWindowLauncher
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();	
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;

        private Window _modalWindow;
        private ModalWindowShell _shell;
        private string _viewName;

        private object _windowResultData;

        public ModalWindowLauncher(IRegionManager regionManager, IEventAggregator eventAggregator, IUserSettingsManager userSettingsManager)
        {
            this._userSettingsManager = userSettingsManager;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Subscribe(ModalWindowClose, true);
            this._eventAggregator.GetEvent<ModalWindowChangeTitleEvent>().Subscribe(ChangeTitle, true);
        }

        public object StartModalWindow(string viewName, string windowTitle, int width = 800, int height = 600,
                                     ResizeMode resizeMode = ResizeMode.CanResize,
                                     Dictionary<string, string> settings = null,
                                     Window owner = null, int minWidth = 300, int minHeight = 200,
                                     bool hiddenWindow = false)
        {
            this._viewName = viewName;
            _logger.Info("StartModalWindow : " + this._viewName);
            this._modalWindow = new Window();
            this._modalWindow.Width = width;
            this._modalWindow.Height = height;
            this._modalWindow.MinWidth = minWidth;
            this._modalWindow.MinHeight = minHeight;
            this._modalWindow.Title = windowTitle;
            this._modalWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this._modalWindow.ShowInTaskbar = false;
            this._modalWindow.ResizeMode = resizeMode;
            this._modalWindow.Owner = owner ?? Application.Current.MainWindow;
            this._modalWindow.Closed += ModalWindow_Closed;
            if (hiddenWindow)
            {
                this._modalWindow.Visibility = Visibility.Hidden;
                this._modalWindow.Width = 0;
                this._modalWindow.Height = 0;
                this._modalWindow.MinWidth = 0;
                this._modalWindow.MinHeight = 0;
                this._modalWindow.WindowStyle = WindowStyle.None;
                this._modalWindow.ShowInTaskbar = false;
                this._modalWindow.ResizeMode = ResizeMode.NoResize;
            }


            enLanguage language = this._userSettingsManager.LanguageGet();
            if (language == enLanguage.Hebrew)
                this._modalWindow.FlowDirection = FlowDirection.RightToLeft;
            try
            {
                this._shell = new ModalWindowShell();
                string buildRegionName = BuildRegionName(this._viewName);
                //RegionManager.SetRegionManager(this._shell.innerContent, this._regionManager);
                try
                {
                    RegionManager.SetRegionManager(this._shell, this._regionManager);
                    RegionManager.SetRegionName(this._shell.innerContent, buildRegionName);
                }
                catch (Exception exp)
                {
                    _logger.ErrorException("StartModalWindow : SetRegionName : " + this._viewName, exp);
                    return this._windowResultData;
                }

                this._modalWindow.Content = this._shell;

				if (this._regionManager.Regions.ContainsRegionWithName(buildRegionName) == true)
				{
					IRegion region = this._regionManager.Regions[buildRegionName];
                //if (region != null)
                //{
                    //}
                    //IRegion region = this._regionManager.Regions[BuildRegionName(this._viewName)];
                    Uri uri = null;
                    if (settings == null)
                        uri = new Uri(viewName, UriKind.Relative);
                    else
                    {
                        UriQuery uriQuery = new UriQuery();
                        foreach (var kvp in settings)
                            uriQuery.Add(kvp.Key, kvp.Value);

                        uri = new Uri(viewName + uriQuery, UriKind.Relative);
                    }
                    region.RequestNavigate(uri);
                }
                else
                {
                    _logger.Info("StartModalWindow : " + "Not Contains Region With Name" + this._viewName + "in this._regionManager.Regions");
                }

                if (this._modalWindow != null)
                    this._modalWindow.ShowDialog();

                return this._windowResultData;
            }
            catch (Exception exp)
            {
                _logger.ErrorException("StartModalWindow : " + this._viewName, exp);
                return this._windowResultData;
            }
        }

        private void ModalWindowClose(object viewModel)
        {
            if (this._modalWindow != null)
            {
				_logger.Info("ModalWindowClose : " + this._viewName);
                if (GetModalWindowViewModel() == viewModel)
                    this._modalWindow.Close();
            }
        }

        void ModalWindow_Closed(object sender, EventArgs e)
        {
			try
			{
				object view = GetModalWindowView();
				if (view != null)
				{
					INavigationAware navigationAwareView = view as INavigationAware;
					if (navigationAwareView != null)
					{
						navigationAwareView.OnNavigatedFrom(null);
					}

					FrameworkElement fe = view as FrameworkElement;
					if (fe != null)
					{
						INavigationAware navigationAwareViewModel = fe.DataContext as INavigationAware;
						if (navigationAwareViewModel != null)
						{
							navigationAwareViewModel.OnNavigatedFrom(null);
						}
					}
				}

				object viewModel = GetModalWindowViewModel();
				if (viewModel != null)
				{
					IChildWindowViewModel childWindowViewModel = viewModel as IChildWindowViewModel;
					if (childWindowViewModel != null)
						this._windowResultData = childWindowViewModel.ResultData;
				}
			}
			catch (Exception exp)
			{
				_logger.ErrorException("ModalWindow_Closed : " + this._viewName, exp);
			}

			this._regionManager.Regions.Remove(BuildRegionName(this._viewName));
			if (this._modalWindow != null)
			{
				this._modalWindow.Closed -= ModalWindow_Closed;
			}
            this._modalWindow = null;
            //this._regionManager.Regions.Remove(BuildRegionName(this._viewName));
        }

        private string BuildRegionName(string viewName)
        {
            return String.Format("{0}_{1}", Common.RegionNames.ModalWindowRegion, viewName);
        }

        private object GetModalWindowViewModel()
        {
            if (this._shell != null && this._shell.innerContent != null && this._shell.innerContent.Content != null)
            {
                UserControl uc = this._shell.innerContent.Content as UserControl;
                if (uc != null && uc.DataContext != null)
                    return uc.DataContext;

                IInnerContent innerContent = uc as IInnerContent;
                if (innerContent != null && innerContent.InnerContent != null && innerContent.InnerContent.Content != null)
                {
                    uc = innerContent.InnerContent.Content as UserControl;
                    if (uc != null && uc.DataContext != null)
                        return uc.DataContext;
                }

            }

            return null;
        }

        private object GetModalWindowView()
        {
            if (this._shell != null && this._shell.innerContent != null && this._shell.innerContent.Content != null)
            {
                UserControl uc = this._shell.innerContent.Content as UserControl;
                if (uc != null)
                    return uc;
            }

            return null;
        }

        private void ChangeTitle(ModalWindowChangeTitleEventPayload payload)
        {
            if (GetModalWindowViewModel() == payload.ViewModel)
            {
                if (this._modalWindow != null)
                    _modalWindow.Title = payload.Title;
            }
        }
    }
}