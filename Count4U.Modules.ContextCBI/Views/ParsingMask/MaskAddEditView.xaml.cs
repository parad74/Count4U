using System.Windows;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels.ParsingMask;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.ParsingMask
{
    /// <summary>
    /// Interaction logic for MaskTemplateAddEditView.xaml
    /// </summary>
    public partial class MaskAddEditView : UserControl, INavigationAware
    {
        private readonly MaskAddEditViewModel _viewModel;
        private readonly ModalWindowLauncher _modalWindowLauncher;

        public MaskAddEditView(MaskAddEditViewModel viewModel, ModalWindowLauncher modalWindowLauncher)
        {            
            InitializeComponent();

            this._viewModel = viewModel;
            this._modalWindowLauncher = modalWindowLauncher;
            this.DataContext = this._viewModel;

            this.Loaded += MaskAddEditView_Loaded;
        }

        void MaskAddEditView_Loaded(object sender, RoutedEventArgs e)
        {
//            txtName.Focus();
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this._viewModel.ModalWindowRequest += ModalWindowRequest;
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

        void ModalWindowRequest(object sender, Count4U.Common.Interfaces.ModalWindowRequestPayload e)
        {
            object result = this._modalWindowLauncher.StartModalWindow(Count4U.Common.ViewNames.MaskSelectView, "Mask select", 480, 540, ResizeMode.NoResize, e.Settings, Window.GetWindow(this));
            if (e.Callback != null)
                e.Callback(result);
        }
    }
}
