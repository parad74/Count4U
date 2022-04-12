using System.Configuration;

namespace Count4U.Common.UserSettings
{
    public class RegionSection : ConfigurationSection
    {
        [ConfigurationProperty("RegionElement")]
        public RegionElement RegionElement
        {
            get
            {
                return ((RegionElement)this["RegionElement"]);
            }
            set
            {
                this["RegionElement"] = value;
            }
        }
    }
}