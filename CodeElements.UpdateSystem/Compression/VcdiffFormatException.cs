using System;

namespace CodeElements.UpdateSystem.Compression
{
	/// <summary>
	/// Summary description for VcdiffFormatException.
	/// </summary>
	[Serializable()]
	public class VcdiffFormatException : Exception
	{
		internal VcdiffFormatException(string message) : base (message)
		{
		}

		internal VcdiffFormatException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
