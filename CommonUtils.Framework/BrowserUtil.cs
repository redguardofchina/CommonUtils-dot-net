using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonUtils
{
    /// <summary>
    /// 模拟浏览器相关接口,应用限制:windows, Page AspCompat="true" ,多线程支持不好
    /// </summary>
    public class BrowserUtil
    {
        /// <summary>
        /// 获取网页标题
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetTitle(string url)
        {
            HtmlDocument doc = GetHtmlDocument(url);
            if (doc == null)
            {
                return "";
            }
            return doc.Title;
        }

        /// <summary>
        /// 获取网页BodyHtml
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetBodyHtml(string url)
        {
            HtmlDocument doc = GetHtmlDocument(url);
            if (doc == null)
            {
                return "";
            }
            return doc.Body.OuterHtml;
        }

        /// <summary>
        /// 获取网页BodyHtml
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetBodyHtml(string url, int waitSeconds)
        {
            HtmlDocument doc = GetHtmlDocument(url, waitSeconds);
            if (doc == null)
            {
                return "";
            }
            return doc.Body.OuterHtml;
        }

        /// <summary>
        /// 获取网页BodyHtml
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetBodyHtml(string url, string keyword)
        {
            HtmlDocument doc = GetHtmlDocument(url, keyword);
            if (doc == null)
            {
                return "";
            }
            return doc.Body.OuterHtml;
        }

        /// <summary>
        /// 获取网页BodyHtml
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetBodyHtml(string url, string keyword, int timeOutSecond)
        {
            HtmlDocument doc = GetHtmlDocument(url, keyword, timeOutSecond);
            if (doc == null)
            {
                return "";
            }
            return doc.Body.OuterHtml;
        }

        /// <summary>
        /// 获取网页BodyHtml
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string[] GetBodyHtmls(string[] urls, string keyword)
        {
            HtmlDocument[] docs = GetHtmlDocuments(urls, keyword);
            return docs.Select(m => m.Body.OuterHtml).ToArray();
        }

        /// <summary>
        /// 模拟浏览器获取网页HTML文档
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HtmlDocument GetHtmlDocument(string url)
        {
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.Navigate(url);
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            HtmlDocument doc = browser.Document;
            browser.Dispose();
            return doc;
        }

        /// <summary>
        /// 模拟浏览器获取网页HTML文档
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HtmlDocument GetHtmlDocument(string url, int waitSecond)
        {
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.Navigate(url);
            DateTime dt1 = DateTime.Now;
            DateTime dt2 = DateTime.Now;
            while (browser.ReadyState != WebBrowserReadyState.Complete || (dt2 - dt1).TotalSeconds < waitSecond)
            {
                Application.DoEvents();
                dt2 = DateTime.Now;
            }
            HtmlDocument doc = browser.Document;
            browser.Dispose();
            return doc;
        }

        /// <summary>
        /// 模拟浏览器获取网页HTML文档
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HtmlDocument GetHtmlDocument(string url, string keyword)
        {
            return GetHtmlDocument(url, keyword, 30);
        }

        /// <summary>
        /// 模拟浏览器获取网页HTML文档
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HtmlDocument GetHtmlDocument(string url, string keyword, int timeOutSecond)
        {
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.Navigate(url);
            DateTime dt1 = DateTime.Now;
            DateTime dt2 = DateTime.Now;
            while (browser.ReadyState != WebBrowserReadyState.Complete || (!browser.Document.Body.OuterHtml.Contains(keyword) && (dt2 - dt1).TotalSeconds < timeOutSecond))
            {
                Application.DoEvents();
                dt2 = DateTime.Now;
            }
            HtmlDocument doc = browser.Document;
            browser.Dispose();
            return doc;
        }

        /// <summary>
        /// 模拟浏览器获取网页HTML文档
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HtmlDocument[] GetHtmlDocuments(string[] urls, string keyword)
        {
            List<WebBrowser> listBrowser = new List<WebBrowser>();
            foreach (string url in urls)
            {
                WebBrowser browser = new WebBrowser();
                browser.ScriptErrorsSuppressed = true;
                browser.Navigate(url);
                listBrowser.Add(browser);
            }
            List<HtmlDocument> listDoc = new List<HtmlDocument>();
            foreach (WebBrowser browser in listBrowser)
            {
                if (browser.ReadyState != WebBrowserReadyState.Complete)
                {

                }
            }
            foreach (WebBrowser browser in listBrowser)
            {
                while (browser.ReadyState != WebBrowserReadyState.Complete || !browser.Document.Body.OuterHtml.Contains(keyword))
                {
                    Application.DoEvents();
                }
                HtmlDocument doc = browser.Document;
                browser.Dispose();
                listDoc.Add(doc);
            }
            return listDoc.ToArray();
        }
    }
}
