using System.Collections.Generic;

namespace CodeElements.UpdateSystem.Windows
{
    /// <summary>
    ///     A custom updater UI
    /// </summary>
    public class UpdaterUi
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="UpdaterUi" />
        /// </summary>
        /// <param name="assemblyPath">The assembly path of the library that contains the interface code</param>
        public UpdaterUi(string assemblyPath)
        {
            AssemblyPath = assemblyPath;
            RequiredLibraries = new List<string>();
        }

        /// <summary>
        ///     The assembly path of the custom UI
        /// </summary>
        public string AssemblyPath { get; }

        /// <summary>
        ///     The paths to libraries that are required by the updater UI
        /// </summary>
        public List<string> RequiredLibraries { get; }
    }
}