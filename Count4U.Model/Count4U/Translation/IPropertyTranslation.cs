using System.Reflection;

namespace Count4U.Model.Count4U.Translation
{
    public interface IPropertyTranslation
    {
        PropertyTranslationItem Get(PropertyInfo pi);
        string GetTranslation(PropertyInfo pi);
    }
}