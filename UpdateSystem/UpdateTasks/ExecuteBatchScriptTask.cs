using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Execute batch code
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class ExecuteBatchScriptTask : UpdateTask
	{
		/// <summary>
		///     The batch code to execute
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex)]
#endif
		[JsonProperty(PropertyName = "code")]
		public virtual string Code { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.ExecuteBatchScript" />
		/// </summary>
#if ZEROFORMATTABLE
		[IgnoreFormat]
#endif
		public override UpdateTaskType TaskType { get; } = UpdateTaskType.ExecuteBatchScript;
	}
}