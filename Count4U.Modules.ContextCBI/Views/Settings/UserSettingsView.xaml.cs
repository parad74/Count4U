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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Modules.ContextCBI.ViewModels.Settings;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using NLog;
using Xceed.Wpf.Toolkit;

namespace Count4U.Modules.ContextCBI.Views.Settings
{
    /// <summary>
    /// Interaction logic for UserSettingsView.xaml
    /// </summary>
    public partial class UserSettingsView : UserControl, INavigationAware
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ModalWindowLauncher _modalWindowLauncher;
        private readonly UserSettingsViewModel _viewModel;


        public UserSettingsView(
            UserSettingsViewModel viewModel, 
            ModalWindowLauncher modalWindowLauncher)
        {            
            InitializeComponent();

            this.DataContext = viewModel;
            this._modalWindowLauncher = modalWindowLauncher;
            this._viewModel = viewModel;            

            this.Loaded += UserSettingsView_Loaded;
        }

        void viewModel_OnResetToDefault(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(FixColorPickers), DispatcherPriority.Background);
        }

        void UserSettingsView_Loaded(object sender, RoutedEventArgs e)
        {
            FixColorPickers();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this._viewModel.OnResetToDefault += viewModel_OnResetToDefault;
            this._viewModel.ModalWindowRequest += ViewModel_ModalWindowRequest;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this._viewModel.OnResetToDefault -= viewModel_OnResetToDefault;
            this._viewModel.ModalWindowRequest -= ViewModel_ModalWindowRequest;
			this.Loaded -= UserSettingsView_Loaded;
		}

        private void FixColorPickers()
        {
            FixColorPickerStretchInListView(listStatuses);
            FixColorPickerStretchInListView(listGroupStatuses);
        }

        private void FixColorPickerStretchInListView(ListView listView)
        {
            try
            {
                foreach (var item in listView.Items)
                {
                    var container = listView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;

                    if (container != null)
                    {
                        var picker = VisualTreeHelpers.FindInVisualTreeByType<ColorPicker>(container);
                        if (picker != null)
                        {
                            var toggleButton = VisualTreeHelpers.FindInVisualTreeFunc(container, r =>
                                                                                                  {
                                                                                                      ToggleButton fe = r as ToggleButton;
                                                                                                      if (fe == null)
                                                                                                          return false;

                                                                                                      return fe.Name == "PART_ColorPickerToggleButton";
                                                                                                  });
                            if (toggleButton != null)
                            {
                                var contentPresenter = VisualTreeHelpers.FindInVisualTreeByType<ContentPresenter>(toggleButton);
                                if (contentPresenter != null)
                                    contentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("FixColorPickerStretchInListView", exc);
            }
        }

        void ViewModel_ModalWindowRequest(object sender, ModalWindowRequestPayload e)
        {
            object result = null;

            if (e.ViewName == Common.ViewNames.ConfigurationSetAddView)
            {
                result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.ConfigurationSetAddView, e.WindowTitle, 230, 130,
                                                                    ResizeMode.NoResize, e.Settings, Window.GetWindow(this), minWidth: 200, minHeight: 120);
            }           

            if (e.Callback != null)
                e.Callback(result);
        }
    }
}
