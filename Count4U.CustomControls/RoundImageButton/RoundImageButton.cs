using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Count4U.CustomControls.RoundImageButton
{
    public class RoundImageButton : Button
    {
        public static DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RoundImageButton), null);

   
        static RoundImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundImageButton),
             new FrameworkPropertyMetadata(typeof(RoundImageButton)));
        }

        public RoundImageButton()
        {

        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
    }
}