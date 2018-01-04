using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Stop a process
	/// </summary>
	public class KillProcessTask : UpdateTask
	{
		/// <summary>
		///     The mode how the <see cref="SearchString" /> should be used to find the process
		/// </summary>
		public ProcessSearchMode SearchMode { get; set; }

		/// <summary>
		///     The search string, matching to the <see cref="SearchMode" />
		/// </summary>
		public string SearchString { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.KillProcess" />
		/// </summary>
		public override UpdateTaskType Type { get; } = UpdateTaskType.KillProcess;
	}

	/// <summary>
	///     Determines how a process should be found
	/// </summary>
	public enum ProcessSearchMode
	{
		/// <summary>
		///     The exact process name is matched
		/// </summary>
		ProcessName,

		/// <summary>
		///     The process name should contain the term
		/// </summary>
		ProcessNameContains,

		/// <summary>
		///     The process path should match the term
		/// </summary>
		Filename
	}
}