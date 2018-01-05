using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Core.Internal
{
    internal class UserSessionDto
    {
        [JsonProperty("v")]
        public SemVersion Version { get; set; }

        [JsonProperty("c")]
        public string UserLanguage { get; set; }

        [JsonProperty("s")]
        public int OperatingSystem { get; set; }

        [JsonProperty("h")]
        public Hash HardwareId { get; set; }
    }
}