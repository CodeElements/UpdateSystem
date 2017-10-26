using System;
using System.Diagnostics;
using CodeElements.UpdateSystem.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal class StartProcessTaskExecuter : UpdateTaskExecuter<StartProcessTask>
    {
        public override RevertableType Type { get; } = RevertableType.TaskStartProcess;

        public int? ProcessId { get; set; }
        public string ProcessName { get; set; }

        public override void Revert()
        {
            if (ProcessId != null)
                try
                {
                    var process = Process.GetProcessById(ProcessId.Value);
                    if (process.ProcessName == ProcessName)
                        process.Kill();
                }
                catch (Exception)
                {
                    // ignored
                }
        }

        public override void ExecuteTask(EnvironmentManager environmentManager, StartProcessTask updateTask,
            Logger logger, StatusUpdater statusUpdater)
        {
            statusUpdater.UpdateStatus(statusUpdater.Translation.StartProcess,
                statusUpdater.FormatFilename(updateTask.Filename));

            var processStartInfo = new ProcessStartInfo(environmentManager.TranslateFilename(updateTask.Filename),
                updateTask.Arguments);
            if (updateTask.RequireElevatedPrivileges)
                processStartInfo.Verb = "runas";

            var process = Process.Start(processStartInfo);
            if (process == null) throw new InvalidOperationException("Unable to start process");

            ProcessId = process.Id;
            ProcessName = process.ProcessName;

            if (updateTask.WaitForExit)
            {
                statusUpdater.UpdateStatus(statusUpdater.Translation.WaitingForProcessToExit, process.ProcessName);

                process.WaitForExit();
                if (updateTask.FailIfProcessReturnsFailure && process.ExitCode != 0)
                    throw new InvalidOperationException(
                        $"The process returned the exit code {process.ExitCode}. Because that code is not zero and \"FailIfProcessReturnsFailure\" is checked, the operation must be aborted.");
            }
        }
    }
}