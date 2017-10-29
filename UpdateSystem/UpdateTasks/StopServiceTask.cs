using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Stop a Windows service
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class StopServiceTask : UpdateTask
	{
		/// <summary>
		///     The service name
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex)]
#endif
		[JsonProperty(PropertyName = "serviceName")]
		public virtual string ServiceName { get; set; }

		/// <summary>
		///     Determines if the updater should wait for the process to stop
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 1)]
#endif
		[JsonProperty(PropertyName = "waitForExit")]
		public virtual bool WaitForExit { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.StopService" />
		/// </summary>
#if ZEROFORMATTABLE
		[IgnoreFormat]
#endif
		public override UpdateTaskType TaskType { get; } = UpdateTaskType.StopService;
	}
}