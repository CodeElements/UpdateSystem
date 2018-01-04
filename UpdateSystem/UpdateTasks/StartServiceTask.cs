using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Start or restart a Windows service
	/// </summary>
	public class StartServiceTask : UpdateTask
	{
		/// <summary>
		///     The service name
		/// </summary>
		public string ServiceName { get; set; }

		/// <summary>
		///     The arguments the service should be started with
		/// </summary>
		public string Arguments { get; set; }

		/// <summary>
		///     Determines if the service should be restarted if it's already running
		/// </summary>
		public bool RestartIfAlreadyRunning { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.StartService" />
		/// </summary>
		public override UpdateTaskType Type { get; } = UpdateTaskType.StartService;
	}
}