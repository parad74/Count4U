using System.Collections.Generic;
using System.Linq;

namespace Count4U.Modules.ContextCBI.Views.DashboardItems.DashboardManager
{
    public class MdiRegionLayoutCollection
    {
        private readonly List<MdiRegionLayout> _items;

        public MdiRegionLayoutCollection()
        {
            _items = new List<MdiRegionLayout>();
        }

        public List<MdiRegionLayout> Items
        {
            get { return _items; }
        }

        public MdiRegionLayout Get(string viewName, string regionName)
        {
            return _items.FirstOrDefault(r => r.ViewName == viewName && r.RegionName == regionName);
        }

        public MdiRegionLayout Get(MdiRegion mdiRegion)
        {
            return Get(mdiRegion.ViewName, mdiRegion.RegionName);
        }
    }
}