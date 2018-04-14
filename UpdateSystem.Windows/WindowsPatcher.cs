using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Windows.Patcher;
using CodeElements.UpdateSystem.Windows.Patcher.Extensions;
using CodeElements.UpdateSystem.Windows.Patcher.Translations;
using CodeElements.UpdateSystem.Windows.Patcher.Utilities;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Windows
{
    /// <summary>
    ///     Defines the default patcher for Microsoft Windows
    /// </summary>
    public class WindowsPatcher : WindowsPatcherConfig, IEnvironmentManager
    {
        private readonly IApplicationCloser _applicationCloser;

        /// <summary>
        ///     Initialize a new instance of <see cref="WindowsPatcher" /> using an application closer
        /// </summary>
        /// <param name="applicationCloser">The application closer defines the procedure to safely shutdown this application</param>
        public WindowsPatcher(IApplicationCloser applicationCloser)
        {
            _applicationCloser = applicationCloser;
            Arguments = new List<UpdateArgument>();
            ApplicationPath = Assembly.GetEntryAssembly().Location;
            BaseDirectory = Path.GetDirectoryName(ApplicationPath);
            Language = WindowsUpdaterTranslation.GetByCulture(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        ///     Make the host application not close. Do not use this application closer unless you are sure that no application
        ///     depdent files get patched
        /// </summary>
        public static IApplicationCloser None { get; } = null;

        /// <summary>
        ///     Defines whether the patcher should be started with administrator privileges. If this property is set to
        ///     <code>false</code>, it will inherit the privileges of the host application (default Windows behavior).
        /// </summary>
        public bool RunAsAdministrator { get; set; } = true;

        /// <summary>
        ///     Use a custom updater user interface.
        /// </summary>
        public UpdaterUi CustomUi { get; set; }

        /// <summary>
        /// The language of the updater 
        /// </summary>
        public IWindowsUpdaterTranslation Language { get; set; }

        private static DirectoryInfo GetTempDirectory(Guid projectId)
        {
            var directory = new DirectoryInfo(projectId.GetTempDirectory());
            if (!directory.Exists) directory.Create();
            return directory;
        }

        internal static string TranslateFilename(string filename, string baseDirectory)
        {
            if (filename.StartsWith("%basedir%", StringComparison.OrdinalIgnoreCase))
                return Path.Combine(baseDirectory, filename.Substring("%basedir%".Length + 1)); //+ 1 for the slash
            return Environment.ExpandEnvironmentVariables(filename);
        }

        Stream IEnvironmentManager.TryOpenRead(string filename)
        {
            var path = TranslateFilename(filename, BaseDirectory);
            if (!File.Exists(path)) return null;
            return File.OpenRead(path);
        }

        IFileInfo IEnvironmentManager.GetStackFile(Guid projectId, Hash hash)
        {
            return new WindowsFileInfo(Path.Combine(GetTempDirectory(projectId).FullName, hash.ToString()));
        }

        IFileInfo IEnvironmentManager.GetDeltaStackFile(Guid projectId, int patchId)
        {
            return new WindowsFileInfo(
                Path.Combine(GetTempDirectory(projectId).CreateSubdirectory("deltaFiles").FullName,
                    patchId.ToString()));
        }

        IFileInfo IEnvironmentManager.GetRandomFile(Guid projectId)
        {
            return new WindowsFileInfo(
                Path.Combine(GetTempDirectory(projectId).CreateSubdirectory("temp").FullName,
                    Guid.NewGuid().ToString("N")));
        }

        void IEnvironmentManager.MoveToStackFiles(Guid projectId, IFileInfo sourceFile, Hash hash)
        {
            var targetFile = ((IEnvironmentManager) this).GetStackFile(projectId, hash);
            File.Move(sourceFile.Filename, targetFile.Filename);

            ((WindowsFileInfo) sourceFile).Refresh();
        }

        void IEnvironmentManager.ExecuteUpdater(PatcherConfig patcherConfig)
        {
            //the patcher directory will contain the patcher executable file aswell as it's dependencies
            var patcherDirectory = GetTempDirectory(patcherConfig.ProjectId).CreateSubdirectory("patcher");

            //copy patcher assembly
            var patcherAssembly = new FileInfo(Assembly.GetAssembly(typeof(WindowsPatcher)).Location);
            var targetLocation = Path.Combine(patcherDirectory.FullName,
                Path.GetFileNameWithoutExtension(patcherAssembly.Name) + ".exe");
            if (File.Exists(targetLocation)) File.Delete(targetLocation);

            patcherAssembly.CopyTo(targetLocation);

            //copy dependencies
            CopyFileSameName(Assembly.GetAssembly(typeof(UpdateController<>)).Location, patcherDirectory);
            CopyFileSameName(Assembly.GetAssembly(typeof(JsonConvert)).Location, patcherDirectory);

            var arguments = new List<string>();

            ActionConfig = patcherConfig;
            File.WriteAllText(Path.Combine(patcherDirectory.FullName, "patcher.cfg"),
                JsonConvert.SerializeObject(this, typeof(WindowsPatcherConfig), Formatting.Indented,
                    Program.JsonSerializerSettings));
            arguments.Add("/config patcher.cfg");

            if (CustomUi != null)
            {
                //copy custom ui assemblies to subfolder
                var updaterUiDirectory = patcherDirectory.CreateSubdirectory("updaterUi");
                var customUiFilename = Path.Combine(updaterUiDirectory.FullName, "CustomUi.dll");
                File.Copy(CustomUi.AssemblyPath, customUiFilename);
                if (CustomUi.RequiredLibraries?.Count > 0)
                    foreach (var requiredLibrary in CustomUi.RequiredLibraries)
                        CopyFileSameName(requiredLibrary, updaterUiDirectory);
                arguments.Add("/updaterUi updaterUi\\CustomUi.dll");
            }

            if (Language == null)
                Language = WindowsUpdaterTranslation.English;

            if (Language is ImplementedUpdaterTranslation implementedUpdaterTranslation)
                arguments.Add("/language " + implementedUpdaterTranslation.KeyName);
            else
            {
                File.WriteAllText(Path.Combine(patcherDirectory.FullName, "language.json"),
                    JsonConvert.SerializeObject(Language.Values));
                arguments.Add("/languageFile language.json");
            }

            var currentProcess = Process.GetCurrentProcess();
            arguments.Add("/hostProcess " + currentProcess.Id);

            var startInfo =
                new ProcessStartInfo(patcherAssembly.FullName, "patch " + string.Join(" ", arguments))
                {
                    UseShellExecute = false,
                    WorkingDirectory = patcherDirectory.FullName
                };
            if (RunAsAdministrator)
                startInfo.Verb = "runas";

            var process = Process.Start(startInfo);
            if (process == null)
                throw new InvalidOperationException("Unable to start patcher process.");

            //close this application
            _applicationCloser?.ExitApplication();
        }

        private static void CopyFileSameName(string fileLocation, DirectoryInfo targetDirectory)
        {
            var targetLocation = Path.Combine(targetDirectory.FullName, Path.GetFileName(fileLocation));
            if (File.Exists(targetLocation)) File.Delete(targetLocation);
            File.Copy(fileLocation, targetLocation);
        }

        void ICleanupUtilities.Cleanup(Guid projectGuid)
        {
            foreach (var directory in Directory.GetDirectories(BaseDirectory,
                "CodeElements.UpdateSystem.Backup*"))
                Swal.low(() => Directory.Delete(directory, true));
        }

        void ICleanupUtilities.NoUpdatesFoundCleanup(Guid projectGuid)
        {
            //cleanup temp directory
            var tempDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"CodeElements.UpdateSystem.{projectGuid:D}"));
            if (tempDirectory.Exists)
                Swal.low(() => tempDirectory.Delete(true));
        }
    }
}