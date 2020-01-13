using System;
using System.Diagnostics;

namespace CommonUtils
{
    /// <summary>
    /// Url工具
    /// </summary>
    public static class UrlUtil
    {
        /// <summary>
        /// URL拼接
        /// </summary>
        public static string Combine(string left, params string[] rights)
        => left.Combine(rights);

        /// <summary>
        /// 格式化Url,http检查:如果有http返回原值,否者添加http://
        /// </summary>
        public static string Format(string url)
        {
            url = url.Trim();
            if (url.Length < 4)
                return "http://" + url;
            if (url.Substring(0, 4) == "http")
                return url;
            return "http://" + url;
        }

        /// <summary>
        /// 获取url路径
        /// </summary>
        public static string GetPath(string url)
        {
            var uri = new Uri(url);
            return uri.LocalPath;
        }

        /// <summary>
        /// 获取协议名
        /// </summary>
        public static string GetScheme(string url)
        {
            var uri = new Uri(url);
            return uri.Scheme;
        }

        /// <summary>
        /// URL父级
        /// </summary>
        public static string Parent(string url)
        => url.Substring(0, url.LastIndexOf('/') + 1);

        /// <summary>
        /// 替换头部
        /// </summary>
        public static string ReplaceHead(string url, string head)
        {
            Uri uri1 = new Uri(url);
            Uri uri2 = new Uri(head);
            return string.Format("{0}://{1}{2}", uri2.Scheme, uri2.Authority, uri1.PathAndQuery);
        }

        /// <summary>
        /// URL根路径
        /// </summary>
        public static string Root(string url)
        {
            Uri uri = new Uri(url);
            return string.Format("{0}://{1}/", uri.Scheme, uri.Authority);
        }

        public static void Open(string url)
        => Process.Start(url);
    }
}
