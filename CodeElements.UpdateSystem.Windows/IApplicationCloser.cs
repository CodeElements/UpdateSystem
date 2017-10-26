namespace CodeElements.UpdateSystem.Windows
{
    /// <summary>
    ///     Defines a method that should close the current application so the updater can modify the files
    /// </summary>
    public interface IApplicationCloser
    {
        /// <summary>
        ///     Shutdown the current application
        /// </summary>
        void ExitApplication();
    }
}