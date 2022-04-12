using System;
using System.Configuration;

namespace Count4U.Common.UserSettings.LogType
{
    public class LogTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (String)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("IsEnabled", IsRequired = true)]
        public bool IsEnabled
        {
            get { return (bool)this["IsEnabled"]; }
            set { this["IsEnabled"] = value; }
        }
         
    }
}