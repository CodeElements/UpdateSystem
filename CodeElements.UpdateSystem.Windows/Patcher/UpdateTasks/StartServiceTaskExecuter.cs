using System;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using CodeElements.UpdateSystem.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal class StartServiceTaskExecuter : UpdateTaskExecuter<StartServiceTask>
    {
        public override RevertableType Type { get; } = RevertableType.TaskStartService;

        public override void Revert()
        {
        }

        public override void ExecuteTask(EnvironmentManager environmentManager, StartServiceTask updateTask,
            Logger logger, StatusUpdater statusUpdater)
        {
            var service = new ServiceController(updateTask.ServiceName);

            if (service.Status != ServiceControllerStatus.Running)
            {
                logger.Info($"Service \"{updateTask.ServiceName}\" is already running");
                if (!updateTask.RestartIfAlreadyRunning)
                    return;

                statusUpdater.UpdateStatus(statusUpdater.Translation.StopService, updateTask.ServiceName);
                logger.Info("Stop service");
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }

            statusUpdater.UpdateStatus(statusUpdater.Translation.StartService, updateTask.ServiceName);
            string[] arguments;
            if (!string.IsNullOrWhiteSpace(updateTask.Arguments))
                arguments = Regex.Matches(updateTask.Arguments, @"[\""].+?[\""]|[^ ]+")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();
            else arguments = new string[0];

            logger.Info($"Start service with {arguments.Length} argument(s)");
            service.Start(arguments);
            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
        }
    }
}