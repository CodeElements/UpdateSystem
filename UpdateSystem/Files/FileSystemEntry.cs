namespace CodeElements.UpdateSystem.Files
{
    /// <summary>
    ///     A defined file location in the UpdateSystem
    /// </summary>
    public class FileSystemEntry
    {
        /// <summary>
        ///     The filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        ///     The platforms of this file
        /// </summary>
        public int Platforms { get; set; }

        /// <summary>
        ///     True if this file is a temporary file
        /// </summary>
        public bool IsTempFile { get; set; }
    }
}