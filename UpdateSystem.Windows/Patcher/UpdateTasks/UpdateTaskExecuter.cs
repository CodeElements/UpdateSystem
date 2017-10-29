using CodeElements.UpdateSystem.UpdateTasks.Base;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks
{
    internal abstract class UpdateTaskExecuter<TUpdateTask> : IUpdateTaskExecuter where TUpdateTask : UpdateTask
    {
        public abstract RevertableType Type { get; }
        public abstract void Revert();

        public virtual void ExecuteTask(EnvironmentManager environmentManager, UpdateTask updateTask, Logger logger,
            StatusUpdater statusUpdater)
        {
            ExecuteTask(environmentManager, (TUpdateTask) updateTask, logger, statusUpdater);
        }

        public abstract void ExecuteTask(EnvironmentManager environmentManager, TUpdateTask updateTask, Logger logger,
            StatusUpdater statusUpdater);
    }
}