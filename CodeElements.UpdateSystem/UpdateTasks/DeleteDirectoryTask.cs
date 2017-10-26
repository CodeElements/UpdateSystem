using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Delete a directory
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class DeleteDirectoryTask : UpdateTask
	{
		/// <summary>
		///     The path of the directory (standard path variables supported)
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex)]
#endif
		[JsonProperty(PropertyName = "directoryPath")]
		public virtual string DirectoryPath { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.DeleteDirectory" />
		/// </summary>
#if ZEROFORMATTABLE
		[IgnoreFormat]
#endif
		public override UpdateTaskType TaskType { get; } = UpdateTaskType.DeleteDirectory;
	}
}