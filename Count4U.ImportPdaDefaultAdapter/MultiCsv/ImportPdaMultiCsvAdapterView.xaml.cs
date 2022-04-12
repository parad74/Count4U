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
using Microsoft.Practices.Prism.Regions;

namespace Count4U.ImportPdaMultiCsvAdapter
{
   public partial class ImportPdaMultiCsvAdapterView : UserControl
    {
	//   private readonly ModalWindowLauncher _modalWindowLauncher;
	   //private readonly ImportPdaMultiCsvAdapterViewModel _viewModel;
	   public ImportPdaMultiCsvAdapterView(ImportPdaMultiCsvAdapterViewModel viewModel)
		 //  ModalWindowLauncher modalWindowLauncher)
        {
            InitializeComponent();

			//this._viewModel = viewModel;
			//this._modalWindowLauncher = modalWindowLauncher;
			this.DataContext = viewModel;
			RegionManager.SetRegionName(maskControl, viewModel.BuildMaskRegionName());
        }

	   //public void OnNavigatedTo(NavigationContext navigationContext)
	   //{
	   //	this._viewModel.ModalWindowRequest += ModalWindowRequest;
	   //}

	   //public bool IsNavigationTarget(NavigationContext navigationContext)
	   //{
	   //	return false;
	   //}

	   //public void OnNavigatedFrom(NavigationContext navigationContext)
	   //{
	   //	this._viewModel.ModalWindowRequest -= ModalWindowRequest;
	   //}

	   //void ModalWindowRequest(object sender, Common.Interfaces.ModalWindowRequestPayload e)
	   //{
	   //	object result = this._modalWindowLauncher.StartModalWindow(Common.ViewNames.MaskSelectView, "Mask select", 480, 540, ResizeMode.NoResize, e.Settings, Window.GetWindow(this));
	   //	if (e.Callback != null)
	   //		e.Callback(result);
	   //}

    }
}
