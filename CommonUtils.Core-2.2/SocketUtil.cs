using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommonUtils
{
    /// <summary>
    /// Socket相关函数
    /// </summary>
    public static class SocketUtil
    {
        //hack 未考虑不定义结束字符 收发短消息的情况(消息非常短时)

        /// <summary>
        /// 结束标识  尽量短  保证不拆包
        /// </summary>
        private static byte[] _endBytes = StringUtil.GetBytes("!end!");

        public static void SendEnd(this Socket socket)
        => socket.Send(_endBytes);

        /// <summary>
        /// 不要使用原生的Send(bytes),因为未封装结束字符
        /// </summary>
        public static void SendBytes(this Socket socket, byte[] bytes)
        {
            socket.Send(bytes);
            socket.SendEnd();
        }

        public static void Send(this Socket socket, string msg)
        => socket.SendBytes(msg.GetBytes());

        public static void Send(this Socket socket, Stream stream, bool closeStream = true)
        => socket.SendBytes(stream.GetBytes(closeStream));

        /// <summary>
        /// 不要使用原生的SendFile,因为未封装结束字符
        /// </summary>
        public static void SendFileEx(this Socket socket, string path)
        {
            socket.SendFile(path);
            socket.SendEnd();
        }

        /// <summary>
        /// 单次发送/接收数据长度
        /// </summary>
        private const int _packageSize = 10240;//10KB

        public static byte[][] ReceiveBytesArray(this Socket socket)
        {
            var bytes = new List<byte>();
            while (true)
            {
                var buffer = new byte[_packageSize];
                var length = socket.Receive(buffer);
                if (length == 0)
                    throw (new Exception("收到长度为0的字节组，远端会话请求断开"));

                var array = buffer.Take(length).ToArray();
                LogUtil.Print("Socket {0} has received bytes with length {1} from {2}.", socket.LocalEndPoint, array.Length, socket.RemoteEndPoint);
                bytes.AddRange(array);//组包

                //hack 未考虑一直收不到结束字符可能造成内存溢出崩溃
                if (array.Length >= _endBytes.Length && array.EqualFllow(_endBytes, array.Length - _endBytes.Length))
                    return bytes.ToArray().SplitBy(_endBytes);//组包完成
            }
        }

        /// <summary>
        /// 优化关闭
        /// </summary>
        public static void DisconnectCloseDispose(this Socket socket)
        {
            if (socket.Connected)
                socket.Disconnect(false);

            socket.Close();
            socket.Dispose();
        }

        /// <summary>
        /// 是否绑定了端口
        /// </summary>
        public static bool IsBind(this Socket socket)
        {
            try
            {
                return socket.LocalEndPoint != null;
            }
            catch
            {
                return false;
            }
        }
    }
}