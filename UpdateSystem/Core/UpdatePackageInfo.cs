using System;
using System.Collections.Generic;

#if ELEMENTSCORE
using CodeElements.Core;

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
		public SemVersion Version { get; set; }

		/// <summary>
		///     The date when the update package was released
		/// </summary>
		public DateTime ReleaseDate { get; set; }

		/// <summary>
		///     True if the update is enforced
		/// </summary>
		public bool IsEnforced { get; set; }

		/// <summary>
		///     Custom fields set when the update package was created
		/// </summary>
		public IDictionary<string, string> CustomFields { get; set; }

		/// <summary>
		///     The changelog of the update package
		/// </summary>
		public ChangelogInfo Changelog { get; set; }
	}
}