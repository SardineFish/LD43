using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LD43GameServer
{
    public class GameServer: Singleton<GameServer>
    {
        public GameHost GameHost;
        Dictionary<Guid, string> Players = new Dictionary<Guid, string>();

        public void Start()
        {
            GameHost = new GameHost();
        }

        public bool Join(Player player)
        {
            if (Players.ContainsKey(player.ID))
                return false;
            Players[player.ID] = player.Name;
            return GameHost.Join(player);
        }

        public bool CheckPlayer(Guid id)
        {
            return Players.ContainsKey(id);
        }

        public bool Record(PlayerRecord record)
        {
            GameHost.AddRecord(record.ID, record);
            return true;
        }

        public PlayerRecord[] GetRecordsFromRoom(Guid roomID)
        {
            var room = GameHost.Rooms.Where(r => r.ID == roomID).FirstOrDefault();
            if (room == null)
                return null;
            return room.Records;
        }
    }
}
