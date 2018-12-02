using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MultiPlayer
{
    public class WebSocketClient
    {
        WebSocket webSocket;
        public string Data { get; private set; }

        public WebSocketClient(string url)
        {
            webSocket = new WebSocket(new Uri(url));
        }

        public IEnumerator Connect()
        {
            yield return webSocket.Connect();
        }

        public IEnumerator WaitMessage()
        {
            string recv;
            while ((recv = webSocket.RecvString())==null)
            {
                yield return null;
            }
            Data = recv;
        }

        public void SendMessage(string data)
        {
            webSocket.SendString(data);
        }
    }
}
