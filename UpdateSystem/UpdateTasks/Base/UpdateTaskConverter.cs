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

            Type updateTaskType;
            if (objectType == typeof(UpdateTask))
            {
                if (!UpdateTask.UpdateTaskTypes.TryGetValue((UpdateTaskType) (int) jo["type"], out updateTaskType))
                    return null;
            }
            else updateTaskType = objectType;

            var result = Activator.CreateInstance(updateTaskType);
            using (var jObjectReader = CopyReaderForObject(reader, jo))
                serializer.Populate(jObjectReader, result);

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UpdateTask);
        }

        /// <summary>
        ///     Creates a new reader for the specified jObject by copying the settings
        ///     from an existing reader.
        /// </summary>
        /// <param name="reader">The reader whose settings should be copied.</param>
        /// <param name="jObject">The jObject to create a new reader for.</param>
        /// <returns>The new disposable reader.</returns>
        //Source: https://stackoverflow.com/questions/8030538/how-to-implement-custom-jsonconverter-in-json-net-to-deserialize-a-list-of-base
        public static JsonReader CopyReaderForObject(JsonReader reader, JObject jObject)
        {
            var jObjectReader = jObject.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateFormatString = reader.DateFormatString;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;
            jObjectReader.MaxDepth = reader.MaxDepth;
            jObjectReader.SupportMultipleContent = reader.SupportMultipleContent;
            return jObjectReader;
        }
    }
}