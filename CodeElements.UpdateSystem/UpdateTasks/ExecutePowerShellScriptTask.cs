using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Execute PowerShell code
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class ExecutePowerShellScriptTask : UpdateTask
	{
		/// <summary>
		///     The code to execute
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex)]
#endif
		[JsonProperty(PropertyName = "code")]
		public virtual string Code { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.ExecutePowerShellScript" />
		/// </summary>
#if ZEROFORMATTABLE
		[IgnoreFormat]
#endif
		public override UpdateTaskType TaskType { get; } = UpdateTaskType.ExecutePowerShellScript;
	}
}