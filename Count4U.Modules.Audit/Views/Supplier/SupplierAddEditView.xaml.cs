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
using Count4U.Modules.Audit.ViewModels.Supplier;

namespace Count4U.Modules.Audit.Views.Supplier
{
    /// <summary>
    /// Interaction logic for SupplierAddEditView.xaml
    /// </summary>
    public partial class SupplierAddEditView : UserControl
    {
        private readonly SupplierAddEditViewModel _viewModel;

        public SupplierAddEditView(SupplierAddEditViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += SupplierAddEditView_Loaded;
        }

        void SupplierAddEditView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsNew)
                txtCode.Focus();
            else
                txtName.Focus();
        }
    }
}
