using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LD43GameServer
{
    public enum PlayerStatus
    {
        Alive,
        Hang,
        Dead,
    }
    public class Player
    {
        public string Name;
        public PlayerStatus Status = PlayerStatus.Alive;
        public Guid ID;
        public bool Active { get; private set; }
        internal GameRoom Room = null;
        public float LastUpdateTime = 0;
        WebSocketHandler handler;
        Task ReceiveTask;
        Task SendTask;
        Queue<Message> MessageQueue = new Queue<Message>();

        public Player(WebSocketHandler handler)
        {
            this.handler = handler;
            ID = Guid.NewGuid();
        }
        public Task Start()
        {
            Active = true;
            SendTask = StartSendInternal();
            ReceiveTask = StartReceiveInternal();
            return ReceiveTask;
        }
        public void Close(PlayerStatus status)
        {
            Status = status;
            if (Active)
            {
                Active = false;
                handler.Close();
            }
        }
        async Task StartReceiveInternal()
        {
            try
            {
                var handshake = await handler.ReceiveObjectAsync<Message>();
                if (handshake.Type == MessageType.HandShake)
                {
                    var msg = (handshake.Body as JObject).ToObject<ClientHandShakeMessage>();
                    if (msg.Type == HandShakeType.Join)
                    {
                        Name = msg.Name;
                        if(!GameServer.Instance.Join(this))
                        {
                            Close(PlayerStatus.Dead);
                            return;
                        }
                        ServerLog.Log($"Handshake with {Name}");
                        var response = new ServerHandShakeMessage()
                        {
                            ID = this.ID.ToString(),
                            RoomID = this.Room.ID.ToString()
                        };
                        MessageQueue.Enqueue(new Message()
                        {
                            Type = MessageType.HandShake,
                            Body = response
                        });
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
                        var sync = (message.Body as JObject).ToObject<SyncMessage>();
                        Room.AddSync(sync.Snapshots.Where(snapshot => snapshot.ID == ID.ToString()).ToArray());
                    }
                }
            }
            catch(Exception ex)
            {
                ServerLog.Error($"Error when receive from [{Name}]: {ex.Message}");
                Close(PlayerStatus.Hang);
            }
        }
        async Task StartSendInternal()
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
                Close(PlayerStatus.Hang);
            }
            MessageQueue = null;
        }

        public void Sync(int serverTick,PlayerSnapShot[] snapShots)
        {
            MessageQueue.Enqueue(new Message()
            {
                Type = MessageType.Sync,
                Body = new SyncMessage()
                {
                    ServerTick = serverTick,
                    Snapshots = snapShots
                }
            });
        }
    }
}
