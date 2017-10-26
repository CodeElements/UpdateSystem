using CodeElements.UpdateSystem.Core;

namespace CodeElements.UpdateSystem.Windows.Extensions
{
    public static class DownloadableExtensions
    {
        public static UpdateDownloader GetDownloader(this IDownloadable downloadable, WindowsPatcher windowsPatcher)
        {
            return new UpdateDownloader(downloadable, windowsPatcher);
        }
    }
}