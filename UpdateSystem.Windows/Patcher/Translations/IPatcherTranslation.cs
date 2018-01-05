namespace CodeElements.UpdateSystem.Windows.Patcher.Translations
{
    public interface IPatcherTranslation
    {
        string ReplaceApplicationFile { get; }
        string RestoreApplicationFile { get; }
        string Cleanup { get; }
        string DeleteFile { get; } //{0} = file
        string UpdateFile { get; } //{0} = file
        string MoveFile { get; } //{0} = source file, {1} = destination file
        string ExecuteBatchScript { get; }
        string WriteBatchScript { get; }
        string DeleteDirectory { get; } //{0} = directory name
        string KillProcess { get; } // {0} = process name
        string WritePowerShellScript { get; }
        string ExecutePowerShellScript { get; }
        string StartProcess { get; } //{0} = filename
        string WaitingForProcessToExit { get; } //{0} = process name
        string StartService { get; } //{0} = service name
        string StopService { get; } //{0} = service name
        string RollbackUpdateProcess { get; }
        string Cancel { get; }
        string ApplyUpdates { get; }
        string SureCancelUpdate { get; }
        string Warning { get; }
        string WaitForApplicationToShutdown { get; }
    }
}