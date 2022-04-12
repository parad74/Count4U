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
using Count4U.Modules.Audit.ViewModels.Family;

namespace Count4U.Modules.Audit.Views.Family
{
  
    public partial class FamilyAddEditView : UserControl
    {
        private readonly FamilyAddEditViewModel _viewModel;

		public FamilyAddEditView(FamilyAddEditViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();

            this.DataContext = viewModel;

			this.Loaded += FamilyAddEditView_Loaded;
        }

		void FamilyAddEditView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsNew)
                txtCode.Focus();
            else
                txtName.Focus();
        }
    }
}
