using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Files.Operations
{
	/// <summary>
	///     Download a new file
	/// </summary>
	public class DownloadFileOperation : IFileOperation, INeedDownload
	{
		/// <summary>
		///     The location of the file
		/// </summary>
		[JsonProperty(PropertyName = "target")]
		public FileInformation Target { get; set; }

		/// <summary>
		///     <see cref="FileOperationType.Download" />
		/// </summary>
		public FileOperationType OperationType { get; } = FileOperationType.Download;
	}
}