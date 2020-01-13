using LinqKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// 类扩展
    /// </summary>
    public static partial class Extension
    {
        public static int IndexOf<T>(this T[] items, T item)
        {
            for (int index = 0; index < items.Length; index++)
            {
                if (items[index].Equals(item))
                    return index;
            }
            return -1;
        }

        public static void IndexUp<T>(this T[] items, T item)
        {
            var index = items.IndexOf(item);
            if (index == 0 || index == -1)
                return;

            var temp = items[index - 1];
            items[index - 1] = items[index];
            items[index] = temp;
        }

        public static void IndexDown<T>(this T[] items, T item)
        {
            var index = items.IndexOf(item);
            if (index == items.Length - 1 || index == -1)
                return;

            var temp = items[index + 1];
            items[index + 1] = items[index];
            items[index] = temp;
        }

        #region LinqKit
        public static Expression<Func<T, bool>> And_<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        => expr1.And(expr2);

        public static Expression<Func<T, bool>> True<T>()
        => PredicateBuilder.New<T>();
        #endregion

        /// <summary>
        /// 转换为16进制表现形式
        /// </summary>
        public static string To16String(this int value)
        {
            return "0x" + Convert.ToString(value, 16).ToUpper();
        }

        /// <summary>
        /// 选择堆栈数量
        /// </summary>
        public static string Take(this StackTrace stack, int count)
        {
            var lines = stack.ToString().GetLines();
            return lines.Take(count).JoinToText();
        }

        /// <summary>
        /// 数组截取
        /// </summary>
        public static T[] Take<T>(this IEnumerable<T> array, int skip, int length)
        {
            return array.Skip(skip).Take(length).ToArray();
        }

        #region ObjectAndArray
        /// <summary>
        /// 异或运算
        /// </summary>
        public static bool Xor(this bool value, bool xor)
        {
            return value ^ xor;
        }

        /// <summary>
        /// 同或运算
        /// </summary>
        public static bool Xnor(this bool value, bool xor)
        {
            return value ^ !xor;
        }

        /// <summary>
        /// 获取内存地址
        /// </summary>
        public static string GetMemoryAddress(this object obj)
        {
            GCHandle hander = GCHandle.Alloc(obj);
            IntPtr pin = GCHandle.ToIntPtr(hander);
            return pin.ToString();
        }

        /// <summary>
        /// 用于类的深复制
        /// </summary>
        public static T Copy<T>(T obj)
        {
            return JsonUtil.Deserialize<T>(obj.ToJson(true));
        }

        /// <summary>
        /// 拆箱或者父类转子类(前提是该父类本身就是子类，只是以父类形式存在)
        /// </summary>
        public static T As<T>(this object item)
        {
            return (T)item;
        }

        /// <summary>
        /// 拆箱或者父类转子类(前提是该父类本身就是子类，只是以父类形式存在)
        /// </summary>
        public static IEnumerable<T> As<T>(this IEnumerable<object> array)
        {
            //return array as IEnumerable<T>值为null
            return array.Select(m => (T)m);
        }

        /// <summary>
        /// 相同数据结构不同类之间转换，同种类或者继承类请用As
        /// </summary>
        public static T To<T>(this object item)
        {
            return JsonUtil.Deserialize<T>(item.ToJson(true));
        }

        /// <summary>
        /// 相同数据结构不同类之间转换，同种类或者继承类请用As
        /// </summary>
        public static IEnumerable<T> To<T>(this IEnumerable<object> array)
        {
            return JsonUtil.Deserialize<IEnumerable<T>>(array.ToJson(true));
        }

        /// <summary>
        /// 数组分割
        /// </summary> 
        public static List<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size)
        {
            List<IEnumerable<T>> set = new List<IEnumerable<T>>();
            while (source.Count() > size)
            {
                set.Add(source.Take(size));
                source = source.Skip(size);
            }
            set.Add(source);
            return set;
        }

        /// <summary>
        /// 数据库分页，一定要用在排序之后
        /// </summary>
        public static IQueryable<T> Paged<T>(this IQueryable<T> source, int pageIndex, int pageSize, out int dataAmount) where T : class
        {
            dataAmount = source.Count();
            return source.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        /// <summary>
        /// LambdaTrue
        /// </summary>
        public static Expression<Func<T, bool>> LambdaTrue<T>(this IEnumerable<T> source) where T : class
        {
            return m => true;
        }

        /// <summary>
        /// 数组取值
        /// </summary>  
        public static T Get<T>(this T[] items, int index, T defaultValue = default)
        {
            if (index < items.Length)
                return items[index];
            else
                return defaultValue;

        }
        #endregion

        #region Dictionary key不可为空，value可以为空
        /// <summary>
        /// 删除空
        /// </summary>
        public static void RemoveEmpty(this Dictionary<string, string> map)
        {
            map.RemoveValue("", null);
        }

        /// <summary>
        /// 判空取值
        /// </summary>
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, TValue defaultValue = default, bool log = true)
        {
            if (key != null && map.ContainsKey(key))
                return map[key];
            if (log)
                LogUtil.Log(string.Format("Dictionary尝试获取不存在的Key：{0}\r\n{1}", key, new StackTrace(true).Take(5)));
            return defaultValue;
        }

        /// <summary>
        /// 按值取键
        /// </summary>
        public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> map, TValue value)
        {
            foreach (var pair in map)
                if (pair.Value.Equals(value))
                    return pair.Key;
            return default;
        }

        /// <summary>
        /// 赋值,默认覆盖，可选保留
        /// </summary>
        public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, TValue value, bool replace = true)
        {
            if (key == null)
                return;

            if (!replace && map.ContainsKey(key))
                return;

            map[key] = value;
        }

        /// <summary>
        /// 填值
        /// </summary>
        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> map, Dictionary<TKey, TValue> otherMap)
        {
            foreach (var item in otherMap)
                map.Add(item.Key, item.Value);
        }

        /// <summary>
        /// 根据Value删除项
        /// </summary>
        public static void RemoveValue<TKey, TValue>(this Dictionary<TKey, TValue> map, params TValue[] values)
        {
            List<TKey> keys = new List<TKey>();
            foreach (var value in values)
            {
                if (map.ContainsValue(value))
                {
                    foreach (var pair in map)
                    {
                        if (pair.Value.Equals(value))
                            keys.Add(pair.Key);
                    }
                }
            }
            foreach (TKey key in keys)
                map.Remove(key);
        }

        /// <summary>
        /// 根据Keys删除项
        /// </summary>
        public static void Remove<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey[] keys)
        {
            foreach (var key in keys)
                map.Remove(key);
        }

        /// <summary>
        /// 根据Keys筛选项
        /// </summary>
        public static void Select<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey[] keys)
        {
            var allKeys = map.Keys.ToArray();
            foreach (var key in allKeys)
                if (!keys.Contains(key))
                    map.Remove(key);
        }

        /// <summary>
        /// 排序
        /// </summary>
        public static Dictionary<TKey, TValue> GetNewSortByKey<TKey, TValue>(this Dictionary<TKey, TValue> map)
        => map.OrderBy(m => m.Key).ToDictionary(m => m.Key, m => m.Value);

        /// <summary>
        /// 排序
        /// </sum
        public static void SortByKey<TKey, TValue>(this Dictionary<TKey, TValue> map)
        {
            var sortedMap = map.GetNewSortByKey();
            map.Clear();
            foreach (var keyValue in sortedMap)
                map.Add(keyValue.Key, keyValue.Value);
        }

        #endregion;

        #region IDataReader
        /// <summary>
        /// 获取object
        /// </summary>
        public static object Get(this IDataReader reader, string columnName)
        {
            try
            {
                return reader[columnName];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取String
        /// </summary>
        public static string GetString(this IDataReader reader, string columnName)
        {
            return reader.Get(columnName).ToString();
        }

        /// <summary>
        /// 获取Int
        /// </summary>
        public static int GetInt(this IDataReader reader, string columnName)
        {
            return reader.Get(columnName).ToInt();
        }

        /// <summary>
        /// 获取Long
        /// </summary>
        public static long GetLong(this IDataReader reader, string columnName)
        {
            return reader.Get(columnName).ToLong();
        }

        /// <summary>
        /// 获取Bool
        /// </summary>
        public static bool GetBool(this IDataReader reader, string columnName)
        {
            return reader.Get(columnName).ToBool();
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        public static DateTime GetTime(this IDataReader reader, string columnName)
        {
            return reader.Get(columnName).ToTime();
        }

        /// <summary>
        /// 获取枚举
        /// </summary>
        public static T GetEnum<T>(this IDataReader reader, string columnName)
        {
            return reader.Get(columnName).ToEnum<T>();
        }
        #endregion

        #region WebRequest WebResponse
        /// <summary>
        /// WebRequest添加文件
        /// </summary>
        /// <param name="request"></param>
        /// <param name="path"></param>
        public static void AddFile(this WebRequest request, string path)
        {
            Stream requestStream = request.GetRequestStream();
            var fileStream = File.OpenRead(path);
            fileStream.CopyTo(requestStream);
            fileStream.Close();
            requestStream.Close();
        }

        /// <summary>
        /// WebRequest添加文本
        /// </summary>
        /// <param name="request"></param>
        /// <param name="path"></param>
        public static void AddText(this WebRequest request, string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            Stream requestStream = request.GetRequestStream();
            StreamWriter sw = new StreamWriter(requestStream, encoding);
            sw.Write(text);
            sw.Close();
            requestStream.Close();
        }

        /// <summary>
        /// WebRequest添加二进制
        /// </summary>
        public static void AddBytes(this WebRequest request, byte[] bytes)
        {
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
        }

        /// <summary>
        /// WebRequest添加内存流
        /// </summary>
        public static void AddStream(this WebRequest request, Stream stream)
        {
            Stream requestStream = request.GetRequestStream();
            stream.CopyTo(requestStream);
            stream.Close();
            requestStream.Close();
        }

        /// <summary>
        /// WebResponse获取文件
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static void SaveFile(this WebResponse response, string path)
        {
            var stream = response.GetResponseStream();
            var fileStream = File.Create(path);
            stream.CopyTo(fileStream);
            fileStream.Close();
            stream.Close();
        }

        /// <summary>
        /// WebResponse获取文本
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GetText(this WebResponse response, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            var stream = response.GetResponseStream();
            StreamReader sd = new StreamReader(stream, encoding);
            string res = sd.ReadToEnd();
            sd.Close();
            stream.Close();
            return res;
        }
        #endregion

        #region Hashtable 无序Map
        public static void Add(this Hashtable hashtable, Hashtable keysValues)
        {
            foreach (var key in keysValues.Keys)
                hashtable.Add(key, hashtable[key]);
        }

        public static object Get(this Hashtable hashtable, object key, bool log = true)
        {
            if (hashtable.Contains(key))
                return hashtable[key];
            if (log)
                LogUtil.LogWithStackTrace("HashtableException 尝试获取不存在的Key:" + key);
            return null;
        }

        public static KeyValuePair<object, object>[] KeyValuePairs(this Hashtable hashtable)
        {
            var pairs = new List<KeyValuePair<object, object>>();
            foreach (var key in hashtable.Keys)
                pairs.Add(new KeyValuePair<object, object>(key, hashtable[key]));
            return pairs.ToArray();
        }

        public static MapStringString ToMap(this Hashtable hashtable)
        {
            var map = new MapStringString();
            foreach (var key in hashtable.Keys)
                map.Add(key.ToString(), hashtable[key].ToString());
            return map;
        }

        public static MapStringString Sort(this Hashtable hashtable)
        {
            var keys = new List<string>();
            foreach (var key in hashtable.Keys)
                keys.Add(key.ToString());
            keys = keys.OrderBy(m => m).ToList();
            MapStringString map = new MapStringString();
            foreach (var key in keys)
                map.Add(key, hashtable[key].ToString());
            return map;
        }
        #endregion
    }
}
