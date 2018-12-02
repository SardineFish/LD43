using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LD43GameServer
{
    public class GameHost
    {
        public Dictionary<Guid, Player> Players = new Dictionary<Guid, Player>();
        public List<GameRoom> Rooms = new List<GameRoom>();
        public Dictionary<Guid, PlayerRecord> PlayerRecords = new Dictionary<Guid, PlayerRecord>();

        

        public bool Join(Player player)
        {
            Players[player.ID] = player;
            if (Rooms.Count <= 0 || !Rooms[Rooms.Count - 1].Join(player))
            {
                var room = AddRoom(PlayerRecords.Values.ToArray());
                room.Join(player);
                return true;
            }
            return true;
        }

        public bool AddRecord(Guid playerID, PlayerRecord record)
        {
            if (!Players.ContainsKey(playerID))
                return false;
            Players[playerID].Status = PlayerStatus.Dead;
            Players[playerID].Close(PlayerStatus.Dead);
            lock (PlayerRecords)
            {
                PlayerRecords[playerID] = record;
            }
            return true;
        }

        public GameRoom AddRoom(PlayerRecord[] records)
        {
            var room = new GameRoom(records);
            lock (Rooms)
            {
                Rooms.Add(room);
            }
            room.Start();
            ServerLog.Log($"New room created with {records.Length} records id={{{room.ID}}}.");
            return room;
        }

        public void RemoveRoom(Guid roomID)
        {
            GameRoom room;
            lock (Rooms)
            {
                room = this.Rooms.Where(r => r.ID == roomID).FirstOrDefault();
                this.Rooms.Remove(room);
            }
            room.Close();
        }
    }
}
