using System.Configuration;

namespace Count4U.Common.UserSettings.LogType
{
    public class LogTypeSection : ConfigurationSection
    {
        [ConfigurationProperty("LogTypeElementCollection")]
        public LogTypeElementCollection LogTypeElementCollection
        {
            get { return ((LogTypeElementCollection) this["LogTypeElementCollection"]); }
            set { this["LogTypeElementCollection"] = value; }
        }
    }
}