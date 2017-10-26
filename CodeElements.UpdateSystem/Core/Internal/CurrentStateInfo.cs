using Newtonsoft.Json;

#if SERVERIMPL
using Elements.Core;

#endif

namespace CodeElements.UpdateSystem.Core.Internal
{
    internal class CurrentStateInfo
    {
        [JsonProperty(PropertyName = "currentVersion")]
        public SemVersion CurrentVersion { get; set; }

        [JsonProperty(PropertyName = "platforms")]
        public int Platforms { get; set; }
    }
}