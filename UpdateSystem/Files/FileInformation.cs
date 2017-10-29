using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
using Elements.Core;

#endif

namespace CodeElements.UpdateSystem.Files
{
	/// <summary>
	///		Information about a file
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class FileInformation : FileSystemEntry
	{
		/// <summary>
		///		The hash value of this file
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex)]
#endif
		[JsonProperty(PropertyName = "hash")]
		public virtual Hash Hash { get; set; }

		/// <summary>
		///		The length of the file in bytes
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 1)]
#endif
		[JsonProperty(PropertyName = "length")]
		public virtual int Length { get; set; }
	}
}