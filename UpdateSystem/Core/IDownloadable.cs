namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     Defines a class that provides information to download an update
    /// </summary>
    public interface IDownloadable
    {
        /// <summary>
        ///     The instructions of the updater
        /// </summary>
        UpdateInstructions Instructions { get; }

        /// <summary>
        ///     The update controller core class that provides the settings
        /// </summary>
        IUpdateController UpdateController { get; }
    }
}