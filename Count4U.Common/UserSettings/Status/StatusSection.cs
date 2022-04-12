using System.Configuration;

namespace Count4U.Common.UserSettings
{
    public class StatusSection : ConfigurationSection
    {
        [ConfigurationProperty("StatusElementCollection")]
        public StatusElementCollection StatusElementCollection
        {
            get
            {
                return ((StatusElementCollection)this["StatusElementCollection"]);
            }
            set
            {
                this["StatusElementCollection"] = value;
            }
        }
    }
}