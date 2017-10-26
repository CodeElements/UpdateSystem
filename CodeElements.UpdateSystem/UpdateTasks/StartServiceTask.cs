using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Start or restart a Windows service
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class StartServiceTask : UpdateTask
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
		///     The arguments the service should be started with
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 1)]
#endif
		[JsonProperty(PropertyName = "arguments")]
		public virtual string Arguments { get; set; }

		/// <summary>
		///     Determines if the service should be restarted if it's already running
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 2)]
#endif
		[JsonProperty(PropertyName = "restartIfAlreadyRunning")]
		public virtual bool RestartIfAlreadyRunning { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.StartService" />
		/// </summary>
#if ZEROFORMATTABLE
		[IgnoreFormat]
#endif
		public override UpdateTaskType TaskType { get; } = UpdateTaskType.StartService;
	}
}