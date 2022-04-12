using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Count4U.Planogram.Lib
{
    public class ToolHand : Tool
    {
        private readonly Scrolling _scrolling;

        public ToolHand(Scrolling scrolling)
        {
            _scrolling = scrolling;
        }
       
        public override void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
           
        }

        public override void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
          
        }

        public override void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
          
        }

        public override void OnScrollMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(_scrolling.ScrollViewer);
                if (mousePos.X <= _scrolling.ScrollViewer.ViewportWidth && mousePos.Y < _scrolling.ScrollViewer.ViewportHeight) //make sure we still can use the scrollbars
                {
                    _scrolling.ScrollViewer.Cursor = Cursors.SizeAll;
                    _scrolling.LastDragPoint = mousePos;
                    Mouse.Capture(_scrolling.ScrollViewer);
                }
            }
        }
        public override void OnScrollMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            if (_scrolling.LastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(_scrolling.ScrollViewer);

                double dX = posNow.X - _scrolling.LastDragPoint.Value.X;
                double dY = posNow.Y - _scrolling.LastDragPoint.Value.Y;

                _scrolling.LastDragPoint = posNow;

                _scrolling.ScrollViewer.ScrollToHorizontalOffset(_scrolling.ScrollViewer.HorizontalOffset - dX);
                _scrolling.ScrollViewer.ScrollToVerticalOffset(_scrolling.ScrollViewer.VerticalOffset - dY);
            }
        }
        public override void OnScrollMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            _scrolling.ScrollViewer.Cursor = Cursors.Arrow;
            _scrolling.ScrollViewer.ReleaseMouseCapture();
            _scrolling.LastDragPoint = null;
        }


        public override void SetCursor(FrameworkElement el)
        {
            el.Cursor = Cursors.SizeAll;
        }
    }
}