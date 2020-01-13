using System;
using System.Diagnostics;
using System.Linq;

namespace CommonUtils
{
    /// <summary>
    /// 日志工具
    /// sub代表分支
    /// </summary>
    public class LogUtil
    {
        #region 消息
        //记录格式化次数
        private static int _formatTime = 0;

        /// <summary>
        /// 格式化消息 加入时间和换行
        /// </summary>
        public static string GetFormatedString(object msg, params object[] args)
        {
            var timedMsg = string.Format("({0}){1}\r\n{2}\r\n\r\n", ++_formatTime, DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss.fff]"), msg);

            if (args.Length == 0)
                return timedMsg;

            return string.Format(timedMsg, args);
        }

        public static void Print(object msg, params object[] args)
        => ConsoleUtil.Print(msg, args);
        #endregion

        #region 路径
        /// <summary>
        /// log路径
        /// </summary>
        private static string GetLogPath(string sub = PathConfig.DefaultSubMark)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (string.IsNullOrWhiteSpace(sub) || sub == PathConfig.DefaultSubMark)
                return PathConfig.LogFloder.Combine(string.Format("log-{0}.log", date));
            else
                return PathConfig.SubLogFloder.Combine(string.Format("{0}/log-{1}.log", sub, date));
        }

        /// <summary>
        /// error路径
        /// </summary>
        private static string GetErrorPath(string sub = PathConfig.DefaultSubMark)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (string.IsNullOrWhiteSpace(sub) || sub == PathConfig.DefaultSubMark)
                return PathConfig.ErrorFloder.Combine(string.Format("error-{0}.log", date));
            else
                return PathConfig.SubErrorFloder.Combine(string.Format("{0}/error-{1}.log", sub, date));
        }
        #endregion

        #region log
        /// <summary>
        /// 打log
        /// </summary>
        public static void SubLog(string sub, string msg, params object[] args)
        {
            var formatedMsg = msg.Print(args);
            FileUtil.AppendWithQueue(GetLogPath(sub), formatedMsg);
        }

        /// <summary>
        /// 打log
        /// </summary>
        public static void Log(string msg, params object[] args)
        => SubLog(PathConfig.DefaultSubMark, msg, args);

        /// <summary>
        /// 打log,记录堆栈
        /// </summary>
        public static void LogWithStackTrace(StackTrace stackTrace, string msg, params object[] args)
        => Log(msg + "\r\n" + stackTrace, args);

        /// <summary>
        /// 打log
        /// </summary>
        public static void LogWithStackTrace(string msg, params object[] args)
        => LogWithStackTrace(new StackTrace(true), msg, args);
        #endregion

        #region error
        /// <summary>
        /// 异常信息缓存
        /// </summary>
        private static MapStringString _mapKeyError = new MapStringString();

        /// <summary>
        /// 记录异常
        /// </summary>
        public static void SubError(string sub, Exception ex)
        {
            //打印错误

            var msg = ex.HelpLink;

            if (!msg.IsNullOrEmpty())
                msg += "\r\n";

            msg += ex.GetType().FullName + ": " + ex.Message;

            if (ex.StackTrace != null)
                msg += "\r\n" + ex.StackTrace.GetLines().Take(5).JoinToText();

            msg.Print();

            //记录错误

            string path = GetErrorPath(sub);

            //考虑到sub为空的情况，加个前缀
            string key = "error-" + sub;
            string value = ex.HelpLink + ex.Message;
            //不重复记录 节省资源
            if (value == _mapKeyError.Get(key, null, false))
            {
                FileUtil.AppendWithQueue(path, "same as last.");
                return;
            }

            _mapKeyError.Set(key, ex.Message);

            if (!ex.HelpLink.IsNullOrEmpty())
                FileUtil.AppendWithQueue(path, GetFormatedString(ex.HelpLink + "\r\n" + ex));
            else
                FileUtil.AppendWithQueue(path, GetFormatedString(ex));
        }

        public static void Error(Exception ex)
        => SubLog(PathConfig.DefaultSubMark, ex);

        /// <summary>
        /// 记录异常
        /// </summary>
        public static void Error(string msg, Exception ex)
        {
            ex.HelpLink += "[" + msg + "]";
            Log(ex);
        }

        public static void SubLog(string sub, Exception ex)
        => SubError(sub, ex);

        /// <summary>
        /// 记录异常
        /// </summary>
        public static void Log(Exception ex)
        => Error(ex);

        /// <summary>
        /// 记录异常
        /// </summary>
        public static void Log(string msg, Exception ex)
        => Error(msg, ex);
        #endregion

        /// <summary>
        /// 获取日志内容
        /// </summary>
        public static string GetLog(string sub = PathConfig.DefaultSubMark)
        => FileUtil.Read(GetLogPath(sub));

        /// <summary>
        /// 获取异常内容
        /// </summary>
        public static string GetError(string sub = PathConfig.DefaultSubMark)
        => FileUtil.Read(GetErrorPath(sub));

        /// <summary>
        /// 清理日志和异常
        /// </summary> 
        public static void Clear(string sub = PathConfig.DefaultSubMark)
        {
            FileUtil.Delete(GetLogPath(sub));
            FileUtil.Delete(GetErrorPath(sub));
        }
    }
}
