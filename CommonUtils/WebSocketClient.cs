using System.Net;
using WebSocketSharp;

namespace CommonUtils
{
    public class WebSocketClient : WebSocket
    {
        public WebSocketClient(string url)
        : base(url) { }

        public WebSocketClient(IPEndPoint iep, string route)
        : base("ws://" + iep + (route[0] == '/' ? route : ("/" + route))) { }
    }
}
