using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Count4U.Planogram.Lib.Enums;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using SelectionMode = Count4U.Planogram.Lib.Enums.SelectionMode;

namespace Count4U.Planogram.Lib
{
    public class ToolPointer : Tool
    {
        private SelectionMode _selectMode;

        private Point _lastPoint;

        private Rectangle _selectionRectangle;
        private Point _selectionStartPoint;

        private DrawingObject _rotateObject;
        private Point _rotateCenterPoint;
        private Vector _rotateStartVector;
        private double _rotateInitialAngle;

        private bool _wasMove;

        public ToolPointer()
        {
            _selectMode = SelectionMode.None;
            _lastPoint = new Point(0, 0);
        }

        public override void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OnMouseLeftDown(drawingCanvas, e);
                return;
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                OnMouseRightDown(drawingCanvas, e);
                return;
            }
        }

        public override void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                drawingCanvas.Cursor = Helpers.DefaultCursor;
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OnMouseLeftMove(drawingCanvas, e);
                return;
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                OnMouseRightMove(drawingCanvas, e);
                return;
            }
        }

        private void OnMouseLeftDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(drawingCanvas);

            _selectMode = SelectionMode.None;

            DrawingObject o;
            DrawingObject movedObject = null;

            // Test for resizing (only if control is selected, cursor is on the handle)
            for (int i = drawingCanvas.Objects.Count - 1; i >= 0; i--)
            {
                o = drawingCanvas.Objects[i];

                if (o.IsSelected)
                {
                    int handleNumber = o.MakeHitTestHandle(point);

                    if (handleNumber >= 0)
                    {
                        _selectMode = SelectionMode.Size;

                        // Since we want to resize only one object, unselect all other objects
                        drawingCanvas.UnselectAll();
                        o.IsSelected = true;

                        break;
                    }
                }
            }

            // Test for move (cursor is on the object)
            if (_selectMode == SelectionMode.None)
            {
                //for (int i = drawingCanvas.Objects.Count - 1; i >= 0; i--)
                foreach (DrawingObject drawingObject in drawingCanvas.Objects.OrderByDescending(r => Panel.GetZIndex(r)))
                {
                    //    o = drawingCanvas.Objects[i];
                    o = drawingObject;

                    if (o.MakeHitTest(point) == true)
                    {
                        movedObject = o;
                        break;
                    }
                }

                if (movedObject != null)
                {
                    _selectMode = SelectionMode.Move;

                    // Unselect all if Ctrl is not pressed and clicked object is not selected yet
                    if (Keyboard.Modifiers != ModifierKeys.Control && !movedObject.IsSelected)
                    {
                        drawingCanvas.UnselectAll();
                    }

                    // Select clicked object
                    movedObject.IsSelected = true;

                    // Set move cursor
                    drawingCanvas.Cursor = Cursors.SizeAll;
                }
            }

            // Click on background
            if (_selectMode == SelectionMode.None)
            {
                // Unselect all if Ctrl is not pressed
                if (Keyboard.Modifiers != ModifierKeys.Control)
                {
                    drawingCanvas.UnselectAll();
                }

                _selectionRectangle = new Rectangle();
                _selectionRectangle.Fill = Brushes.Transparent;
                _selectionRectangle.Stroke = Brushes.Black;
                _selectionRectangle.StrokeDashArray = new DoubleCollection() { 5, 2 };
                _selectionRectangle.StrokeThickness = 1;
                Canvas.SetLeft(_selectionRectangle, point.X);
                Canvas.SetTop(_selectionRectangle, point.Y);
                Panel.SetZIndex(_selectionRectangle, Helpers.ZIndexSelectionRectangle);
                drawingCanvas.AddHandle(_selectionRectangle);

                _selectMode = SelectionMode.GroupSelection;
                _selectionStartPoint = point;
            }

            _lastPoint = point;

            drawingCanvas.CaptureMouse();
        }

        private void OnMouseRightDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            DrawingObject o = null;
            Point point = e.GetPosition(drawingCanvas);

            // Test for rotate (only if control is selected, cursor is on the handle)
            for (int i = drawingCanvas.Objects.Count - 1; i >= 0; i--)
            {
                o = drawingCanvas.Objects[i];

                if (o.IsSelected)
                {
                    int handleNumber = o.MakeHitTestHandle(point);

                    if (handleNumber >= 0)
                    {
                        _selectMode = SelectionMode.Rotate;

                        // keep resized object in class member
                        _rotateObject = o;

                        // Since we want to rotate only one object, unselect all other objects
                        drawingCanvas.UnselectAll();
                        o.IsSelected = true;

                        break;
                    }
                }
            }
            if (o != null && _rotateObject != null)
            {
                if (_rotateObject.Inner.IsLocked)
                {
                    _rotateObject = null;
                }
                else
                {
                    _rotateCenterPoint = o.TranslatePoint(
                        new Point(o.ActualWidth * o.RenderTransformOrigin.X, o.ActualHeight * o.RenderTransformOrigin.Y),
                        drawingCanvas);

                    Point startPoint = Mouse.GetPosition(drawingCanvas);
                    _rotateStartVector = Point.Subtract(startPoint, _rotateCenterPoint);

                    RotateTransform rotateTransform = o.RotateTransform;
                    if (rotateTransform != null)
                    {
                        _rotateInitialAngle = rotateTransform.Angle;
                    }
                }
            }

            //test for selection
            if (_selectMode == SelectionMode.None)
            {
                DrawingObject selectedObject = null;

                //for (int i = drawingCanvas.Objects.Count - 1; i >= 0; i--)
                foreach (DrawingObject drawingObject in drawingCanvas.Objects.OrderByDescending(r => Panel.GetZIndex(r)))
                {
                    //    o = drawingCanvas.Objects[i];
                    o = drawingObject;

                    if (o.MakeHitTest(point) == true)
                    {
                        selectedObject = o;
                        break;
                    }
                }

                if (selectedObject != null)
                {
                    _selectMode = SelectionMode.None;

                    // Unselect all if Ctrl is not pressed and clicked object is not selected yet
                    if (Keyboard.Modifiers != ModifierKeys.Control && !selectedObject.IsSelected)
                    {
                        drawingCanvas.UnselectAll();
                    }

                    // Select clicked object
                    selectedObject.IsSelected = true;
                }
            }
        }

        private void OnMouseRightMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            if (_rotateObject != null)
            {
                Point point = e.GetPosition(drawingCanvas);
                Vector deltaVector = Point.Subtract(point, _rotateCenterPoint);

                double angle = Vector.AngleBetween(_rotateStartVector, deltaVector);

                RotateTransform rotateTransform = _rotateObject.RenderTransform as RotateTransform;
                if (rotateTransform != null)
                {
                    rotateTransform.Angle = _rotateInitialAngle + Math.Round(angle, 0);
                }

                OnDrawingChanged();
            }
        }

        private void OnMouseLeftMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            Point point = e.GetPosition(drawingCanvas);

            if (!drawingCanvas.IsMouseCaptured)
            {
                return;
            }

            // Find difference between previous and current position
            double dx = point.X - _lastPoint.X;
            double dy = point.Y - _lastPoint.Y;


            // Resize
            if (_selectMode == SelectionMode.Size)
            {

            }

            // Move
            if (_selectMode == SelectionMode.Move)
            {
                foreach (DrawingObject o in drawingCanvas.Objects)
                {
                    if (o.IsSelected)
                    {
                        if (o.Inner.IsLocked)
                        {
                            continue;
                        }

                        o.Move(dx, dy);

                        if (dx != 0 || dy != 0)
                        {
                            _wasMove = true;
                        }
                    }
                }

                OnDrawingChanged();
            }

            // Group selection
            if (_selectMode == SelectionMode.GroupSelection)
            {
                double newLeft, newTop = 0;
                double newWidth, newHeight = 0;

                if (_selectionStartPoint.X <= point.X)
                {
                    newLeft = _selectionStartPoint.X;
                    newWidth = point.X - _selectionStartPoint.X;
                }
                else
                {
                    newLeft = point.X;
                    newWidth = _selectionStartPoint.X - point.X;
                }

                if (_selectionStartPoint.Y <= point.Y)
                {
                    newTop = _selectionStartPoint.Y;
                    newHeight = point.Y - _selectionStartPoint.Y;
                }
                else
                {
                    newTop = point.Y;
                    newHeight = _selectionStartPoint.Y - point.Y;
                }

                if (newWidth < 2 || newHeight < 2) return;

                Canvas.SetLeft(_selectionRectangle, newLeft);
                Canvas.SetTop(_selectionRectangle, newTop);

                _selectionRectangle.Width = newWidth;
                _selectionRectangle.Height = newHeight;
            }

            _lastPoint = point;
        }

        public override void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            _rotateObject = null;

            if (!drawingCanvas.IsMouseCaptured)
            {
                switch (_selectMode)
                {
                    case SelectionMode.None:
                        break;
                    case SelectionMode.Move:
                        break;
                    case SelectionMode.Size:
                        break;
                    case SelectionMode.Rotate:
                        bool wasChanged = false;
                        foreach (DrawingObject drawingObject in drawingCanvas.Objects.Where(r => r.IsSelected))
                        {
                            if (drawingObject.Inner.IsLocked == false)
                            {
                                drawingObject.IsDirty = true;
                                wasChanged = true;
                            }
                        }
                        if (wasChanged)
                        {
                            drawingCanvas.IsDirty = true;
                            base.OnDrawingChanged();
                        }

                        e.Handled = true;
                        break;
                    case SelectionMode.GroupSelection:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                drawingCanvas.Cursor = Helpers.DefaultCursor;
                _selectMode = SelectionMode.None;
                return;
            }

            if (_selectMode == SelectionMode.GroupSelection)
            {
                RemoveSelectionRectangle(drawingCanvas);

                System.Windows.Rect rec = new Rect(
                    Canvas.GetLeft(_selectionRectangle),
                    Canvas.GetTop(_selectionRectangle),
                    _selectionRectangle.ActualWidth,
                    _selectionRectangle.ActualHeight);

                foreach (DrawingObject drawingObject in drawingCanvas.Objects)
                {
                    if (drawingObject.IntersectsWith(rec))
                    {
                        drawingObject.IsSelected = true;
                    }
                }
            }

            drawingCanvas.ReleaseMouseCapture();

            drawingCanvas.Cursor = Helpers.DefaultCursor;

            switch (_selectMode)
            {
                case SelectionMode.None:
                    break;
                case SelectionMode.Move:
                    if (_wasMove)
                    {
                        foreach (DrawingObject drawingObject in drawingCanvas.Objects.Where(r => r.IsSelected))
                        {
                            drawingObject.IsDirty = true;
                        }
                        drawingCanvas.IsDirty = true;
                        base.OnDrawingChanged();
                    }
                    break;
                case SelectionMode.Size:
                    base.OnDrawingChanged();
                    break;
                case SelectionMode.Rotate:
                    break;
                case SelectionMode.GroupSelection:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _selectMode = SelectionMode.None;
            _wasMove = false;
        }

        public override void SetCursor(FrameworkElement el)
        {
            el.Cursor = Helpers.DefaultCursor;
        }

        public void RemoveSelectionRectangle(DrawingCanvas drawingCanvas)
        {
            if (_selectionRectangle != null)
            {
                drawingCanvas.RemoveHandle(_selectionRectangle);
            }
        }

        public override void OnKeyDown(DrawingCanvas drawingCanvas, KeyEventArgs e)
        {
            base.OnKeyDown(drawingCanvas, e);

            if (e.Key == Key.Delete)
            {
                drawingCanvas.CommandExecuted(this, enCommand.Delete, null);
            }

            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            {
                double dx = 0;
                double dy = 0;
                switch (e.Key)
                {
                    case Key.Left:
                        dx = -1;
                        break;
                    case Key.Right:
                        dx = 1;
                        break;
                    case Key.Up:
                        dy = -1;
                        break;
                    case Key.Down:
                        dy = 1;
                        break;
                }
                foreach (DrawingObject drawingObject in drawingCanvas.Objects.Where(r => r.IsSelected))
                {
                    drawingObject.Move(dx, dy);
                }

                if (drawingCanvas.Objects.Any(r => r.IsSelected))
                {
                    drawingCanvas.IsDirty = true;
                }

                base.OnDrawingChanged();

                e.Handled = true;
            }
        }

        public override void OnKeyUp(DrawingCanvas drawingCanvas, KeyEventArgs e)
        {
            base.OnKeyUp(drawingCanvas, e);
        }
    }
}