using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 缓存工具
    /// </summary>
    public static class CacheUtil
    {
        #region 短存储

        /*
         * var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
         * cache.Set("hello", KeysUtil.ChinaInnovation);
         * cache.Get("hello").Print();
         * 框架自带方法，这边暂不使用
         * 无法转为长存储 设置过期时间ExpirationScanFrequency无效 程序结束时释放
         */

        /// <summary>
        /// 缓存
        /// </summary>
        private static MapStringObject _memoryMap { get; } = new MapStringObject();

        /// <summary>
        /// 存储到缓存中
        /// </summary>
        public static void Set(string key, object value)
        => _memoryMap.Set(key, value);

        /// <summary>
        /// 存储到缓存中
        /// </summary>
        public static void Save(string key, object value)
        => Set(key, value);

        /// <summary>
        /// 从缓存中获取
        /// </summary>
        /// <returns></returns>
        public static TValue Get<TValue>(string key, TValue defaultValue = default)
        {
            if (_memoryMap.ContainsKey(key))
                return (TValue)_memoryMap.Get(key);
            return defaultValue;
        }

        /// <summary>
        /// 首次访问
        /// </summary>
        public static bool FirstAccess(string key)
        {
            if (_memoryMap.ContainsKey(key))
                return false;

            _memoryMap.Set(key, true);
            return true;
        }

        #endregion

        #region 长存储
        /// <summary>
        /// 缓存
        /// </summary>
        private static MapStringObject _nameValueMap { get; } = new MapStringObject();

        private static string _defaultSaveFloder = PathUtil.GetFull("settings");

        public static string SaveFloder = _defaultSaveFloder;

        public static void SetSaveFloder(string path)
        {
            SaveFloder = path;
            ConsoleUtil.Print("SaveFloder = {0}", path);
        }

        public static string ResetSaveFloder()
        => SaveFloder = _defaultSaveFloder;

        /// <summary>
        /// 存储路径
        /// </summary>
        private static string GetSavePath(string name)
        => SaveFloder.Combine(name + ".json");

        /// <summary>
        /// 存储到文件中
        /// </summary>
        public static void SetWithFile(string name, object value)
        {
            _nameValueMap.Set(name, value);
            FileUtil.Save(GetSavePath(name), value.ToJson(true));
        }

        /// <summary>
        /// 存储到文件中
        /// </summary>
        public static void SaveWithFile(string name, object value)
        => SetWithFile(name, value);

        /// <summary>
        /// 从文件中读取
        /// </summary>
        public static TValue GetFromFile<TValue>(string name, TValue defaultValue = default)
        {
            if (_nameValueMap.ContainsKey(name))
                return (TValue)_nameValueMap.Get(name);
            var path = GetSavePath(name);
            if (FileUtil.Exists(path))
                return JsonUtil.DeserializeFromFile<TValue>(path);
            return defaultValue;
        }
        #endregion
    }
}
