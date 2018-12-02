using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MultiPlayer
{
    public enum HandShakeType
    {
        Join,
        Reconnect,
    }
    [JsonObject]
    public class ClientHandShakeMessage : MessageBody
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public HandShakeType Type;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("id")]
        public string ID;

        [JsonProperty("room")]
        public string RoomID;

    }
    public class ServerHandShakeMessage : MessageBody
    {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("room")]
        public string RoomID;
    }
}
