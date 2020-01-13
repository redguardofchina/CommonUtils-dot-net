using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace CommonUtils
{
    /// <summary>
    /// 页面相关接口
    /// </summary>
    public static class RequestUtil
    {
        public static string Root(this HttpRequest request, string path = null)
        {
            return request.Url.Scheme.Append("://", request.Url.Host).Combine(path);
        }

        public static string Url(this HttpRequest request)
        {
            return request.RawUrl;
        }

        public static string UrlNoParms(this HttpRequest request)
        {
            return request.Url.Scheme.Append("://", request.Url.Host, request.Path);
        }

        public static JObject Info(this HttpRequest request)
        {
            var data = new JObject();
            data.Add("Root", request.Root());
            data.Add("Url", request.Url());
            data.Add("UrlNoParms", request.UrlNoParms());
            return data;
        }

        /// <summary>
        /// 当前的Request
        /// </summary>
        public static HttpRequest Current
        {
            get
            {
                try
                {
                    return HttpContext.Current.Request;
                }
                catch
                {
                    return new HttpRequest("empty string filename", "http://empty/string/url", "empty string queryString");
                }
            }
        }

        /// <summary>
        /// 重写访问地址
        /// </summary>
        public static void RewritePath(string url)
        {
            HttpContext.Current.RewritePath(url);
        }

        /// <summary>
        /// 分页索引
        /// </summary>
        public static int PagingIndex
        {
            get
            {
                int index = ParamInt("page");
                if (index < 1)
                    return 1;
                return index;
            }
        }

        /// <summary>
        /// 请求中的文件
        /// </summary>
        public static HttpFileCollection Files
        {
            get
            {
                return Current.Files;
            }
        }

        /// <summary>
        /// 获取post数据
        /// </summary>
        public static string GetString()
        {
            return GetPostData();
        }

        /// <summary>
        /// 获取post数据
        /// </summary>
        public static string GetPostData()
        {
            return Current.InputStream.GetString();
        }

        /// <summary>
        /// 访问路径,不带域名
        /// </summary>
        public static string RawUrl
        {
            get
            {
                return Current.RawUrl;
            }
        }

        #region Params
        /// <summary>
        /// 获取参数,Key不区分大小写
        /// </summary>
        public static string Param(string key)
        {
            return Current.Params[key];
        }

        /// <summary>
        /// 所有参数,Key不区分大小写
        /// </summary>
        public static NameValueCollection Params()
        {
            return Current.Params;
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        public static int ParamInt(string key, int defaultValue = 0)
        {
            return ConvertUtil.ToInt(Param(key), defaultValue);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        public static Enum ParamEnum<Enum>(string key)
        {
            return ConvertUtil.ToEnum<Enum>(Param(key));
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        public static long ParamLong(string key)
        {
            return ConvertUtil.ToLong(Param(key));
        }

        #endregion

        /// <summary>
        /// 获取网页根目录,用RawUrl判断,与Action有关
        /// </summary>
        public static string GetTreeRoot(string urlPath = "")
        {
            if (string.IsNullOrEmpty(urlPath))
            {
                urlPath = Current.RawUrl;
            }
            if (urlPath.Length == 1)
            {
                return "";
            }
            int index = urlPath.IndexOf('/', 1);
            if (index > 0)
            {
                urlPath = urlPath.Substring(0, index + 1);
            }
            return urlPath;
        }

        /// <summary>
        /// 获取网页最终节点,用RawUrl判断,与Action有关
        /// </summary>
        public static string GetTreeNode(string urlPath = "")
        {
            if (string.IsNullOrEmpty(urlPath))
            {
                urlPath = Current.RawUrl;
            }
            if (urlPath.Length == 1)
            {
                return "";
            }
            int index = urlPath.LastIndexOf('?');
            if (index > 0)
            {
                urlPath = urlPath.Substring(0, index);
            }
            index = urlPath.LastIndexOf('/');
            if (index > 0)
            {
                urlPath = urlPath.Substring(index + 1);
            }
            return urlPath;
        }

        /// <summary>
        /// 获取服务器的域名系统 (DNS) 主机名或 IP 地址和端口号。
        /// </summary>
        public static string Authority
        {
            get
            {
                return Current.Url.Authority;
            }
        }

        /// <summary>
        /// 获取虚拟路径
        /// </summary>
        public static string GetVirPath()
        {
            return Current.Url.LocalPath;
        }

        /// <summary>
        /// 获取页面文件名
        /// </summary>
        public static string GetPathName()
        {
            string url = GetUrl();
            return Path.GetFileName(url);
        }

        /// <summary>
        /// 获取页面文件名
        /// </summary>
        public static string GetPageName()
        {
            string url = GetUrl();
            return Path.GetFileNameWithoutExtension(url);
        }

        /// <summary>
        /// 获取页面路径名
        /// </summary>
        public static string GetPagePath()
        {
            return Current.RawUrl;
        }

        /// <summary>
        /// 获取页面文件名
        /// </summary>
        public static string GetPageNameWithoutExtention()
        {
            string url = GetUrl();
            return Path.GetFileNameWithoutExtension(url);
        }

        /// <summary>
        /// 获取资源完整域名路径
        /// </summary>
        public static string GetUrl(string append)
        {
            return GetDirUrl(append);
        }

        /// <summary>
        /// 获取网站完整域名路径
        /// </summary>
        public static string GetTopUrl(string virPath = "")
        {
            string str = Current.Url.Scheme + "://" + Current.Url.Authority + "/";
            return str + virPath;
        }

        /// <summary>
        /// 获取网站头部域名路径
        /// </summary>
        public static string HeadUrl
        {
            get
            {
                string localPath = GetVirPath();
                string url = GetUrl();
                return url.Replace(localPath, "/");
            }
        }

        /// <summary>
        /// 获取页面位置
        /// </summary>
        public static string GetRootUrl(string append = "")
        {
            if (append.ToLower().Contains("http"))
            {
                return append;
            }
            return Current.Url.AbsoluteUri + append;
        }

        /// <summary>
        /// 获取页面位置
        /// </summary>
        public static string GetDirUrl(string append = "")
        {
            string pageFileName = GetPageName();
            string url = GetUrl();
            if (!string.IsNullOrEmpty(pageFileName))
            {
                url = url.Replace(pageFileName, "");
            }
            return url + append;
        }

        /// <summary>
        /// 获取网站完整域名路径
        /// </summary>
        public static string GetUrl()
        {
            return ClientUrl;
        }

        /// <summary>
        /// 获取网站完整域名路径
        /// </summary>
        public static string ClientUrl
        {
            get
            {
                return Current.Url.Scheme + "://" + Current.Url.Authority + Current.RawUrl;
            }
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        public static string ClientIp
        {
            get
            {
                return Current.UserHostAddress;
            }
        }

        /// <summary>
        /// 获取客户端平台信息
        /// </summary>
        public static string ClientBrowser
        {
            get
            {
                return Current.UserAgent;
            }
        }

        /// <summary>
        /// 获取客户端平台信息
        /// </summary>
        public static string ClientInfo
        {
            get
            {
                string info = "IP信息:" + ClientIp;
                info += "<br />浏览器信息:" + ClientBrowser;
                info += "<br />地址信息:" + ClientUrl;
                info += "<br />时间信息：" + DateTime.Now;
                return info;
            }
        }

        /// <summary>
        /// 获取网站服务器信息
        /// </summary>
        public static string ServerInfo
        {
            get
            {
                StringBuilder info = new StringBuilder();
                info.AppendLine("Content:ServerInfo");
                for (int index = 0; index < Current.ServerVariables.Count; index++)
                    info.AppendLine(Current.ServerVariables.GetKey(index) + ":" + Current.ServerVariables.Get(index));
                return info.Html();
            }
        }


        /// <summary>
        /// 获取物理路径
        /// </summary>
        public static string GetPath()
        {
            return HttpContext.Current.Server.MapPath("");
        }

        /// <summary>
        /// 获取物理路径
        /// </summary>
        public static string GetPath(string virPath)
        {
            virPath = StringUtil_.GetFront(virPath, "?");
            return HttpContext.Current.Server.MapPath(virPath);
        }

        /// <summary>
        /// 获取网站根路径
        /// </summary>
        public static string GetRootPath()
        {
            return Current.Url.AbsoluteUri;
        }

        /// <summary>
        /// 获取网站上层路径
        /// </summary>
        public static string GetRootFatherPath()
        {
            string webPath = GetRootPath();
            string tempPath = Path.GetDirectoryName(webPath);
            tempPath = Path.GetDirectoryName(tempPath);
            return tempPath + "\\";
        }

        /// <summary>
        /// 获取网站上层路径
        /// </summary>
        public static string GetRootFatherPath(string addPath)
        {
            return GetRootFatherPath() + addPath;
        }

        /// <summary>
        /// 通过域名和端口获取合法标识
        /// </summary>
        public static string GetMarkByHostAndPort()
        {
            string mark = Current.Url.Scheme + Current.Url.Host.Replace(".", "") + Current.Url.Port;
            mark = mark.ToUpper();
            return mark;
        }

        /// <summary>
        /// 通过访问路径获取合法标识
        /// </summary>
        public static string MarkPath
        {
            get
            {
                return RawUrl.GetLettersAndDigits();
            }
        }

        /// <summary>
        /// 通过访问URL获取合法标识
        /// </summary>
        public static string MarkUrl
        {
            get
            {
                return ClientUrl.GetLettersAndDigits();
            }
        }
    }
}
