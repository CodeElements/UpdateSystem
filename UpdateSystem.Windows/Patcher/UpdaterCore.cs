using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Files.Operations;
using CodeElements.UpdateSystem.UpdateTasks.Base;
using CodeElements.UpdateSystem.Windows.Patcher.Extensions;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Translations;
using CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;
using CodeElements.UpdateSystem.Windows.Properties;
using CodeElements.UpdateSystem.Windows.RollbackApp;
using Microsoft.Win32;

namespace CodeElements.UpdateSystem.Windows.Patcher
{
    internal class UpdaterCore : INotifyPropertyChanged
    {
        private readonly EnvironmentManager _environmentManager;
        private readonly Process _hostProcess;
        private readonly Logger _logger;
        private readonly RollbackInfo _rollbackInfo;
        private readonly StatusUpdater _statusUpdater;
        private readonly WindowsPatcherConfig _windowsPatcherConfig;
        private double _progress;
        private string _status;

        public UpdaterCore(WindowsPatcherConfig windowsPatcherConfig, Process hostProcess,
            IPatcherTranslation translation)
        {
            _windowsPatcherConfig = windowsPatcherConfig;
            _hostProcess = hostProcess;
            _environmentManager = new EnvironmentManager(windowsPatcherConfig);
            _rollbackInfo = new RollbackInfo("rollbackInfo.json")
            {
                ApplicationPath = windowsPatcherConfig.ApplicationPath,
                PatcherConfig = windowsPatcherConfig
            };
            _logger = new Logger(Path.Combine(windowsPatcherConfig.ActionConfig.ProjectId.GetTempDirectory(), "patcher", "log.txt"));
            Translation = translation;

            _statusUpdater = new StatusUpdater(Translation);
            _statusUpdater.StatusUpdated += StatusUpdaterOnStatusUpdated;
        }

        public IPatcherTranslation Translation { get; }
        public bool IsShutdownRequested { get; private set; }

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

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Execute the update process. This method is absolutely fail-safe, everything is managed inside, it is not possible
        ///     that this method throws an exception
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the update process</param>
        public void Update(CancellationToken cancellationToken)
        {
            var totalOps = 0;
            double currentOp = 0;

            void UpdateProgress()
            {
                Progress = ++currentOp / totalOps;
            }

            void UpdateProgressOrCancel()
            {
                cancellationToken.ThrowIfCancellationRequested();
                UpdateProgress();
            }

            var updateTasksBefore = _windowsPatcherConfig.ActionConfig.UpdateTasks
                ?.Where(x => x != null && x.ExecutionOrderNumber < 0).OrderBy(x => x.ExecutionOrderNumber).ToList();
            var updateTasksAfter = _windowsPatcherConfig.ActionConfig.UpdateTasks
                ?.Where(x => x != null && x.ExecutionOrderNumber > 0).OrderBy(x => x.ExecutionOrderNumber).ToList();

            if (_windowsPatcherConfig.ReplaceApplicationExecutable) totalOps += 2; //for replacing and reverting
            totalOps += updateTasksBefore?.Count ?? 0;
            totalOps += _windowsPatcherConfig.ActionConfig.FileOperations?.Count ?? 0;
            totalOps += updateTasksAfter?.Count ?? 0;
            totalOps++; //cleanup

            Progress = 0;

            List<IRevertable> revertReplaceApplicationFile = null;
            try
            {
                if (_windowsPatcherConfig.ReplaceApplicationExecutable)
                {
                    revertReplaceApplicationFile = ReplaceExecutableFile();
                    UpdateProgressOrCancel();
                }

                if (updateTasksBefore?.Count > 0)
                {
                    _logger.Info($"Execute {updateTasksBefore.Count} tasks (before)");
                    foreach (var updateTask in updateTasksBefore)
                    {
                        ExecuteTask(updateTask);
                        UpdateProgressOrCancel();
                    }
                }

                var cachedOperations = new List<IFileOperation>();
                Dictionary<Hash, int> fileAccesses = null;

                if (_windowsPatcherConfig.ActionConfig.FileOperations?.Count > 0)
                {
                    _logger.Info($"Execute {_windowsPatcherConfig.ActionConfig.FileOperations.Count} file operations");
                    var applicationPath = Path.GetFullPath(_windowsPatcherConfig.ApplicationPath);

                    fileAccesses = _windowsPatcherConfig.ActionConfig.FileOperations.OfType<INeedDownload>()
                        .GroupBy(x => x.Target.Hash).ToDictionary(x => x.Key, y => y.Count());

                    foreach (var fileOperation in _windowsPatcherConfig.ActionConfig.FileOperations.OrderBy(x =>
                        x.OperationType))
                    {
                        //if the file operation has anything to do with the application executable file and we replaced this file
                        //we must cache the file operation so we can execute it at last
                        if (_windowsPatcherConfig.ReplaceApplicationExecutable && fileOperation.GetTargetFiles().Any(x =>
                            _environmentManager.TranslateFilename(x.Filename) == applicationPath))
                        {
                            cachedOperations.Add(fileOperation);
                            continue;
                        }

                        ExecuteFileOperation(fileOperation, fileAccesses);
                        UpdateProgressOrCancel();
                    }
                }

                if (updateTasksAfter?.Count > 0)
                {
                    _logger.Info($"Execute {updateTasksAfter.Count} tasks (after)");
                    foreach (var updateTask in updateTasksAfter)
                    {
                        ExecuteTask(updateTask);
                        UpdateProgressOrCancel();
                    }
                }

                if (_windowsPatcherConfig.ReplaceApplicationExecutable)
                {
                    _logger.Info("Revert replace application executable");
                    Status = Translation.RestoreApplicationFile;
                    foreach (var revertable in revertReplaceApplicationFile)
                        Swal.low(() => revertable.Revert(), _logger);
                    UpdateProgress();

                    foreach (var cachedOperation in cachedOperations)
                    {
                        ExecuteFileOperation(cachedOperation, fileAccesses);
                        UpdateProgressOrCancel();
                    }
                }

                //done
            }
            catch (Exception e)
            {
                _logger.Fatal($"Exception occurred in update process: {e}");
                _logger.Info("Begin reversion");
                _statusUpdater.UpdateStatus(Translation.RollbackUpdateProcess);

                _rollbackInfo.Rollback();
                CompleteUpdateProcess(true);
                return;
            }

            Status = Translation.Cleanup;
            UpdateProgress();
            CompleteUpdateProcess(false);
        }

        public void CompleteUpdateProcess(bool failed)
        {
            CleanupTempDirectory();

            if (_windowsPatcherConfig.RestartApplication)
            {
                var mainApplication = _windowsPatcherConfig.ApplicationPath;
                var executionOption = failed ? ExecutionOption.Failed : ExecutionOption.Succeeded;
                var arguments =
                    _windowsPatcherConfig.Arguments?.Where(x =>
                        x.ExecutionOption == ExecutionOption.Always || x.ExecutionOption == executionOption).ToList();

                Process.Start(mainApplication,
                    arguments == null ? null : string.Join(" ", arguments.Select(x => "\"" + x + "\"")));
            }

            //delete the temp directory after 10 seconds
            Process.Start(
                new ProcessStartInfo("cmd.exe",
                    $"/C choice /C Y /N /D Y /T 10 & rmdir /s /q \"{_windowsPatcherConfig.ActionConfig.ProjectId.GetTempDirectory()}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            IsShutdownRequested = true;
            Application.Exit();
        }

        public void CleanupTempDirectory()
        {
            try
            {
                _logger.Info("Delete temp directory for reversion");
                Retry.Do(() =>
                    {
                        _environmentManager.RevertTempDirectory.Refresh();
                        if (_environmentManager.RevertTempDirectory.Exists)
                            _environmentManager.RevertTempDirectory.Delete(true);
                    }, TimeSpan.FromSeconds(2), 3,
                    _logger);
            }
            catch (Exception)
            {
                _logger.Error("Deleting temp directory failed. Add RunOnce entry to delete that directory.");
                //https://stackoverflow.com/a/27037195/4166138
                Swal.low(() => AddRunOnceExecution("CodeElements.UpdateSystem.DeleteRollbackDirectory",
                    $"rmdir /s /q \"{_environmentManager.RevertTempDirectory.FullName}\""), _logger);
            }
        }

        private void ExecuteTask(UpdateTask updateTask)
        {
            var executer = GetExecuter(updateTask);
            if (executer == null)
            {
                //we do not fail the process here because the new update may fix that
                _logger.Fatal(
                    $"The update task {updateTask.Type} was not linked to an executer. Please fix that asap.");
                return;
            }

            try
            {
                executer.ExecuteTask(_environmentManager, updateTask, _logger, _statusUpdater);
            }
            catch (Exception e)
            {
                _logger.Error($"Exception occurred when executing task {updateTask.Type}: {e.Message}");
                if (updateTask.IsImportantForUpdateProcess)
                    throw; //sorry bro

                _logger.Warning(
                    "Luckily the task was not very important for the update process, just continue with the next task");
                return;
            }

            _rollbackInfo.AppendOperation(executer);
        }

        private void ExecuteFileOperation(IFileOperation fileOperation, Dictionary<Hash, int> fileAccesses)
        {
            _logger.Info($"Execute file operation \"{fileOperation.GetType().Name}\"");

            if (fileOperation is MoveFileOperation moveFileOperation)
            {
                var sourceFile = new FileInfo(moveFileOperation.SourceFile.Filename);
                if (!sourceFile.Exists)
                    throw new InvalidOperationException("The source file was not found.");

                for (var i = 0; i < moveFileOperation.Targets.Count; i++)
                {
                    var targetFile = moveFileOperation.Targets[i];
                    _statusUpdater.UpdateStatus(Translation.MoveFile, sourceFile.Name,
                        _statusUpdater.FormatFilename(targetFile.Filename));

                    var filename = _environmentManager.TranslateFilename(targetFile.Filename);
                    DeleteRevertable(filename);

                    _logger.Info($"Move file \"{sourceFile}\" to \"{filename}");

                    if (i == moveFileOperation.Targets.Count - 1)
                    {
                        var sourceFilename = sourceFile.FullName;
                        Retry.Do(() => sourceFile.MoveTo(filename), TimeSpan.FromSeconds(2), 3, _logger);
                        _rollbackInfo.AppendOperation(new RevertMoveFile(filename, sourceFilename));
                    }
                    else
                    {
                        sourceFile.CopyTo(filename);
                        _rollbackInfo.AppendOperation(new RevertDeleteFile(filename));
                    }
                }
            } //DeltaPatchOperation, DownloadFileOperation, UpdateFileOperation
            else if (fileOperation is INeedDownload updateFileOperation)
            {
                _statusUpdater.UpdateStatus(Translation.UpdateFile,
                    _statusUpdater.FormatFilename(updateFileOperation.Target.Filename));

                var sourceFile =
                    new FileInfo(_windowsPatcherConfig.ActionConfig.AvailableFiles[updateFileOperation.Target.Hash]);
                var accessesLeft = fileAccesses[updateFileOperation.Target.Hash];
                fileAccesses[updateFileOperation.Target.Hash] = accessesLeft - 1; //decrease

                var targetFile = _environmentManager.TranslateFilename(updateFileOperation.Target.Filename);
                DeleteRevertable(targetFile);

                if (accessesLeft == 1)
                    Retry.Do(() => sourceFile.MoveTo(targetFile), TimeSpan.FromSeconds(2), 3, _logger);
                else Retry.Do(() => sourceFile.CopyTo(targetFile), TimeSpan.FromSeconds(2), 3, _logger);

                _rollbackInfo.AppendOperation(new RevertDeleteFile(targetFile));
            }
            else if (fileOperation is DeleteFileOperation deleteFileOperation)
            {
                _statusUpdater.UpdateStatus(Translation.DeleteFile,
                    _statusUpdater.FormatFilename(deleteFileOperation.Target.Filename));

                var targetFile = _environmentManager.TranslateFilename(deleteFileOperation.Target.Filename);
                DeleteRevertable(targetFile);

                var directory = new DirectoryInfo(Path.GetDirectoryName(targetFile));
                if (!directory.EnumerateFiles("*", SearchOption.AllDirectories).Any())
                {
                    _logger.Info($"Directory {directory.FullName} was found empty, attempt to remove");
                    Retry.Do(() => directory.Delete(true), TimeSpan.FromSeconds(2), 3, _logger);
                }
            }
        }

        private void DeleteRevertable(string filename)
        {
            var currentFile = new FileInfo(filename);
            if (currentFile.Exists)
            {
                _logger.Info($"File {filename} exists, attempt to move to temp file");
                var tempFile = _environmentManager.GetRandomTempFile();
                Retry.Do(() => currentFile.MoveTo(tempFile.FullName), TimeSpan.FromSeconds(2), 3, _logger);
                _rollbackInfo.AppendOperation(new RevertMoveFile(tempFile.FullName, filename));
            }
        }

        private IUpdateTaskExecuter GetExecuter(UpdateTask updateTask)
        {
            switch (updateTask.Type)
            {
                case UpdateTaskType.DeleteDirectory:
                    return new DeleteDirectoryTaskExecuter();
                case UpdateTaskType.DeleteFiles:
                    return new DeleteFilesTaskExecuter();
                case UpdateTaskType.StartProcess:
                    return new StartProcessTaskExecuter();
                case UpdateTaskType.KillProcess:
                    return new KillProcessTaskExecuter();
                case UpdateTaskType.StartService:
                    return new StartServiceTaskExecuter();
                case UpdateTaskType.StopService:
                    return new StopServiceTaskExecuter();
                case UpdateTaskType.ExecuteBatchScript:
                    return new BatchScriptTaskExecuter();
                case UpdateTaskType.ExecutePowerShellScript:
                    return new PowerShellScriptTaskExecuter();
                default:
                    return null; //that cannot happen, but better safe than sorry
            }
        }

        private List<IRevertable> ReplaceExecutableFile()
        {
            Status = Translation.ReplaceApplicationFile;
            _logger.Info("Begin replace executable file with patcher");

            var newApplicationFile = FileExtensions.MakeUnique(_windowsPatcherConfig.ApplicationPath);
            _logger.Debug("Temporary application path: " + newApplicationFile + ". Attempt to move file.");

            void FileMoveAction()
            {
                File.Move(_windowsPatcherConfig.ApplicationPath, newApplicationFile);
            }

            var result = new List<IRevertable>();
            try
            {
                Retry.Do(FileMoveAction, TimeSpan.FromSeconds(2), 3, _logger);
            }
            catch (IOException)
            {
                if (_hostProcess != null && !_hostProcess.HasExited)
                {
                    _logger.Error(
                        $"Moving temp file failed and host process (PID: {_hostProcess.Id}) is still running. Wait 10 seconds for the exit");

                    Status = Translation.ReplaceApplicationFile;
                    if (_hostProcess.WaitForExit(10000))
                    {
                        _logger.Info("Process exited, retry moving file now.");
                        Status = Translation.ReplaceApplicationFile;
                        try
                        {
                            Retry.Do(FileMoveAction, TimeSpan.FromSeconds(2), 3, _logger);
                        }
                        catch (Exception)
                        {
                            _logger.Error("Cancel replacing executable file.");
                            return null;
                        }
                    }
                    else
                    {
                        _logger.Error("Process did not exit. Cancel replacing executable file.");
                        return null;
                    }
                }
                else
                {
                    _logger.Error("Host process is not running. Cancel replacing executable file.");
                    return null;
                }
            }

            var moveApplicationFileBack = new RevertMoveFile(newApplicationFile, _windowsPatcherConfig.ApplicationPath);
            result.Add(moveApplicationFileBack);
            _rollbackInfo.AppendOperation(moveApplicationFileBack);

            var rollbackAppInfo = new RollbackAppInfo
            {
                PatcherPath = Application.ExecutablePath,
                RequireAdministratorPrivileges = User.IsAdministrator
            };

            var rollbackInfoFile = Path.Combine(Path.GetDirectoryName(_windowsPatcherConfig.ApplicationPath),
                "CodeElements.UpdateSystem.RollbackApp.Info.xml");

            try
            {
                using (var fileStream = new FileStream(rollbackInfoFile, FileMode.Create, FileAccess.Write))
                    new XmlSerializer(typeof(RollbackAppInfo)).Serialize(fileStream, rollbackAppInfo);
            }
            catch (Exception e)
            {
                _logger.Error("Error occurred when trying to write rollbackapp info file: " + e.Message);
                return result; //we can just continue because this operation shouldn't make the update process fail
            }

            var deleteRollbackInfoFile = new RevertDeleteFile(rollbackInfoFile);
            result.Add(deleteRollbackInfoFile);
            _rollbackInfo.AppendOperation(deleteRollbackInfoFile);

            //that file is automatically deleted by the move file operation
            File.WriteAllBytes(_windowsPatcherConfig.ApplicationPath,
                Resources.CodeElements_UpdateSystem_Windows_RollbackApp);

            return result;
        }

        private void AddRunOnceExecution(string name, string command)
        {
            var registryKey =
                (User.IsAdministrator ? Registry.LocalMachine : Registry.CurrentUser).CreateSubKey(
                    "Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce");

            using (registryKey)
            {
                var existingValues = registryKey.GetValueNames();
                var newName = name;
                for (var i = 1;; i++)
                    if (Array.IndexOf(existingValues, newName) > -1)
                        newName = name + " (" + i + ")";
                    else break;

                registryKey.SetValue(newName, command, RegistryValueKind.String);
            }
        }

        private void StatusUpdaterOnStatusUpdated(object sender, string s)
        {
            Status = s;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}