using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Execute PowerShell code
	/// </summary>
	public class ExecutePowerShellScriptTask : UpdateTask
	{
		/// <summary>
		///     The code to execute
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.ExecutePowerShellScript" />
		/// </summary>
		public override UpdateTaskType Type { get; } = UpdateTaskType.ExecutePowerShellScript;
	}
}