using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CodeElements.UpdateSystem.Windows.Patcher.Translations
{
    internal class ImplementedUpdaterTranslation : IWindowsUpdaterTranslation
    {
        public ImplementedUpdaterTranslation(string keyName, string resourceString)
        {
            KeyName = keyName;
            CultureInfo = new CultureInfo(keyName);

            Values = resourceString
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split('='))
                .ToDictionary(x => x[0], y => y[1]);
        }

        public CultureInfo CultureInfo { get; }
        public string KeyName { get; }
        public Dictionary<string, string> Values { get; }
    }
}