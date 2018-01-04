using System.Globalization;

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
		public CultureInfo Language { get; set; }

		/// <summary>
		///     The content of the changelog, eventually formatted with MarkDown
		/// </summary>
		public string Content { get; set; }
	}
}