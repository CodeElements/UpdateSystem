using System;
using System.Collections.Generic;
using System.IO;
using CodeElements.UpdateSystem.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal class DeleteFilesTaskExecuter : UpdateTaskExecuter<DeleteFilesTask>
    {
        public List<RevertMoveFile> MoveFileInfos { get; set; }

        public override RevertableType Type { get; } = RevertableType.TaskDeleteFiles;

        public override void Revert()
        {
            if (MoveFileInfos?.Count > 0)
                foreach (var moveFileInfo in MoveFileInfos)
                    moveFileInfo.Revert();
        }

        public override void ExecuteTask(EnvironmentManager environmentManager, DeleteFilesTask updateTask,
            Logger logger, StatusUpdater statusUpdater)
        {
            if (!(updateTask.Filenames?.Count > 0))
                return;

            foreach (var filename in updateTask.Filenames)
            {
                statusUpdater.UpdateStatus(statusUpdater.Translation.DeleteFile, filename);

                var file = new FileInfo(environmentManager.TranslateFilename(filename));
                if (!file.Exists)
                {
                    logger.Info($"File \"{file.FullName}\" does not exist, continue.");
                    continue;
                }

                if (MoveFileInfos == null)
                    MoveFileInfos = new List<RevertMoveFile>();

                var tempFile = environmentManager.GetRandomTempFile();
                logger.Info($"Move file \"{file.FullName}\" to \"{tempFile.FullName}\"");

                try
                {
                    Retry.Do(() => file.MoveTo(tempFile.FullName), TimeSpan.FromSeconds(2));
                }
                catch (Exception)
                {
                    if (updateTask.IsImportantForUpdateProcess)
                    {
                        logger.Error("Moving file failed, revert other file movements of this task");
                        Revert();
                        throw;
                    }

                    logger.Warning(
                        "Moving file failed, but because the task is not very important, we can just continue.");
                    continue;
                }

                MoveFileInfos.Add(new RevertMoveFile(tempFile.FullName, file.FullName));
            }
        }
    }
}