﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LD43GameServer
{
    public class GameRoom
    {
        const float FrameTime = 1 / 60;
        const float WaitTiime = 5;
        const float JoinTime = 5;
        public Guid ID = Guid.NewGuid();
        public Dictionary<Guid, Player> Players = new Dictionary<Guid, Player>();
        public PlayerRecord[] Records;
        public int GameTick = 0;
        public float GameTime = 0;
        public bool Active { get; private set; }
        public List<PlayerSnapShot> PendingSync = new List<PlayerSnapShot>();
        public List<PlayerSnapShot> SendingSync;
        Thread GameThread;

        public GameRoom(PlayerRecord[] records)
        {
            Records = records;
        }

        public bool Join(Player player)
        {
            if (GameTime > JoinTime)
                return false;
            lock(Players)
            {
                Players.Add(player.ID, player);
            }
            player.Room = this;
            ServerLog.Log($"Player {player.Name} join room {ID}");
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
        public void Close()
        {
            Active = false;
        }

        void StartInternal()
        {
            GameTime = 0;
            GameTick = 0;
            Thread.Sleep(TimeSpan.FromSeconds(WaitTiime));
            lock (Players)
            {
                foreach(var pair in Players)
                {
                    pair.Value.Sync(0, new PlayerSnapShot[0]);
                }
            }
            while (Active)
            {
                Thread.Sleep(TimeSpan.FromSeconds(FrameTime));
                GameTime += FrameTime;
                GameTick++;
                lock (PendingSync)
                {
                    SendingSync = PendingSync;
                    PendingSync = new List<PlayerSnapShot>();
                }
                var snapshots = SendingSync.ToArray();
                bool shouldClose = true;
                lock (Players)
                {
                    foreach (var pair in Players)
                    {
                        var player = pair.Value;
                        if (player.Status == PlayerStatus.Alive)
                        {
                            shouldClose = false;
                            player.Sync(GameTick, snapshots);
                        }
                        else if (player.Status == PlayerStatus.Hang && GameTime - player.LastUpdateTime <= WaitTiime)
                        {
                            shouldClose = false;
                        }
                    }
                }
                if (shouldClose)
                    Close();
                break;
            }
            lock (Players)
            {
                foreach (var pair in Players)
                {
                    pair.Value.Close(PlayerStatus.Dead);
                }
            }
            GameServer.Instance.GameHost.RemoveRoom(this.ID);
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