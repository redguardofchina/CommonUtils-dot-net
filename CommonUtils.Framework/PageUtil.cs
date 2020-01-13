using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace CommonUtils
{
    /// <summary>
    /// APS.NET页面相关类
    /// </summary>
    public class PageUtil
    {
        /// <summary>
        /// 添加标题
        /// </summary>
        /// <param name="title"></param>
        public static void AppendTitle(string title)
        {
            System.Web.UI.Page page = ((System.Web.UI.Page)HttpContext.Current.CurrentHandler);
            page.Title = title + "-" + page.Title;
        }

        /// <summary>
        /// 将form的action换成绝对路径
        /// </summary>
        public static void FormatForm()
        {
            ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Form.Action = RequestUtil.GetVirPath();
        }

        /// <summary>
        /// 获取检验过的页码序号
        /// </summary>
        public static int GetRevisedPageIndex(int pageIndex, int pageAmount)
        {
            if (pageIndex < 1)
            {
                return 1;
            }
            if (pageIndex > pageAmount)
            {
                return pageAmount;
            }
            return pageIndex;
        }

        /// <summary>
        /// 获取分页数量
        /// </summary>
        public static int GetPageAmount(int dataAmount, int pageSize)
        {
            return (dataAmount - 1) / pageSize + 1;
        }

        /// <summary>
        /// Url编码
        /// </summary>
        public static string GetUrlEncode(string url)
        {
            return HttpContext.Current.Server.UrlEncode(url);
        }

        /// <summary>
        /// Url解码
        /// </summary>
        public static string GetUrlDecode(string url)
        {
            return HttpContext.Current.Server.UrlDecode(url);
        }

        /// <summary>
        /// JS脚本提示浏览器信息
        /// </summary>
        public static void Alert()
        {
            ((Page)HttpContext.Current.Handler).ClientScript.RegisterClientScriptBlock(typeof(Page), string.Empty, AlertScript());
        }

        /// <summary>
        /// JS脚本提示消息
        /// </summary>
        public static void Alert(string msg)
        {
            ((Page)HttpContext.Current.Handler).ClientScript.RegisterClientScriptBlock(typeof(Page), string.Empty, AlertScript(msg));
        }

        /// <summary>
        /// JS脚本提示消息并跳转
        /// </summary>
        public static void Alert(string msg, string url)
        {
            ((Page)HttpContext.Current.Handler).ClientScript.RegisterClientScriptBlock(typeof(Page), string.Empty, AlertScript(msg, url));
        }

        /// <summary>
        /// 获取提示浏览器信息的JS脚本
        /// </summary>
        public static string AlertScript()
        {
            return "<script type=\"text/javascript\">alert(navigator.appVersion);</script>";
        }

        /// <summary>
        /// 获取弹出消息的JS脚本
        /// </summary>
        public static string AlertScript(string msg)
        {
            return String.Format("<script type=\"text/javascript\">alert(\"{0}\");</script>", msg);
        }

        /// <summary>
        /// 获取弹出消息的JS脚本
        /// </summary>
        public static string AlertScript(string msg, string url)
        {
            return String.Format("<script type=\"text/javascript\">alert(\"{0}\");top.window.location.href = \"{1}\";</script>", msg, url);
        }

        /// <summary>
        /// 显示自定义消息框
        /// </summary>
        public static void Write(string msg)
        {
            HttpContext.Current.Response.Write(msg + "<br />");
        }

        /// <summary>
        /// 显示自定义消息框
        /// </summary>
        public static void Message(string msg, string url = "")
        {
            string rehrefScript = "";
            if (!string.IsNullOrEmpty(url))
            {
                rehrefScript = "top.window.location.href = \"" + url + "\";";
            }
            ((Page)HttpContext.Current.Handler).ClientScript.RegisterClientScriptBlock(typeof(Page), string.Empty, "<script type=\"text/javascript\"> function HiddenDiv() { var obj1 = document.getElementById(\"divMessage\"); obj1.style.display = \"none\"; " + rehrefScript + "} var obj2 = document.getElementsByTagName(\"body\")[0]; obj2.onkeypress = function () { if (window.event.keyCode == 13) { HiddenDiv(); } }; </script> <div id=\"divMessage\" style=\"border-radius: 21px; background-color: #efefef; width: 353px; height: 201px; position: fixed; top: 39%; left: 50%; margin-top: -100px; margin-left: -176px;\"> <div style=\"font-size: 35px; height: 50px; line-height: 50px; font-family: Microsoft YaHei; text-align: center; margin-top: 30px; color: #999999; font-weight: 700;\">提&nbsp;&nbsp;示 </div> <div style=\"font-size: 14px; height: 50px; font-family: Microsoft YaHei; text-align: center; color: #7b7b7b; text-wrap: normal;\"> <table width=\"100%\" height=\"100%\"> <tr> <td align=\"center\">" + msg + "</td> </tr> </table> </div> <div style=\"width: 100%; height: 71px; text-align: center; vertical-align: middle;\"> <input type=\"button\" value=\"确&nbsp;&nbsp;定\" onclick='javascript: HiddenDiv();' style=\"width: 134px; height: 32px; line-height: 32px; border-style: none; cursor: pointer; border-radius: 5px; font-size: 17px; font-family: Microsoft YaHei; background-color: #28b69a; color: #FFFFFF; text-align: center; display: inline-block; margin-top: 19px;\" /> </div> </div>");
        }
        /// <summary>
        /// JS脚本跳转页面
        /// </summary>
        public static void Redirect(string url)
        {
            ((Page)HttpContext.Current.Handler).ClientScript.RegisterClientScriptBlock(typeof(Page), string.Empty, RedirectScript(url));
        }

        /// <summary>
        /// 获取页面跳转的JS脚本
        /// </summary>
        public static string RedirectScript(string url)
        {
            return String.Format(" <script type=\"text/javascript\">top.window.location.href = \"{0}\";</script>", url);
        }

        /// <summary>
        /// 网址转换成href
        /// </summary>
        public static string WebHref(string webUrl)
        {
            return WebHref(webUrl, webUrl);
        }

        /// <summary>
        /// 建立一个WebHref
        /// </summary>
        public static string WebHref(string url, string name)
        {
            return String.Format("<a Title=\"打开网站:{0}\" href = \"{0}\" target=\"_blank\">{1}</a>", UrlUtil.Format(url), name);
        }

        /// <summary>
        /// 建立一个SelfHref
        /// </summary>
        public static string SelfHref(string url, string name)
        {
            return String.Format("<a href = \"{0}\" target=\"_self\">{1}</a>", url, name);
        }

        /// <summary>
        /// 建立一个BlankHref
        /// </summary>
        public static string BlankHref(string url, string name)
        {
            return String.Format("<a href = \"{0}\" target=\"_blank\">{1}</a>", url, name);
        }

        /// <summary>
        /// 邮箱转换成href
        /// </summary>
        public static string MailHref(string mail)
        {
            return String.Format("<a Title=\"发送电子邮件给:{0}\" href = \"mailto:{0}\" target=\"_blank\">{0}</a>", mail);
        }

        /// <summary>
        /// qq转换成href
        /// </summary>
        public static string QqHref(string qq)
        {
            return String.Format("<a Title=\"与QQ{0}进行会话\" href=\"tencent://message/?Uin={0}&amp;Site=Web&amp;Menu=yes\" target=\"_blank\">{0}</a>", qq);
        }

        /// <summary>
        /// 绑定分页数据库
        /// </summary>
        public static void BindPds(Repeater repeater, object viewState, int pagedSize, int pageIndex)
        {
            DataTable dt = (DataTable)viewState;
            PagedDataSource pagedDataSource = new PagedDataSource();
            if (!dt.Columns.Contains("row_index"))
            {
                dt.Columns.Add("row_index");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["row_index"] = (i + 1).ToString();
                }
            }
            pagedDataSource.DataSource = dt.DefaultView;
            pagedDataSource.AllowPaging = true;
            pagedDataSource.PageSize = pagedSize;
            pagedDataSource.CurrentPageIndex = pageIndex - 1;
            repeater.DataSource = pagedDataSource;
            repeater.DataBind();
        }

        /// <summary>
        /// 建立翻页代码,并返回当前页码
        /// </summary>
        public static int GetPageIndexBuildPaged(int pageSize, int dataAmount, HtmlContainerControl hcc, string otherParam)
        {
            int pageIndex;
            int pageAmount = (dataAmount - 1) / pageSize + 1;
            if (StringUtil.IsDigit(HttpContext.Current.Request.QueryString["page"]))
            {
                pageIndex = Convert.ToInt32(HttpContext.Current.Request.QueryString["page"]);
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }
                if (pageIndex > pageAmount)
                {
                    pageIndex = pageAmount;
                }
            }
            else
            {
                pageIndex = 1;
            }
            string url = HttpContext.Current.Request.Path + "?page=";
            string deadHref = "javascript:void(0);";
            string firstPageHref = (pageIndex == 1) ? deadHref : String.Format("{0}1{1}", url, otherParam);
            string prevPageHref = (pageIndex == 1) ? deadHref : url + (pageIndex - 1) + otherParam;
            string nextPageHref = (pageIndex == pageAmount) ? deadHref : url + (pageIndex + 1) + otherParam;
            string lastPageHref = (pageIndex == pageAmount) ? deadHref : url + pageAmount + otherParam;
            string pageGoScript = String.Format("javascript:window.location.href='{0}'+document.getElementById('txtpageindex').value+'{1}';", url, otherParam);
            hcc.InnerHtml = String.Format("共有<span style=\"color: #FF0000;\">{0}</span>条记录,当前第<span style=\"color: #FF0000;\">{1}</span>/<span style=\"color: #FF0000;\">{2}</span>页,每页<span style=\"color: #FF0000;\">{3}</span>条记录&nbsp; <a href=\"{4}\">|&lt;首页</a><a href=\"{5}\">&nbsp;&lt;上一页</a><a href=\"{6}\">&nbsp;下一页&gt;</a><a href=\"{7}\">&nbsp;尾页&gt;|</a> &nbsp;转到第<input Type=\"text\" ID=\"txtpageindex\" style=\"width: 25px;\" />页<input Type=\"button\" value=\"GO\" onclick=\"{8}\" />", dataAmount, pageIndex, pageAmount, pageSize, firstPageHref, prevPageHref, nextPageHref, lastPageHref, pageGoScript);
            return pageIndex;
        }


        /// <summary>
        /// 验证登录,未登录将跳转
        /// </summary>
        public static bool IsLoginedAndRedirectUnlogined(string sessionName, string loginPage)
        {
            if (HttpContext.Current.Session[sessionName] == null)
            {
                HttpContext.Current.Response.Write(RedirectScript(loginPage + "?Response=" + HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.RawUrl)));
                return false;
            }
            else
            { return true; }
        }

        /// <summary>
        /// 验证登录,未登录将跳转
        /// </summary>
        public static bool IsLoginedAndRedirectUnlogined(string loginPage)
        {
            return IsLoginedAndRedirectUnlogined("User", loginPage);
        }
    }
}
