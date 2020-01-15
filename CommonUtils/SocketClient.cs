using Newtonsoft.Json.Linq;
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
    /// Socket客户端
    /// </summary>
    public class SocketClient
    {
        /// <summary>
        /// 服务端
        /// </summary>
        private IPEndPoint _remote { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public SocketClient(IPEndPoint remote)
        {
            _remote = remote;
        }

        public SocketClient(string remote) : this(NetworksUtil.GetIPEndPoint(remote)) { }
        public SocketClient(string host, int port) : this(NetworksUtil.GetIPEndPoint(host, port)) { }
        public SocketClient(string host, string port) : this(host, int.Parse(port)) { }
        public SocketClient(int port) : this("127.0.0.1", port) { }

        /// <summary>
        /// 异常回调
        /// </summary>
        public event Action<Exception> OnException = LogUtil.Log;

        /// <summary>
        /// 连接回调
        /// </summary>
        public event Action OnConnect;

        /// <summary>
        /// 收到数据回调
        /// </summary>
        public event Action<byte[]> OnReceiveBytes;

        /// <summary>
        /// 收到消息回调
        /// </summary>
        public event Action<string> OnReceiveString;

        /// <summary>
        /// 断开回调
        /// </summary>
        public event Action OnDisconnect;

        /// <summary>
        /// 会话
        /// </summary>
        public Socket Session { get; set; }

        /// <summary>
        /// 监听
        /// </summary>
        private Thread _listener { get; set; }

        /// <summary>
        /// 记录主动关闭状态
        /// </summary>
        private bool _isClosed { get; set; }

        /// <summary>
        /// 开启
        /// </summary>
        public void OpenConnect()
        {
            //主动开启
            _isClosed = false;

            try
            {
                //创建Socket
                Session = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //连接到服务器 
                Session.Connect(_remote);
                //判断状态
                if (!Session.Connected)
                {
                    LogUtil.Log("Socket client can not connect to {0}", _remote);
                    Session.DisconnectCloseDispose();
                    return;
                }
                LogUtil.Log("Socket client has connected to {0}", _remote);
                //监听服务器消息
                _listener = new Thread(delegate ()
                {
                    try
                    {
                        while (true)
                        {
                            var bytesArray = Session.ReceiveBytesArray();
                            LogUtil.Print("Socket client has received bytes with blocks length {0} from server {1}.", bytesArray.Length, Session.RemoteEndPoint);
                            try
                            {
                                foreach (var bytes in bytesArray)
                                {
                                    LogUtil.Print("Socket client is dealing with block of length {0}.", bytesArray.Length);
                                    OnReceiveBytes?.Invoke(bytes);
                                    OnReceiveString?.Invoke(bytes.GetString());
                                }
                            }
                            catch (Exception ex)
                            {
                                //此处try catch不能与外部合并，以免造成重连循环
                                //回调函数出错
                                OnException?.Invoke(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //基本就是断开了
                        OnException?.Invoke(ex);
                        //判断是否为主动关闭
                        if (!_isClosed)
                        {
                            Session.DisconnectCloseDispose();
                            OnDisconnect?.Invoke();
                        }
                    }
                });
                //开启监听
                _listener.Start();
                LogUtil.Log("Socket client has started listening {0}", Session.RemoteEndPoint);
                //连接回调
                OnConnect?.Invoke();
            }
            catch (Exception ex)
            {
                //没连上
                OnException?.Invoke(ex);
                Session.DisconnectCloseDispose();
            }
        }

        public void Send(byte[] bytes)
        {
            try
            {
                Session.SendBytes(bytes);
                LogUtil.Log("Socket client has sended bytes with length {0} to server {1}", bytes.Length, _remote);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        public void Send(string msg)
        {
            try
            {
                Session.Send(msg);
                LogUtil.Log("Socket client has sended msg with length {0} to server {1}", msg.Length, _remote);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        public void Send(Stream stream, bool closeStream = true)
        {
            try
            {
                var length = stream.Length;
                Session.Send(stream, closeStream);
                LogUtil.Log("Socket client has sended stream with length {0} to server {1}", length, _remote);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        public void SendFile(string path)
        {
            try
            {
                Session.SendFileEx(path);
                LogUtil.Log("Socket client has sended file with path [{0}] to server {1}", path, _remote);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        /// <summary>
        /// 关闭，如果重启，断线重连也要重启
        /// </summary>
        public void CloseConnect()
        {
            //主动关闭
            if (_isClosed)
                return;
            _isClosed = true;

            try
            {
                _reconnectTimer?.Close();
                _reconnectTimer?.Dispose();

                Session?.DisconnectCloseDispose();

                _listener?.Interrupt();
                //_listener?.Abort();

                OnDisconnect?.Invoke();
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        /// <summary>
        /// 对象状态
        /// </summary>
        public JObject GetState()
        {
            var state = new JObject();
            try
            {
                state.Put("远程地址", _remote.ToString());
                state.Put("连接状态", (Session != null && Session.Connected));
                state.Put("监听状态: ", (_listener != null && _listener.IsAlive));
                state.Put("开启断线重连", (_reconnectTimer != null));
            }
            catch (Exception ex)
            {
                state.Put("获取状态时发生异常异常", ex.Message);
            }
            return state;
        }

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return Session != null && Session.Connected && _listener != null && _listener.IsAlive;
            }
        }

        public void Reconnect()
        {
            if (!IsConnected)
                OpenConnect();
        }

        /// <summary>
        /// 断线重连
        /// </summary>
        private System.Timers.Timer _reconnectTimer { get; set; }

        /// <summary>
        /// 开启断线重连，开启周期握手？
        /// </summary>
        public void StartReconnect(int second = 300)
        {
            if (_reconnectTimer != null)
                _reconnectTimer.Close();

            _reconnectTimer = ThreadUtil.TimerDelay(Reconnect, second);
        }
    }
}