using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#if ELEMENTSCORE
namespace CodeElements.UpdateSystem.UpdateTasks.Base
{
    public class UpdateTaskSignatureDataContractResolver : CamelCasePropertyNamesContractResolver
    {
#else
using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.Core.Internal
{
    internal class UpdateTaskSignatureDataContractResolver : CamelCasePropertyNamesContractResolver
    {
#endif

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.PropertyName == ToCamelCase(nameof(UpdateTask.Signature)))
                property.ShouldSerialize = o => false;

            return property;
        }

        private static string ToCamelCase(string name)
        {
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}