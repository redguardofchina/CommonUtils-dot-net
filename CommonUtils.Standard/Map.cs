using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 键值对
    /// </summary>
    public class Map<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Map(TKey key, TValue value)
        {
            Add(key, value);
        }
    }
}
