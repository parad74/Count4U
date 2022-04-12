using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation : IPropertyTranslation
    {
        private readonly Dictionary<string, PropertyTranslationItem> _values;

        public PropertyTranslation()
        {
            _values = new Dictionary<string, PropertyTranslationItem>();

            IturAnalyzes();
            Supplier();
			Family();
            Section();
            Location();
            InventProduct();
            Customer();
            Branch();
            Inventor();
            Itur();
            Product();
        }

        private void Add(PropertyInfo pi, string translation)
        {
            string key = BuildKey(pi.DeclaringType, pi);
            PropertyTranslationItem value = new PropertyTranslationItem();
            value.PropertyInfo = pi;
            value.Translation = translation;
            _values.Add(key, value);
        }

        private string BuildKey(System.Type type, PropertyInfo info)
        {
            return String.Format("{0}_{1}", type.FullName, info.Name);
        }

        public PropertyTranslationItem Get(PropertyInfo pi)
        {
            string key = BuildKey(pi.DeclaringType, pi);

            return _values[key];
        }

        public string GetTranslation(PropertyInfo pi)
        {
            return Get(pi).Translation;
        }
    }
}