using System;
using System.Windows;
using System.Windows.Input;

namespace Count4U.Planogram.Lib
{
    public abstract class Tool
    {
        public abstract void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e);
        public abstract void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e);
        public abstract void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e);

        public abstract void SetCursor(FrameworkElement el);

        public virtual void OnScrollMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {

        }

        public virtual void OnScrollMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {

        }

        public virtual void OnScrollMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {

        }        

        public virtual void OnKeyDown(DrawingCanvas drawingCanvas, KeyEventArgs e)
        {
            
        }

        public virtual void OnKeyUp(DrawingCanvas drawingCanvas, KeyEventArgs e)
        {

        }

        public event EventHandler DrawingChanged = delegate { };

        protected virtual void OnDrawingChanged()
        {
            EventHandler handler = DrawingChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }    
}