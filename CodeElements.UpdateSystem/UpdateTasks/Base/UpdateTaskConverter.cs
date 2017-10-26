using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeElements.UpdateSystem.UpdateTasks.Base
{
	internal class UpdateTaskConverter : JsonConverter
	{
		public override bool CanWrite { get; } = false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			var jo = JObject.Load(reader);
			var taskType = (UpdateTaskType) (int) jo["type"];

		    if (UpdateTask.UpdateTaskTypes.TryGetValue(taskType, out var updateTaskType))
		        return serializer.Deserialize(reader, updateTaskType);

		    return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(UpdateTask);
		}
	}
}