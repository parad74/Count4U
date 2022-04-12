using System;
using System.Configuration;
using System.Windows.Media;

namespace Count4U.Common.UserSettings.Menu
{
    public class MenuElement : ConfigurationElement
    {
        public MenuElement()
        {
            this.IsVisible = true;
            BackgroundColor = UserSettingsHelpers.ColorToString(Color.FromRgb(100, 193, 255));
        }

        [ConfigurationProperty("Key", IsRequired = true)]
        public string Key
        {
            get { return (String)this["Key"]; }
            set { this["Key"] = value; }
        }

        [ConfigurationProperty("DashboardName", IsRequired = true)]
        public string DashboardName
        {
            get { return (String)this["DashboardName"]; }
            set { this["DashboardName"] = value; }
        }

        [ConfigurationProperty("PartName", IsRequired = true)]
        public string PartName
        {
            get { return (String)this["PartName"]; }
            set { this["PartName"] = value; }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (String)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("IsVisible", IsRequired = true)]
        public bool IsVisible
        {
            get { return (bool)this["IsVisible"]; }
            set { this["IsVisible"] = value; }
        }

        [ConfigurationProperty("BackgroundColor", IsRequired = true)]
        public string BackgroundColor
        {
            get { return (String)this["BackgroundColor"]; }
            set { this["BackgroundColor"] = value; }
        }

        [ConfigurationProperty("SortIndex", IsRequired = true)]
        public int SortIndex
        {
            get { return (int)this["SortIndex"]; }
            set { this["SortIndex"] = value; }
        }
    }
}