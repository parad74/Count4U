using System;
using System.Collections.Generic;
using System.IO;
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

namespace Count4U.Modules.ContextCBI.Views.Customer
{
    /// <summary>
    /// Interaction logic for CustomerFormView.xaml
    /// </summary>
    public partial class CustomerFormView : UserControl
    {
        public CustomerFormView()
        {
			this.InitializeComponent();

           // txtPhone.Mask = txtFax.Mask = "&CC-CCCCCCC";

            txtName.Focus();
        }

     
    }
}
