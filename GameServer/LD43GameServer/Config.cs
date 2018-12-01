using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LD43GameServer
{
    [JsonObject]
    public class Config
    {
        [JsonProperty("host")]
        public string Host;

        [JsonProperty("port")]
        public int Port;

        [JsonProperty("saveFolder")]
        public string SaveFolder;
    }
}
