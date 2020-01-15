using System;
using System.Net;

namespace CommonUtils.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var iep = NetworksUtil.GetIPEndPoint("192.168.1.1:1");
            iep.Print();

            ConsoleUtil.Pause();
        }
    }
}
