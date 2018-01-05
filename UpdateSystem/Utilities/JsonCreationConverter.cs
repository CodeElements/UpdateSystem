using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeElements.UpdateSystem.Utilities
{
    /// <summary>
    ///     Base Generic JSON Converter that can help quickly define converters for specific types by automatically
    ///     generating the CanConvert, ReadJson, and WriteJson methods, requiring the implementer only to define a strongly
    ///     typed Create method.
    /// </summary>
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        public override bool CanWrite { get; } = false;

        /// <summary>Create an instance of objectType, based properties in the JSON object</summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">contents of JSON object that will be deserialized</param>
        protected abstract T Create(Type objectType, JObject jObject);

        /// <summary>Determines if this converted is designed to deserialization to objects of the specified type.</summary>
        /// <param name="objectType">The target type for deserialization.</param>
        /// <returns>True if the type is supported.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(objectType);
        }

        /// <summary>Parses the json to the specified type.</summary>
        /// <param name="reader">Newtonsoft.Json.JsonReader</param>
        /// <param name="objectType">Target type.</param>
        /// <param name="existingValue">Ignored</param>
        /// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
        /// <returns>Deserialized Object</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            //Create a new reader for this jObject, and set all properties to match the original reader.
            using (var jObjectReader = CopyReaderForObject(reader, jObject))
            {
                serializer.Populate(jObjectReader, target);
            }

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Creates a new reader for the specified jObject by copying the settings
        ///     from an existing reader.
        /// </summary>
        /// <param name="reader">The reader whose settings should be copied.</param>
        /// <param name="jObject">The jObject to create a new reader for.</param>
        /// <returns>The new disposable reader.</returns>
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