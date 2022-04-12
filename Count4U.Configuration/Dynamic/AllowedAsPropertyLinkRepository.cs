using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Count4U.Configuration.Interfaces;
using Count4U.Model.Extensions;
using NLog;

namespace Count4U.Configuration.Dynamic
{
    public class AllowedAsPropertyLinkRepository : IAllowedAsPropertyLinkRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Type, List<PropertyInfo>> _allowed;

        public AllowedAsPropertyLinkRepository()
        {
            _allowed = new Dictionary<Type, List<PropertyInfo>>();
        }

        public List<PropertyInfo> Get(Type type)
        {
            if (type == null)
                return null;

            if (!_allowed.ContainsKey(type))
            {
                List<PropertyInfo> result = new List<PropertyInfo>();

                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (pi.GetCustomAttributes(false).Any(r => r is PropertyLinkAttribute))
                    {
                        result.Add(pi);
                    }
                    else
                    {
                       // _logger.Warn(String.Format("Property {0} is not allowed by attribute", pi.Name));
                    }
                }

                _allowed[type] = result;
            }

            return _allowed[type];
        }
    }
}