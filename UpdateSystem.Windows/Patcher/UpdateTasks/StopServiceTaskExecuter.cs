using System;
using System.ServiceProcess;
using CodeElements.UpdateSystem.UpdateTasks;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal class StopServiceTaskExecuter : UpdateTaskExecuter<StopServiceTask>
    {
        public string ServiceName { get; set; }

        public override RevertableType Type { get; } = RevertableType.TaskStartService;

        public override void Revert()
        {
            if (ServiceName == null)
                return;

            var service = new ServiceController(ServiceName);
            service.Start();
        }

        public override void ExecuteTask(EnvironmentManager environmentManager, StopServiceTask updateTask,
            Logger logger, StatusUpdater statusUpdater)
        {
            var service = new ServiceController(updateTask.ServiceName);
            if (service.Status == ServiceControllerStatus.Running)
            {
                statusUpdater.UpdateStatus(statusUpdater.Translation.StopService, updateTask.ServiceName);

                //only save the service name when the service was running
                ServiceName = updateTask.ServiceName;
                service.Stop();
                if (updateTask.WaitForExit) //throws a timeout exception
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }
            else
            {
                logger.Info($"Service \"{updateTask.ServiceName}\" isn't running");
            }
        }
    }
}