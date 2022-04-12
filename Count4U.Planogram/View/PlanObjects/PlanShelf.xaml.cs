using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Count4U.Planogram.Lib;
using Count4U.Common.Helpers;
using Count4U.Planogram.Lib.Enums;

namespace Count4U.Planogram.View.PlanObjects
{
    /// <summary>
    /// Interaction logic for PlanShelf.xaml
    /// </summary>
    public partial class PlanShelf : PlanSpecialObject
    {
        public PlanShelf()
            : base(null)
        {

        }

        public PlanShelf(DrawingCanvas canvas)
            : base(canvas)
        {
            InitializeComponent();

            //            this.ellipse.PreviewMouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
            this.ellipse.PreviewMouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            this.PreviewMouseLeftButtonDown += PlanShelf_PreviewMouseLeftButtonDown;

            //            this.PreviewMouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

            border.BorderBrush = Brushes.Gray;

            this.SizeChanged += PlanShelf_SizeChanged;
        }      

        public override enPlanObjectType PlanType { get { return enPlanObjectType.Shelf; } }

        public override void SetBorderColor(SolidColorBrush brush)
        {
	
            if (brush == null)
            {
                border.BorderBrush = Brushes.Gray;
				borderDone.BorderBrush = Brushes.Gray;
				borderDone.Background = Brushes.Gray;
				borderBottom.BorderBrush = Brushes.Gray;
				borderBottom.Background = Brushes.Gray;

            }
            else
            {
                border.BorderBrush = brush;
				borderDone.BorderBrush = brush;
				borderDone.Background = brush;
				borderBottom.BorderBrush = brush;
				borderBottom.Background = brush;
            }

        }

        public override void SetBackgroundColor(double done, SolidColorBrush brush)
        {
            if (brush == null)
            {
                grid.Background = Brushes.LightGray;
            }
            else
            {
                grid.Background = brush;
            }

            tbDone.Text = String.Format("{0}%", (int)done);


        }

        void Ellipse_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        void Ellipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _canvas.CommandExecuted(this, enCommand.Statistic, null);
                e.Handled = true;
            }

        }

        void PlanShelf_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _canvas.CommandExecuted(this, enCommand.Statistic, null);
                e.Handled = true;
            }
        }     

        void PlanShelf_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
			ellipse.Visibility = Visibility.Collapsed;
				//(e.NewSize.Width > Helpers.MinWidthToCircleDisappear && e.NewSize.Height > Helpers.MinHeightToCircleDisappear) 
				//? Visibility.Visible 
				//: Visibility.Collapsed;

			if (e.NewSize.Width < e.NewSize.Height)
			{
				borderBottom.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
				borderBottom.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
				borderBottom.Height = 4;
				borderBottom.Width = e.NewSize.Width;
			}
			else
			{
				borderBottom.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
				borderBottom.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
				borderBottom.Width = 4;
				borderBottom.Height = e.NewSize.Height;
			}

        }
    }
}
