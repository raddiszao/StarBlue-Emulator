using Newtonsoft.Json;

namespace StarBlue.WebSocket
{
    class WebEvent
    {
        [JsonProperty(PropertyName = "UserId")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "SSO")]
        public string SSO { get; set; }

        [JsonProperty(PropertyName = "EventName")]
        public string EventName { get; set; }

        [JsonProperty(PropertyName = "ExtraData")]
        public string ExtraData { get; set; }

        [JsonProperty(PropertyName = "Bypass")]
        public bool Bypass { get; set; }

        [JsonProperty(PropertyName = "JSON")]
        public bool IsJSON { get; set; }
        public WebEvent(int UserId, string SSO, string EventName, string ExtraData, bool Bypass, bool IsJSON)
        {
            this.UserId = UserId;
            this.SSO = SSO;
            this.EventName = EventName;
            this.ExtraData = ExtraData;
            this.Bypass = Bypass;
            this.IsJSON = IsJSON;
        }
    }
}
