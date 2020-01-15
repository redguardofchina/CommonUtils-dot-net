using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CommonUtils
{
    public class WebSocketSession : WebSocketBehavior
    {
        public Action<Exception, WebSocketSession> OnError_;
        public Action<WebSocketSession> OnOpen_;
        public Action<string, WebSocketSession> OnReceive_;
        public Action<WebSocketSession> OnClose_;

        protected override void OnError(ErrorEventArgs e)
        => OnError_?.Invoke(e.Exception, this);

        protected override void OnOpen()
        => OnOpen_?.Invoke(this);

        protected override void OnMessage(MessageEventArgs e)
        => OnReceive_?.Invoke(e.Data, this);

        protected override void OnClose(CloseEventArgs e)
        => OnClose_?.Invoke(this);

        public void Send_(string data)
        => Send(data);
    }
}
