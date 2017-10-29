using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CodeElements.UpdateSystem.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal class KillProcessTaskExecuter : UpdateTaskExecuter<KillProcessTask>
    {
        public override RevertableType Type { get; } = RevertableType.TaskKillProcess;

        public List<string> KilledProcesses { get; set; }

        public override void Revert()
        {
            if (KilledProcesses?.Count > 0)
                foreach (var killedProcess in KilledProcesses)
                    Process.Start(killedProcess);
        }

        public override void ExecuteTask(EnvironmentManager environmentManager, KillProcessTask updateTask,
            Logger logger, StatusUpdater statusUpdater)
        {
            Process[] processes;
            switch (updateTask.SearchMode)
            {
                case ProcessSearchMode.ProcessName:
                    processes = Process.GetProcessesByName(updateTask.SearchString);
                    break;
                case ProcessSearchMode.ProcessNameContains:
                    processes = Process.GetProcesses().Where(x =>
                            x.ProcessName.IndexOf(updateTask.SearchString, StringComparison.OrdinalIgnoreCase) > -1)
                        .ToArray();
                    break;
                case ProcessSearchMode.Filename:
                    var filename = environmentManager.TranslateFilename(updateTask.SearchString);
                    processes = Process.GetProcesses().Where(x =>
                    {
                        try
                        {
                            return string.Equals(x.MainModule.FileName, filename, StringComparison.OrdinalIgnoreCase);
                        }
                        catch (Exception) //invalid access exception or something else
                        {
                            return false;
                        }
                    }).ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (processes.Length == 0)
            {
                logger.Info("No processes found, finish task");
                return;
            }

            logger.Info($"{processes.Length} process(es) found");
            foreach (var process in processes)
            {
                statusUpdater.UpdateStatus(statusUpdater.Translation.KillProcess, process.ProcessName);

                var closedMainWindow = false;
                string processFilename = null;

                logger.Debug($"Get process filename of process {process.ProcessName}");
                try
                {
                    processFilename = process.MainModule.FileName;
                }
                catch (Exception e)
                {
                    logger.Warning($"Getting process filename failed: {e.Message}");
                }

                logger.Debug("Attempt to close main window");
                try
                {
                    closedMainWindow = process.CloseMainWindow();
                }
                catch (Exception e)
                {
                    logger.Warning($"Closing main window failed: {e.Message}");
                }

                logger.Debug($"Closing main window result: {closedMainWindow}");
                try
                {
                    if (!closedMainWindow || !process.WaitForExit(3000))
                    {
                        logger.Info("Kill process");
                        process.Kill();
                    }
                }
                catch (Exception e)
                {
                    logger.Warning($"Killing process failed: {e.Message}");

                    if (updateTask.IsImportantForUpdateProcess)
                        throw;

                    continue;
                }

                if (processFilename != null)
                {
                    if (KilledProcesses == null) KilledProcesses = new List<string>();
                    KilledProcesses.Add(processFilename);
                }
            }
        }
    }
}