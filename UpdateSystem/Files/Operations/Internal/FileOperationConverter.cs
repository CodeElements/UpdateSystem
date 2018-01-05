using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeElements.UpdateSystem.Files.Operations.Internal
{
	internal class FileOperationConverter : JsonConverter
	{
	    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	    {
	        throw new NotImplementedException();
	    }

	    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jo = JObject.Load(reader);
			var taskType = (FileOperationType) (int) jo["operationType"];
			IFileOperation fileOperation;
			switch (taskType)
			{
				case FileOperationType.MoveFile:
				    fileOperation = new MoveFileOperation();
					break;
				case FileOperationType.Update:
				    fileOperation = new UpdateFileOperation();
					break;
				case FileOperationType.Delete:
				    fileOperation = new DeleteFileOperation();
					break;
				case FileOperationType.Download:
				    fileOperation = new DownloadFileOperation();
					break;
				case FileOperationType.DeltaPatch:
				    fileOperation = new DeltaPatchOperation();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

		    using (JsonReader jObjectReader = CopyReaderForObject(reader, jo))
		        serializer.Populate(jObjectReader, fileOperation);

		    return fileOperation;
        }

	    public override bool CanWrite { get; } = false;

	    public override bool CanConvert(Type objectType)
		{
			return objectType.GetTypeInfo().IsAssignableFrom(typeof(IFileOperation));
		}

        /// <summary>Creates a new reader for the specified jObject by copying the settings
        /// from an existing reader.</summary>
        /// <param name="reader">The reader whose settings should be copied.</param>
        /// <param name="jObject">The jObject to create a new reader for.</param>
        /// <returns>The new disposable reader.</returns>
        //https://stackoverflow.com/questions/8030538/how-to-implement-custom-jsonconverter-in-json-net-to-deserialize-a-list-of-base/8031283#8031283
        private static JsonReader CopyReaderForObject(JsonReader reader, JObject jObject)
	    {
	        JsonReader jObjectReader = jObject.CreateReader();
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