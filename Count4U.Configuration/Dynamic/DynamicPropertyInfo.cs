using System.Reflection;

namespace Count4U.Configuration.Dynamic
{
    public class DynamicPropertyInfo
    {
        private readonly PropertyLink _propertyLink;        

        public DynamicPropertyInfo(PropertyLink propertyLink)
        {
            _propertyLink = propertyLink;
        }

        public System.Type Type { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public string Header { get; set; }
        public int Index { get; set; }

        public PropertyLink PropertyLink
        {
            get { return _propertyLink; }
        }

        public bool IsDouble()
        {
            return PropertyInfo.PropertyType.FullName == typeof(double).FullName;
        }

        public bool IsInt()
        {
            return PropertyInfo.PropertyType.FullName == typeof(int).FullName;
        }

        public bool IsString()
        {
            return PropertyInfo.PropertyType.FullName == typeof(string).FullName;
        }

        public bool IsBool()
        {
            return PropertyInfo.PropertyType.FullName == typeof(bool).FullName;
        }
    }
}