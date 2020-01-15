using System;

namespace CommonUtils.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var enpoint = NetworksUtil.GetIPEndPoint("127.0.0.1:8401");
            var server = new WebSocketServer(enpoint, "unity");
            server.OnSessionError += delegate (Exception ex, WebSocketSession client) { ConsoleUtil.Print(ex); };
            server.OnSessionReceive += delegate (string msg, WebSocketSession client) { ConsoleUtil.Print(msg); };
            server.Start();
            server.Send("111");

            ConsoleUtil.Pause();
        }
    }
}
