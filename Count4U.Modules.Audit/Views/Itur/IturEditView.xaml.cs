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

namespace Count4U.Modules.Audit.Views.Itur
{
    /// <summary>
    /// Interaction logic for IturEditView.xaml
    /// </summary>
    public partial class IturEditView : UserControl
    {
        public IturEditView(IturEditViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += new RoutedEventHandler(IturEditView_Loaded);
        }

        void IturEditView_Loaded(object sender, RoutedEventArgs e)
        {
            txtCode.Focus();
        }
    }
}
