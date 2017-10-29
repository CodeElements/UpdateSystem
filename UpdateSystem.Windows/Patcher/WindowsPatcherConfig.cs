using System.Collections.Generic;
using CodeElements.UpdateSystem.Core;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Windows.Patcher
{
    public class WindowsPatcherConfig
    {
        [JsonProperty]
        internal PatcherConfig ActionConfig { get; set; }

        /// <summary>
        ///     Run the patcher without opening a window or displaying any status information
        /// </summary>
        public bool RunSilently { get; set; } = false;

        /// <summary>
        ///     Restart this application when the update process finished
        /// </summary>
        public bool RestartApplication { get; set; } = true;

        /// <summary>
        ///     The path to the main application (the executable file that has to be started in order to run the current
        ///     application). This application will be started after the update process finished if
        ///     <see cref="RestartApplication" /> is set to <code>true</code>. The default value is the location of the entry
        ///     assembly
        /// </summary>
        public string ApplicationPath { get; set; }

        /// <summary>
        ///     The path to the folder of the main application. This path will replace the environemnt variable %basedir%. The
        ///     default value is the folder name of <see cref="ApplicationPath" />
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        ///     Replace the executable file of the main application (<see cref="ApplicationPath" />) with a simple file that would
        ///     invoke the patcher to revert changes while the patcher is running. That will prevent a broken filebase in case that
        ///     the patcher/system crashes.
        ///     The patcher will always track what has to be done to revert the changes it made. In case that the patcher crashes
        ///     while patching and the user wants to execute the core application, the patcher will be executed and it will revert
        ///     the changes. Please note that the application must be completely closed when this option is activated.
        /// </summary>
        public bool ReplaceApplicationExecutable { get; set; } = true;

        /// <summary>
        ///     Arguments that should given to this application on specific events when the application is opened by the patcher.
        ///     This property will have no effect unless <see cref="RestartApplication" /> is true
        /// </summary>
        [JsonProperty]
        public List<UpdateArgument> Arguments { get; protected set; }
    }
}