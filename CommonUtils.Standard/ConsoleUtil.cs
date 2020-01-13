using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils
{
    /// <summary>
    /// 控制台相关方法
    /// </summary>
    public static class ConsoleUtil
    {
        private static void ColorChange()
        => Console.ForegroundColor = ConsoleColor.Yellow;

        private static void ColorReset()
        => Console.ResetColor();

        /// <summary>
        /// 输出到控制台
        /// </summary>
        public static string Print(this object value, params object[] args)
        {
            var msg = LogUtil.GetFormatedString(value, args);
            ColorChange();
            Console.Write(msg);
            ColorReset();
            return msg;
        }

        /// <summary>
        /// 输出到控制台
        /// </summary>
        public static void PrintJson(this object value)
        => Print(value.ToJson());

        public static void Write(object value, params object[] args)
        {
            if (args == null || args.Length == 0)
                Console.Write(value);
            else
                Console.Write(value.ToString(), args);
        }

        public static void WriteLine()
         => Console.WriteLine();

        public static void WriteLine(object value, params object[] args)
        {
            if (args == null || args.Length == 0)
                Console.WriteLine(value);
            else
                Console.WriteLine(value.ToString(), args);
        }

        /// <summary>
        /// 控制台暂停
        /// </summary>
        public static void Pause()
        {
            ColorChange();
            Console.WriteLine("Press enter to continue...");
            ColorReset();
            Console.ReadLine();
        }

        public static void PrintLine()
        {
            ColorChange();
            Console.WriteLine("=====================================================================");
            ColorReset();
        }
    }
}
