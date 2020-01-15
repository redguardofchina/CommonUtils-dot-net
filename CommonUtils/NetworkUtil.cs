using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace CommonUtils
{
    public static class NetworksUtil
    {
        public static IPAddress GetIp(byte byte1, byte byte2, byte byte3, byte byte4) => new IPAddress(new byte[] { byte1, byte2, byte3, byte4 });

        /// <summary>
        /// 获取IP
        /// </summary>
        public static IPAddress GetIp(string ipOrDomain)
        {
            try
            {
                return Dns.GetHostAddresses(ipOrDomain)[0];
            }
            catch (Exception ex)
            {
                LogUtil.Log(ipOrDomain, ex);
                return null;
            }
        }

        /// <summary>
        /// 获取EndPoint
        /// </summary>
        public static IPEndPoint GetIPEndPoint(string remote)
        {
            try
            {
                var cells = remote.Split(':', ',', ' ');
                var ip = GetIp(cells[0]);
                return new IPEndPoint(ip, int.Parse(cells[1]));
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取EndPoint
        /// </summary>
        public static IPEndPoint GetEndPoint(string ipOrDomain, int port)
        {
            try
            {
                return new IPEndPoint(GetIp(ipOrDomain), port);
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// 是否能ping通
        /// </summary>
        public static bool CanPing(string host)
        {
            var ping = new Ping();
            var options = new PingOptions();
            options.DontFragment = true;
            var data = "Test Data!";
            var buffer = Encoding.ASCII.GetBytes(data);
            var timeout = 1000; // Timeout 时间，单位：毫秒
            var reply = ping.Send(host, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }

        public static void PrintPingStatus(string ipOrDomain)
        {
            var ip = GetIp(ipOrDomain);
            if (ip == null)
                return;

            var ping = new Ping();
            var reply = ping.Send(ip);
            ConsoleUtil.Print("来自 {0} [{1}] [{2}] 的回复: 结果={3} 字节={4} 时间={5}ms TTL={6}", ipOrDomain, ip, reply.Address, reply.Status, reply.Buffer.Length, reply.RoundtripTime, reply.Options == null ? 0 : reply.Options.Ttl);
        }

        public static void Ping(string ipOrDomain) => PrintPingStatus(ipOrDomain);

        public static void PingTest()
        {
            Ping("localhost");
            Ping("127.0.0.1");
            Ping("baidu.com");
            Ping("jd.com");

            for (int index = 0; index < 20; index++)
                PrintPingStatus("v" + index + ".oculusss.pro");
        }
    }
}
