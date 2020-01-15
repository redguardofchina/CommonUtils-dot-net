using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CommonUtils
{
    public class WebSocketServer
    {
        private WebSocketSharp.Server.WebSocketServer _server;

        private List<WebSocketSession> _sessions = new List<WebSocketSession>();

        public WebSocketServer(IPEndPoint iep, string route)
        {
            if (route[0] != '/')
                route = "/" + route;
            _server = new WebSocketSharp.Server.WebSocketServer(iep.Address, iep.Port);
            _server.AddWebSocketService(route, delegate (WebSocketSession session)
            {
                session.OnError_ = OnSessionError;
                session.OnOpen_ = OnSessionConnect;
                session.OnReceive_ = OnSessionReceive;
                session.OnClose_ = OnSessionClose;
                _sessions.Add(session);
            });
        }

        public event Action<Exception, WebSocketSession> OnSessionError;

        public event Action<WebSocketSession> OnSessionConnect;

        public event Action<string, WebSocketSession> OnSessionReceive;

        public event Action<WebSocketSession> OnSessionClose;

        public void Start()
        => _server.Start();

        public void Send(string data)
        {
            foreach (var session in _sessions)
                session.Send_(data);
        }

        public void Stop()
        => _server.Stop();
    }
}
