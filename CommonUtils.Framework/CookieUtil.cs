using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CommonUtils
{
    public class CookieUtil
    {
        public static void Set(string key, string value)
        {
            HttpContext.Current.Response.Cookies[key].Value = value;
        }

        public static string Get(string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
                return cookie.Value;
            return null;
        }

        public static void Remove(string key)
        {
            HttpContext.Current.Response.Cookies.Remove(key);
        }
    }
}
