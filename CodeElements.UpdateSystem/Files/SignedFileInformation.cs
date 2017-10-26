using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.Files
{
	/// <summary>
	///     Information about a file together with a signature
	/// </summary>
	public class SignedFileInformation : FileInformation
	{
		/// <summary>
		///     The signature data
		/// </summary>
#if ZEROFORMATTABLE
		[Index(StartIndex + 2)]
#endif
		[JsonProperty(PropertyName = "signature")]
		public virtual byte[] Signature { get; set; }
	}
}