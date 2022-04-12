using System.Configuration;

namespace Count4U.Common.UserSettings
{
    public class CommonSection: ConfigurationSection
    {
        [ConfigurationProperty("CommonElement")]
        public CommonElement CommonElement
        {
            get
            {
                return ((CommonElement)this["CommonElement"]);
            }
            set
            {
                this["CommonElement"] = value;
            }
        }
    }
}