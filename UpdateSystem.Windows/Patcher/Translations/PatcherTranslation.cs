using System.Runtime.CompilerServices;

namespace CodeElements.UpdateSystem.Windows.Patcher.Translations
{
    public abstract class PatcherTranslation : IPatcherTranslation
    {
        public string ReplaceApplicationFile => GetValue();
        public string RestoreApplicationFile => GetValue();
        public string Cleanup => GetValue();
        public string DeleteFile => GetValue();
        public string UpdateFile => GetValue();
        public string MoveFile => GetValue();
        public string ExecuteBatchScript => GetValue();
        public string WriteBatchScript => GetValue();
        public string DeleteDirectory => GetValue();
        public string KillProcess => GetValue();
        public string WritePowerShellScript => GetValue();
        public string ExecutePowerShellScript => GetValue();
        public string StartProcess => GetValue();
        public string WaitingForProcessToExit => GetValue();
        public string StartService => GetValue();
        public string StopService => GetValue();
        public string RollbackUpdateProcess => GetValue();
        public string Cancel => GetValue();
        public string ApplyUpdates => GetValue();
        public string SureCancelUpdate => GetValue();
        public string Warning => GetValue();
        public string WaitForApplicationToShutdown => GetValue();


        protected abstract string GetValue([CallerMemberName] string key = null);
    }
}