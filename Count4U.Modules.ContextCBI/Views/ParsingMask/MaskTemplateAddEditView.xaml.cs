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
using Count4U.Modules.ContextCBI.ViewModels.ParsingMask;

namespace Count4U.Modules.ContextCBI.Views.ParsingMask
{
    /// <summary>
    /// Interaction logic for MaskTemplateAddEditView.xaml
    /// </summary>
    public partial class MaskTemplateAddEditView : UserControl
    {
        public MaskTemplateAddEditView(MaskTemplateAddEditViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += MaskAddEditView_Loaded;
        }

        void MaskAddEditView_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.Focus();
        }
    }
}
