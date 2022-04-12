using System;
using System.Configuration;

namespace Count4U.Common.UserSettings
{
    public class StatusElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (String)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Color", IsRequired = true)]
        public string Color
        {
            get { return (String)this["Color"]; }
            set { this["Color"] = value; }
        }
    }
}