using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// Asp.Net Core Cookie
    /// </summary>
    public static class CookieUtil
    {
        /// <summary>
        /// (HttpContext)context.Response.Cookies
        /// </summary>
        public static void Set(this IResponseCookies cookies, string key, string value)
        {
            cookies.Append(key, value, new CookieOptions { Expires = DateTime.Now.AddMonths(1) });
        }

        public static void SetAccount(this IResponseCookies cookies, string value)
        {
            cookies.Set("Account", value);
        }

        public static void SetPassword(this IResponseCookies cookies, string value)
        {
            cookies.Set("Password", value);
        }

        public static void ClearPassword(this IResponseCookies cookies)
        {
            cookies.Delete("Password");
        }

        /// <summary>
        /// (HttpContext)context.Request.Cookies
        /// </summary>
        public static string Get(this IRequestCookieCollection cookies, string key)
        {
            cookies.TryGetValue(key, out string value);
            return value;
        }

        public static string GetAccount(this IRequestCookieCollection cookies)
        {
            return cookies.Get("Account");
        }

        public static string GetPassword(this IRequestCookieCollection cookies)
        {
            return cookies.Get("Password");
        }
    }
}
