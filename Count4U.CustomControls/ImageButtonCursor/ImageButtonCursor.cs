using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Count4U.CustomControls.ImageButtonCursor
{
    public class ImageButtonCursor : Button
    {
          public static DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButtonCursor), null);

          public static DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButtonCursor), new FrameworkPropertyMetadata(Double.NaN));

          public static DependencyProperty ImageHeightProperty =
              DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButtonCursor), new FrameworkPropertyMetadata(Double.NaN));



        static ImageButtonCursor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButtonCursor),
             new FrameworkPropertyMetadata(typeof(ImageButtonCursor)));
        }

        public ImageButtonCursor()
        {

        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }
    }
}