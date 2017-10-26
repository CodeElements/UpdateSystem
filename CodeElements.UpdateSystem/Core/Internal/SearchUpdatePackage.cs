using System.Globalization;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Core.Internal
{
	internal class SearchUpdatePackage : CurrentStateInfo
	{
		[JsonProperty(PropertyName = "filter")]
		public string[] VersionFilter { get; set; }

		[JsonProperty(PropertyName = "desiredChangelogLanguage")]
		public CultureInfo DesiredChangelogLanguage { get; set; }
	}
}