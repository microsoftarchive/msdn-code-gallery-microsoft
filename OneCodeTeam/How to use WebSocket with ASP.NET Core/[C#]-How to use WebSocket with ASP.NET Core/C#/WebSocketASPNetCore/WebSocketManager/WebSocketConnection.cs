using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketASPNetCore.WebSocketManager
{
    public abstract class WebSocketConnection
    {
        public WebSocketHandler Handler { get; }

        public WebSocket WebSocket { get; set; }

        public WebSocketConnection(WebSocketHandler handler)
        {
            Handler = handler;
        }

        public virtual async Task SendMessageAsync(string message)
        {
            if (WebSocket.State != WebSocketState.Open) return;
            var arr = Encoding.UTF8.GetBytes(message);

            var buffer = new ArraySegment<byte>(
                    array: arr,
                    offset: 0,
                    count: arr.Length);

            await WebSocket.SendAsync(
                buffer: buffer,
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None
                );
        }

        public abstract Task ReceiveAsync(string message);
    }
}
