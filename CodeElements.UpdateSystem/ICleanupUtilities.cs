using System;

namespace CodeElements.UpdateSystem
{
    /// <summary>
    ///     Provides utilities to cleanup redundant files created by the update system. Reasons may be a failed update
    /// </summary>
    public interface ICleanupUtilities
    {
        /// <summary>
        ///     Cleanup temporary files that were not downloaded or may not be reused
        /// </summary>
        /// <param name="projectGuid">The current project guid</param>
        void Cleanup(Guid projectGuid);

        /// <summary>
        ///     Cleanup all temporary files after no new updates were found. If no new updates were found, it cannot be possible
        ///     that the downloaded files by this update system may be reused
        /// </summary>
        /// <param name="projectGuid">The current project guid</param>
        void NoUpdatesFoundCleanup(Guid projectGuid);
    }
}