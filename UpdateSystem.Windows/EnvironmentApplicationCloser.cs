using System;

namespace CodeElements.UpdateSystem.Windows
{
    /// <summary>
    ///     A simple application closer that invokes <see cref="Environment.Exit" />. Please do not use this unless you have no
    ///     other possibility. For WPF or WinForms, you should use the custom ones
    /// </summary>
    public class EnvironmentApplicationCloser : IApplicationCloser
    {
        private EnvironmentApplicationCloser()
        {
        }

        /// <summary>
        ///     Get an instance of this class
        /// </summary>
        public static IApplicationCloser Instance { get; } = new EnvironmentApplicationCloser();

        void IApplicationCloser.ExitApplication()
        {
            Environment.Exit(0);
        }
    }
}