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
                session.ErrorEvent = SessionErrorEvent;
                session.ConnectEvent = SessionConnectEvent;
                session.ReceiveEvent = SessionReceiveEvent;
                session.DisconnectEvent = SessionDisconnectEvent;
                _sessions.Add(session);
            });
        }

        public event Action<Exception, WebSocketSession> SessionErrorEvent;

        public event Action<WebSocketSession> SessionConnectEvent;

        public event Action<string, WebSocketSession> SessionReceiveEvent;

        public event Action<WebSocketSession> SessionDisconnectEvent;

        public void Start()
        {
            _server.Start();
            Console.WriteLine("WebSocketServer started: " + new IPEndPoint(_server.Address, _server.Port));
        }

        public void Send(string data)
        {
            foreach (var session in _sessions)
                session.Send_(data);
        }

        public void Stop()
        {
            _server.Stop();
            Console.WriteLine("WebSocketServer stopped");
        }
    }
}
