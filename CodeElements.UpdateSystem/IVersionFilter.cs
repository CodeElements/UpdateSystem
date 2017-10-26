namespace CodeElements.UpdateSystem
{
	/// <summary>
	///     Filter the versions that should be found
	/// </summary>
	public interface IVersionFilter
	{
		/// <summary>
		///     Get the supported pre-releases. The return strings are case insensitive and must match the
		///     <see cref="SemVersion.Prerelease" /> part of the version in order to find the update package. Releases (
		///     <see cref="SemVersion.Prerelease" /> = null) are always found.
		/// </summary>
		/// <returns>Return the supported pre-releases</returns>
		string[] GetSupportedPrereleases();
	}
}