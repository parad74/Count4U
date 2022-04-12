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
using Count4U.Modules.Audit.ViewModels;

namespace Count4U.Modules.Audit.Views
{
    /// <summary>
    /// Interaction logic for IturimAddInsertView.xaml
    /// </summary>
    public partial class IturAddView : UserControl
    {
        public IturAddView(IturAddViewModel viewModel)
        {
			this.InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += IturAddView_Loaded;
        }

        void IturAddView_Loaded(object sender, RoutedEventArgs e)
        {
            txtNumberPrefix.Focus();
        }
    }
}
