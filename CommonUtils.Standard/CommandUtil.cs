using System;
using System.Diagnostics;

namespace CommonUtils
{
    /// <summary>
    /// 命令行
    /// </summary>
    public class CommandUtil
    {
        /// <summary>
        /// 执行cmd命令
        /// </summary>      
        public static ProcessResult Run(string args, string workspace = null)
        {
            var startInfo = new ProcessStartInfo("cmd.exe");
            startInfo.Arguments = "/c" + args; // /c不可省略

            if (!string.IsNullOrEmpty(workspace))
                startInfo.WorkingDirectory = workspace;

            return ProcessUtil.Start(startInfo);
        }

        /// <summary>
        /// 执行cmd命令
        /// </summary> 
        public static ProcessResult Execute(string args, string workspace = null)
        => Run(args, workspace);

        public static ProcessResult Ping(string ip)
        => Run("ping " + ip);

        /// <summary>
        /// 改变当前系统时间
        /// </summary> 
        public static void SetTime(DateTime time)
        {
            Execute("date " + time.ToString("yyyy-MM-dd"));
            Execute("time " + time.ToString("HH:mm:ss"));
            ThreadUtil.Sleep(3);
            TimeUtil.NetRefresh();
        }

        /// <summary>
        /// 数据迁移命令
        /// 数据库实体类必须包含默认构造函数，否者EF创建实体失败，抛出此异常：
        /// Database operation expected to affect 1 row(s) but actually affected 0 row(s).
        /// </summary>
        public static string GetAddMigrationCommand(string name)
        => "add-migration " + name;

        /// <summary>
        /// 命令
        /// </summary>
        public static void CreateCoreRunCmdFile(string @namespace = null)
        {
            if (string.IsNullOrEmpty(@namespace))
                @namespace = ReflectionUtil.IndexNamespace(2);
            var cmd = string.Format("dotnet {0}.dll", @namespace);
            FileUtil.Save("~run.cmd", cmd, Encodings.UTF8NoBom);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public static void OpenFile(string path)
        {
            string cmd = string.Format("start \"\" \"{0}\"", path);
            Run(cmd);
        }
    }
}
