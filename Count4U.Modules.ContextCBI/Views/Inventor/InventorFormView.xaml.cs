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
using Count4U.Common.Helpers;

namespace Count4U.Modules.ContextCBI.Views.Inventor
{
    /// <summary>
    /// Interaction logic for InventorFormView.xaml
    /// </summary>
    public partial class InventorFormView : UserControl
    {
        public InventorFormView()
        {
			this.InitializeComponent();
            
            dtpInventorDate.FormatString = UtilsConvert.DateFormatLong();
        }
    }
}
