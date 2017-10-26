using System.Collections.Generic;

namespace CodeElements.UpdateSystem.Client
{
	/// <summary>
	/// A default version filter that supports all pre-releases that are selectable when creating an update package using the administration
	/// </summary>
	public class DefaultVersionFilter : IVersionFilter
	{
		/// <summary>
		/// Include release candiates (<see cref="SemVersion.Prerelease"/> = "rc")
		/// </summary>
		public bool IncludeReleaseCandidate { get; set; }

		/// <summary>
		/// Include beta versions (<see cref="SemVersion.Prerelease"/> = "beta")
		/// </summary>
		public bool IncludeBeta { get; set; }

		/// <summary>
		/// Include alpha versions (<see cref="SemVersion.Prerelease"/> = "alpha")
		/// </summary>
		public bool IncludeAlpha { get; set; }

		string[] IVersionFilter.GetSupportedPrereleases()
		{
			var supportedPrereleases = new List<string>();
			if (IncludeReleaseCandidate)
				supportedPrereleases.Add("rc");
			if (IncludeBeta)
				supportedPrereleases.Add("beta");
			if (IncludeAlpha)
				supportedPrereleases.Add("alpha");

			return supportedPrereleases.ToArray();
		}
	}
}