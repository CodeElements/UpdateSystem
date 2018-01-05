using System.Collections.Generic;
using Newtonsoft.Json;

#if !ELEMENTSCORE
using System.Linq;

#endif

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     The result of an update search
    /// </summary>
    public class UpdatePackageSearchResult
#if !ELEMENTSCORE
        : IDownloadable
#endif
    {
#if !ELEMENTSCORE
        private IUpdateController _updateController;

        /// <summary>
        ///     The update package that will be targeted (the final version after the patch process)
        /// </summary>
        [JsonIgnore]
        public UpdatePackageInfo TargetPackage { get; private set; }

        /// <summary>
        ///     Set to true if an update is enforced. This may be because the current version was rolled back or because a newer
        ///     update package has the property "IsEnforced" set to true. To find out the exact reason, call
        ///     <see cref="Rollback" />.
        /// </summary>
        [JsonIgnore]
        public bool IsUpdateEnforced { get; private set; }

        /// <summary>
        ///     Set to true if an update package is available.
        /// </summary>
        [JsonIgnore]
        public bool IsUpdateAvailable { get; private set; }
#endif

        /// <summary>
        ///     Set to true if the current version must be rolled back. The target package is either a newer package if one was
        ///     available or an older package.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Rollback
        {
            get;
#if !ELEMENTSCORE
            private
#endif
            set;
        }

        /// <summary>
        ///     All update packages that are between the current version and the <see cref="TargetPackage" />. The last item is
        ///     always the <see cref="TargetPackage" /> and inbetween are the update packages that were skipped.
        /// </summary>
        [JsonProperty]
        public List<UpdatePackageInfo> UpdatePackages
        {
            get;
#if !ELEMENTSCORE
            private
#endif
            set;
        }

        /// <summary>
        ///     The instructions for the updater
        /// </summary>
        [JsonProperty]
        public UpdateInstructions Instructions
        {
            get;
#if !ELEMENTSCORE
            private
#endif
            set;
        }

        /// <summary>
        ///     The json web token to access further ressources
        /// </summary>
        [JsonProperty]
        public string Jwt
        {
            get;
#if !ELEMENTSCORE
            private
#endif
            set;
        }

#if !ELEMENTSCORE
        /// <summary>
        ///     The update controller the search was made with
        /// </summary>
        [JsonIgnore]
        IUpdateController IDownloadable.UpdateController => _updateController;

        internal void Initialize(IUpdateController updateController)
        {
            _updateController = updateController;

            if (UpdatePackages?.Count > 0)
            {
                IsUpdateAvailable = true;
                TargetPackage = UpdatePackages.Last();
                IsUpdateEnforced = UpdatePackages.Any(x => x.IsEnforced) || Rollback;

                foreach (var updatePackageInfo in UpdatePackages)
                    updatePackageInfo.ReleaseDate = updatePackageInfo.ReleaseDate.ToLocalTime();
            }
        }
#endif
    }
}