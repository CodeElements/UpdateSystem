using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Files.Operations
{
	/// <summary>
	///     Delete a file
	/// </summary>
	public class DeleteFileOperation : IFileOperation
	{
		/// <summary>
		///     The target file that should be removed
		/// </summary>
		[JsonProperty(PropertyName = "target")]
		public FileInformation Target { get; set; }

		/// <summary>
		///     <see cref="FileOperationType.Delete" />
		/// </summary>
		public FileOperationType OperationType { get; } = FileOperationType.Delete;
	}
}