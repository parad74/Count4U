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
using Count4U.Common.UserSettings;
using Count4U.Modules.ContextCBI.ViewModels;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views
{
    /// <summary>
    /// Interaction logic for BackForwardView.xaml
    /// </summary>
    public partial class BackForwardView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        public BackForwardView(BackForwardViewModel viewModel, IUserSettingsManager userSettingsManager)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            btnBack.PreviewMouseDoubleClick += btnBack_PreviewMouseDoubleClick;
            btnForward.PreviewMouseDoubleClick += btnForward_PreviewMouseDoubleClick;

            enLanguage language = userSettingsManager.LanguageGet();
            switch (language)
            {
                case enLanguage.English:
                case enLanguage.Italian:
                case enLanguage.Russian:
                    break;
                case enLanguage.Hebrew:
                    btnBack.FlowDirection = FlowDirection.RightToLeft;
                    btnForward.FlowDirection = FlowDirection.RightToLeft;
                    break;
             
                    
                    
            }
        }        

        void btnForward_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void btnBack_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        #endregion

        #region Implementation of IRegionMemberLifetime

        public bool KeepAlive
        {
            get { return false; }
        }

        #endregion
    }
}
