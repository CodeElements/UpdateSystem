using System.IO;

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     Information about a file
    /// </summary>
    public interface IFileInfo
    {
        /// <summary>
        ///     The filename
        /// </summary>
        string Filename { get; }

        /// <summary>
        ///     Gets a value indicating whether a file exists
        /// </summary>
        bool Exists { get; }

        /// <summary>
        ///     Get a read-only stream
        /// </summary>
        /// <returns>Returns a read-only stream</returns>
        Stream OpenRead();

        /// <summary>
        ///     Create the file
        /// </summary>
        /// <returns>Returns a writeable stream</returns>
        Stream Create();

        /// <summary>
        ///     Delete the file
        /// </summary>
        void Delete();
    }
}