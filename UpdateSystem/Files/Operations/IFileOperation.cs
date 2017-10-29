using CodeElements.UpdateSystem.Files.Operations.Internal;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Files.Operations
{
	/// <summary>
	///     Defines a simple file operation
	/// </summary>
	[JsonConverter(typeof(FileOperationConverter))]
	public interface IFileOperation
	{
		/// <summary>
		///     The type of the file operation
		/// </summary>
		FileOperationType OperationType { get; }
	}
}