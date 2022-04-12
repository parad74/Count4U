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
using Count4U.Common.UserSettings;
using Count4U.Planogram.ViewModel;

namespace Count4U.Planogram.View
{
    /// <summary>
    /// Interaction logic for PlanInfoView.xaml
    /// </summary>
    public partial class PlanInfoView : UserControl
    {
        private readonly IUserSettingsManager _userSettingsManager;

        public PlanInfoView(
            PlanInfoViewModel viewModel,
            IUserSettingsManager userSettingsManager)
        {
            _userSettingsManager = userSettingsManager;
            InitializeComponent();

            this.DataContext = viewModel;


//            if (_userSettingsManager.LanguageGet() == enLanguage.Hebrew)
//            {
//                Grid.SetColumn(txtProcess, 1);
//                Grid.SetColumn(rectangleProcess, 0);
//
//                rectangleProcess.Margin = new Thickness(0, 0, 5, 0);
//
//                txtTotalItems.HorizontalAlignment = HorizontalAlignment.Right;
//                txtTotalIturs.HorizontalAlignment = HorizontalAlignment.Right;
//                gridBottomInner.HorizontalAlignment = HorizontalAlignment.Right;
//            }

            if (_userSettingsManager.LanguageGet() == enLanguage.Hebrew)
            {
                this.FlowDirection = FlowDirection.RightToLeft;
            }
        }
    }
}
