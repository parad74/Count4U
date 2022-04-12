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
using Count4U.Modules.ContextCBI.ViewModels.Misc.Script;

namespace Count4U.Modules.ContextCBI.Views.Misc.Script
{
    /// <summary>
    /// Interaction logic for CBIScriptSaveView.xaml
    /// </summary>
    public partial class CBIScriptSaveView : UserControl
    {
        public CBIScriptSaveView(CBIScriptSaveViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
