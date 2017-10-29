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
			var fileOperation = (IFileOperation) value;
			serializer.Serialize(writer, value);
			writer.WriteToken(JsonToken.Integer, (int) fileOperation.OperationType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jo = JObject.Load(reader);
			var taskType = (FileOperationType) (int) jo["type"];
			Type fileOperationType;
			switch (taskType)
			{
				case FileOperationType.MoveFile:
					fileOperationType = typeof(MoveFileOperation);
					break;
				case FileOperationType.Update:
					fileOperationType = typeof(UpdateFileOperation);
					break;
				case FileOperationType.Delete:
					fileOperationType = typeof(DeleteFileOperation);
					break;
				case FileOperationType.Download:
					fileOperationType = typeof(DownloadFileOperation);
					break;
				case FileOperationType.DeltaPatch:
					fileOperationType = typeof(DeltaPatchOperation);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return serializer.Deserialize(reader, fileOperationType);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.GetTypeInfo().IsAssignableFrom(typeof(IFileOperation));
		}
	}
}