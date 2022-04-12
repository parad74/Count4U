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
using Count4U.Modules.Audit.ViewModels.Section;

namespace Count4U.Modules.Audit.Views.Section
{
    /// <summary>
    /// Interaction logic for SectionAddEditView.xaml
    /// </summary>
    public partial class SectionAddEditView : UserControl
    {
        private readonly SectionAddEditViewModel _viewModel;

        public SectionAddEditView(SectionAddEditViewModel viewModel)
        {
            
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            this.Loaded += SectionAddEditView_Loaded;
        }

        void SectionAddEditView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsNew)
                txtCode.Focus();
            else
                txtName.Focus();
        }
    }
}
