using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Core.Internal
{
    internal class UserSessionDto
    {
        [JsonProperty("c")]
        public string UserLanguage { get; set; }

        [JsonProperty("s")]
        public int OperatingSystem { get; set; }
    }
}