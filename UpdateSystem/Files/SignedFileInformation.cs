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
		public byte[] Signature { get; set; }
	}
}