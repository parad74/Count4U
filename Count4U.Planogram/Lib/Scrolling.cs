using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Count4U.Planogram.Lib
{
    public class Scrolling
    {
        private readonly ScrollViewer _scrollViewer;
        private readonly Grid _grid;
        private readonly ScaleTransform _scaleTransform;

        private double _zoomPercentage;

        private Point? _lastCenterPositionOnTarget;
        private Point? _lastMousePositionOnTarget;
        private Point? _lastDragPoint;

        public Scrolling(ScrollViewer scrollViewer, Grid grid, ScaleTransform scaleTransform)
        {
            _scaleTransform = scaleTransform;
            _grid = grid;
            _scrollViewer = scrollViewer;

            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;

            _zoomPercentage = Helpers.DefaultCanvasZoom;
        }

        public event EventHandler ZoomChanged = delegate { };

        protected virtual void OnZoomChanged()
        {
            EventHandler handler = ZoomChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        //        public Point? LastCenterPositionOnTarget
        //        {
        //            get { return _lastCenterPositionOnTarget; }
        //            set { _lastCenterPositionOnTarget = value; }
        //        }

        //        public Point? LastMousePositionOnTarget
        //        {
        //            get { return _lastMousePositionOnTarget; }
        //            set { _lastMousePositionOnTarget = value; }
        //        }

        public Point? LastDragPoint
        {
            get { return _lastDragPoint; }
            set { _lastDragPoint = value; }
        }

        public ScrollViewer ScrollViewer
        {
            get { return _scrollViewer; }
        }

        public double ZoomPercentage
        {
            get { return _zoomPercentage; }
        }

        public double OffsetX
        {
            get { return _scrollViewer.HorizontalOffset; }
        }

        public double OffsetY
        {
            get { return _scrollViewer.VerticalOffset; }
        }

        public void SetOffset(double x, double y)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _scrollViewer.ScrollToHorizontalOffset(x);
                    _scrollViewer.ScrollToVerticalOffset(y);
                }), DispatcherPriority.Background);
        }

        public void SetZoom(double percentage, bool useMouse)
        {
            if (percentage < 12.5)
            {
                percentage = 12.5;
            }

            if (percentage > 800)
            {
                percentage = 800;
            }

            _zoomPercentage = percentage;

            double zoom = 1 * percentage / 100;

            _scaleTransform.ScaleX = zoom;
            _scaleTransform.ScaleY = zoom;

            if (useMouse)
            {
                _lastMousePositionOnTarget = Mouse.GetPosition(_grid);
            }
            else
            {
                var centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
                _lastCenterPositionOnTarget = _scrollViewer.TranslatePoint(centerOfViewport, _grid);
            }

            OnZoomChanged();

            System.Diagnostics.Debug.Print("x: {0}, y: {1}", _scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset);
        }

        public void IncreaseZoom(bool huge = false)
        {
            SetZoom(ZoomPercentage + (huge ? 20 : 10), true);
        }

        public void DecreaseZoom(bool huge = false)
        {
            SetZoom(ZoomPercentage - (huge ? 20 : 10), true);
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                IncreaseZoom();
            }
            if (e.Delta < 0)
            {
                DecreaseZoom();
            }

            e.Handled = true;
        }

        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!_lastMousePositionOnTarget.HasValue)
                {
                    if (_lastCenterPositionOnTarget.HasValue)
                    {
                        //                        Point centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
                        //                        Point centerOfTargetNow = _scrollViewer.TranslatePoint(centerOfViewport, _grid);
                        //
                        //                        targetBefore = _lastCenterPositionOnTarget;
                        //                        targetNow = centerOfTargetNow;

                        //                        Point centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
                        //                        _scrollViewer.ScrollToHorizontalOffset(centerOfViewport.X);
                        //                        _scrollViewer.ScrollToVerticalOffset(centerOfViewport.Y);
                        //                        return;

                        _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ScrollableHeight / 2);
                        _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.ScrollableWidth / 2);
                    }
                }
                else
                {
                    targetBefore = _lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(_grid);

                    _lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / _grid.ActualWidth;
                    double multiplicatorY = e.ExtentHeight / _grid.ActualHeight;

                    double newOffsetX = _scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                    double newOffsetY = _scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    _scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    _scrollViewer.ScrollToVerticalOffset(newOffsetY);

                    //   System.Diagnostics.Debug.Print("x: {0}, y: {1}", newOffsetX, newOffsetY);
                }
            }
        }
    }
}