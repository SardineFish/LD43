using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LD43GameServer
{
    public static class GameServer
    {
        public static GameHost GameHost;

        public static void Start()
        {
            GameHost = new GameHost();
        }


    }
}
