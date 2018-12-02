using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LD43GameServer
{
    public class GameServer: Singleton<GameServer>
    {
        public GameHost GameHost;

        public void Start()
        {
            GameHost = new GameHost();
        }

        public bool Join(Player player)
        {
            return GameHost.Join(player);
        }
    }
}
