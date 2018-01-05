using System.Collections.Generic;
using CodeElements.UpdateSystem.Files;

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     Contains information about the file base of an update package
    /// </summary>
    public class UpdatePackageFilebase : IDownloadable
    {
        private readonly IUpdateController _updateController;

        internal UpdatePackageFilebase(List<SignedFileInformation> files, IUpdateController updateController, SemVersion version)
        {
            Instructions = new UpdateInstructions(files);
            _updateController = updateController;
            Version = version;
        }

        /// <summary>
        ///     The instructions for the updater
        /// </summary>
        public UpdateInstructions Instructions { get; }

        IUpdateController IDownloadable.UpdateController => _updateController;

        /// <summary>
        /// The version of the update package
        /// </summary>
        public SemVersion Version { get; }
    }
}