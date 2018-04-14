using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Extensions;
using CodeElements.UpdateSystem.Windows.WinForms.Internal;

namespace CodeElements.UpdateSystem.Windows.WinForms
{
    /// <summary>
    ///     Extensions for the <see cref="IUpdateController" />
    /// </summary>
    public static class UpdateControllerExtensions
    {
        /// <summary>
        ///     Update the Windows Forms applications using a dialog
        /// </summary>
        /// <param name="updateController">The update controller</param>
        /// <param name="owner">The owner window the dialogs should use</param>
        /// <param name="showSearchDialog">
        ///     Set to true if the search process should also by visualised by a window - else only a
        ///     window is opened if an update was found.
        /// </param>
        public static async Task UpdateInteractively(this IUpdateController updateController, IWin32Window owner,
            bool showSearchDialog = true)
        {
            var task = updateController.SearchForNewUpdatePackagesAsync();
            if (showSearchDialog)
            {
                var progressForm = new SearchUpdatesForm();
                var _ = task.ContinueWith(t => progressForm.Close(),
                    TaskScheduler.FromCurrentSynchronizationContext());
                progressForm.ShowDialog(owner);
            }

            var searchResult = await task;
            if (!searchResult.IsUpdateAvailable)
                return; //no new updates available

            if (searchResult.IsUpdateEnforced)
            {
                (await searchResult.GetDownloader().DownloadAsync(CancellationToken.None)).Patch();
                return;
            }

            var updatesAvailableForm = new UpdatesAvailableForm(updateController, searchResult);
            updatesAvailableForm.ShowDialog(owner);
        }
    }
}