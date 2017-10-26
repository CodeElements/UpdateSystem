using System.Reflection;
using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeElements.UpdateSystem.Core.Internal
{
    internal class UpdateTaskSignatureDataContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            switch (property.PropertyName)
            {
                case nameof(UpdateTask.Signature):
                    property.ShouldSerialize = o => false;
                    break;
            }

            return property;
        }
    }
}