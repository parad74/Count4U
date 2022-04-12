using System;
using System.Windows;
using System.Windows.Input;
using Count4U.Planogram.Lib.Enums;
using Count4U.Planogram.Lib.Infrastructure;
using Count4U.Planogram.View;
using System.Windows.Controls;
using Count4U.Planogram.View.PlanObjects;
using Microsoft.Practices.Unity;

namespace Count4U.Planogram.Lib
{
    public abstract class ToolRectangle : Tool
    {
        private PlanObjectDecorator _planObjectDecorator;

        private Point _startPoint;

        protected abstract PlanObject GetObject(DrawingCanvas drawingCanvas);

        public override void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            drawingCanvas.UnselectAll();

            Point point = e.GetPosition(drawingCanvas);

            PlanObjectDecorator planObjectDecorator = new PlanObjectDecorator(drawingCanvas);            

            PlanObject element = GetObject(drawingCanvas);
            DrawingCanvasCommandResult codeResult = new DrawingCanvasCommandResult();
            drawingCanvas.CommandExecuted(this, enCommand.CodeGenerate, codeResult);
            if ((codeResult.Result is String) == false)
            {
                return;
            }
            element.Code = codeResult.Result as String;
            planObjectDecorator.Add(element);

            planObjectDecorator.Width = 0;
            planObjectDecorator.Height = 0;

            Canvas.SetLeft(planObjectDecorator, point.X);
            Canvas.SetTop(planObjectDecorator, point.Y);
            Panel.SetZIndex(planObjectDecorator, Helpers.GetDefaultZIndex(element.PlanType));

            drawingCanvas.Add(planObjectDecorator);

            _planObjectDecorator = planObjectDecorator;

            _startPoint = point;

            drawingCanvas.CaptureMouse();
        }

        public override void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed ||
                e.RightButton == MouseButtonState.Pressed)
            {
                drawingCanvas.Cursor = Helpers.DefaultCursor;
                return;
            }

            if (!drawingCanvas.IsMouseCaptured)
            {
                return;
            }

            Point point = e.GetPosition(drawingCanvas);

            double newLeft, newTop = 0;
            double newWidth, newHeight = 0;

            if (_startPoint.X <= point.X)
            {
                newLeft = _startPoint.X;
                newWidth = point.X - _startPoint.X;
            }
            else
            {
                newLeft = point.X;
                newWidth = _startPoint.X - point.X;
            }

            if (_startPoint.Y <= point.Y)
            {
                newTop = _startPoint.Y;
                newHeight = point.Y - _startPoint.Y;
            }
            else
            {
                newTop = point.Y;
                newHeight = _startPoint.Y - point.Y;
            }

            if (newWidth < 2 || newHeight < 2) return;

            Canvas.SetLeft(_planObjectDecorator, newLeft);
            Canvas.SetTop(_planObjectDecorator, newTop);

            _planObjectDecorator.Width = newWidth;
            _planObjectDecorator.Height = newHeight;

            OnDrawingChanged();
        }

        public override void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            drawingCanvas.ReleaseMouseCapture();

            drawingCanvas.Cursor = Helpers.DefaultCursor;

            drawingCanvas.Tool = enToolType.Pointer;

            if (_planObjectDecorator != null)
            {
                _planObjectDecorator.IsSelected = true;
                _planObjectDecorator.IsDirty = true;
            }

            drawingCanvas.IsDirty = true;
            OnDrawingChanged();
        }        

        public override void SetCursor(FrameworkElement el)
        {
            el.Cursor = Cursors.Cross;
        }
    }
}