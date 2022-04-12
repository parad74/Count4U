using System;

namespace Count4U.Model.Extensions
{
    [AttributeUsageAttribute(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class PropertyNotBulkAttribute : Attribute
    {
         
    }
}