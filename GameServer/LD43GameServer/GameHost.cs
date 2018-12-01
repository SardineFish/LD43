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
            return false;
        }

        public void AddRecord(Guid playerID, PlayerSnapShot[] snapshots)
        {
            lock (PlayerRecords)
            {
                PlayerRecords[playerID] = snapshots;
            }
        }
    }
}
