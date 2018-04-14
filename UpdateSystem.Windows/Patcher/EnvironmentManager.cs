using System.IO;
using CodeElements.UpdateSystem.Windows.Patcher.Extensions;

namespace CodeElements.UpdateSystem.Windows.Patcher
{
    internal class EnvironmentManager
    {
        public EnvironmentManager(WindowsPatcherConfig config)
        {
            BaseDirectory = config.BaseDirectory;

            //use a directory in the base directory if possible because the move operations will be much faster then
            if (FileExtensions.HasWriteAccessToFolder(BaseDirectory))
                RevertTempDirectory =
                    new DirectoryInfo(FileExtensions.MakeDirectoryUnique(Path.Combine(BaseDirectory,
                        "CodeElements.UpdateSystem.Backup")));
            else RevertTempDirectory = new DirectoryInfo(Path.Combine(config.ActionConfig.ProjectId.GetTempDirectory(), "backup"));
            RevertTempDirectory.Create();
        }

        public string BaseDirectory { get; }
        public DirectoryInfo RevertTempDirectory { get; }

        public DirectoryInfo GetRandomTempDirectory()
        {
            while (true)
            {
                var directoryInfo =
                    new DirectoryInfo(Path.Combine(RevertTempDirectory.FullName, Path.GetRandomFileName()));
                if (!directoryInfo.Exists)
                    return directoryInfo;
            }
        }

        public FileInfo GetRandomTempFile()
        {
            while (true)
            {
                var fileInfo =
                    new FileInfo(Path.Combine(RevertTempDirectory.FullName, Path.GetRandomFileName()));
                if (!fileInfo.Exists)
                    return fileInfo;
            }
        }

        public string TranslateFilename(string filename)
        {
            return WindowsPatcher.TranslateFilename(filename, BaseDirectory);
        }
    }
}