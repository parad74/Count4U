using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Count4U.Planogram.Lib
{
    public class ToolZoom : Tool
    {
        private readonly Scrolling _scrolling;

        private FrameworkElement _el;

        public ToolZoom(Scrolling scrolling)
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
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                _scrolling.DecreaseZoom(huge: true);
            else
                _scrolling.IncreaseZoom(huge: true);
        }

        public override void OnKeyDown(DrawingCanvas drawingCanvas, KeyEventArgs e)
        {
            if (e.Key == Key.System && (e.SystemKey == Key.RightAlt || e.SystemKey == Key.LeftAlt))
            {
                SetCursor(false);
            }
        }

        public override void OnKeyUp(DrawingCanvas drawingCanvas, KeyEventArgs e)
        {
            if (e.Key == Key.System && (e.SystemKey == Key.RightAlt || e.SystemKey == Key.LeftAlt))
            {
                SetCursor(true);
            }
        }

        public override void SetCursor(FrameworkElement el)
        {
            _el = el;

            SetCursor(true);
        }

        private void SetCursor(bool zoomIn)
        {
            if (_el == null)
                return;

            string path = zoomIn ? @"/Count4U.Media;component/Cursors/zoom-in.cur" : @"/Count4U.Media;component/Cursors/zoom-out.cur";
            System.Windows.Resources.StreamResourceInfo info = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            _el.Cursor = new System.Windows.Input.Cursor(info.Stream);
        }
    }
}