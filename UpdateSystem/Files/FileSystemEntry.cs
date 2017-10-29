using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.Files
{
	/// <summary>
	///     A defined file location in the UpdateSystem
	/// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
#endif
	public class FileSystemEntry
	{
#if ZEROFORMATTABLE
		protected const int StartIndex = 3;
#endif

		/// <summary>
		///     The filename
		/// </summary>
#if ZEROFORMATTABLE
		[Index(0)]
#endif
		[JsonProperty(PropertyName = "filename")]
		public virtual string Filename { get; set; }

		/// <summary>
		///     The platforms of this file
		/// </summary>
#if ZEROFORMATTABLE
		[Index(1)]
#endif
		[JsonProperty(PropertyName = "platforms")]
		public virtual int Platforms { get; set; }

		/// <summary>
		///     True if this file is a temporary file
		/// </summary>
#if ZEROFORMATTABLE
		[Index(2)]
#endif
		[JsonProperty(PropertyName = "isTempFile")]
		public virtual bool IsTempFile { get; set; }
	}
}