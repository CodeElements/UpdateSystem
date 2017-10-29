using System;
using System.IO;

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     Provides operation for the current environment which may be platform dependent
    /// </summary>
    public interface IEnvironmentManager : ICleanupUtilities
    {
        /// <summary>
        ///     Get a temporary directory where all required files will be downloaded to. If the temp directory already contains files, they may be reused.
        /// </summary>
        /// <param name="projectGuid">The guid of the current project, may be used in the folder name.</param>
        /// <returns>Return the directory</returns>
        DirectoryInfo GetTempDirectory(Guid projectGuid);

        /// <summary>
        ///     Translate the variable based filename to an absolute path. All supported variables must be replaced to adapt the
        ///     location of the file
        /// </summary>
        /// <param name="filename">The filename which may contain variables</param>
        /// <returns>Return the absolute and valid path of the file</returns>
        FileInfo TranslateFilename(string filename);

        /// <summary>
        ///     Execute the updater
        /// </summary>
        /// <param name="patcherConfig">The information that are needed for the update to run</param>
        void ExecuteUpdater(PatcherConfig patcherConfig);
    }
}