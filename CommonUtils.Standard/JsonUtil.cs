using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonUtils
{
    public static class JsonUtil
    {
        #region 转换
        /// <summary>
        /// 类序列化为json
        /// </summary>
        public static string Serialize(object obj, bool min = false)
        {
            if (obj == null)
                return default;

            try
            {
                if (min)
                    return JsonConvert.SerializeObject(obj);
                else
                    return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return default;
            }
        }

        /// <summary>
        /// 将实体转换为Json
        /// </summary>
        public static string ToJson(this object obj, bool min = false)
        => Serialize(obj, min);
        public static string ToJsonString(this object obj, bool min = false)
        => Serialize(obj, min);
        public static string GetJson(this object obj, bool min = false)
        => Serialize(obj, min);
        public static string GetJsonString(this object obj, bool min = false)
        => Serialize(obj, min);

        /// <summary>
        /// 类序列化为json再转为uft8.bytes
        /// </summary>
        public static byte[] ToJsonBytes(this object obj)
        => obj.ToJson(true).ToBytes();

        /// <summary>
        /// json反序列化为类
        /// </summary>
        public static T Deserialize<T>(string json, T defaultValue = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return defaultValue;
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                LogUtil.Log(string.Format("【json deserialize error】\r\n{0}\r\n【json deserialize trace】\r\n{1}\r\n【json deserialize text】\r\n{2}", ex.Message, new StackTrace(true), json));
                return defaultValue;
            }
        }

        /// <summary>
        /// json反序列化为类
        /// </summary>
        public static T DeserializeFromFile<T>(string path, T defaultValue = default)
        {
            string json = FileUtil.Read(path);
            return Deserialize(json, defaultValue);
        }

        public static T ReadFromFile<T>(string path)
        => Deserialize<T>(FileUtil.Read(path));

        /// <summary>
        /// json反序列化为类
        /// </summary>
        public static T DeserializeFromBytes<T>(byte[] bytes, T defaultValue = default)
        {
            string json = bytes.ToText();
            return Deserialize<T>(json, defaultValue);
        }

        /// <summary>
        /// 获取JObject
        /// </summary>
        public static JObject ParseToJObject(this string json)
        {
            try
            {
                return JObject.Parse(json);
            }
            catch
            {
                return default;
            }
        }

        public static JObject GetJObjectFromFile(string path)
        => FileUtil.Read(path).ParseToJObject();
        public static JObject ParseToJObjectFromFile(string path)
        => GetJObjectFromFile(path);

        /// <summary>
        /// 获取JArray
        /// </summary>
        public static JArray ParseToJArray(this string json)
        {
            try
            {
                return JArray.Parse(json);
            }
            catch
            {
                return default;
            }
        }

        public static JArray GetJArrayFromFile(string path)
        => FileUtil.Read(path).ParseToJArray();

        /// <summary>
        /// 获取JToken
        /// </summary>
        public static JToken ParseToJToken(this string json)
        {
            try
            {
                return JToken.Parse(json);
            }
            catch
            {
                return default;
            }
        }

        public static JToken GetJTokenFromFile(string path)
        => FileUtil.Read(path).ParseToJToken();

        /// <summary>
        /// 获取JToken
        /// </summary>
        public static JToken ToJToken(this object obj)
        {
            if (obj == null)
                return default;
            return JToken.FromObject(obj);
        }

        /// <summary>
        /// 获取JObject
        /// </summary>
        public static JObject ToJObject(this object obj)
        {
            if (obj == null)
                return default;
            return JObject.FromObject(obj);
        }

        /// <summary>
        /// 获取JArray
        /// </summary>
        public static JArray ToJArray(this object obj)
        {
            if (obj == null)
                return default;
            return JArray.FromObject(obj);
        }
        #endregion

        #region 设值取值
        /// <summary>
        /// 设值
        /// </summary>
        public static void Set(this JToken jToken, string key, object value, bool replace = true)
        {
            if (jToken.Type == JTokenType.Null)
                return;

            if (!replace && jToken[key] != null)
                return;

            jToken[key] = value.ToJToken();
        }

        /// <summary>
        /// 设值
        /// </summary>
        public static void Put(this JToken jToken, string key, object value, bool replace = true)
        => jToken.Set(key, value, replace);

        /// <summary>
        /// 取值
        /// </summary>
        public static T Get<T>(this JToken jToken, string key, T defaultValue = default, bool log = true)
        {
            if (jToken.Type == JTokenType.Null)
                return defaultValue;

            var subJToken = jToken[key];
            if (subJToken == null)
            {
                if (log)
                    LogUtil.Log("【JToken尝试获取不存在的Key: {0}】\r\n{1}", key, new StackTrace(true).Take(5));
                return defaultValue;
            }

            try
            {
                if (typeof(T).IsSubclassOf(typeof(JToken)) || typeof(T) == typeof(JToken))
                    return subJToken.Value<T>();//引用 同jToken.Value<T>(key)
                else
                    return subJToken.ToObject<T>();//非引用
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return defaultValue;
            }
        }

        public static T GetValue<T>(this JToken jToken, string key, T defaultValue = default, bool log = true)
        => Get(jToken, key, defaultValue, log);

        public static JToken GetJToken(this JToken jToken, string key, JToken defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static JObject GetJObject(this JToken jToken, string key, JObject defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static JArray GetJArray(this JToken jToken, string key, JArray defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static string GetString(this JToken jToken, string key, string defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static string[] GetLines(this JToken jToken, string key, string[] defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static bool GetBool(this JToken jToken, string key, bool defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static DateTime GetTime(this JToken jToken, string key, DateTime defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static int GetInt(this JToken jToken, string key, int defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static float GetFloat(this JToken jToken, string key, float defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 取值
        /// </summary>
        public static object GetObject(this JToken jToken, string key, object defaultValue = default, bool log = true)
        => jToken.Get(key, defaultValue, log);

        /// <summary>
        /// 获取长度
        /// </summary>
        public static int Size(this JToken jToken)
        => ((JContainer)jToken).Count;

        /// <summary>
        /// 获取字符串内容
        /// </summary>
        public static string ToNormalString(this JToken jToken)
        => jToken.ToObject<string>();
        #endregion

        /// <summary>
        /// 用于有部分相同结构的类的深复制
        /// </summary>
        public static T Copy<T>(object obj)
        => Deserialize<T>(obj.ToJson(true));
    }
}
