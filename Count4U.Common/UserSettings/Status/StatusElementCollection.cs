using System.Configuration;

namespace Count4U.Common.UserSettings
{
    

//    [ConfigurationCollection(typeof(StatusElement),
//    CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class StatusElementCollection : ConfigurationElementCollection
    {
        #region Constructors
        static StatusElementCollection()
        {
            _properties = new ConfigurationPropertyCollection();
        }

        public StatusElementCollection()
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
                return "status";
            }
        }
        
        #endregion

        #region Indexers
        public StatusElement this[int index]
        {
            get { return (StatusElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new StatusElement this[string name]
        {
            get { return (StatusElement)base.BaseGet(name); }
        }
        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new StatusElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as StatusElement).Name;
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