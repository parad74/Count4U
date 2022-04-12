using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Count4U.Planogram.Lib;

namespace Count4U.Planogram.View.PlanObjects
{
    /// <summary>
    /// Interaction logic for PlanObjectDecorator.xaml
    /// </summary>
    public partial class PlanObjectDecorator : DrawingRectangle
    {
     

        public PlanObjectDecorator(DrawingCanvas canvas)
            : base(canvas)
        {
            InitializeComponent();            

            thumbLeftTop.Visibility = Visibility.Hidden;
            thumbRightTop.Visibility = Visibility.Hidden;
            thumbRightBottom.Visibility = Visibility.Hidden;
            thumbLeftBottom.Visibility = Visibility.Hidden;         

            thumbLeftTop.DragDelta += Thumb_DragDelta;
            thumbRightTop.DragDelta += Thumb_DragDelta;
            thumbRightBottom.DragDelta += Thumb_DragDelta;
            thumbLeftBottom.DragDelta += Thumb_DragDelta;

            thumbLeftTop.DragCompleted += Thumb_DragCompleted;
            thumbRightTop.DragCompleted += Thumb_DragCompleted;
            thumbRightBottom.DragCompleted += Thumb_DragCompleted;
            thumbLeftBottom.DragCompleted += Thumb_DragCompleted;

            base._handles.Add(thumbLeftTop);
            base._handles.Add(thumbRightTop);
            base._handles.Add(thumbRightBottom);
            base._handles.Add(thumbLeftBottom);   
            
            this.SetCursor();
        }

        public PlanObjectDecorator()
        {
            InitializeComponent();
        }

        protected override void Add(UserControl uc)
        {
            grid.Children.Add(uc);
        }

        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.Inner.IsLocked) return;

            Thumb thumb = sender as Thumb;

            if (thumb == null)
                return;

            double deltaVertical, deltaHorizontal;

            RotateTransform rotateTransform = this.RotateTransform;

            if (rotateTransform == null)
                return;

            double angle = rotateTransform.Angle * Math.PI / 180.0;

            switch (thumb.HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    deltaHorizontal = Math.Min(e.HorizontalChange, this.ActualWidth - this.MinWidth);
                    Canvas.SetTop(this, Canvas.GetTop(this) + deltaHorizontal * Math.Sin(angle) - this.RenderTransformOrigin.X * deltaHorizontal * Math.Sin(angle));
                    Canvas.SetLeft(this, Canvas.GetLeft(this) + deltaHorizontal * Math.Cos(angle) + (this.RenderTransformOrigin.X * deltaHorizontal * (1 - Math.Cos(angle))));
                    this.Width -= deltaHorizontal;
                    break;
                case System.Windows.HorizontalAlignment.Right:
                    deltaHorizontal = Math.Min(-e.HorizontalChange, this.ActualWidth - this.MinWidth);
                    Canvas.SetTop(this, Canvas.GetTop(this) - this.RenderTransformOrigin.X * deltaHorizontal * Math.Sin(angle));
                    Canvas.SetLeft(this, Canvas.GetLeft(this) + (deltaHorizontal * this.RenderTransformOrigin.X * (1 - Math.Cos(angle))));
                    this.Width -= deltaHorizontal;
                    break;
                default:
                    break;
            }

            switch (thumb.VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    deltaVertical = Math.Min(-e.VerticalChange, this.ActualHeight - this.MinHeight);
                    Canvas.SetTop(this, Canvas.GetTop(this) + (this.RenderTransformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
                    Canvas.SetLeft(this, Canvas.GetLeft(this) - deltaVertical * this.RenderTransformOrigin.Y * Math.Sin(-angle));
                    this.Height -= deltaVertical;
                    break;
                case System.Windows.VerticalAlignment.Top:
                    deltaVertical = Math.Min(e.VerticalChange, this.ActualHeight - this.MinHeight);
                    Canvas.SetTop(this, Canvas.GetTop(this) + deltaVertical * Math.Cos(-angle) + (this.RenderTransformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
                    Canvas.SetLeft(this, Canvas.GetLeft(this) + deltaVertical * Math.Sin(-angle) - (this.RenderTransformOrigin.Y * deltaVertical * Math.Sin(-angle)));
                    this.Height -= deltaVertical;
                    break;
                default:
                    break;
            }

            OnDrawingSizeChanged();
        }

        public override void ShowHandles()
        {
           thumbLeftTop.Visibility = Visibility.Visible;
           thumbRightTop.Visibility = Visibility.Visible;
           thumbRightBottom.Visibility = Visibility.Visible;
           thumbLeftBottom.Visibility = Visibility.Visible;
        }

        public override void HideHandles()
        {
            thumbLeftTop.Visibility = Visibility.Hidden;
            thumbRightTop.Visibility = Visibility.Hidden;
            thumbRightBottom.Visibility = Visibility.Hidden;
            thumbLeftBottom.Visibility = Visibility.Hidden;
        }

        public override void SetCursor()
        {
            thumbLeftTop.Cursor = Cursors.SizeNWSE;
            thumbRightTop.Cursor = Cursors.SizeNESW;
            thumbRightBottom.Cursor = Cursors.SizeNWSE;
            thumbLeftBottom.Cursor = Cursors.SizeNESW;
        }
        
        public override void UnsetCursor()
        {
            thumbLeftTop.Cursor = null;
            thumbRightTop.Cursor = null;
            thumbRightBottom.Cursor = null;
            thumbLeftBottom.Cursor = null;
        }

        void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.Inner.IsLocked) return;

            OnDrawingSizeChanged();
        }
    }
}
