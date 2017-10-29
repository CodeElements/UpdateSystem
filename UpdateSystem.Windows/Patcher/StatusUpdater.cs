using System;
using System.IO;
using CodeElements.UpdateSystem.Windows.Patcher.Translations;

namespace CodeElements.UpdateSystem.Windows.Patcher
{
    internal class StatusUpdater
    {
        public StatusUpdater(IPatcherTranslation translation)
        {
            Translation = translation;
        }

        public IPatcherTranslation Translation { get; }

        public event EventHandler<string> StatusUpdated;

        public void UpdateStatus(string newStatus)
        {
            StatusUpdated?.Invoke(this, newStatus);
        }

        public void UpdateStatus(string newStatus, params object[] args)
        {
            UpdateStatus(string.Format(newStatus, args));
        }

        public string FormatFilename(string filename)
        {
            if (filename.Length > 32)
                return Path.GetFileName(filename);
            return filename;
        }
    }
}