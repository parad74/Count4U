using System;
using System.Windows.Media;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items
{
    [Serializable]
    public class MenuFilterItem
    {
        public string Name { get; set; }
        public string PartName { get; set; }
        public string DashboardName { get; set; }

        public string Title { get; set; }
        public bool IsOpen { get; set; }
        public int SortIndex { get; set; }
        public int SortIndexOriginal { get; set; }
        public string Color { get; set; }
    }
}