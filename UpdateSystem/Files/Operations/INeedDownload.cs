namespace CodeElements.UpdateSystem.Files.Operations
{
	public interface INeedDownload
	{
		/// <summary>
		///     The location of the file
		/// </summary>
		FileInformation Target { get; }
	}
}