using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonUtils
{
    /// <summary>
    /// Html处理
    /// </summary>
    public static class HtmlUtil
    {
        /// <summary>
        /// 换行转为br
        /// </summary>
        public static string ToHtml(this string text)
        {
            string html = text.Replace(" ", "&nbsp;");
            html = html.Replace("\r\n", "<br />");
            html = html.Replace("\n", "<br />");
            return html;
        }

        /// <summary>
        /// 换行转为br
        /// </summary>
        public static string ToHtml(this string[] lines)
        {
            return lines.JoinToText().ToHtml();
        }

        /// <summary>
        /// 换行转为br
        /// </summary>
        public static string Html(this StringBuilder sb)
        {
            return sb.ToString().ToHtml();
        }

        /// <summary>
        /// 过滤html代码,保留文字字符
        /// </summary>
        public static string FilterTag(string html)
        {
            html = html.Trim();
            return string.IsNullOrEmpty(html) ? string.Empty : Regex.Replace(html, "<[^>]*>", string.Empty);
        }

        /// <summary>
        /// 获取用于显示的html代码
        /// </summary> 
        public static string GetDisplayXml(string html)
        {
            return html.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        /// <summary>
        /// <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        /// </summary>
        public static string CharsetMeta { get; } = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />";

        /// <summary>
        /// ViewPort
        /// </summary>
        public static string ViewPortMeta { get; } = "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />";

        /// <summary>
        /// 网页重定位,尽量填写/绝对路径 <meta http-equiv="refresh" content="0;url=/test" />
        /// </summary>
        public static string RelocalMeta(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            return string.Format("<meta http-equiv=\"refresh\" content=\"0;url={0}\" />", url);
        }

        /// <summary>
        /// ICON
        /// </summary>
        public static string IconLink { get; } = "<link href=\"/favicon.ico\" mce_href=\"favicon.ico\" rel=\"shortcut icon\" type=\"image/x-icon\" />";

        /// <summary>
        /// 把字符串转换为UnicodeHtml
        /// </summary>
        public static string CompletePage(string innerBoday)
        {
            var html = "<!DOCTYPE html><html><head>{0}</head><body>{1}</body></html>";
            return string.Format(html, CharsetMeta, innerBoday);
        }

        /// <summary>
        /// 添加htmlA标签
        /// </summary> 
        public static string Href(string url, bool blank = true, string content = null)
        {
            url = UrlUtil.Format(url);
            return string.Format("<a href=\"{0}\"{1}>{2}</a>", url, blank ? " target=\"_blank\"" : null, content);
        }

        /// <summary>
        /// 添加htmlA标签
        /// </summary> 
        public static string Href(string[] urls, bool blank = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string url in urls)
                sb.Append(string.Format("<a href=\"{0}\"{1}>{0}</a><hr />", url, blank ? " target=\"_blank\"" : null));
            return sb.ToString();
        }

        /// <summary>
        /// 邮件链接
        /// </summary>
        public static string MailHref(string mail, string content = null)
        {
            if (string.IsNullOrEmpty(content))
                content = mail;
            string html = "<a href=\"mailto:{0}\">{1}</a>";
            return string.Format(html, mail, content);
        }

        /// <summary>
        /// QQ链接
        /// </summary>
        public static string QQUrl(string qq)
        {
            return "tencent://message/?uin=" + qq;
        }

        /// <summary>
        /// QQ链接
        /// </summary>
        public static string QQHref(string qq, string content = null)
        {
            if (string.IsNullOrEmpty(content))
                content = qq;
            return string.Format("<a href=\"{0}\">{1}</a>", QQUrl(qq), content);
        }

        /// <summary>
        /// 网页背景音乐
        /// </summary>
        public static string Audio(string virPath = "/audio/bgm.mp3", bool display = true, bool autoplay = true)
        {
            /*
            <audio autoplay="autoplay" controls="controls" loop="loop" preload="auto" src="assets/Log-in-long2.wav" style="position:fixed;left:0;top:0">
            <source src="assets/Log-in-long2.wav" />
            Your browser does not support the audio element.
            </audio>
            */
            var sb = new StringBuilder();
            sb.AppendFormat("<audio {2}controls=\"controls\" loop=\"loop\" preload=\"auto\" src=\"{0}\" style=\"position:fixed;left:0;top:0;{1}\">", virPath, display ? null : "display:none", autoplay ? "autoplay=\"autoplay\" " : null);
            sb.AppendFormat("<source src=\"{0}\" />", virPath);
            sb.Append("Your browser does not support the audio element.");
            sb.Append("</audio>");
            return sb.ToString();
        }

        /// <summary>
        /// 关闭网页
        /// </summary>
        public static string CloseScript { get; } = "<script>window.close();</script><button onclick=\"javascript:window.close();\">close</button>";

        /// <summary>
        /// 通过数组获取Table的Html代码
        /// </summary>
        public static string GetTable<T>(T[] array, string css)
        {
            var html = new StringBuilder();
            html.AppendFormatLine("<table class=\"{0}\">", css);
            var properties = typeof(T).GetProperties();
            html.Append("<tr>");
            foreach (var property in properties)
            {
                html.Append("<th>");
                html.Append(property.Name);
                html.Append("</th>");
            }
            html.AppendLine("</tr>");
            foreach (var item in array)
            {
                html.Append("<tr>");
                foreach (var property in properties)
                {
                    html.Append("<td>");
                    html.Append(property.GetValue(item));
                    html.Append("</td>");
                }
                html.AppendLine("</tr>");
            }
            html.Append("</table>");
            return html.ToString();
        }
    }
}
