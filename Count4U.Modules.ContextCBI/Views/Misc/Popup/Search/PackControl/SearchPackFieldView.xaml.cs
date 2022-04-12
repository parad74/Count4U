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
using System.Windows.Threading;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.PackControl;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.PackControl
{
    /// <summary>
    /// Interaction logic for SearchPackFieldView.xaml
    /// </summary>
    public partial class SearchPackFieldView : UserControl
    {
        public SearchPackFieldView(SearchPackFieldViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += SearchPackFieldView_Loaded;

        }

        void SearchPackFieldView_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => txtCode.Focus()), DispatcherPriority.Background);
        }
    }
}
