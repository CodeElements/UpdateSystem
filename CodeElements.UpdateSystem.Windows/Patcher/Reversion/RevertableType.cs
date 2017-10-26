namespace CodeElements.UpdateSystem.Windows.Patcher.Reversion
{
    public enum RevertableType
    {
        TaskDeleteDirectory,
        TaskDeleteFiles,
        TaskExecuteBatchScript,
        TaskExecutePowerShellScript,
        TaskKillProcess,
        TaskStartProcess,
        TaskStartService,
        TaskStopService,
        MoveFile,
        DeleteFile
    }
}