using System.Configuration;

namespace Count4U.Common.UserSettings.Menu
{
    public class MenuElementCollection : ConfigurationElementCollection
    {
        #region Constructors
        static MenuElementCollection()
        {
            _properties = new ConfigurationPropertyCollection();
        }

        public MenuElementCollection()
        {
        }
        #endregion

        #region Fields
        private static ConfigurationPropertyCollection _properties;
        #endregion

        #region Properties
        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get
            {
                return "Menu";
            }
        }

        #endregion

        #region Indexers
        public MenuElement this[int index]
        {
            get { return (MenuElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new MenuElement this[string name]
        {
            get { return (MenuElement)base.BaseGet(name); }
        }
        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new MenuElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as MenuElement).Key;
        }

        public void Add(ConfigurationElement element)
        {
            base.BaseAdd(element);
        }

        public void Remove(ConfigurationElement item)
        {
            base.BaseRemove(item);
        }

        #endregion
    }
}