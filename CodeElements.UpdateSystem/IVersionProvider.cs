namespace CodeElements.UpdateSystem
{
	/// <summary>
	///     A provider for the application version
	/// </summary>
	public interface IVersionProvider
	{
		/// <summary>
		///     Get the current version of the application
		/// </summary>
		/// <returns>Return the current version of the application</returns>
		SemVersion GetVersion();
	}
}