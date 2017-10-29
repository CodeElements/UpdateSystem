using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Stop a process
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class KillProcessTask : UpdateTask
	{
		/// <summary>
		///     The mode how the <see cref="SearchString" /> should be used to find the process
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex)]
#endif
		[JsonProperty(PropertyName = "searchMode")]
		public virtual ProcessSearchMode SearchMode { get; set; }

		/// <summary>
		///     The search string, matching to the <see cref="SearchMode" />
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 1)]
#endif
		[JsonProperty(PropertyName = "searchString")]
		public virtual string SearchString { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.KillProcess" />
		/// </summary>
#if ZEROFORMATTABLE
		[IgnoreFormat]
#endif
		public override UpdateTaskType TaskType { get; } = UpdateTaskType.KillProcess;
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