using System.Diagnostics;
using System.IO;
using CodeElements.UpdateSystem.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal class BatchScriptTaskExecuter : UpdateTaskExecuter<ExecuteBatchScriptTask>
    {
        public override RevertableType Type { get; } = RevertableType.TaskExecuteBatchScript;

        public override void Revert()
        {
            //cannot revert this
        }

        public override void ExecuteTask(EnvironmentManager environmentManager, ExecuteBatchScriptTask updateTask,
            Logger logger, StatusUpdater statusUpdater)
        {
            statusUpdater.UpdateStatus(statusUpdater.Translation.WriteBatchScript);

            var batchFilename = FileExtensions.MakeUnique(Path.Combine(environmentManager.RevertTempDirectory.FullName,
                "Batch-Script.bat"));

            logger.Info($"Batch script filename: {batchFilename}");

            File.WriteAllText(batchFilename, updateTask.Code);

            statusUpdater.UpdateStatus(statusUpdater.Translation.ExecuteBatchScript);
            var processStartInfo = new ProcessStartInfo(batchFilename) {CreateNoWindow = true, UseShellExecute = false};
            var process = Process.Start(processStartInfo);
            logger.Info("Script started, wait for exit");

            process.WaitForExit();
        }
    }
}