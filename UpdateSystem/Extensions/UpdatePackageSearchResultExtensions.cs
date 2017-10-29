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
	        {
	            foreach (var fileInformation in updatePackageSearchResult.Instructions.TargetFiles)
	                result += fileInformation.Length;
	        }
	        else
	            foreach (var fileOperation in updatePackageSearchResult.Instructions.FileOperations)
	                if (fileOperation is DeltaPatchOperation deltaPatchOperation)
	                    result += deltaPatchOperation.Patches.Sum(x => (long) x.Length);
	                else if (fileOperation is INeedDownload needDownload)
	                    result += needDownload.Target.Length;

	        return result;
	    }

	    public static long GetRealLength(this INeedDownload needDownload)
	    {
	        if (needDownload is DeltaPatchOperation deltaPatchOperation)
	            return deltaPatchOperation.Patches.Sum(x => x.Length);
	        return needDownload.Target.Length;
	    }

	    public static UpdateDownloader GetDownloader(this IDownloadable downloadable)
	    {
	        return new UpdateDownloader(downloadable);
	    }
	}
}