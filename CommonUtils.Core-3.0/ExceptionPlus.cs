using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils
{
    /// <summary>
    /// 异常信息
    /// </summary>
    public class ExceptionPlus : Exception
    {
        private string _stackTrace;
        public override string StackTrace { get { return _stackTrace; } }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="message">@message</param>
        /// <param name="stackTrace">new StackTrace(true)</param>
        public ExceptionPlus(string message = "ExceptionPlus", StackTrace stackTrace = null) : base(message)
        {
            if (stackTrace == null)
                stackTrace = new StackTrace(1, true);
            _stackTrace = stackTrace.ToString();
        }

        public override string ToString()
        {
            return GetType().FullName + ": " + Message + "\r\n" + StackTrace;
        }
    }
}
