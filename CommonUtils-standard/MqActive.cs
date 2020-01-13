using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using Newtonsoft.Json.Linq;
using System;

namespace CommonUtils
{
    /// <summary>
    /// ActiveMQ
    /// </summary>
    public class MqActive
    {
        /// <summary>
        /// 连接
        /// </summary>
        private IConnection _connection;

        /// <summary>
        /// 会话
        /// </summary>
        private Session _session;

        /// <summary>
        /// 初始化 ActiveMQ好像不需要账号密码，传与不传一样，RabbitMQ要的！
        /// </summary>
        public MqActive(string url, string user = null, string pwd = null)
        {
            var client = ApplicationUtil.FriendlyName + "@" + DateTime.Now.Stamp();

            //构建MQ工厂
            var factory = new ConnectionFactory(url);
            factory.OnException += Factory_OnException;

            //ActiveMQ不需要账号密码，传与不传一样，RabbitMQ要的！
            if (!string.IsNullOrEmpty(user))
                factory.UserName = user;
            if (!string.IsNullOrEmpty(pwd))
                factory.Password = pwd;

            //通过工厂构建连接
            Console.WriteLine(string.Format("ActiveMQ is trying to connect to {0} with client name {1}.", url, client));
            //如果服务器不通，此处启动失败mConnection，mSession都为null
            _connection = factory.CreateConnection();
            Console.WriteLine(string.Format("ActiveMQ has connectted to {0},waitting for adding listener...", url));
            _connection.ClientId = client;
            _connection.ConnectionInterruptedListener += MConnection_ConnectionInterruptedListener;
            _connection.ConnectionResumedListener += MConnection_ConnectionResumedListener;
            _connection.ExceptionListener += MConnection_ExceptionListener;

            //通过连接创建一个会话
            _session = _connection.CreateSession() as Session;
        }

        /// <summary>
        /// 默认消费者
        /// </summary>
        private MessageConsumer _consumer;

        /// <summary>
        /// 添加监听
        /// </summary>
        public void AddListener(string topic, Action<string> callback = null)
        {
            if (callback == null)
                callback = Console.WriteLine;

            //通过会话创建一个客户，这里就是Queue这种会话类型的监听参数设置
            _consumer = _session.CreateConsumer(new ActiveMQTopic(topic)) as MessageConsumer;
            _consumer.Listener += delegate (IMessage message)
            {
                string msg = ((ITextMessage)message).Text;
                callback(msg);
            };
            Console.WriteLine(string.Format("ActiveMQ has listened {0},waitting for start.", topic));
        }

        private bool mStart = false;

        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start()
        {
            //启动连接，监听的话要主动启动连接
            _connection.Start();
            mStart = true;
            Console.WriteLine("ActiveMQ has started.");
        }

        /// <summary>
        /// 状态
        /// 断线重连
        /// </summary>
        public string State
        {
            get
            {
                JObject state = new JObject();
                state.Add("ActiveMQ.IsStarted", mStart);
                state.Add("ActiveMQ.Connection.IsStarted", _connection.IsStarted);
                state.Add("ActiveMQ.Session.Started", _session.Started);
                state.Add("ActiveMQ.Consumer.FailureError", _consumer.FailureError != null);
                if (!_session.Started)
                    _session.Start();
                return state.ToString();
            }
        }

        #region 异常处理
        private void Factory_OnException(Exception exception)
        {
            throw exception;
        }

        private void MConnection_ExceptionListener(Exception exception)
        {
            throw exception;
        }

        private void MConnection_ConnectionResumedListener()
        {
            throw new Exception("ConnectionResumed");
        }

        private void MConnection_ConnectionInterruptedListener()
        {
            throw new Exception("ConnectionInterrupted");
        }
        #endregion
    }
}
