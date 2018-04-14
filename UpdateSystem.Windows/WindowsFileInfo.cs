using System.IO;
using CodeElements.UpdateSystem.Core;

namespace CodeElements.UpdateSystem.Windows
{
    internal class WindowsFileInfo : IFileInfo
    {
        private readonly FileInfo _fileInfo;

        public WindowsFileInfo(string path)
        {
            _fileInfo = new FileInfo(path);
        }

        public string Filename => _fileInfo.FullName;
        public bool Exists => _fileInfo.Exists;

        public Stream OpenRead()
        {
            return _fileInfo.OpenRead();
        }

        public Stream Create()
        {
            return _fileInfo.Create();
        }

        public void Delete()
        {
            _fileInfo.Delete();
        }

        public void Refresh()
        {
            _fileInfo.Refresh();
        }
    }
}