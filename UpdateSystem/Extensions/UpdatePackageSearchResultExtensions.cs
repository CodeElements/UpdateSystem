using System.Linq;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Files.Operations;

namespace CodeElements.UpdateSystem.Extensions
{
    /// <summary>
    ///     Extensions for <see cref="UpdatePackageSearchResult" />
    /// </summary>
    public static class UpdatePackageSearchResultExtensions
    {
        /// <summary>
        ///     Get the total download size of all file operations
        /// </summary>
        /// <param name="updatePackageSearchResult">The search result</param>
        /// <returns>Return the total download size of all operations</returns>
        public static long GetTotalSize(this IDownloadable updatePackageSearchResult)
        {
            var result = 0L;

            if (updatePackageSearchResult.Instructions.FileOperations == null)
                foreach (var fileInformation in updatePackageSearchResult.Instructions.TargetFiles)
                    result += fileInformation.Length;
            else
                foreach (var fileOperation in updatePackageSearchResult.Instructions.FileOperations)
                    if (fileOperation is DeltaPatchOperation deltaPatchOperation)
                        result += deltaPatchOperation.Patches.Sum(x => (long) x.Length);
                    else if (fileOperation is INeedDownload needDownload)
                        result += needDownload.Target.Length;

            return result;
        }

        /// <summary>
        ///     Get real length of an <see cref="INeedDownload" /> considering a possible delta patch.
        /// </summary>
        /// <param name="needDownload">The file information</param>
        /// <returns>The actual length of the file</returns>
        public static long GetRealLength(this INeedDownload needDownload)
        {
            if (needDownload is DeltaPatchOperation deltaPatchOperation)
                return deltaPatchOperation.Patches.Sum(x => x.Length);
            return needDownload.Target.Length;
        }

        /// <summary>
        ///     Get the downloader of the <see cref="IDownloadable" />
        /// </summary>
        /// <param name="downloadable">The class that contains information about what do download</param>
        /// <returns>Return a downloader that downloads the files</returns>
        public static UpdateDownloader GetDownloader(this IDownloadable downloadable)
        {
            return new UpdateDownloader(downloadable);
        }
    }
}