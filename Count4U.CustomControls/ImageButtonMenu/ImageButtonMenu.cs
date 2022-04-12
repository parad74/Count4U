using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Count4U.CustomControls.ImageButtonMenu
{
    public class ImageButtonMenu : Count4U.CustomControls.ImageButton.ImageButton
    {
        public ImageButtonMenu()
        {
        }       

        static ImageButtonMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButtonMenu),
             new FrameworkPropertyMetadata(typeof(ImageButtonMenu)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PreviewMouseLeftButtonUp += ImageButtonMenu_PreviewMouseLeftButtonUp;
            this.PreviewMouseRightButtonUp += ImageButtonMenu_PreviewMouseRightButtonUp;
        }

        void ImageButtonMenu_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void ImageButtonMenu_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.ContextMenu;
            if (cm != null)
            {
                cm.PlacementTarget = this;
                cm.IsOpen = true;
            }

            e.Handled = true;
        }

      
    }
}