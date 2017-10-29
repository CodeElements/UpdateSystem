namespace CodeElements.UpdateSystem.Client
{
	/// <summary>
	///     Use a preset version
	/// </summary>
	public class CustomVersionProvider : IVersionProvider
	{
		/// <summary>
		///     Initialize a new instance of <see cref="CustomVersionProvider" /> with a version
		/// </summary>
		/// <param name="version">The version that should be returned by the <see cref="IVersionProvider" />.</param>
		public CustomVersionProvider(SemVersion version)
		{
			Version = version;
		}

		/// <summary>
		///     Get current version
		/// </summary>
		public SemVersion Version { get; }

		SemVersion IVersionProvider.GetVersion()
		{
			return Version;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return "CustomVersion: " + Version;
		}
	}
}