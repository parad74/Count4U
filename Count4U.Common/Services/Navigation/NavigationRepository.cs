using System;
using System.Collections.Generic;
using Count4U.Common.Interfaces;

namespace Count4U.Common.Services.Navigation
{
    public class NavigationRepository : INavigationRepository
    {
        private readonly Dictionary<string, object> _storage;

        public NavigationRepository()
        {
            _storage = new Dictionary<string, object>();
        }

        public string Add(object obj)
        {
            Guid guid = Guid.NewGuid();

            string key = guid.ToString();
            _storage.Add(key, obj);

            return key;
        }

        public object Get(string key)
        {
            if (_storage.ContainsKey(key))
                return _storage[key];

            return null;
        }

        public void Remove(string key)
        {
            _storage.Remove(key);
        }
    }
}