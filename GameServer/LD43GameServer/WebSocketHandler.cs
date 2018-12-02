using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Newtonsoft.Json;
using System.Threading;
using System.Text;
using Newtonsoft.Json.Linq;

namespace LD43GameServer
{
    public class WebSocketHandler
    {
        const int BufferSize = 4096;
        
        WebSocket webSocket;
        byte[] receiveBuffer = new byte[BufferSize];

        public WebSocketHandler(WebSocket webSocket)
        {
            this.webSocket = webSocket;
        }

        public async Task<string> ReceiveStringAsync()
        {
            Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
            var idx = 0;
            var length = receiveBuffer.Length;
            ValueWebSocketReceiveResult result;
            Next:
            do
            {
                if (idx >= receiveBuffer.Length)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.MessageTooBig, "Message too big.", CancellationToken.None);
                    return null;
                }
                var mem = new Memory<byte>(receiveBuffer, idx, length);
                result = await webSocket.ReceiveAsync(mem, CancellationToken.None);
                idx += result.Count;
                length -= result.Count;
            } while (!result.EndOfMessage);
            if (result.Count == 0)
                goto Next;
            return Encoding.UTF8.GetString(receiveBuffer);
        }
        public async Task SendStringAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new Memory<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public async Task<T> ReceiveObjectAsync<T>() where T : class
        {
            var text = await ReceiveStringAsync();
            return JsonConvert.DeserializeObject<T>(text);
        }
        public async Task SendObjectAsync(object obj)
        {
            await SendStringAsync(JsonConvert.SerializeObject(obj));
        }

        public async void Close()
        {
            ServerLog.Log("Connection closing");
            try
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Server Close", CancellationToken.None);
                webSocket.Dispose();
            }
            catch
            {

            }
        }
    }
}
