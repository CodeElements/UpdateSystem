using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeElements.UpdateSystem.Compression;
using CodeElements.UpdateSystem.Core.Internal;
using CodeElements.UpdateSystem.Extensions;
using CodeElements.UpdateSystem.Files.Operations;
using CodeElements.UpdateSystem.Utilities;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     The update downloader for downloading the update packages
    /// </summary>
    public class UpdateDownloader : INotifyPropertyChanged
    {
        private readonly IDownloadable _downloadable;
        private readonly IUpdateController _updateController;
        private long _bytesDownloaded;
        private ProgressAction _currentAction;
        private string _currentFilename;
        private double _downloadSpeed;
        private double _progress;
        private long _totalBytesToDownload;

        /// <summary>
        ///     Initialize a new instance of <see cref="UpdateDownloader" />
        /// </summary>
        /// <param name="downloadable">The information needed to download the required files</param>
        public UpdateDownloader(IDownloadable downloadable)
        {
            _downloadable = downloadable;
            _updateController = downloadable.UpdateController;
        }

        /// <summary>
        ///     Gets the number of bytes downloaded to the local computer.
        /// </summary>
        public long BytesDownloaded
        {
            get => _bytesDownloaded;
            set
            {
                if (_bytesDownloaded != value)
                {
                    _bytesDownloaded = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets the name of the file that is currently processed
        /// </summary>
        public string CurrentFilename
        {
            get => _currentFilename;
            set
            {
                if (_currentFilename != value)
                {
                    _currentFilename = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets the total number of bytes for the download operation.
        /// </summary>
        public long TotalBytesToDownload
        {
            get => _totalBytesToDownload;
            set
            {
                if (_totalBytesToDownload != value)
                {
                    _totalBytesToDownload = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     The total progress
        /// </summary>
        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     The current action that is processing
        /// </summary>
        public ProgressAction CurrentAction
        {
            get => _currentAction;
            set
            {
                if (_currentAction != value)
                {
                    _currentAction = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     The current download speed in bytes per second
        /// </summary>
        public double DownloadSpeed
        {
            get => _downloadSpeed;
            set
            {
                if (_downloadSpeed != value)
                {
                    _downloadSpeed = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Download the required files
        /// </summary>
        /// <returns>Return an application patcher that can apply the updates from the downloaded files</returns>
        public async Task<ApplicationPatcher> DownloadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var projectId = _downloadable.UpdateController.ProjectId;

            var filesDictionary = new Dictionary<Hash, IFileInfo>();
            var processCollection = new ProcessCollection();
            processCollection.PropertyChanged += (sender, args) => Progress = processCollection.Current;

            ProcessColumn validateTasksProcessItem = null;
            if (_downloadable.Instructions.Tasks?.Count > 0)
                validateTasksProcessItem =
                    processCollection.AddColumn(_downloadable.Instructions.Tasks.Count, 0.2);

            var downloadFiles = processCollection.AddColumnCollection(0.8);
            var scanFiles = _downloadable.Instructions.FileOperations == null
                ? new ProcessColumn(_downloadable.Instructions.TargetFiles.Count, 0.35)
                : null;

#if NETSTANDARD
            using (var rsa = RSA.Create())
#else
			using (var rsa = new RSACng())
#endif
            using (var sha256 = SHA256.Create())
            {
                rsa.ImportParameters(_updateController.PublicKey);

                //validate tasks
                if (_downloadable.Instructions.Tasks?.Count > 0)
                {
                    CurrentAction = ProgressAction.ValidateTasks;
                    var serializerSettings =
                        new JsonSerializerSettings
                        {
                            ContractResolver = new UpdateTaskSignatureDataContractResolver()
                        };
                    foreach (var updateTask in _downloadable.Instructions.Tasks)
                    {
                        var hash = sha256.ComputeHash(
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(updateTask, serializerSettings)));

                        if (!rsa.VerifyHash(hash, updateTask.Signature, HashAlgorithmName.SHA256,
                            RSASignaturePadding.Pss))
                            throw new InvalidOperationException("The signature of a task could not be validated.");

                        validateTasksProcessItem.Increment();
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }

                List<IFileOperation> fileOperations;
                if (_downloadable.Instructions.FileOperations == null)
                {
                    //we have to build our own operation list from the target files and the current file base
                    fileOperations = new List<IFileOperation>();

                    //key hash is new file hash, hash in tuple is current hash
                    var updateOperations =
                        new Dictionary<Hash, Tuple<UpdateFileOperation, Hash>>();
                    await Task.Run(() =>
                    {
                        foreach (var targetFile in _downloadable.Instructions.TargetFiles)
                        {
                            var currentFs = _updateController.Environment.TryOpenRead(targetFile.Filename);
                            if (currentFs == null)
                                fileOperations.Add(new DownloadFileOperation {Target = targetFile});
                            else
                                using (currentFs)
                                {
                                    var hash = new Hash(sha256.ComputeHash(currentFs));
                                    if (!hash.Equals(targetFile.Hash))
                                    {
                                        var updateOperation = new UpdateFileOperation {Target = targetFile};
                                        fileOperations.Add(updateOperation);
                                        updateOperations.Add(updateOperation.Target.Hash,
                                            new Tuple<UpdateFileOperation, Hash>(updateOperation, hash));
                                    }
                                }

                            cancellationToken.ThrowIfCancellationRequested();
                            scanFiles.Increment();
                        }
                    }, cancellationToken);

                    //if (updateOperations.Count > 0)
                    //{
                    //    //Dictionary: Current File => Target file
                    //    var updates = updateOperations.ToDictionary(x => x.Value.Item2, y => y.Key);
                    //    var response = await _updateController.HttpClient.PostAsync(
                    //        new Uri(_updateController.UpdateSystemApiUri,
                    //            $"projects/{_updateController.ProjectId:N}/findDeltaPatches"),
                    //        new StringContent(JsonConvert.SerializeObject(updates)), cancellationToken);
                    //    if (response.StatusCode == HttpStatusCode.OK)
                    //    {
                    //        //Dictionary target hash => delta patches from source hash
                    //        var patches =
                    //            JsonConvert.DeserializeObject<Dictionary<Hash, List<DeltaPatchInfo>>>(
                    //                await response.Content.ReadAsStringAsync());
                    //        if (patches?.Count > 0)
                    //        {
                    //            foreach (var patch in patches)
                    //            {
                    //                var updateOperation = updateOperations[patch.Key];
                    //                fileOperations.Remove(updateOperation.Item1);
                    //                fileOperations.Add(new DeltaPatchOperation{Patches = patch.Value, Target = updateOperation.Item1.Target});
                    //            }
                    //        }
                    //    } //else ignore, delta patches are not that important
                    //}
                }
                else
                {
                    fileOperations = _downloadable.Instructions.FileOperations;
                }

                var downloadOperations = fileOperations.OfType<INeedDownload>().ToList();

                var downloadFilesProcessing =
                    downloadOperations.ToDictionary(x => x, y => downloadFiles.AddColumn(y.GetRealLength(), 1));

                TotalBytesToDownload = downloadOperations.Sum(x => x.GetRealLength());

                var downloadBuffer = new byte[8192];
                var dataDownloaded = 0L;

                //we process the delta patches first
                foreach (var needDownload in downloadOperations.OrderByDescending(x => x is DeltaPatchOperation))
                {
                    var processingItem = downloadFilesProcessing[needDownload];

                    if (filesDictionary.ContainsKey(needDownload.Target.Hash))
                    {
                        processingItem.Complete();
                        continue;
                    }

                    var tempFile = _updateController.Environment.GetStackFile(projectId, needDownload.Target.Hash);
                    if (tempFile.Exists)
                    {
                        //may be because of an older download
                        Hash tempFileHash;
                        using (var fileStream = tempFile.OpenRead())
                        {
                            tempFileHash = new Hash(sha256.ComputeHash(fileStream));
                        }

                        if (!tempFileHash.Equals(needDownload.Target.Hash))
                            tempFile.Delete();
                    }

                    if (!tempFile.Exists)
                    {
                        CurrentFilename = Path.GetFileName(needDownload.Target.Filename);
                        if (needDownload is DeltaPatchOperation deltaPatchOperation)
                        {
                            var sourceFile =
                                _updateController.Environment.TryOpenRead(deltaPatchOperation.Target.Filename);
                            if (sourceFile != null)
                                try
                                {
                                    CurrentAction = ProgressAction.DownloadFile;

                                    var patchedFile = new PatchedFile(sourceFile, null); //the original file
                                    foreach (var deltaPatchInfo in deltaPatchOperation.Patches)
                                    {
                                        var deltaFile =
                                            _updateController.Environment.GetDeltaStackFile(projectId, deltaPatchInfo.PatchId);
                                        if (!deltaFile.Exists)
                                        {
                                            await DownloadFile(
                                                new Uri(
                                                    $"projects/{_updateController.ProjectId:N}/download?patchId={deltaPatchInfo.PatchId}"),
                                                deltaFile, downloadBuffer, processingItem, cancellationToken);
                                            BytesDownloaded = dataDownloaded += needDownload.Target.Length;
                                        }

                                        CurrentAction = ProgressAction.ApplyDeltaPatch;
                                        var newFile = _updateController.Environment.GetRandomFile(projectId);
                                        var outputStream = newFile.Create();

                                        using (var deltaFileStream = deltaFile.OpenRead())
                                        {
                                            await Task.Run(() =>
                                                VcdiffDecoder.Decode(patchedFile.Stream, deltaFileStream, outputStream));
                                        }

                                        patchedFile.Dispose(); //deletes the temp file and closes the stream

                                        outputStream.Position = 0;
                                        patchedFile = new PatchedFile(outputStream, newFile);
                                    }

                                    //it is not possible to calculate the hash above because the vcdiff encoder jumps around in the output file.
                                    //(to calculate the hash using a hashStream)
                                    //do not dispose patchedFile!! (else the file would get removed)
                                    using (patchedFile.Stream)
                                    {
                                        if (!await Task.Run(() =>
                                            sha256.ComputeHash(patchedFile.Stream)
                                                .Equals(needDownload.Target.Hash)))
                                            throw new InvalidOperationException(
                                                "The file hash does not match the hash of the patched file.");
                                    }

                                    _updateController.Environment.MoveToStackFiles(projectId, patchedFile.TempFile,
                                        needDownload.Target.Hash);
                                    tempFile = patchedFile.TempFile; //exists now
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                        }
                        CurrentAction = ProgressAction.DownloadFile;

                        if (!tempFile.Exists)
                        {
                            var downloadHash = await DownloadFile(
                                new Uri($"download?file={needDownload.Target.Hash}", UriKind.Relative), tempFile,
                                downloadBuffer, processingItem, cancellationToken);
                            BytesDownloaded = dataDownloaded += needDownload.Target.Length;

                            if (!downloadHash.Equals(needDownload.Target.Hash))
                                throw new InvalidOperationException(
                                    "The hash value of the downloaded file and the expected hash do no match.");
                        }
                    }

                    var fileHash = needDownload.Target.Hash;
                    if (!_downloadable.Instructions.FileSignatures.TryGetValue(fileHash, out var fileSignature))
                        throw new InvalidOperationException(
                            $"The signature of the file with the hash '{fileHash}' was not found.");

                    CurrentAction = ProgressAction.ValidateFile;

                    if (!rsa.VerifyHash(fileHash.HashData, fileSignature, HashAlgorithmName.SHA256,
                        RSASignaturePadding.Pss))
                        throw new InvalidOperationException(
                            $"The signature of the file '{needDownload.Target.Filename}' ({fileHash}) could not be validated.");


                    filesDictionary.Add(fileHash, tempFile);
                    processingItem.Complete();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                return new ApplicationPatcher(_updateController.Environment,
                    new PatcherConfig
                    {
                        AvailableFiles = filesDictionary.ToDictionary(x => x.Key, y => y.Value.Filename),
                        FileOperations = fileOperations,
                        UpdateTasks = _downloadable.Instructions.Tasks
                    });
            }
        }

        private async Task<Hash> DownloadFile(Uri uri, IFileInfo fileInfo, byte[] buffer, ProcessColumn processColumn, CancellationToken cancellationToken)
        {
            var response =
                await _updateController.HttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw await UpdateSystemResponseExtensions.GetResponseException(response, _updateController);

            //double actualLength = response.Content.Headers.ContentLength.Value;

            var lastUpdate = DateTimeOffset.UtcNow;
            var dataDownloadedSinceLastUpdate = 0;

            using (var netStream = await response.Content.ReadAsStreamAsync())
            using (var countingStream = new CountingStreamWrapper(netStream))
            using (var fileStream = fileInfo.Create())
#if NETSTANDARD
            using (var hashStream =
                new IncrementalHashStream(fileStream, IncrementalHash.CreateHash(HashAlgorithmName.SHA256)))
#else
			using(var sha256 = new SHA256Cng())
			using(var hashStream = new CryptoStream(fileStream, sha256, CryptoStreamMode.Write))
#endif
            using (var gzipStream = new GZipStream(countingStream, CompressionMode.Decompress, true))
            {
                var sourceBytesDownloaded = BytesDownloaded;
                while (true)
                {
                    var read = await gzipStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (read == 0)
#if NETSTANDARD
                        return new Hash(hashStream.IncrementalHash.GetHashAndReset());
#else
					{
						hashStream.FlushFinalBlock();
						return new Hash(sha256.Hash);
					}
#endif

                    // ReSharper disable once MethodSupportsCancellation
                    var writeOperation = hashStream.WriteAsync(buffer, 0, read);
                    //var relativeBytesDownladed = (long) (read / actualLength * length);
                    BytesDownloaded = sourceBytesDownloaded + countingStream.TotalDataRead;
                    processColumn.Current = countingStream.TotalDataRead;
                    dataDownloadedSinceLastUpdate += countingStream.LastDataRead;

                    if (DateTimeOffset.UtcNow - lastUpdate > TimeSpan.FromMilliseconds(100)) // || currentSpeed == 0
                    {
                        var period = DateTimeOffset.UtcNow - lastUpdate;
                        var currentSpeed = dataDownloadedSinceLastUpdate / period.TotalSeconds;

                        lastUpdate = DateTimeOffset.UtcNow;
                        dataDownloadedSinceLastUpdate = 0;
                        DownloadSpeed = currentSpeed;
                    }
                    await writeOperation;
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class PatchedFile : IDisposable
        {
            public PatchedFile(Stream stream, IFileInfo tempFile)
            {
                Stream = stream;
                TempFile = tempFile;
            }

            public bool IsOriginal => TempFile != null;
            public Stream Stream { get; }
            public IFileInfo TempFile { get;}

            public void Dispose()
            {
                Stream?.Dispose();
                TempFile?.Delete();
            }
        }
    }
}