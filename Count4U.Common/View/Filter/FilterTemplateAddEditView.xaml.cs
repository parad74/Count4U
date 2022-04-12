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
using Count4U.Common.ViewModel.Filter.FilterTemplate;

namespace Count4U.Common.View.Filter
{
    /// <summary>
    /// Interaction logic for FilterTemplateAddEditView.xaml
    /// </summary>
    public partial class FilterTemplateAddEditView : UserControl
    {
        public FilterTemplateAddEditView(FilterTemplateAddEditViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += FilterTemplateAddView_Loaded;            
        }

        void FilterTemplateAddView_Loaded(object sender, RoutedEventArgs e)
        {
            txtDisplayName.Focus();
        }
    }
}
