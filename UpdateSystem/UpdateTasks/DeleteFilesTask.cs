using System.Collections.Generic;
using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Delete files from a directory
	/// </summary>
	public class DeleteFilesTask : UpdateTask
	{
		/// <summary>
		///     The path of the directory (standard path variables supported)
		/// </summary>
		public string DirectoryPath { get; set; }

		/// <summary>
		///     The files which should be removed (relative names to <see cref="DirectoryPath" />)
		/// </summary>
		public List<string> Filenames { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.DeleteFiles" />
		/// </summary>
		public override UpdateTaskType Type { get; } = UpdateTaskType.DeleteFiles;
	}
}