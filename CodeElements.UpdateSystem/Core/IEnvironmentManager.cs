﻿using System;
using System.IO;

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     Provides operation for the current environment which may be platform dependent
    /// </summary>
    public interface IEnvironmentManager
    {
        /// <summary>
        ///     Get an empty temporary directory where all required files will be downloaded to
        /// </summary>
        /// <param name="projectGuid">The guid of the current project, may be used in the folder name.</param>
        /// <returns>Return the free directory</returns>
        DirectoryInfo GetEmptyTempDirectory(Guid projectGuid);

        /// <summary>
        ///     Delete the temporary directory
        /// </summary>
        /// <param name="directoryInfo">
        ///     The temporary directory info that was received using <see cref="GetEmptyTempDirectory" />
        /// </param>
        void DeleteTempDirectory(DirectoryInfo directoryInfo);

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