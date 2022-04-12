using System.Configuration;

namespace Count4U.Common.UserSettings
{
    public class StatusGroupSection : ConfigurationSection
    {
        [ConfigurationProperty("StatusGroupElementCollection")]
        public StatusGroupElementCollection StatusGroupElementCollection
        {
            get
            {
                return ((StatusGroupElementCollection)this["StatusGroupElementCollection"]);
            }
            set
            {
                this["StatusGroupElementCollection"] = value;
            }
        }
    }
}