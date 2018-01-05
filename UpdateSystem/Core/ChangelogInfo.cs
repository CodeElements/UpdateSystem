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
		public string Language { get; set; }

        /// <summary>
        ///     The culture of the changelog
        /// </summary>
        [JsonIgnore]
	    public CultureInfo CultureInfo => new CultureInfo(Language);

		/// <summary>
		///     The content of the changelog, eventually formatted with MarkDown
		/// </summary>
		public string Content { get; set; }
	}
}