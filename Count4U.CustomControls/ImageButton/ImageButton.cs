using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Count4U.CustomControls.ImageButton
{
    public class ImageButton : Button
    {
        public static DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton));        

        public static DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton), new FrameworkPropertyMetadata(32.0));

        public static DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButton), new FrameworkPropertyMetadata(32.0));

        public static DependencyProperty CenterXProperty =
           DependencyProperty.Register("CenterX", typeof(double), typeof(ImageButton), new FrameworkPropertyMetadata(16.0));

        public static DependencyProperty CenterYProperty =
         DependencyProperty.Register("CenterY", typeof(double), typeof(ImageButton), new FrameworkPropertyMetadata(16.0));

        public static DependencyProperty ScaleXProperty =
          DependencyProperty.Register("ScaleX", typeof(double), typeof(ImageButton), new FrameworkPropertyMetadata(1.1));

        public static DependencyProperty ScaleYProperty =
         DependencyProperty.Register("ScaleY", typeof(double), typeof(ImageButton), new FrameworkPropertyMetadata(1.1));

        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton),
             new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public ImageButton()
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

        public double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        public double ScaleX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public double ScaleY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }
    }
}