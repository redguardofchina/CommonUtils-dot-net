using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 键值对
    /// </summary>
    public class MapStringObject : Dictionary<string, object>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public MapStringObject()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public MapStringObject(string key, object value)
        {
            Add(key, value);
        }
    }
}
