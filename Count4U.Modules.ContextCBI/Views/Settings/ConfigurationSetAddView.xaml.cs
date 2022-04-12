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
using Count4U.Modules.ContextCBI.ViewModels.Settings;

namespace Count4U.Modules.ContextCBI.Views.Settings
{
    /// <summary>
    /// Interaction logic for ConfigurationSetAddView.xaml
    /// </summary>
    public partial class ConfigurationSetAddView : UserControl
    {
        public ConfigurationSetAddView(ConfigurationSetAddViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += ConfigurationSetAddView_Loaded;
        }

        void ConfigurationSetAddView_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.Focus();
        }
    }
}
