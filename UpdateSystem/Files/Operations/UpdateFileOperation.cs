namespace CodeElements.UpdateSystem.Files.Operations
{
	/// <summary>
	///     Update an existing file
	/// </summary>
	public class UpdateFileOperation : IFileOperation, INeedDownload
	{
		/// <summary>
		///     The location of the file
		/// </summary>
		public FileInformation Target { get; set; }

		/// <summary>
		///     <see cref="FileOperationType.Update" />
		/// </summary>
		public FileOperationType OperationType { get; } = FileOperationType.Update;
	}
}