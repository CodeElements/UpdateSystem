using CodeElements.UpdateSystem.UpdateTasks.Base;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;

namespace CodeElements.UpdateSystem.Windows.Patcher
{
    internal interface IUpdateTaskExecuter : IRevertable
    {
        void ExecuteTask(EnvironmentManager environmentManager, UpdateTask updateTask, Logger logger,
            StatusUpdater statusUpdater);
    }
}