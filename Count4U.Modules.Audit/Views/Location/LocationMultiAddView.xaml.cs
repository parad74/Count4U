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
using Count4U.Modules.Audit.ViewModels;

namespace Count4U.Modules.Audit.Views
{
    /// <summary>
    /// Interaction logic for LocationAddView.xaml
    /// </summary>
    public partial class LocationMultiAddView : UserControl
    {
		public LocationMultiAddView(LocationMultiAddViewModel viewModel)
        {
            this.InitializeComponent();

            this.DataContext = viewModel;

            this.Loaded += LocationNewView_Loaded;

            //System.Windows.Interactivity.Interaction.GetBehaviors(colorPicker).Add(new ColorPickerSortBehavior());
        }

        void LocationNewView_Loaded(object sender, RoutedEventArgs e)
        {
			LocationMultiAddViewModel viewModel = this.DataContext as LocationMultiAddViewModel;
            if (viewModel == null)
                return;

            this.txtCountMultAdd.Focus();
			//else
			//	txtName.Focus();
        }
    }    
}
