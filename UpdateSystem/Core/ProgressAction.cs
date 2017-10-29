namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     The action that is currently processed
    /// </summary>
    public enum ProgressAction
    {
        /// <summary>
        ///     Downloading a file.
        /// </summary>
        DownloadFile,

        /// <summary>
        ///     Validate file signature
        /// </summary>
        ValidateFile,

        /// <summary>
        ///     Validate the update tasks signature
        /// </summary>
        ValidateTasks,

        /// <summary>
        ///     Collecting file information to prepare the download of the required files
        /// </summary>
        CollectFileInformation,

        /// <summary>
        ///     Apply a delta patch on a file
        /// </summary>
        ApplyDeltaPatch
    }
}