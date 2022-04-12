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
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.SectionControl;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.SectionControl
{
    /// <summary>
    /// Interaction logic for SearchSectionView.xaml
    /// </summary>
    public partial class SearchSectionView : UserControl
    {
        public SearchSectionView(SearchSectionViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            viewModel.View = this;
        }
    }
}
