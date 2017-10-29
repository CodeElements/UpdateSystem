using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Start a new process
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class StartProcessTask : UpdateTask
	{
		/// <summary>
		///     The path to the process (standard path variables supported)
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex)]
#endif
		[JsonProperty(PropertyName = "filename")]
		public virtual string Filename { get; set; }

		/// <summary>
		///     The arguments the process should be started with
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 1)]
#endif
		[JsonProperty(PropertyName = "arguments")]
		public virtual string Arguments { get; set; }

		/// <summary>
		///     Determines if the process required elevated privileges (administrator/root)
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 2)]
#endif
		[JsonProperty(PropertyName = "requireElevatedPrivileges")]
		public virtual bool RequireElevatedPrivileges { get; set; }

		/// <summary>
		///     Determines if the updater should wait for the process to exit
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 3)]
#endif
		[JsonProperty(PropertyName = "waitForExit")]
		public virtual bool WaitForExit { get; set; }

		/// <summary>
		///     If true, the update process fails when the exit code of the process is not 0
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 4)]
#endif
		[JsonProperty(PropertyName = "failIfProcessReturnsFailure")]
		public virtual bool FailIfProcessReturnsFailure { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.StartProcess" />
		/// </summary>
#if ZEROFORMATTABLE
		[IgnoreFormat]
#endif
		public override UpdateTaskType TaskType { get; } = UpdateTaskType.StartProcess;
	}
}