using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Extensions;
using CodeElements.UpdateSystem.Windows.Wpf.Internal;
using CommonMark;

namespace CodeElements.UpdateSystem.Windows.Wpf
{
    public class WpfUpdaterViewModel : INotifyPropertyChanged
    {
        private readonly UpdatePackageSearchResult _searchResult;
        private RelayCommand _cancelCommand;
        private bool? _dialogResult;
        private RelayCommand _installUpdatesCommand;
        private bool _isDownloadingUpdates;
        private CancellationTokenSource _downloadCancellationTokenSource;
        private UpdateDownloader _updateDownloader;

        public WpfUpdaterViewModel(UpdatePackageSearchResult searchResult)
        {
            _searchResult = searchResult;

            TargetUpdatePackage = searchResult.TargetPackage;
            UpdatePackages = searchResult.UpdatePackages;

            var downloadable = (IDownloadable) searchResult;
            CurrentVersion = downloadable.UpdateController.VersionProvider.GetVersion();

            ChangelogsHtml = GenerateChangelogs(searchResult);
        }

        public string ChangelogsHtml { get; }
        public List<UpdatePackageInfo> UpdatePackages { get; }
        public SemVersion CurrentVersion { get; }
        public UpdatePackageInfo TargetUpdatePackage { get; }

        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                OnPropertyChanged();
            }
        }

        public bool IsDownloadingUpdates
        {
            get => _isDownloadingUpdates;
            set
            {
                _isDownloadingUpdates = value;
                OnPropertyChanged();
            }
        }

        public UpdateDownloader UpdateDownloader
        {
            get => _updateDownloader;
            set
            {
                _updateDownloader = value;
                OnPropertyChanged();
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(parameter =>
                {
                    if (IsDownloadingUpdates)
                        _downloadCancellationTokenSource.Cancel();
                    else
                        DialogResult = false;
                }));
            }
        }

        public ICommand InstallUpdatesCommand
        {
            get
            {
                return _installUpdatesCommand ??
                       (_installUpdatesCommand = new RelayCommand(async parameter =>
                       {
                           _downloadCancellationTokenSource = new CancellationTokenSource();

                           IsDownloadingUpdates = true;
                           try
                           {
                               UpdateDownloader = _searchResult.GetDownloader();
                               var patcher = await UpdateDownloader.Download(_downloadCancellationTokenSource.Token);
                               patcher.Patch();
                           }
                           catch (TaskCanceledException)
                           {
                               DialogResult = false;
                           }
                           catch(Exception e)
                           {
                               MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                               return;
                           }
                           finally
                           {
                               IsDownloadingUpdates = false;
                               _downloadCancellationTokenSource.Dispose();
                           }

                           DialogResult = true;
                       }));
            }
        }

        private static string GenerateChangelogs(UpdatePackageSearchResult searchResult)
        {
            var markdown = searchResult.UpdatePackages.Aggregate(new StringBuilder(),
                (builder, info) => builder.Append("# ").Append("Update")
                    .Append(" ").AppendLine(info.Version.ToString())
                    .Append(Properties.Resources.UpdaterViewModel_ReleaseDate).Append(": ")
                    .AppendLine(info.ReleaseDate.ToString("D", CultureInfo.CurrentUICulture)).AppendLine("___")
                    .AppendLine(
                        string.IsNullOrWhiteSpace(info.Changelog.Content)
                            ? "_" + Properties.Resources.UpdaterViewModel_ChangelogIsEmpty + "_"
                            : info.Changelog.Content)).ToString();
            return CommonMarkConverter.Convert(markdown, CommonMarkSettings.Default);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}