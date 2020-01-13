using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    /// <summary>
    /// Asp.Net Core Session
    /// (ActionContext)context.HttpContext.Session
    /// (ControllerBase)controller.HttpContext.Session
    /// </summary>
    public static class SessionUtil
    {
        public static void Set(this ISession session, string key, string value)
        {
            session.Set(key, value.GetBytes());
        }

        public static string Get(this ISession session, string key, string value)
        {
            return session.Get(key).Decode();
        }

        public static void Login(this ISession session, string sign)
        {
            session.Set("login", sign.GetBytes());
        }

        public static bool IsLogin(this ISession session)
        {
            return session.Get("login") != null;
        }

        public static string LoginInfo(this ISession session)
        {
            return session.Get("login").Decode();
        }
    }
}
