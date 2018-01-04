using CodeElements.UpdateSystem.UpdateTasks.Base;
namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Delete a directory
	/// </summary>
	public class DeleteDirectoryTask : UpdateTask
	{
		/// <summary>
		///     The path of the directory (standard path variables supported)
		/// </summary>
		public string DirectoryPath { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.DeleteDirectory" />
		/// </summary>
		public override UpdateTaskType Type { get; } = UpdateTaskType.DeleteDirectory;
	}
}