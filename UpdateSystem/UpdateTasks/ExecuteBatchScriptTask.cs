using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Execute batch code
	/// </summary>
	public class ExecuteBatchScriptTask : UpdateTask
	{
		/// <summary>
		///     The batch code to execute
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.ExecuteBatchScript" />
		/// </summary>
		public override UpdateTaskType Type { get; } = UpdateTaskType.ExecuteBatchScript;
	}
}