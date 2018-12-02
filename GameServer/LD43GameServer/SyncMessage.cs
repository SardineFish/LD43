using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LD43GameServer
{
    [JsonObject]
    public class SyncMessage: MessageBody
    {
        [JsonProperty("tick")]
        public int ServerTick;

        [JsonProperty("data")]
        public PlayerSnapShot[] Snapshots;
    }

    [JsonObject]
    public class PlayerSnapShot
    {
        [JsonProperty("tick")]
        public int Tick;

        [JsonProperty("id")]
        public string ID;

        [JsonProperty("pos")]
        public double[] Position;

        [JsonProperty("v")]
        public double[] Velocity;

        [JsonProperty("ctrl")]
        public PlayerControl Control;
    }

    [JsonObject]
    public class PlayerControl
    {
        [JsonProperty("action")]
        public int Action;
        [JsonProperty("dir")]
        public float direction;
    }
}
