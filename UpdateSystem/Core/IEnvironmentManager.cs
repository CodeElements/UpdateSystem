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
        ///     Try to open an existing file. If the file does not exist, return null (do not throw an exception).
        /// </summary>
        /// <param name="filename">The file name which may include path variables</param>
        /// <returns>Return the stream of the file if the file exists, else return null.</returns>
        Stream TryOpenRead(string filename);

        /// <summary>
        ///     Get a file of the current process stack
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <param name="hash">The hash of the file</param>
        /// <returns>Return the file info</returns>
        IFileInfo GetStackFile(Guid projectId, Hash hash);

        /// <summary>
        ///     Get a delta stack file
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <param name="patchId">The id of the delta patch</param>
        /// <returns>Return the file info</returns>
        IFileInfo GetDeltaStackFile(Guid projectId, int patchId);

        /// <summary>
        ///     Get a random, non-existing temp file
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <returns>Return a random, non-existing temp file</returns>
        IFileInfo GetRandomFile(Guid projectId);

        /// <summary>
        ///     Move a file to the stack files
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <param name="sourceFile">The source file that should be moved</param>
        /// <param name="hash">The hash of the stack file</param>
        void MoveToStackFiles(Guid projectId, IFileInfo sourceFile, Hash hash);

        /// <summary>
        ///     Execute the updater
        /// </summary>
        /// <param name="patcherConfig">The information that are needed for the update to run</param>
        void ExecuteUpdater(PatcherConfig patcherConfig);
    }
}