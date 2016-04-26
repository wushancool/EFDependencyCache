using EFChangeNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestConsole
{
    public class Cachable : ICache
    {
        private static Dictionary<String, Object> cacheHash = new Dictionary<string, object>();
        public T Get<T>(string key)
            where T : class
        {
            if (!cacheHash.ContainsKey(key)) return default(T);
            return cacheHash[key] as T;
        }

        public void Set(string key, object value)
        {
            if (cacheHash.ContainsKey(key)) cacheHash[key] = value;
            else cacheHash.Add(key, value);
        }
    }
}
