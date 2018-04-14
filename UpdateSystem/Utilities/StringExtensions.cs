using System;

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
	        if (source.Length % 2 == 1)
	            throw new ArgumentException("The binary key cannot have an odd number of digits", nameof(source));

	        int GetHexVal(char hex)
	        {
	            var isHex = hex >= '0' && hex <= '9' ||
	                        hex >= 'a' && hex <= 'f' ||
	                        hex >= 'A' && hex <= 'F';
	            if (!isHex)
	                throw new ArgumentException($"The char '{hex}' is not a valid hexadecimal character.",
	                    nameof(source));

	            return hex - (hex < 58 ? 48 : (hex < 97 ? 55 : 87));
	        }

	        var arr = new byte[source.Length >> 1];
	        for (var i = 0; i < source.Length >> 1; ++i)
	            arr[i] = (byte)((GetHexVal(source[i << 1]) << 4) + GetHexVal(source[(i << 1) + 1]));

	        return arr;
	    }
    }
}