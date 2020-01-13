using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace CommonUtils
{
    /// <summary>
    /// RabbitMQ
    /// </summary>
    public class MqRabbit
    {
        /// <summary>
        /// 连接
        /// </summary>
        private IConnection _connection;

        /// <summary>
        /// 会话
        /// </summary>
        private IModel _session;

        /// <summary>
        /// 初始化 api中的/不能省略
        /// </summary>
        public MqRabbit(string host, int port = 0, string user = null, string pwd = null, string api = null)
        {
            var client = ApplicationUtil.FriendlyName + "@" + DateTime.Now.Stamp();

            //构建MQ工厂
            var factory = new ConnectionFactory();
            factory.HostName = host;
            if (port > 0)
                factory.Port = port;
            if (!string.IsNullOrEmpty(user))
                factory.UserName = user;
            if (!string.IsNullOrEmpty(pwd))
                factory.Password = pwd;
            if (!string.IsNullOrEmpty(api))
                factory.VirtualHost = api;

            //通过工厂构建连接
            Console.WriteLine(string.Format("RabbitMQ is trying to connect to {0} whith client name {1}.", factory.Endpoint, client));
            //如果服务器不通，此处启动失败mConnection，mSession都为null
            _connection = factory.CreateConnection(client);
            Console.WriteLine(string.Format("RabbitMQ has connectted to {0},waitting for adding listener...", factory.Endpoint));

            //通过连接创建一个会话
            _session = _connection.CreateModel();
        }

        /// <summary>
        /// 队列名-消费者
        /// </summary>
        private Dictionary<string, EventingBasicConsumer> mQueueNameConsumers = new Dictionary<string, EventingBasicConsumer>();

        /// <summary>
        /// 默认消费者
        /// </summary>
        private EventingBasicConsumer _consumer;

        /// <summary>
        /// 添加监听
        /// </summary>
        public void AddListener(string queueName, Action<string> callback = null)
        {
            if (callback == null)
                callback = Console.WriteLine;

            _consumer = new EventingBasicConsumer(_session);
            _consumer.Received += delegate (object sender, BasicDeliverEventArgs e)
            {
                callback(e.Body.ToText());
                _session.BasicAck(e.DeliveryTag, false);
            };
            mQueueNameConsumers.Set(queueName, _consumer);
            Console.WriteLine(string.Format("RabbitMQ has listened {0},waitting for start.", queueName));
        }

        private bool mStart = false;

        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start()
        {
            foreach (var queueNameConsumer in mQueueNameConsumers)
                _session.BasicConsume(queueNameConsumer.Key, false, queueNameConsumer.Value);
            mStart = true;
            Console.WriteLine("RabbitMQ has started.");
        }

        /// <summary>
        /// 状态
        /// 启动成功之后，即使断线，复联之后会立即恢复正常，所以确保启动成功
        /// </summary>
        public string State
        {
            get
            {
                JObject state = new JObject();
                state.Add("RabbitMQ.IsStarted", mStart);
                state.Add("RabbitMQ.Connection.IsOpened", _connection.IsOpen);
                state.Add("RabbitMQ.Session.IsOpened", _session.IsOpen);
                state.Add("RabbitMQ.Consumer.IsRunning", _consumer.IsRunning);
                return state.ToString();
            }
        }
    }
}
