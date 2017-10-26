using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     The result of an update search
    /// </summary>
    public class UpdatePackageSearchResult : IDownloadable
    {
        /// <summary>
        ///     The update controller the search was made with
        /// </summary>
        [JsonIgnore]
        internal UpdateController UpdateController { get; private set; }

        /// <summary>
        ///     Set to true if an update is enforced. This may be because the current version was rolled back or because a newer
        ///     update package has the property "IsEnforced" set to true. To find out the exact reason, call
        ///     <see cref="Rollback" />.
        /// </summary>
        [JsonIgnore]
        public bool IsUpdateEnforced { get; private set; }

        /// <summary>
        ///     Set to true if the current version must be rolled back. The target package is either a newer package if one was
        ///     available or an older package.
        /// </summary>
        [JsonProperty("rollback")]
        public bool Rollback { get; private set; }

        /// <summary>
        ///     Set to true if an update package is available.
        /// </summary>
        [JsonIgnore]
        public bool IsUpdateAvailable { get; private set; }

        /// <summary>
        ///     The update package that will be targeted (the final version after the patch process)
        /// </summary>
        [JsonIgnore]
        public UpdatePackageInfo TargetPackage { get; private set; }

        /// <summary>
        ///     All update packages that are between the current version and the <see cref="TargetPackage" />. The last item is
        ///     always the <see cref="TargetPackage" /> and inbetween are the update packages that were skipped.
        /// </summary>
        [JsonProperty("updatePackages")]
        public List<UpdatePackageInfo> UpdatePackages { get; private set; }

        /// <summary>
        ///     The instructions for the updater
        /// </summary>
        [JsonProperty("updateInstructions")]
        public UpdateInstructions Instructions { get; set; }

        Guid IDownloadable.ProjectGuid => UpdateController.ProjectGuid;

        RSAParameters IDownloadable.PublicKey => UpdateController.PublicKey;

        internal void Initialize(UpdateController updateController)
        {
            UpdateController = updateController;

            if (UpdatePackages?.Count > 0)
            {
                IsUpdateAvailable = true;
                TargetPackage = UpdatePackages.Last();
                IsUpdateEnforced = UpdatePackages.Any(x => x.IsEnforced) || Rollback;
            }
        }
    }
}