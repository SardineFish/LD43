using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MultiPlayer
{
    public enum MessageType
    {
        HandShake,
        Sync
    }

    [JsonObject]
    public class Message
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type;

        [JsonProperty("message")]
        public object Body;
    }

    [JsonObject]
    public abstract class MessageBody
    {

    }
}
