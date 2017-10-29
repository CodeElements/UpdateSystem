using System.Collections.Generic;
using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks
{
	/// <summary>
	///     Delete files from a directory
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class DeleteFilesTask : UpdateTask
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
		///     The files which should be removed (relative names to <see cref="DirectoryPath" />)
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 1)]
#endif
		[JsonProperty(PropertyName = "filenames")]
		public virtual List<string> Filenames { get; set; }

		/// <summary>
		///     <see cref="UpdateTaskType.DeleteFiles" />
		/// </summary>
#if ZEROFORMATTABLE
		[IgnoreFormat]
#endif
		public override UpdateTaskType TaskType { get; } = UpdateTaskType.DeleteFiles;
	}
}