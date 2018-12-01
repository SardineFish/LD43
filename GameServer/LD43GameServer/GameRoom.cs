using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LD43GameServer
{
    public class GameRoom
    {
        public Guid ID;
        public Dictionary<Guid, Player> Players = new Dictionary<Guid, Player>();
        public int GameTick = 0;
        public bool Active { get; private set; }
        public List<PlayerSnapShot> PendingSync = new List<PlayerSnapShot>();
        public List<PlayerSnapShot> SendingSync;
        Thread GameThread;

        public bool Join(Player player)
        {
            Players.Add(player.ID, player);
            return true;
        }

        public bool UpdatePlayer(Player player)
        {
            lock (Players)
            {
                if (!Players.ContainsKey(player.ID))
                    return false;
                Players[player.ID] = player;
                return true;
            }
        }

        public void Start()
        {
            Active = true;
            GameThread = new Thread(StartInternal);
            GameThread.Start();
        }

        void StartInternal()
        {
            lock (Players)
            {
                foreach(var pair in Players)
                {
                    pair.Value.Sync(new PlayerSnapShot[0]);
                }
            }
            while (Active)
            {
                Thread.Sleep(30);
                lock (PendingSync)
                {
                    SendingSync = PendingSync;
                    PendingSync = new List<PlayerSnapShot>();
                }
                var snapshots = SendingSync.ToArray();
                lock (Players)
                {
                    foreach (var pair in Players)
                    {
                        pair.Value.Sync(snapshots);
                    }
                }
            }
        }

        public void AddSync(PlayerSnapShot[] snapShots)
        {
            lock (PendingSync)
            {
                PendingSync.AddRange(snapShots);
            }
        }
    }
}
