using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Stop a Windows service
	/// </summary>
	public class StopServiceTask : UpdateTask
	{
		/// <summary>
		///     The service name
		/// </summary>
		public string ServiceName { get; set; }

		/// <summary>
		///     Determines if the updater should wait for the process to stop
		/// </summary>
		public bool WaitForExit { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.StopService" />
		/// </summary>
		public override UpdateTaskType Type { get; } = UpdateTaskType.StopService;
	}
}