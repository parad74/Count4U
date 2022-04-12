using System;
using System.Diagnostics;
using System.Windows.Input;
using NLog;

namespace Count4U.Common.Helpers
{
    public class MouseCursorWait : IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _message;
        private readonly Stopwatch _watch;

        public MouseCursorWait(string message = null)
        {
            _message = message;
            _watch = Stopwatch.StartNew();

            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void Dispose()
        {
            _watch.Stop();
            string log = String.Format("{0} : {1} seconds", _message, _watch.Elapsed.TotalSeconds.ToString());
            if (!String.IsNullOrEmpty(_message))
            {
                _logger.Info(log);
                System.Diagnostics.Debug.Print(log);
            }
            Mouse.OverrideCursor = null;
        }
    }
}