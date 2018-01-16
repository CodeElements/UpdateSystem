using System;

namespace CodeElements.UpdateSystem
{
	/// <summary>
	///     Attribute to define the application version as a semantic version
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class UpdateSystemVersionAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="UpdateSystemVersionAttribute" /> class.
		/// </summary>
		/// <param name="version">The version to use.</param>
		public UpdateSystemVersionAttribute(string version)
		{
			Version = version;
		}

		/// <summary>
		///     Gets the version
		/// </summary>
		public SemVersion Version { get; }
	}
}