using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    public static class RequestUtil
    {
        public static string GetHttpHead(this HttpRequest request)
        => GetUrlHead(request);

        /// <summary>
        /// Scheme+Host+Path+QueryString
        /// </summary>
        public static string GetUrl(this HttpRequest request)
        => request.Scheme.Append("://", request.Host.Value, request.Path.Value, request.QueryString.Value);

        public static string GetCurrentUrl(this HttpRequest request)
        => GetUrl(request);

        public static string GetUrlHead(this HttpRequest request)
        => request.Scheme + "://" + request.Host;

        public static JObject Info(this HttpRequest request)
        {
            var data = new JObject();
            data.Add("Root", request.UrlRoot());
            data.Add("Url", request.GetUrl());
            data.Add("UrlNoParms", request.UrlNoParms());
            return data;
        }

        /// <summary>
        /// Scheme+Host+Path
        /// </summary>
        public static string UrlNoParms(this HttpRequest request)
        => request.Scheme.Append("://", request.Host.Value, request.Path.Value);

        /// <summary>
        /// Scheme+Host
        /// </summary>
        public static string UrlRoot(this HttpRequest request, params string[] rights)
        => request.Scheme.Append("://", request.Host.Value).Combine(rights);
    }
}
