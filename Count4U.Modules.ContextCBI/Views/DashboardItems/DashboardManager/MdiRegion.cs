using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Count4U.Modules.ContextCBI.Views.DashboardItems.DashboardManager
{
    [Serializable]
    public class MdiRegion
    {
        [NonSerialized]
        private Point _position;
        [NonSerialized]
        private Brush _brush;

        public MdiRegion()
        {
            Settings = new Dictionary<string, string>();
            this.Brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(123, 123, 192));
        }

        public string DashboardName { get; set; }
        public string ViewName { get; set; }
        public string RegionName { get; set; }
        public string Title { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }               
        public Dictionary<String, String> Settings { get; set; }
        public bool IsOpen { get; set; }
        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Brush Brush
        {
            get { return _brush; }
            set { _brush = value; }
        }

        public double? MinWidth { get; set; }
        public double? MinHeight { get; set; }
    }
}