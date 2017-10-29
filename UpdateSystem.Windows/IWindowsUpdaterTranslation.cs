using System.Collections.Generic;
using CodeElements.UpdateSystem.Windows.Patcher.Translations;

namespace CodeElements.UpdateSystem.Windows
{
    /// <summary>
    ///     Represents a translation. The values must be the properties of <see cref="IPatcherTranslation" />
    /// </summary>
    public interface IWindowsUpdaterTranslation
    {
        /// <summary>
        ///     The keys that are required can be found in <see cref="IPatcherTranslation" /> and the values should be the
        ///     translations.
        /// </summary>
        Dictionary<string, string> Values { get; }
    }
}