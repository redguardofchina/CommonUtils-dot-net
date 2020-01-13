using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 键值对
    /// </summary>
    public class MapStringString : Map<string, string>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public MapStringString()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public MapStringString(string key, string value)
        {
            Add(key, value);
        }

        /// <summary>
        /// 初始化 xxx:xxx;xxxx
        /// </summary>
        public MapStringString(string[] lines)
        {
            int splitIndex;
            foreach (var line in lines)
            {
                splitIndex = line.IndexOf(':');
                Add(line.Substring(0, splitIndex), line.Substring(splitIndex + 1));
            }
        }
    }
}
