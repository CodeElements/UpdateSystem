using System.Globalization;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Core
{
	/// <summary>
	///     A changelog of an <see cref="UpdatePackageInfo" />
	/// </summary>
	public class ChangelogInfo
	{
		/// <summary>
		///     The language of the changelog
		/// </summary>
		[JsonProperty(PropertyName = "language")]
		public CultureInfo Language { get; set; }

		/// <summary>
		///     The content of the changelog, eventually formatted with MarkDown
		/// </summary>
		[JsonProperty(PropertyName = "content")]
		public string Content { get; set; }
	}
}