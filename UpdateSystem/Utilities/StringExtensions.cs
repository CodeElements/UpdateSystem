using System;
using System.Linq;

namespace CodeElements.UpdateSystem.Utilities
{
	/// <summary>
	///     Extensions for <see cref="string" />
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		///     Convert a hex string to a byte array
		/// </summary>
		/// <param name="source">The hex string</param>
		/// <returns>Return the byte array from the hex string</returns>
		public static byte[] ToByteArray(this string source)
		{
			return
				Enumerable.Range(0, source.Length / 2)
					.Select(x => Convert.ToByte(source.Substring(x * 2, 2), 16))
					.ToArray();
		}
	}
}