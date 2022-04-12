using System.Configuration;

namespace Count4U.Common.UserSettings
{
    

//    [ConfigurationCollection(typeof(StatusElement),
//    CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class StatusGroupElementCollection : ConfigurationElementCollection
    {
        #region Constructors
        static StatusGroupElementCollection()
        {
            _properties = new ConfigurationPropertyCollection();
        }

        public StatusGroupElementCollection()
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
        public StatusGroupElement this[int index]
        {
            get { return (StatusGroupElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new StatusGroupElement this[string name]
        {
            get { return (StatusGroupElement)base.BaseGet(name); }
        }
        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new StatusGroupElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as StatusGroupElement).Name;
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