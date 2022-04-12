using System.Configuration;
using System;

namespace Count4U.Common.UserSettings
{
    public class RegionElement : ConfigurationElement
    {
        [Obsolete("was substituted by translation mechanism")]
        [ConfigurationProperty("Title", DefaultValue = "", IsRequired = false)]
        public string Title
        {
            get { return (String)this["Title"]; }
            set { this["Title"] = value; }
        }

        [ConfigurationProperty("X", DefaultValue = 0.0, IsRequired = true)]
        public double X
        {
            get { return (double)this["X"]; }
            set { this["X"] = value; }
        }

        [ConfigurationProperty("Y", DefaultValue = 0.0, IsRequired = true)]
        public double Y
        {
            get { return (double)this["Y"]; }
            set { this["Y"] = value; }
        }

        [ConfigurationProperty("Width", DefaultValue = 0.0, IsRequired = true)]
        public double Width
        {
            get { return (double)this["Width"]; }
            set { this["Width"] = value; }
        }

        [ConfigurationProperty("Height", DefaultValue = 0.0, IsRequired = true)]
        public double Height
        {
            get { return (double)this["Height"]; }
            set { this["Height"] = value; }
        }

         [Obsolete("was substituted by theming")]
        [ConfigurationProperty("ColorR", IsRequired = true)]
        public byte ColorR
        {
            get { return (byte)this["ColorR"]; }
            set { this["ColorR"] = value; }
        }

        [Obsolete("was substituted by theming")]
        [ConfigurationProperty("ColorG", IsRequired = true)]
        public byte ColorG
        {
            get { return (byte)this["ColorG"]; }
            set { this["ColorG"] = value; }
        }

        [Obsolete("was substituted by theming")]
        [ConfigurationProperty("ColorB", IsRequired = true)]
        public byte ColorB
        {
            get { return (byte)this["ColorB"]; }
            set { this["ColorB"] = value; }
        }

        [ConfigurationProperty("Theme", IsRequired = false, DefaultValue = "Theme2")]
        public string Theme
        {
            get { return (string)this["Theme"]; }
            set { this["Theme"] = value; }
        }

        [ConfigurationProperty("IsOpen", IsRequired = false, DefaultValue = true)]
        public bool IsOpen
        {
            get { return (bool)this["IsOpen"]; }
            set { this["IsOpen"] = value; }
        }
    }
}