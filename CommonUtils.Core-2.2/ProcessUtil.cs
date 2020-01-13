using System;
using System.Diagnostics;

namespace CommonUtils
{
    /// <summary>
    /// 进程管理
    /// </summary>
    public class ProcessUtil
    {
        public static ProcessResult Run(ProcessStartInfo startInfo, double exitSeconds = 0)
        {
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            var result = new ProcessResult();
            result.Command = startInfo.FileName;

            if (!string.IsNullOrEmpty(startInfo.Arguments))
                result.Command += " " + startInfo.Arguments;

            if (!string.IsNullOrEmpty(startInfo.WorkingDirectory))
                result.Command += " @" + startInfo.WorkingDirectory;

            if (string.IsNullOrEmpty(startInfo.FileName))
            {
                result.HasError = true;
                result.Error = "Process lost !";
                return result;
            }

            try
            {
                var process = Process.Start(startInfo);

                if (exitSeconds != 0)
                    process.WaitForExit((int)(exitSeconds * 1000));
                else
                    process.WaitForExit();

                result.Result = process.StandardOutput.ReadToEnd();
                result.Error = process.StandardError.ReadToEnd();

                process.StandardOutput.Close();
                process.StandardError.Close();
                process.Close();
                //process.Dispose();

                if (!string.IsNullOrEmpty(result.Error))
                    result.HasError = true;

                return result;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Error += ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 运行进程
        /// </summary>
        /// <param name="path">.net core环境下这里只能写exe .net环境可以直接写文件路径，框架会查找默认程序</param>
        /// <param name="args">参数</param>
        public static ProcessResult Run(string path, string args = null)
        {
            var info = new ProcessStartInfo(path);
            if (!string.IsNullOrEmpty(args))
                info.Arguments = args;
            return Run(info);
        }

        public static ProcessResult Excute(string path, string args = null)
        => Run(path, args);

        public static ProcessResult Start(ProcessStartInfo startInfo, double exitSeconds = 0)
        => Run(startInfo, exitSeconds);

        public static ProcessResult Open(string path)
        => Run(path);

        /// <summary>
        /// 关闭进程
        /// </summary>
        public static void Kill(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                if (process != null)
                    process.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pid: " + pid);
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 关闭进程
        /// </summary>
        public static void Kill(string name)
        {
            try
            {
                var processes = Process.GetProcessesByName(name);
                foreach (var process in processes)
                    process.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProcessName: " + name);
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 注册IIS
        /// </summary>
        public static void RegistIIS()
        => Run(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe", "-i");

        /// <summary>
        /// 安装服务
        /// </summary>
        public static void ServiceInstall(string path)
        => Run(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe", path.AddQuotation());

        /// <summary>
        /// 卸载服务
        /// </summary>
        public static void ServiceUninstall(string path)
        => Run(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe", "/u " + path.AddQuotation());

        /// <summary>
        /// 卸载服务
        /// </summary>
        public static void ServiceUninstallByName(string name)
        => Run("sc", "delete " + name.AddQuotation());
    }
}
