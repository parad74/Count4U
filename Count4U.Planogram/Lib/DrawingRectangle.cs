using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using Shapes = System.Windows.Shapes;

namespace Count4U.Planogram.Lib
{
    public abstract class DrawingRectangle : DrawingObject
    {
        protected DrawingRectangle()
        {

        }

        protected DrawingRectangle(DrawingCanvas canvas)
            : base(canvas)
        {

        }

        private Rect Shape
        {
            get
            {
                GeneralTransform transform = this.TransformToVisual(_canvas);
                Rect transformBounds = transform.TransformBounds(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
                return transformBounds;
            }
        }

        public override bool MakeHitTest(Point point)
        {
            if (this.Shape.Contains(point))
                return true;

            return false;
        }

        public override int MakeHitTestHandle(Point point)
        {
            for (int i = 0; i < _handles.Count; i++)
            {
                FrameworkElement handle = _handles[i];

                Rect handleBounds = VisualTreeHelper.GetDescendantBounds(handle);
                Rect rectangleBounds = handle.RenderTransform.TransformBounds(handleBounds);

                Rect rec = new Rect(
                    handle.TransformToAncestor(_canvas).Transform(rectangleBounds.TopLeft),
                    handle.TransformToAncestor(_canvas).Transform(rectangleBounds.BottomRight));

                rec = new Rect(new Point(rec.TopLeft.X - 5, rec.TopLeft.Y - 5), new Point(rec.BottomRight.X + 5, rec.BottomRight.Y + 5));

                if (rec.Contains(point))
                {
                    return i;
                }
            }
            return -1;
        }

        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 0:
                    return Cursors.SizeNWSE;
                case 1:
                    return Cursors.SizeNESW;
                case 2:
                    return Cursors.SizeNWSE;
                case 3:
                    return Cursors.SizeNESW;
                default:
                    return null;
            }
        }

        public override void Move(double deltaX, double deltaY)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + deltaX);
            Canvas.SetTop(this, Canvas.GetTop(this) + deltaY);
        }

        public override bool IntersectsWith(System.Windows.Rect rectangle)
        {
            return Shape.IntersectsWith(rectangle);
        }
    }
}