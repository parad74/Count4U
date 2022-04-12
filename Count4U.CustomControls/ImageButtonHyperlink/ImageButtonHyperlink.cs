using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Count4U.CustomControls.ImageButtonHyperlink
{
    public class ImageButtonHyperlink : Button
    {
        public static DependencyProperty ImageSourceProperty =
          DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButtonHyperlink), null);

        public static DependencyProperty ImageWidthProperty =
          DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButtonHyperlink), new FrameworkPropertyMetadata(Double.NaN));

        public static DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButtonHyperlink), new FrameworkPropertyMetadata(Double.NaN));

        public static DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(ImageButtonHyperlink), new FrameworkPropertyMetadata(null));

        static ImageButtonHyperlink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButtonHyperlink), new FrameworkPropertyMetadata(typeof(ImageButtonHyperlink)));
        }

        public ImageButtonHyperlink()
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

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}