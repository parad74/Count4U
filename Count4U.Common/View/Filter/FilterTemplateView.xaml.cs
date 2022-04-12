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
    /// Interaction logic for FilterSetView.xaml
    /// </summary>
    public partial class FilterTemplateView : UserControl
    {
        public FilterTemplateView(FilterTemplateViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            viewModel.View = this;

            //btnMenu.Click += btnMenu_Click;
            btnMenu.PreviewMouseLeftButtonUp += btnMenu_PreviewMouseLeftButtonUp;
        }

        void btnMenu_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnMenu.ContextMenu.PlacementTarget = this;
            btnMenu.ContextMenu.IsOpen = true;
        }

        void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            btnMenu.ContextMenu.IsOpen = true;
        }
    }
}
