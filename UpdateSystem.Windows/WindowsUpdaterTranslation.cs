using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CodeElements.UpdateSystem.Windows.Patcher.Translations;

namespace CodeElements.UpdateSystem.Windows
{
    /// <summary>
    ///     A simple class that implements the <see cref="IWindowsUpdaterTranslation" /> interface
    /// </summary>
    public class WindowsUpdaterTranslation : IWindowsUpdaterTranslation
    {
        /// <summary>
        /// Initialize a new instance of <see cref="WindowsUpdaterTranslation"/>
        /// </summary>
        /// <param name="values">The translated terms</param>
        public WindowsUpdaterTranslation(Dictionary<string, string> values)
        {
            Values = values;
        }

        /// <summary>
        ///     The default english translation
        /// </summary>
        public static IWindowsUpdaterTranslation English { get; } =
            new ImplementedUpdaterTranslation("en", CodeElements.UpdateSystem.Windows.Properties.Resources.EnglishTranslation);

        /// <summary>
        ///     The default german translation
        /// </summary>
        public static IWindowsUpdaterTranslation German { get; } =
            new ImplementedUpdaterTranslation("de", CodeElements.UpdateSystem.Windows.Properties.Resources.GermanTranslation);

        /// <summary>
        ///     The translated terms
        /// </summary>
        public Dictionary<string, string> Values { get; }

        internal static readonly Dictionary<string, ImplementedUpdaterTranslation> ImplementedLanguages =
            new Dictionary<string, ImplementedUpdaterTranslation>(new[] {English, German}
                .Cast<ImplementedUpdaterTranslation>().ToDictionary(x => x.KeyName, y => y));

        /// <summary>
        ///     Get the best translation match from the existing ones by a culture info
        /// </summary>
        /// <param name="cultureInfo">The culture info that should represent the language of the client.</param>
        /// <returns>Return the best matched language.</returns>
        public static IWindowsUpdaterTranslation GetByCulture(CultureInfo cultureInfo)
        {
            var existingTranslatios =
                new Dictionary<CultureInfo, ImplementedUpdaterTranslation>(
                    ImplementedLanguages.ToDictionary(x => x.Value.CultureInfo, y => y.Value));
            if (existingTranslatios.TryGetValue(cultureInfo, out var translation))
                return translation;

            var languageMatch = existingTranslatios.Where(x =>
                    x.Key.TwoLetterISOLanguageName.Substring(2) == cultureInfo.TwoLetterISOLanguageName.Substring(2))
                .ToArray();
            if (languageMatch.Length > 0)
                return languageMatch[0].Value;

            return English;
        }

        /// <summary>
        ///     Create a new language with a Dictionary
        /// </summary>
        /// <param name="values">The translated terms</param>
        /// <returns>Return a translation that contains the terms given as parameter</returns>
        public static IWindowsUpdaterTranslation Create(Dictionary<string, string> values)
        {
            return new WindowsUpdaterTranslation(values);
        }
    }
}