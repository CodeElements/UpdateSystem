using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Extensions;
using CodeElements.UpdateSystem.Windows.WinForms.Internal.Utilities;
using CommonMark;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace CodeElements.UpdateSystem.Windows.WinForms.Internal
{
    public partial class UpdatesAvailableForm : Form
    {
        private readonly UpdatePackageSearchResult _updatePackageSearchResult;
        private CancellationTokenSource _downloadCancellationTokenSource;

        public UpdatesAvailableForm(IUpdateController updateController, UpdatePackageSearchResult updatePackageSearchResult)
        {
            _updatePackageSearchResult = updatePackageSearchResult;
            InitializeComponent();

            tableLayoutPanel1.RowStyles[2].Height = 0;

            Text = Properties.Resources.UpdatesAvailableForm_Title;
            installButton.Text = Properties.Resources.UpdatesAvailableForm_InstallUpdates;
            cancelButton.Text = Properties.Resources.UpdatesAvailableForm_Cancel;
            newUpdatesAvailableLabel.Text = updatePackageSearchResult.UpdatePackages.Count > 1
                ? string.Format(Properties.Resources.UpdatesAvailableForm_NewUpdatesAvailable,
                    updatePackageSearchResult.UpdatePackages.Count)
                : Properties.Resources.UpdatesAvailableForm_NewUpdateAvailable;

            newUpdatesDescriptionLabel.Text = string.Format(
                Properties.Resources.UpdatesAvailableForm_NewUpdateCanBeDownloaded,
                updateController.VersionProvider.GetVersion().ToString(false),
                updatePackageSearchResult.TargetPackage.Version.ToString(false));

            //aggregate changelogs
            //produce something like this for every changelog:
            /* 
             * # Update 1.0.1
             * Release date: Saturday, 1 July 2017
             * ___
             * <changelog> / _The changelog is empty_
             */
            var changelogs = updatePackageSearchResult.UpdatePackages.Aggregate(new StringBuilder(),
                (builder, info) => builder.Append("# ").Append(Properties.Resources.UpdatesAvailableForm_Update)
                    .Append(" ").AppendLine(info.Version.ToString())
                    .Append(Properties.Resources.UpdatesAvailableForm_ReleaseDate).Append(": ")
                    .AppendLine(info.ReleaseDate.ToString("D", CultureInfo.CurrentUICulture)).AppendLine("___").AppendLine(
                        string.IsNullOrWhiteSpace(info.Changelog.Content)
                            ? "_" + Properties.Resources.UpdatesAvailableForm_ChangelogIsEmpty + "_"
                            : info.Changelog.Content)).ToString();

            var htmlPanel = new HtmlPanel
            {
                IsSelectionEnabled = false,
                IsContextMenuEnabled = false,
                Dock = DockStyle.Fill,
                BaseStylesheet = MarkdownHtmlStylesheet
            };

            try
            {
                htmlPanel.Text = CommonMarkConverter.Convert(changelogs, CommonMarkSettings.Default);
                tableLayoutPanel1.Controls.Add(htmlPanel, 0, 1);
            }
            catch (CommonMarkException)
            {
                //strength of markdown, being nicely readable without formatting
                tableLayoutPanel1.Controls.Add(new RichTextBox
                {
                    ReadOnly = true,
                    Text = changelogs,
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.FixedSingle
                }, 0, 1);
            }
        }

        private async void installButton_Click(object sender, EventArgs args)
        {
            installButton.Enabled = false;

            //reset controls
            updateActionLabel.Text = "";
            downloadStatsLabel.Text = "";
            downloadSpeedLabel.Text = "";
            downloadProgressBar.Value = 0;

            var downloader = _updatePackageSearchResult.GetDownloader();
            downloader.PropertyChanged += DownloaderOnPropertyChanged;

            tableLayoutPanel1.RowStyles[2].Height = 50;
            _downloadCancellationTokenSource = new CancellationTokenSource();

            try
            {
                var patcher = await downloader.DownloadAsync(_downloadCancellationTokenSource.Token);
                patcher.Patch();
            }
            catch (TaskCanceledException)
            {
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message, Properties.Resources.UpdatesAvailableForm_Error, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
            }
        }

        private void DownloaderOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var downloader = (UpdateDownloader) sender;
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(UpdateDownloader.BytesDownloaded):
                case nameof(UpdateDownloader.TotalBytesToDownload):
                    InvokeIfRequired(() =>
                        downloadStatsLabel.Text =
                            $"{FormatBytesConverter.BytesToString(downloader.BytesDownloaded)} / {FormatBytesConverter.BytesToString(downloader.TotalBytesToDownload)}");
                    break;
                case nameof(UpdateDownloader.Progress):
                    InvokeIfRequired(() => downloadProgressBar.Value = (int) (downloader.Progress * 100));
                    break;
                case nameof(UpdateDownloader.DownloadSpeed):
                    InvokeIfRequired(() =>
                        downloadSpeedLabel.Text =
                            FormatBytesConverter.BytesToString((int) downloader.DownloadSpeed) + "/s");
                    break;
                case nameof(UpdateDownloader.CurrentAction):
                case nameof(UpdateDownloader.CurrentFilename):
                    InvokeIfRequired(() =>
                    {
                        var downloadSpeedLabelVisibility = false;
                        var progressBarMarquee = false;
                        switch (downloader.CurrentAction)
                        {
                            case ProgressAction.DownloadFile:
                                downloadSpeedLabelVisibility = true;
                                updateActionLabel.Text =
                                    string.Format(Properties.Resources.UpdatesAvailableForm_DownloadFile,
                                        downloader.CurrentFilename);
                                break;
                            case ProgressAction.ValidateFile:
                                updateActionLabel.Text =
                                    string.Format(Properties.Resources.UpdatesAvailableForm_ValidateFile,
                                        downloader.CurrentFilename);
                                break;
                            case ProgressAction.ValidateTasks:
                                updateActionLabel.Text = Properties.Resources.UpdatesAvailableForm_ValidateTasks;
                                break;
                            case ProgressAction.CollectFileInformation:
                                updateActionLabel.Text = Properties.Resources.UpdatesAvailableForm_CollectInformation;
                                progressBarMarquee = true;
                                break;
                            case ProgressAction.ApplyDeltaPatch:
                                updateActionLabel.Text =
                                    string.Format(Properties.Resources.UpdatesAvailableForm_ApplyDeltaPatch,
                                        downloader.CurrentFilename);
                                break;
                        }

                        downloadSpeedLabel.Visible = downloadSpeedLabelVisibility;
                        downloadProgressBar.Style =
                            progressBarMarquee ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
                    });
                    break;
            }
        }

        private void InvokeIfRequired(MethodInvoker action)
        {
            if (InvokeRequired)
                Invoke(action);
            else action();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (_downloadCancellationTokenSource == null)
                DialogResult = DialogResult.Cancel;
            else _downloadCancellationTokenSource.Cancel();
        }

        private const string MarkdownHtmlStylesheet = @"
p {
    font-size: 90%;
}
h1 {
    font-size: 1.25em;
    margin-top: 0.4em;
    margin-bottom: 0.2em;
}
h2 {
    font-size: 1.1em;
    margin-top: 0.6em;
    margin-bottom: 0.1em;
}
h4 {
    font-size: 1.05em;
    margin-top: 20px;
    margin-bottom: 5px;
}
li {
    font-size: 90%;
}
hr {
    margin-top: 8px;
    margin-right: 10px;
    margin-bottom: 8px;
}
p {
    margin-bottom: 5px;
    margin-top: 5px;
}
ul {
    margin-top: 5px;
    margin-bottom: 15px;
}";
    }
}