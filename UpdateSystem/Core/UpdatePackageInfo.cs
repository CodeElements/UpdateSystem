using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#if SERVERIMPL
using Elements.Core;
#endif

namespace CodeElements.UpdateSystem.Core
{
	/// <summary>
	///     Information about an update package
	/// </summary>
	public class UpdatePackageInfo
	{
		/// <summary>
		///     The version of the update package
		/// </summary>
		[JsonProperty(PropertyName = "version")]
		public SemVersion Version { get; set; }

		/// <summary>
		///     The date when the update package was released
		/// </summary>
		[JsonProperty(PropertyName = "releaseDate")]
		public DateTime ReleaseDate { get; set; }

		/// <summary>
		///     True if the update is enforced
		/// </summary>
		[JsonProperty(PropertyName = "isEnforced")]
		public bool IsEnforced { get; set; }

		/// <summary>
		///     Custom fields set when the update package was created
		/// </summary>
		[JsonProperty(PropertyName = "customFields")]
		public Dictionary<string, string> CustomFields { get; set; }

		/// <summary>
		///     The changelog of the update package
		/// </summary>
		[JsonProperty(PropertyName = "changelog")]
		public ChangelogInfo Changelog { get; set; }
	}
}