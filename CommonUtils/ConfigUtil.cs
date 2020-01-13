using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;

namespace CommonUtils
{
    /// <summary>
    /// 配置文件
    /// sub代表分支
    /// </summary>
    public static class ConfigUtil
    {
        #region 路径
        /// <summary>
        /// 获取存储位置
        /// </summary>
        public static string GetPath(string fileName)
        => PathConfig.ConfigFloder.Combine(fileName);

        /// <summary>
        /// 获取存储位置
        /// </summary>
        public static string GetSubPath(string sub, string fileName)
        {
            if (string.IsNullOrWhiteSpace(sub) || sub == PathConfig.DefaultSubMark)
                return GetPath(fileName);
            else
                return PathConfig.SubConfigFloder.Combine(sub, fileName);
        }
        #endregion

        #region 文件操作
        /// <summary>
        /// 获取所有配置文件
        /// </summary>
        public static string[] GetFiles()
        => FloderUtil.GetFiles(PathConfig.ConfigFloder).Select(m => m.Name).ToArray();

        /// <summary>
        /// 获取所有配置文件
        /// </summary>
        public static string[] GetSubFiles(string sub)
        => FloderUtil.GetFiles(PathConfig.SubConfigFloder.Combine(sub)).Select(m => m.Name).ToArray();

        /// <summary>
        /// 存储文件到配置
        /// </summary>
        public static void Save(string fileName, string text)
        => FileUtil.Save(GetPath(fileName), text);

        /// <summary>
        /// 存储文件到配置
        /// </summary>
        public static void SaveSub(string sub, string fileName, string text)
        => FileUtil.Save(GetSubPath(sub, fileName), text);

        /// <summary>
        /// 读取配置信息
        /// </summary>
        public static string GetText(string fileName)
        => FileUtil.GetText(GetPath(fileName));

        /// <summary>
        /// 读取配置信息
        /// </summary>
        public static string GetSubText(string sub, string fileName)
        => FileUtil.GetText(GetSubPath(sub, fileName));

        /// <summary>
        /// 读取配置信息
        /// </summary>
        public static string[] GetLines(string fileName)
        => FileUtil.GetLines(GetPath(fileName));

        /// <summary>
        /// 读取配置信息
        /// </summary>
        public static string[] GetSubLines(string sub, string fileName)
        => FileUtil.GetLines(GetSubPath(sub, fileName));

        /// <summary>
        /// 读取配置信息
        /// </summary>
        public static JObject GetJObject(string fileName)
        => JsonUtil.GetJObjectFromFile(GetPath(fileName));

        public static JObject Deserialize(string fileName)
       => JsonUtil.GetJObjectFromFile(GetPath(fileName));

        /// <summary>
        /// 读取配置信息
        /// </summary>
        public static JObject GetSubJObject(string sub, string fileName)
        => JsonUtil.GetJObjectFromFile(GetSubPath(sub, fileName));

        /// <summary>
        /// 反序列化
        /// </summary>
        public static T Deserialize<T>(string fileName)
        => JsonUtil.DeserializeFromFile<T>(GetPath(fileName));

        /// <summary>
        /// 反序列化
        /// </summary>
        public static T DeserializeSub<T>(string sub, string fileName)
        => JsonUtil.DeserializeFromFile<T>(GetSubPath(sub, fileName));
        #endregion

        #region 默认配置
        private static string _defaultPath = GetPath(PathConfig.DefaultConfigFileName);
        private static JObject _default;

        /// <summary>
        /// 默认配置
        /// </summary>
        public static JObject Default
        {
            get
            {
                if (_default == null)
                {
                    if (!FileUtil.Exists(_defaultPath))
                        LogUtil.Log(GetMissingException());

                    _default = JsonUtil.GetJObjectFromFile(_defaultPath);
                    if (_default == null)
                        _default = new JObject();
                }
                return _default;
            }
            set
            {
                _default = value;
            }
        }

        /// <summary>
        /// 提醒配置可抛出此报错
        /// </summary>
        /// <param name="stackTrace">new StackTrace(true)</param>
        public static Exception GetMissingException(StackTrace stackTrace = null)
        => new ExceptionPlus("配置缺失！！！@" + _defaultPath, stackTrace);

        /// <summary>
        /// 赋值 附带存储
        /// </summary>
        public static void Set(string key, object value, bool replace = true)
        {
            Default.Set(key, value, replace);
            FileUtil.Save(_defaultPath, Default.ToJson());
        }

        /// <summary>
        /// 取值
        /// </summary>
        public static T Get<T>(string key, T defaultValue = default)
        => Default.Get(key, defaultValue);

        /// <summary>
        /// 取值
        /// </summary>
        public static T GetValue<T>(string key, T defaultValue = default)
        => Default.Get(key, defaultValue);

        /// <summary>
        /// 取值
        /// </summary>
        public static string GetString(string key, string defaultValue = default)
        => Default.Get(key, defaultValue);

        /// <summary>
        /// 取值
        /// </summary>
        public static string Get(string key)
        => GetString(key);

        public static int GetInteger(string key)
        => Get<int>(key);
        #endregion
    }
}
