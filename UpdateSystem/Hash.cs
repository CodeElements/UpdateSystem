using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using CodeElements.UpdateSystem.Utilities;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem
{
    public class HashTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
                return Hash.Parse(stringValue);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return value.ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

	/// <summary>
	///     Represents a hash value
	/// </summary>
	[Serializable]
	[JsonConverter(typeof(HashConverter))]
    [TypeConverter(typeof(HashTypeConverter))]
	public class Hash : IEquatable<Hash>
	{
		/// <summary>
		///     Initialize a new instance of <see cref="Hash" />
		/// </summary>
		/// <param name="hashData">The data of the hash</param>
		public Hash(byte[] hashData)
		{
		    if (hashData == null)
		        throw new ArgumentNullException(nameof(hashData));
		    if (hashData.Length == 0)
		        throw new ArgumentException("An empty array cannot represent a hash value.");

			HashData = hashData;
		}

		/// <summary>
		///     The data of the hash
		/// </summary>
		public byte[] HashData { get; }

		/// <summary>
		///     True if the hash length matches the length of a SHA256 hash (32 bytes/256 bits)
		/// </summary>
		public bool IsSha256Hash => HashData.Length == 32;

		/// <summary>
		///     Returns a value indicating whether this instance and a specified <see cref="Hash" /> object represent the same
		///     value.
		/// </summary>
		/// <param name="other">A <see cref="Hash" /> to compare to this instance.</param>
		/// <returns>true if obj is equal to this instance; otherwise, false.</returns>
		public bool Equals(Hash other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return HashData.SequenceEqual(other.HashData);
		}

		/// <summary>
		///     Parse a hex string
		/// </summary>
		/// <param name="value">The hex string which represents a hash value</param>
		/// <returns>Return</returns>
		public static Hash Parse(string value)
		{
			return new Hash(value.ToByteArray());
		}

	    /// <summary>
	    ///     Try parse a hex string
	    /// </summary>
	    /// <param name="value">The hex string which represents a hash value</param>
	    /// <param name="hash">The result hash</param>
	    /// <returns>True if the <see cref="value" /> was successfully parsed, else false.</returns>
	    public static bool TryParse(string value, out Hash hash)
	    {
	        if (value.Length % 2 == 1)
	        {
	            hash = null;
	            return false;
	        }

	        try
	        {
	            hash = new Hash(value.ToByteArray());
	            return true;
	        }
	        catch (Exception)
	        {
	            hash = null;
	            return false;
	        }
	    }

        /// <summary>
        ///     Convert the <see cref="HashData" /> to a hexadecimal string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
		{
			return BitConverter.ToString(HashData).Replace("-", null).ToLowerInvariant();
		}

		/// <summary>
		///     Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns>true if obj is equal to this instance; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Hash) obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Hash" />.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var result = 0;
				foreach (var b in HashData)
					result = (result * 31) ^ b;
				return result;
			}
		}

	    public static bool operator ==(Hash obj1, Hash obj2)
	    {
	        if (ReferenceEquals(obj1, obj2))
	            return true;

	        if (ReferenceEquals(obj1, null))
	            return false;

	        if (ReferenceEquals(obj2, null))
	            return false;

	        return obj1.HashData.Length == obj2.HashData.Length
	               && obj1.HashData.SequenceEqual(obj2.HashData);
	    }

	    public static bool operator !=(Hash obj1, Hash obj2)
	    {
	        return !(obj1 == obj2);
	    }
	}
}