using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;

namespace CommonUtils
{
    /// <summary>
    /// 系统工具
    /// </summary>
    public class SystemUtil
    {
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public static double ScreenWidth { get; } = SystemParameters.VirtualScreenWidth;

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public static double ScreenHeight { get; } = SystemParameters.VirtualScreenHeight;

        /// <summary>
        /// 桌面宽度
        /// </summary>
        public static double DesktopWidth { get; } = SystemParameters.WorkArea.Width;

        /// <summary>
        /// 桌面高度
        /// </summary>
        public static double DesktopHeight { get; } = SystemParameters.WorkArea.Height;

        /// <summary>
        /// 主机名
        /// </summary>
        public static string HostName { private set; get; }

        /// <summary>
        /// 主机名
        /// </summary>
        public static string HostNameAndTime
        {
            get
            {
                return string.Format("{0}[{1}]", HostName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        /// <summary>
        /// 主机类
        /// </summary>
        public static IPHostEntry HostEntry { private set; get; }

        /// <summary>
        /// ip集
        /// </summary>
        public static IPAddress[] HostIps { private set; get; }

        /// <summary>
        /// ip
        /// </summary>
        public static IPAddress HostIp { private set; get; }

        static SystemUtil()
        {
            HostName = Dns.GetHostName();
            HostEntry = Dns.GetHostEntry(HostName);
            HostIps = Dns.GetHostAddresses(HostName);

            if (HostIps.Length > 0)
                HostIp = HostIps[0];
        }

        /// <summary>
        /// Hosts路径
        /// </summary>
        public const string DnsPath = @"C:\Windows\System32\drivers\etc\hosts";

        /// <summary>
        /// 设置解析
        /// </summary>
        public static void SetDns(string domainName, string ip)
        {
            string newHost = ip + " " + domainName;
            string[] hosts = FileUtil.GetLines(DnsPath);
            foreach (string host in hosts)
                if (host == newHost)
                    return;
            FileUtil.AppendLine(DnsPath, newHost);
        }

        /// <summary>
        /// 删除解析
        /// </summary>
        public static void RemoveDns(string domainName, string ip)
        {
            string delHost = ip + " " + domainName;
            string[] hosts = FileUtil.GetLines(DnsPath);
            List<string> nesHosts = new List<string>();
            bool needReplace = false;
            foreach (string host in hosts)
            {
                if (host != delHost)
                    nesHosts.Add(host);
                else
                    needReplace = true;
            }
            if (needReplace)
                FileUtil.Save(DnsPath, nesHosts);
        }

        /// <summary>
        /// ping通返回true,否则返回false
        /// </summary>
        public static bool Ping(string ipOrUrl)
        {
            int timeOut = 3000;
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(ipOrUrl, timeOut);
                bool result = reply.Status == IPStatus.Success;
                ping.Dispose();
                return result;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 复制文本到剪切板
        /// </summary>
        public static bool SetClipboard(string text)
        {
            var result = false;
            var thread = new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    Clipboard.SetText(text);
                    result = true;
                }
                catch (Exception ex)
                {
                    LogUtil.Log(ex);
                }
            }));
            thread.TrySetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return result;
        }

        /// <summary>
        /// 获取剪切板文本
        /// </summary>
        public static string GetClipboard()
        {
            string text = null;
            Thread th = new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    text = Clipboard.GetText();
                }
                catch (Exception ex)
                {
                    LogUtil.Log(ex);
                }
            }));
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
            th.Join();
            return text;
        }

        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        public static System.Drawing.Point GetMousePosition()
        {
            return new System.Drawing.Point(SystemParameters.MouseHoverWidth.ToInt(), SystemParameters.MouseHoverHeight.ToInt());
        }

        /// <summary>
        /// 获取截屏
        /// </summary>
        public static Bitmap GetScreenShoot()
        {
            Bitmap bitmap = new Bitmap(SystemParameters.FullPrimaryScreenWidth.ToInt(), SystemParameters.FullPrimaryScreenHeight.ToInt());
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), bitmap.Size);
            graphics.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 获取屏幕指定坐标颜色
        /// </summary>
        public static Color GetScreenColor(int x, int y)
        {
            return GetScreenShoot().GetPixel(x, y);
        }

        /// <summary>
        /// 获取鼠标所在坐标颜色
        /// </summary>
        public static Color GetMouseColor()
        {
            var point = GetMousePosition();
            return GetScreenColor(point.X, point.Y);
        }
    }
}
