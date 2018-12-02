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
        public Dictionary<Guid, PlayerSnapShot[]> PlayerRecords = new Dictionary<Guid, PlayerSnapShot[]>();

        

        public bool Join(Player player)
        {
            if (Rooms.Count <= 0 || !Rooms[Rooms.Count-1].Join(player))
            {
                var room = AddRoom();
                room.Join(player);
                return true;
            }
            return true;
        }

        public void AddRecord(Guid playerID, PlayerSnapShot[] snapshots)
        {
            lock (PlayerRecords)
            {
                PlayerRecords[playerID] = snapshots;
            }
        }

        public GameRoom AddRoom()
        {
            var room = new GameRoom();
            lock (Rooms)
            {
                Rooms.Add(room);
            }
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
