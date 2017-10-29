using System.Collections.Generic;

namespace CodeElements.UpdateSystem.Windows.Patcher.Translations
{
    internal class WindowsPatcherTranslation : PatcherTranslation
    {
        private readonly IDictionary<string, string> _translations;

        public WindowsPatcherTranslation(IWindowsUpdaterTranslation windowsUpdaterTranslation)
        {
            _translations = windowsUpdaterTranslation.Values;
        }

        public static WindowsPatcherTranslation Default { get; } =
            new WindowsPatcherTranslation(WindowsUpdaterTranslation.English);

        protected override string GetValue(string key = null)
        {
            return _translations[key];
        }
    }
}