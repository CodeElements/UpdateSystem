#if ELEMENTSCORE
using CodeElements.Core;

#endif

namespace CodeElements.UpdateSystem.Files
{
    /// <summary>
    ///     Information about a file
    /// </summary>
    public class FileInformation : FileSystemEntry
    {
        /// <summary>
        ///     The hash value of this file
        /// </summary>
        public Hash Hash { get; set; }

        /// <summary>
        ///     The length of the file in bytes
        /// </summary>
        public int Length { get; set; }
    }
}