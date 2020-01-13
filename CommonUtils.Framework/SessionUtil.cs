using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace CommonUtils
{
    /// <summary>
    /// 网页会话管理
    /// </summary>
    public static class SessionUtil
    {
        /// <summary>
        /// 当前的Session
        /// </summary>
        public static HttpSessionState Session
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        /// <summary>
        /// 登录状态
        /// </summary>
        public static bool IsLogin
        {
            get
            {
                return Session["login"] != null;
            }
        }

        /// <summary>
        /// 登录状态
        /// </summary>
        public static void SetLogin(bool result = true)
        {
            Session["login"] = result ? "login" : null;
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        public static bool Contains(string key)
        {
            foreach (string sKey in Session.Keys)
            {
                if (sKey == key)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 添加/变更
        /// </summary>
        public static void Add(string key, object value)
        {
            Set(key, value);
        }

        /// <summary>
        /// 添加/变更
        /// </summary>
        public static void Set(string key, object value)
        {
            Session[key] = value;
        }

        /// <summary>
        /// 获取
        /// </summary>
        public static object Get(string key)
        {
            try
            {
                return Session[key];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        public static T Get<T>(string key)
        {
            object obj = Get(key);
            if (obj == null)
                return default(T);
            else
                return (T)obj;
        }

        /// <summary>
        /// 获取
        /// </summary>
        public static int GetInt(string key)
        {
            return Get(key).ToInt();
        }

        /// <summary>
        /// 获取
        /// </summary>
        public static string GetString(string key)
        {
            return Get(key).ToString();
        }

        /// <summary>
        /// 删除
        /// </summary>
        public static void Remove(string key)
        {
            Session.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public static void Clear()
        {
            Session.Clear();
        }
    }
}
