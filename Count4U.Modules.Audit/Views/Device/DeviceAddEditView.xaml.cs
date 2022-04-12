using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Behaviours;
using Count4U.Modules.Audit.ViewModels;

namespace Count4U.Modules.Audit.Views.Device
{

    public partial class DeviceAddEditView : UserControl
    {
		public DeviceAddEditView(DeviceAddEditViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

			this.Loaded += DeviceAddEditView_Loaded;
			this.PreviewKeyUp += DeviceAddEditView_KeyUp;

            TextChangedDelayedBehavior behavior = new TextChangedDelayedBehavior();
            behavior.IsTimerEnabled = true;
			behavior.Attach(txtWorkerID);            
        }

		void DeviceAddEditView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
			txtWorkerID.Focus();
        }

		void DeviceAddEditView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
				DeviceAddEditViewModel viewModel = this.DataContext as DeviceAddEditViewModel;
                if (viewModel != null)
                {
                    viewModel.CancelCommand.Execute();
                    e.Handled = true;
                }
            }
        }
    }
}
