using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LD43GameServer
{
    public class Singleton<T> where T:Singleton<T>
    {
        public static Singleton<T> Instance { get; private set; }
        public Singleton()
        {
            Instance = this;
        }
    }
}
