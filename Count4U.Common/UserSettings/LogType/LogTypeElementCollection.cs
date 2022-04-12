using System.Configuration;

namespace Count4U.Common.UserSettings.LogType
{
    public class LogTypeElementCollection: ConfigurationElementCollection
    {
        #region Constructors
        static LogTypeElementCollection()
        {
            _properties = new ConfigurationPropertyCollection();
        }

        public LogTypeElementCollection()
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
                return "LogType";
            }
        }
        
        #endregion

        #region Indexers
        public LogTypeElement this[int index]
        {
            get { return (LogTypeElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new LogTypeElement this[string name]
        {
            get { return (LogTypeElement)base.BaseGet(name); }
        }
        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as LogTypeElement).Name;
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