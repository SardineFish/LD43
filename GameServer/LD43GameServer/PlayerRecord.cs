using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LD43GameServer
{
    [JsonObject]
    public class PlayerRecord
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("msg")]
        public string LeaveMessage;
        [JsonProperty("records")]
        public PlayerSnapShot[] Records;
    }
}
