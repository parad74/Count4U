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
using Count4U.Common.Controls;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup
{
    /// <summary>
    /// Interaction logic for MdiFilterView.xaml
    /// </summary>
    public partial class MdiFilterView : UserControl, IPopupChildControl
    {
        public MdiFilterView(MdiFilterViewModel viewModel)
        {											 
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.View = this;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            UtilsPopup.Close(this);
        }

        public void Apply()
        {
            MdiFilterViewModel viewModel = this.DataContext as MdiFilterViewModel;
            if (viewModel != null)
                viewModel.ApplyCommand.Execute();
        }

        public void Reset()
        {
            MdiFilterViewModel viewModel = this.DataContext as MdiFilterViewModel;
            if (viewModel != null)
                viewModel.ResetCommand.Execute();
        }

		public void Pick(bool yes)
		{

		}
    }
}
