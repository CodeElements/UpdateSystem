using System.Collections.Generic;

namespace CodeElements.UpdateSystem.Files.Operations
{
	/// <summary>
	///     Move a file to specific locations
	/// </summary>
	public class MoveFileOperation : IFileOperation
	{
		/// <summary>
		///     The file from the source file base
		/// </summary>
		public FileInformation SourceFile { get; set; }

		/// <summary>
		///     The targets that the <see cref="SourceFile" /> should be moved to. If this collection contains more than 1 item,
		///     the file must obviously be copied once
		/// </summary>
		public List<FileInformation> Targets { get; set; }

		/// <summary>
		///     <see cref="FileOperationType.MoveFile" />
		/// </summary>
		public FileOperationType OperationType { get; } = FileOperationType.MoveFile;
	}
}