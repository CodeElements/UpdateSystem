using System.IO;

namespace CodeElements.UpdateSystem.Windows.Patcher.Reversion
{
    internal class RevertDeleteFile : IRevertable
    {
        public RevertDeleteFile(string filename)
        {
            Filename = filename;
        }

        public RevertDeleteFile()
        {
        }

        public string Filename { get; set; }

        public void Revert()
        {
            File.Delete(Filename);
        }

        public RevertableType Type { get; } = RevertableType.DeleteFile;
    }
}