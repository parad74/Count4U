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
using Count4U.Modules.ContextCBI.ViewModels.ParsingMask.Script;

namespace Count4U.Modules.ContextCBI.Views.ParsingMask.Script
{
    /// <summary>
    /// Interaction logic for MaskScriptOpenView.xaml
    /// </summary>
    public partial class MaskScriptOpenView : UserControl
    {
        public MaskScriptOpenView(MaskScriptOpenViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
