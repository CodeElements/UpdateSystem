using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Windows.Patcher
{
    [JsonConverter(typeof(RevertableConverter))]
    internal interface IRevertable
    {
        RevertableType Type { get; }
        void Revert();
    }
}