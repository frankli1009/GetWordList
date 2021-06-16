using System;
using GetWords.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GetWords.Utilities
{
    public class SessionKeys
    {
        public SessionKeys()
        {
        }

        public static string SessionKey_Requirement(string letters, int length)
        {
            return string.Format("SK_Requirement_{0}_{1}_{2:ddHHmmssfffff}",
                letters, length, DateTime.Now);
        }

        public static T GetSessionValue<T>(IMemoryCache cache, string key, T defaultValue)
        {
            T value;
            if (!cache.TryGetValue(key, out value))
            {
                // Key not in cache, so get data.
                value = defaultValue;

                // Save data in cache and set the relative expiration time to one day
                cache.Set(key, value, TimeSpan.FromDays(1));
            }
            return value;
        }
    }
}
