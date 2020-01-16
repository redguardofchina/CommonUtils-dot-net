using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CommonUtils
{
    public class WebSocketServer
    {
        private WebSocketSharp.Server.WebSocketServer _server;

        public event Action<Exception, WebSocketSession> SessionErrorEvent;

        public event Action<WebSocketSession> SessionConnectEvent;

        public event Action<string, WebSocketSession> SessionReceiveEvent;

        public event Action<WebSocketSession> SessionDisconnectEvent;

        private List<WebSocketSession> _sessions = new List<WebSocketSession>();

        public WebSocketServer(IPEndPoint iep, string route)
        {
            if (route[0] != '/')
                route = "/" + route;
            _server = new WebSocketSharp.Server.WebSocketServer(iep.Address, iep.Port);
            _server.AddWebSocketService(route, delegate (WebSocketSession session)
            {
                session.ErrorEvent = SessionErrorEvent;
                session.ConnectEvent = SessionConnectEvent;
                session.ReceiveEvent = SessionReceiveEvent;
                session.DisconnectEvent = SessionDisconnectEvent;
                _sessions.Add(session);
            });
        }

        public WebSocketServer(string iep, string route)
        : this(NetworksUtil.GetIPEndPoint(iep), route) { }

        public void Start()
        {
            _server.Start();
            LogUtil.Print("WebSocketServer started: ws://"
                + new IPEndPoint(_server.Address, _server.Port)
                + _server.WebSocketServices.Paths.FirstOrDefault());
        }

        public void Send(string data)
        {
            foreach (var session in _sessions)
                session.Send(data);
        }

        public void Stop()
        {
            _server.Stop();
            Console.WriteLine("WebSocketServer stopped");
        }
    }
}
