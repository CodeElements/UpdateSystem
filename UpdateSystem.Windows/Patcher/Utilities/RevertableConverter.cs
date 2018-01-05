using System;
using CodeElements.UpdateSystem.Utilities;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.UpdateTasks;
using Newtonsoft.Json.Linq;

namespace CodeElements.UpdateSystem.Windows.Patcher.Utilities
{
    internal class RevertableConverter : JsonCreationConverter<IRevertable>
    {
        protected override IRevertable Create(Type objectType, JObject jObject)
        {
            var revertableType = (RevertableType) (int) jObject["type"];
            switch (revertableType)
            {
                case RevertableType.TaskDeleteDirectory:
                    return new DeleteDirectoryTaskExecuter();
                case RevertableType.TaskDeleteFiles:
                    return new DeleteFilesTaskExecuter();
                case RevertableType.TaskExecuteBatchScript:
                    return new BatchScriptTaskExecuter();
                case RevertableType.TaskExecutePowerShellScript:
                    return new PowerShellScriptTaskExecuter();
                case RevertableType.TaskKillProcess:
                    return new KillProcessTaskExecuter();
                case RevertableType.TaskStartProcess:
                    return new StartProcessTaskExecuter();
                case RevertableType.TaskStartService:
                    return new StartServiceTaskExecuter();
                case RevertableType.TaskStopService:
                    return new StopServiceTaskExecuter();
                case RevertableType.MoveFile:
                    return new RevertMoveFile();
                case RevertableType.DeleteFile:
                    return new RevertDeleteFile();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}