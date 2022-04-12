using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
using Count4U.Common.Behaviours;
using Count4U.Common.Interfaces;
using Count4U.Modules.ContextCBI.ViewModels;

namespace Count4U.Modules.ContextCBI.View
{
    /// <summary>
    /// Interaction logic for LocationAddView.xaml
    /// </summary>
    public partial class ProcessAddEditView : UserControl
    {
		public ProcessAddEditView(ProcessAddEditViewModel viewModel)
        {
            this.InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += LocationNewView_Loaded;

           // System.Windows.Interactivity.Interaction.GetBehaviors(colorPicker).Add(new ColorPickerSortBehavior());
        }

        void LocationNewView_Loaded(object sender, RoutedEventArgs e)
        {
			ProcessAddEditViewModel viewModel = this.DataContext as ProcessAddEditViewModel;
            if (viewModel == null)
                return;

            if (viewModel.IsNewMode)
                txtCode.Focus();
            else
                txtName.Focus();
        }
    }    
}
