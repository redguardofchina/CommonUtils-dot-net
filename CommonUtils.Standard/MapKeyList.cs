using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 数据收集器
    /// </summary>
    public class MapKeyList<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        /// <summary>
        /// 收集
        /// </summary>
        private void CollectRange(TKey key, IEnumerable<TValue> items)
        {
            if (!ContainsKey(key))
                this[key] = new List<TValue>();
            this[key].AddRange(items);
        }

        /// <summary>
        /// 收集
        /// </summary>
        public void Collect(TKey key, params TValue[] items)
        {
            CollectRange(key, items);
        }

        /// <summary>
        /// 收集
        /// </summary>
        public void AddRange(TKey key, IEnumerable<TValue> items)
        {
            CollectRange(key, items);
        }

        /// <summary>
        /// 收集
        /// </summary>
        public void Add(TKey key, params TValue[] items)
        {
            CollectRange(key, items);
        }

        /// <summary>
        /// 通过Value查找Key
        /// </summary>
        public TKey GetKey(TValue value)
        {
            foreach (var item in this)
            {
                if (item.Value.Contains(value))
                    return item.Key;
            }
            LogUtil.LogWithStackTrace("尝试查找不存在的Value");
            return default;
        }
    }
}
