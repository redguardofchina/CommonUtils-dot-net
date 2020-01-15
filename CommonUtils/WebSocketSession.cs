using System;
using System.Collections.Generic;
using System.Text;
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
        => ErrorEvent?.Invoke(e.Exception, this);

        protected override void OnOpen()
        => ConnectEvent?.Invoke(this);

        protected override void OnMessage(MessageEventArgs e)
        => ReceiveEvent?.Invoke(e.Data, this);

        protected override void OnClose(CloseEventArgs e)
        => DisconnectEvent?.Invoke(this);

        public void Send_(string data)
        => Send(data);
    }
}
