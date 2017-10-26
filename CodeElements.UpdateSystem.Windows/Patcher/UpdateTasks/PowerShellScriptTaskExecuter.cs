using System.Diagnostics;
using System.IO;
using CodeElements.UpdateSystem.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal class PowerShellScriptTaskExecuter : UpdateTaskExecuter<ExecutePowerShellScriptTask>
    {
        public override RevertableType Type { get; } = RevertableType.TaskExecutePowerShellScript;

        public override void Revert()
        {
            //cannot revert this
        }

        public override void ExecuteTask(EnvironmentManager environmentManager, ExecutePowerShellScriptTask updateTask,
            Logger logger, StatusUpdater statusUpdater)
        {
            statusUpdater.UpdateStatus(statusUpdater.Translation.WritePowerShellScript);

            var scriptFilename = FileExtensions.MakeUnique(Path.Combine(environmentManager.RevertTempDirectory.FullName,
                "PowerShell-Script.ps1"));

            logger.Info($"PowerShell script filename: {scriptFilename}");

            File.WriteAllText(scriptFilename, updateTask.Code);

            statusUpdater.UpdateStatus(statusUpdater.Translation.ExecutePowerShellScript);
            var processStartInfo = new ProcessStartInfo("powershell.exe")
            {
                Arguments = scriptFilename,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var process = Process.Start(processStartInfo);
            logger.Info("Script started, wait for exit");

            process.WaitForExit();
        }
    }
}