using System;
using System.Net;
using WebSocketSharp;

namespace CommonUtils
{
    public class WebSocketClient : WebSocket
    {
        public event Action<Exception> ErrorEvent;
        public event Action OpenEvent;
        public event Action<string> MessageEvent;
        public event Action CloseEvent;

        public WebSocketClient(string url)
        : base(url)
        {
            OnError += delegate (object sender, ErrorEventArgs e) { ErrorEvent?.Invoke(e.Exception); };
            OnOpen += delegate (object sender, EventArgs e) { OpenEvent?.Invoke(); };
            OnMessage += delegate (object sender, MessageEventArgs e) { MessageEvent?.Invoke(e.Data); };
            OnClose += delegate (object sender, CloseEventArgs e) { CloseEvent?.Invoke(); };
        }

        public WebSocketClient(IPEndPoint iep, string route)
        : base("ws://" + iep + (route[0] == '/' ? route : ("/" + route))) { }
    }
}
