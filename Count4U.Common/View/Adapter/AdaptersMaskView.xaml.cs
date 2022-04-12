using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Adapters;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Common.View.Adapter
{
    /// <summary>
    /// Interaction logic for AdaptersMaskView.xaml
    /// </summary>
    public partial class AdaptersMaskView : UserControl, INavigationAware
    {
        private readonly AdapterMaskViewModel _viewModel;
        private readonly ModalWindowLauncher _modalWindowLauncher;

        public AdaptersMaskView(AdapterMaskViewModel viewModel,
                                ModalWindowLauncher modalWindowLauncher)
        {            
            InitializeComponent();

            this._viewModel = viewModel;
            this._modalWindowLauncher = modalWindowLauncher;

            this.DataContext = this._viewModel;
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this._viewModel.ModalWindowRequest += ModalWindowRequest;
			this._viewModel.IsMakatMaskVisible = true;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this._viewModel.ModalWindowRequest -= ModalWindowRequest;
        }

        #endregion

        void ModalWindowRequest(object sender, Common.Interfaces.ModalWindowRequestPayload e)
        {
            object result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.MaskSelectView, "Mask select", 480, 540, ResizeMode.NoResize, e.Settings, Window.GetWindow(this));
            if (e.Callback != null)
                e.Callback(result);
        }
    }
}
