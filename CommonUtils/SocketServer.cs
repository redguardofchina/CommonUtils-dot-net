using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommonUtils
{
    /// <summary>
    /// Socket服务器
    /// </summary>
    public class SocketServer
    {
        /// <summary>
        /// 绑定终端
        /// </summary>
        public IPEndPoint BindPoint { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public SocketServer(IPEndPoint bind)
        {
            BindPoint = bind;
        }

        public SocketServer(string remote) : this(NetworksUtil.GetIPEndPoint(remote)) { }
        public SocketServer(string host, int port) : this(NetworksUtil.GetIPEndPoint(host, port)) { }
        public SocketServer(string host, string port) : this(host, port.ToInt()) { }
        public SocketServer(int port) : this("127.0.0.1", port) { }

        /// <summary>
        /// 异常回调
        /// </summary>
        public event Action<Exception> OnException = LogUtil.Log;

        /// <summary>
        /// 启动回调
        /// </summary>
        public event Action OnStart;

        /// <summary>
        /// 接入回调
        /// </summary>
        public event Action<Socket> OnConnect;

        /// <summary>
        /// 收到字节回调
        /// </summary>
        public event Action<byte[], Socket> OnReceiveBytes;

        /// <summary>
        /// 收到消息回调
        /// </summary>
        public event Action<string, Socket> OnReceiveString;

        /// <summary>
        /// 断开回调
        /// </summary>
        public event Action<Socket> OnDisconnect;

        /// <summary>
        /// 关闭回调
        /// </summary>
        public event Action OnStop;

        /// <summary>
        /// 主连接
        /// </summary>
        private Socket _mainSession { get; set; }

        /// <summary>
        /// 主监听
        /// </summary>
        private Thread _mainListener { get; set; }

        /// <summary>
        /// 分支
        /// </summary>
        private Map<Socket, Thread> _mapSubSessionListener { get; } = new Map<Socket, Thread>();

        /// <summary>
        /// 记录主动关闭状态
        /// </summary>
        private bool _isStopped { get; set; }

        /// <summary>
        /// 开启
        /// </summary>
        public void Start()
        {
            //主动开启
            _isStopped = false;

            try
            {
                //创建一个Socket对象,如果用UDP协议,则要用SocketTyype.Dgram类型的套接字
                _mainSession = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //绑定端口
                _mainSession.Bind(BindPoint);
                LogUtil.Log("Socket server has bound {0}", BindPoint);
                //设置挂起连接队列的最大长度,不可省略
                _mainSession.Listen(50000);
                //监听连接
                _mainListener = new Thread(delegate ()
                {
                    try
                    {
                        while (true)
                        {
                            //接受到Client连接,为此连接建立新的Socket,并接受消息
                            var subSession = _mainSession.Accept();
                            //接入回调
                            LogUtil.Print("Socket server has accepted client {0}.", subSession.RemoteEndPoint);
                            OnConnect?.Invoke(subSession);
                            //判断状态
                            if (!subSession.Connected)
                            {
                                subSession.DisconnectCloseDispose();
                                continue;
                            }
                            //开启新终端监听
                            var subListener = new Thread(delegate ()
                            {
                                try
                                {
                                    while (true)
                                    {
                                        var bytesArray = subSession.ReceiveBytesArray();
                                        LogUtil.Print("Socket server has received {0} blocks from client {1}.",
                                            bytesArray.Length, subSession.RemoteEndPoint);
                                        //此处try catch不能与外部合并，以免造subServer不接收消息
                                        try
                                        {
                                            foreach (var bytes in bytesArray)
                                            {
                                                LogUtil.Print("Socket server is dealing with block of length {0}.", bytes.Length);
                                                OnReceiveBytes?.Invoke(bytes, subSession);
                                                OnReceiveString?.Invoke(bytes.GetString(), subSession);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            OnException?.Invoke(ex);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //基本是客户端断开了
                                    OnException?.Invoke(ex);
                                    if (_mapSubSessionListener.ContainsKey(subSession))
                                    {
                                        Disconnect(subSession);
                                    }
                                }
                            });
                            subListener.Start();
                            //添加到连接表
                            _mapSubSessionListener.Add(subSession, subListener);
                        }
                    }
                    catch (Exception ex)
                    {
                        //基本是网络中断或主动停服
                        OnException?.Invoke(new Exception("网络中断或主动停服 即" + ex.Message));
                        if (!_isStopped)
                        {
                            _mainSession.DisconnectCloseDispose();
                            OnStop?.Invoke();
                        }
                    }
                });
                _mainListener.Start();
                //启动回调
                LogUtil.Log("Socket server has started listening");
                OnStart?.Invoke();
            }
            catch (Exception ex)
            {
                //基本是端口被占用
                OnException?.Invoke(ex);
                _mainSession.DisconnectCloseDispose();
            }
        }

        /// <summary>
        /// 异常重启
        /// </summary>
        private System.Timers.Timer _reopenTimer { get; set; }

        /// <summary>
        /// 开启异常重启
        /// </summary>
        public void StartReopen(int second = 300)
        {
            if (_reopenTimer != null)
                _reopenTimer.Close();

            _reopenTimer = ThreadUtil.TimerDelay(delegate ()
            {
                if (_mainListener == null || !_mainListener.IsAlive)
                    Start();
            }, second);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendTo(Socket subSession, string msg)
        {
            if (subSession.Connected)
                subSession.Send(msg);
        }

        public void SendTo(Socket subSession, object msg) => SendTo(subSession, msg.ToJsonString(true));

        /// <summary>
        /// 发送一条消息
        /// </summary>
        public void SendToOne(string msg)
        {
            foreach (var subSession in _mapSubSessionListener.Keys.ToArray())
            {
                if (subSession.Connected)
                {
                    subSession.Send(msg);
                    break;
                }
            }
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        public void SendToAll(string msg)
        {
            foreach (var subSession in _mapSubSessionListener.Keys.ToArray())
            {
                if (subSession.Connected)
                    subSession.Send(msg);
            }
        }

        /// <summary>
        /// 关闭分支
        /// </summary>
        public void Disconnect(Socket session)
        {
            if (!_mapSubSessionListener.ContainsKey(session))
                return;

            try
            {
                var subListener = _mapSubSessionListener.Get(session);

                session?.DisconnectCloseDispose();

                subListener?.Interrupt();
                //subListener?.Abort();

                _mapSubSessionListener.Remove(session);
                OnDisconnect?.Invoke(session);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        /// <summary>
        /// 关闭所有分支
        /// </summary>
        public void DisconnectAll()
        {
            try
            {
                foreach (var session in _mapSubSessionListener.Keys.ToArray())
                    Disconnect(session);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Stop()
        {
            //主动关闭
            if (_isStopped)
                return;
            _isStopped = true;

            try
            {
                _reopenTimer?.Close();
                _reopenTimer?.Dispose();

                _mainSession?.DisconnectCloseDispose();

                _mainListener?.Interrupt();
                //_mainListener?.Abort();

                DisconnectAll();

                OnStop?.Invoke();
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
        }

        /// <summary>
        /// 对象状态
        /// </summary>
        public string GetState()
        {
            var state = new StringBuilder();
            try
            {
                state.AppendLine("断线重启: " + (_reopenTimer != null));
                state.AppendLine("服务状态: " + (_mainSession != null && _mainSession.IsBound));
                state.Append("监听状态:" + (_mainListener != null && _mainListener.IsAlive));
            }
            catch (Exception ex)
            {
                state.Append("异常: ", ex);
            }
            return state.ToString();
        }

        /// <summary>
        /// 监听状态
        /// </summary>
        public bool IsListening
        {
            get
            {
                return _mainSession != null && _mainSession.IsBind() && _mainListener != null && _mainListener.IsAlive;
            }
        }
    }
}