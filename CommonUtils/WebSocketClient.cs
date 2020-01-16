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
            OnError += delegate (object sender, ErrorEventArgs e)
            {
                LogUtil.Print("ErrorEvent");
                ErrorEvent?.Invoke(e.Exception);
            };
            OnOpen += delegate (object sender, EventArgs e)
            {
                LogUtil.Print("WebSocketClient connect: sw://" + Url);
                OpenEvent?.Invoke();
            };
            OnMessage += delegate (object sender, MessageEventArgs e)
            {
                LogUtil.Print("MessageEvent");
                MessageEvent?.Invoke(e.Data);
            };
            OnClose += delegate (object sender, CloseEventArgs e)
            {
                LogUtil.Print("CloseEvent");
                CloseEvent?.Invoke();
            };
        }

        public WebSocketClient(IPEndPoint iep, string route)
        : this("ws://" + iep + (route[0] == '/' ? route : ("/" + route))) { }
    }
}
