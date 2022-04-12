using System;
using System.Collections.Generic;
using System.Reflection;

namespace Count4U.Configuration.Interfaces
{
    public interface IAllowedAsPropertyLinkRepository
    {
        List<PropertyInfo> Get(Type type);
    }
}