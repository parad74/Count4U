using System;
using System.Collections.Generic;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter
{
    [Serializable]
    public class MdiFilterState
    {
        public MdiFilterState()
        {
            Mdis = new List<MdiFilterItem>();
            Menus = new List<MenuFilterItem>();
        }

        public List<MdiFilterItem> Mdis { get; set; }
        public List<MenuFilterItem> Menus { get; set; }
    }
}