using System.Configuration;

namespace Count4U.Common.UserSettings.Menu
{
    public class MenuSection : ConfigurationSection
    {
        [ConfigurationProperty("MenuElementCollection")]
        public MenuElementCollection MenuElementCollection
        {
            get { return ((MenuElementCollection)this["MenuElementCollection"]); }
            set { this["MenuElementCollection"] = value; }
        }
    }
}