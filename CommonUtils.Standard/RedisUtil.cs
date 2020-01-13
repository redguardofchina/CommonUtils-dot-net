using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    public static class RedisUtil
    {
        private static RedisClient mClient = new RedisClient();

        public static void Set<T>(string key, T value)
        {
            mClient.Set(key, value);
        }

        public static T Get<T>(string key)
        {
            return mClient.Get<T>(key);
        }
    }
}
