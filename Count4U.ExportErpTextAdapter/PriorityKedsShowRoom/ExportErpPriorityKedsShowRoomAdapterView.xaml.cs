using System.Windows.Controls;

namespace Count4U.ExportErpTextAdapter.PriorityKedsShowRoom
{
  
    public partial class ExportErpPriorityKedsShowRoomAdapterView : UserControl
    {
		public ExportErpPriorityKedsShowRoomAdapterView(ExportErpPriorityKedsShowRoomAdapterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
