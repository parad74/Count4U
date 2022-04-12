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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Count4U.Modules.ContextCBI.Views.Customer
{
    /// <summary>
    /// Interaction logic for CustomerCommon.xaml
    /// </summary>
    public partial class CustomerCommon : UserControl
    {
        public CustomerCommon()
        {
            InitializeComponent();

            tbGeneral.MouseLeftButtonUp += tbGeneral_MouseLeftButtonUp;
            tbImportAdapters.MouseLeftButtonUp += tbImportAdapters_MouseLeftButtonUp;
            tbPda.MouseLeftButtonUp += tbPda_MouseLeftButtonUp;
            tbErp.MouseLeftButtonUp += tbErp_MouseLeftButtonUp;
            tbUpdate.MouseLeftButtonUp += tbUpdate_MouseLeftButtonUp;
            tbDynamicColumns.MouseLeftButtonUp += tbDynamicColumns_MouseLeftButtonUp;
            btnNext.Click += btnNext_Click;
        }

        void tbGeneral_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MakeVisible(ctrForm);
        }

        void tbImportAdapters_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MakeVisible(importFolders);
        }

        void tbErp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MakeVisible(exportErpSettings);
        }

        void tbPda_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MakeVisible(exportPdaSettings);
        }

        void tbUpdate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MakeVisible(updateFolders);
        }

        void tbDynamicColumns_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MakeVisible(dynamicColumns);
        }      

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (ctrForm.Visibility == Visibility.Visible)
            {
                MakeVisible(importFolders);
            }
            else if (importFolders.Visibility == Visibility.Visible)
            {
                MakeVisible(exportPdaSettings);
            }
            else if (exportPdaSettings.Visibility == Visibility.Visible)
            {
                MakeVisible(exportErpSettings);
            }
            else if (exportErpSettings.Visibility == Visibility.Visible)
            {
                MakeVisible(updateFolders);
            }
            else if (updateFolders.Visibility == Visibility.Visible)
            {
                MakeVisible(dynamicColumns);
            }
            else if (dynamicColumns.Visibility == Visibility.Visible)
            {
                MakeVisible(ctrForm);
            }
        }

        private void MakeVisible(FrameworkElement fe)
        {
            Thickness to = default(Thickness);

            if (fe == ctrForm)
            {
                ctrForm.Visibility = Visibility.Visible;
                importFolders.Visibility = Visibility.Collapsed;
                exportPdaSettings.Visibility = Visibility.Collapsed;
                exportErpSettings.Visibility = Visibility.Collapsed;
                updateFolders.Visibility = Visibility.Collapsed;
                dynamicColumns.Visibility = Visibility.Collapsed;

                to = new Thickness(-10, 45, 0, 0);
            }
            else if (fe == importFolders)
            {
                ctrForm.Visibility = Visibility.Collapsed;
                importFolders.Visibility = Visibility.Visible;
                exportPdaSettings.Visibility = Visibility.Collapsed;
                exportErpSettings.Visibility = Visibility.Collapsed;
                updateFolders.Visibility = Visibility.Collapsed;
                dynamicColumns.Visibility = Visibility.Collapsed;

                to = new Thickness(-10, 89, 0, 0);
            }
            else if (fe == exportPdaSettings)
            {
                ctrForm.Visibility = Visibility.Collapsed;
                importFolders.Visibility = Visibility.Collapsed;
                exportPdaSettings.Visibility = Visibility.Visible;
                exportErpSettings.Visibility = Visibility.Collapsed;
                updateFolders.Visibility = Visibility.Collapsed;
                dynamicColumns.Visibility = Visibility.Collapsed;

                to = new Thickness(-10, 132, 0, 0);
            }
            else if (fe == exportErpSettings)
            {
                ctrForm.Visibility = Visibility.Collapsed;
                importFolders.Visibility = Visibility.Collapsed;
                exportPdaSettings.Visibility = Visibility.Collapsed;
                exportErpSettings.Visibility = Visibility.Visible;
                updateFolders.Visibility = Visibility.Collapsed;
                dynamicColumns.Visibility = Visibility.Collapsed;

                to = new Thickness(-10, 177, 0, 0);
            }
            else if (fe == updateFolders)
            {
                ctrForm.Visibility = Visibility.Collapsed;
                importFolders.Visibility = Visibility.Collapsed;
                exportPdaSettings.Visibility = Visibility.Collapsed;
                exportErpSettings.Visibility = Visibility.Collapsed;
                updateFolders.Visibility = Visibility.Visible;
                dynamicColumns.Visibility = Visibility.Collapsed;

                to = new Thickness(-10, 221, 0, 0);
            }
            else if (fe == dynamicColumns)
            {
                ctrForm.Visibility = Visibility.Collapsed;
                importFolders.Visibility = Visibility.Collapsed;
                exportPdaSettings.Visibility = Visibility.Collapsed;
                exportErpSettings.Visibility = Visibility.Collapsed;
                updateFolders.Visibility = Visibility.Collapsed;
                dynamicColumns.Visibility = Visibility.Visible;

                to = new Thickness(-10, 265, 0, 0);
            }


            //175

            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = pathArrow.Margin;
            animation.To = to;
            animation.Duration = TimeSpan.FromMilliseconds(200);
            pathArrow.BeginAnimation(MarginProperty, animation);
        }
    }
}
