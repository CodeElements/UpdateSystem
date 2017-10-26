using System;
using System.IO;
using CodeElements.UpdateSystem.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal class DeleteDirectoryTaskExecuter : UpdateTaskExecuter<DeleteDirectoryTask>
    {
        public string SourceDirectoryPath { get; set; }
        public string TempDirectory { get; set; }

        public override RevertableType Type { get; } = RevertableType.TaskDeleteDirectory;

        public override void Revert()
        {
            if (TempDirectory == null)
                return;

            Retry.Do(() => FileExtensions.RobustMoveDirectory(TempDirectory, SourceDirectoryPath),
                TimeSpan.FromSeconds(2));
        }

        public override void ExecuteTask(EnvironmentManager environmentManager, DeleteDirectoryTask updateTask,
            Logger logger, StatusUpdater statusUpdater)
        {
            var directory = new DirectoryInfo(environmentManager.TranslateFilename(updateTask.DirectoryPath));
            if (!directory.Exists)
            {
                logger.Info("The directory does not exist.");
                return;
            }

            statusUpdater.UpdateStatus(statusUpdater.Translation.DeleteDirectory,
                statusUpdater.FormatFilename(directory.FullName));

            var tempDirectory = environmentManager.GetRandomTempDirectory();

            SourceDirectoryPath = directory.FullName;
            TempDirectory = tempDirectory.FullName;

            logger.Info($"Move directory {directory.FullName} to {tempDirectory.FullName}");
            Retry.Do(() => FileExtensions.RobustMoveDirectory(directory.FullName, tempDirectory.FullName),
                TimeSpan.FromSeconds(2), 3, logger);
        }
    }
}