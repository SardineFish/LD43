using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LD43GameServer
{
    public class Player
    {
        public string Name;
        public Guid ID;
        public bool Active { get; private set; }
        internal GameRoom Room;
        WebSocketHandler handler;
        Task ReceiveTask;
        Task SendTask;
        Queue<Message> MessageQueue = new Queue<Message>();

        public Player(WebSocketHandler handler)
        {
            this.handler = handler;
            this.ID = Guid.NewGuid();
        }
        public void Start()
        {
            Active = true;
            ReceiveTask = new Task(StartReceiveInternal);
            ReceiveTask.Start();
            SendTask = new Task(StartSendInternal);
            SendTask.Start();
        }
        public void Close()
        {
            if (Active)
            {
                Active = false;
                handler.Close();
            }
        }
        async void StartReceiveInternal()
        {
            try
            {
                var handshake = await handler.ReceiveObjectAsync<Message>();
                if (handshake.Type == MessageType.HandShake)
                {
                    var msg = (handshake.Body as JObject).Annotation<ClientHandShakeMessage>();
                    if (msg.Type == HandShakeType.Join)
                    {
                        Name = msg.Name;
                    }
                    else if (msg.Type == HandShakeType.Reconnect)
                    {
                        ID = Guid.Parse(msg.ID);
                        var room = msg.RoomID;
                    }

                }
                else
                {
                    throw new Exception("Hand shake error.");
                }
                while (Active)
                {
                    var message = await handler.ReceiveObjectAsync<Message>();
                    if(message.Type == MessageType.Sync)
                    {
                        var sync = (message.Body as JObject).Annotation<SyncMessage>();
                        Room.AddSync(sync.Snapshots.Where(snapshot => snapshot.ID == ID.ToString()).ToArray());
                    }
                }
            }
            catch(Exception ex)
            {
                ServerLog.Error($"Error when receive from [{Name}]: {ex.Message}");
                Close();
            }
        }
        async void StartSendInternal()
        {
            try
            {
                while (Active)
                {
                    await Task.Delay(30);
                    while (MessageQueue.Count > 0)
                    {
                        var message = MessageQueue.Peek();
                        await handler.SendObjectAsync(message);
                        MessageQueue.Dequeue();
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.Error($"Error when send to [{Name}]: {ex.Message}");
                Close();
            }
        }

        public void Sync(PlayerSnapShot[] snapShots)
        {
            MessageQueue.Enqueue(new Message()
            {
                Type = MessageType.Sync,
                Body = new SyncMessage()
                {
                    Snapshots = snapShots
                }
            });
        }
    }
}
