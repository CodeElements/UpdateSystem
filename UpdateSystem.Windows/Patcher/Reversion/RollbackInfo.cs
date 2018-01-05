using System.Collections.Generic;
using System.IO;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Windows.Patcher.Reversion
{
    internal class RollbackInfo
    {
        private readonly string _path;

        public RollbackInfo(string path)
        {
            _path = path;
            Operations = new List<IRevertable>();
        }

        private RollbackInfo()
        {
        }

        public string ApplicationPath { get; set; }
        public WindowsPatcherConfig PatcherConfig { get; set; }
        public List<IRevertable> Operations { get; set; }

        public void AppendOperation(IRevertable revertable)
        {
            Operations.Add(revertable);
            Save();
        }

        public void Save()
        {
            File.WriteAllText(_path,
                JsonConvert.SerializeObject(this, Formatting.Indented, Program.JsonSerializerSettings));
        }

        public void Rollback()
        {
            for (int i = Operations.Count - 1; i >= 0; i--)
                Swal.low(() => Operations[i].Revert());
        }
    }
}