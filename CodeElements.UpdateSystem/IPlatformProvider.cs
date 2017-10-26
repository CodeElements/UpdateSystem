namespace CodeElements.UpdateSystem
{
	/// <summary>
	///     Provide the encoded platform to determine which files are relevant for this install
	/// </summary>
	public interface IPlatformProvider
	{
		/// <summary>
		///     Get the encoded platforms
		/// </summary>
		/// <returns>Return the platforms encoded as <see cref="int" /></returns>
		int GetEncodedPlatforms();
	}
}