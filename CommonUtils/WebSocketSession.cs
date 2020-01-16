using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CommonUtils
{
    public class WebSocketSession : WebSocketBehavior
    {
        public Action<Exception, WebSocketSession> ErrorEvent;
        public Action<WebSocketSession> ConnectEvent;
        public Action<string, WebSocketSession> ReceiveEvent;
        public Action<WebSocketSession> DisconnectEvent;

        protected override void OnError(ErrorEventArgs e)
        {
            LogUtil.Print("ErrorEvent");
            ErrorEvent?.Invoke(e.Exception, this);
        }

        protected override void OnOpen()
        {
            LogUtil.Print("ConnectEvent");
            ConnectEvent?.Invoke(this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            LogUtil.Print("ReceiveEvent");
            ReceiveEvent?.Invoke(e.Data, this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            LogUtil.Print("DisconnectEvent");
            DisconnectEvent?.Invoke(this);
        }

        /// <summary>
        /// send new new new
        /// </summary>
        public new void Send(string data)
        => base.Send(data);
    }
}
