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
using Count4U.Modules.Audit.ViewModels.Catalog;

namespace Count4U.Modules.Audit.Views.Catalog
{
    /// <summary>
    /// Interaction logic for ProductAddEditView.xaml
    /// </summary>
    public partial class ProductAddEditView : UserControl
    {
        public ProductAddEditView(ProductAddEditViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += ProductAddEditView_Loaded;
        }

        void ProductAddEditView_Loaded(object sender, RoutedEventArgs e)
        {            
            txtMakat.Focus();

            ProductAddEditViewModel viewModel = this.DataContext as ProductAddEditViewModel;
            viewModel.IsTimerEnabled = true;
        }
    }
}
