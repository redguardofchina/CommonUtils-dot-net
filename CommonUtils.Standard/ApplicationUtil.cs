using System;
using System.Collections;
using System.Diagnostics;

namespace CommonUtils
{
    /// <summary>
    /// 应用工具
    /// undone show sln path
    /// </summary>
    public static class ApplicationUtil
    {
        public static DateTime StartTime { get; private set; }

        public static void InitStartTime()
        {
            StartTime = DateTime.Now;
            LogUtil.Log("初始化启动时间：" + StartTime);
        }

        //Process

        public static readonly Process Process = System.Diagnostics.Process.GetCurrentProcess();

        public static readonly string ProcessPath = Process.MainWindowTitle;

        public static readonly string ProcessName = Process.ProcessName;

        public static readonly bool IsIIS = ProcessName.ToLower().Contains("iis", "w3wp");

        public static readonly string FriendlyName = AppDomain.CurrentDomain.FriendlyName;

        public static readonly ProcessStartInfo ProcessStartInfo = Process.StartInfo;

        //other

        public static readonly int PlatformBit = IntPtr.Size * 8;

        public static readonly string MachineName = Environment.MachineName;

        public static readonly OperatingSystem OperatingSystem = Environment.OSVersion;

        /// <summary>
        /// 逻辑磁盘
        /// </summary>
        public static readonly string[] LogicalDrives = Environment.GetLogicalDrives();

        /// <summary>
        /// 环境变量
        /// </summary>
        public static readonly IDictionary EnvironmentVariables = Environment.GetEnvironmentVariables();

        /// <summary>
        /// 系统文件夹
        /// </summary>
        public static readonly string SystemDirectory = Environment.SystemDirectory;

        /// <summary>
        /// 程序文件夹
        /// </summary>
        public static readonly string CurrentDirectory = Environment.CurrentDirectory;
    }
}
