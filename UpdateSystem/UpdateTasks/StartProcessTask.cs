using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Start a new process
	/// </summary>
	public class StartProcessTask : UpdateTask
	{
		/// <summary>
		///     The path to the process (standard path variables supported)
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		///     The arguments the process should be started with
		/// </summary>
		public string Arguments { get; set; }

		/// <summary>
		///     Determines if the process required elevated privileges (administrator/root)
		/// </summary>
		public bool RequireElevatedPrivileges { get; set; }

		/// <summary>
		///     Determines if the updater should wait for the process to exit
		/// </summary>
		public bool WaitForExit { get; set; }

		/// <summary>
		///     If true, the update process fails when the exit code of the process is not 0
		/// </summary>
		public bool FailIfProcessReturnsFailure { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.StartProcess" />
		/// </summary>
		public override UpdateTaskType Type { get; } = UpdateTaskType.StartProcess;
	}
}