using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CodeElements.UpdateSystem.Windows.Patcher.Reversion;
using CodeElements.UpdateSystem.Windows.Patcher.Translations;
using CodeElements.UpdateSystem.Windows.Patcher.UI;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Windows.Patcher
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            switch (args[0])
            {
                case "patch":
                    string configFilename = null;
                    var hostProcessId = -1;
                    string customUpdaterUiFile = null;
                    var language = WindowsUpdaterTranslation.English;

                    for (var i = 1; i < args.Length; i++)
                        switch (args[i])
                        {
                            case "/config":
                                configFilename = args[++i];
                                break;
                            case "/hostProcess":
                                hostProcessId = int.Parse(args[++i]);
                                break;
                            case "/updaterUi":
                                customUpdaterUiFile = args[++i];
                                break;
                            case "/language":
                                language = WindowsUpdaterTranslation.ImplementedLanguages[args[++i]];
                                break;
                            case "/languageFile":
                                language = new WindowsUpdaterTranslation(
                                    JsonConvert.DeserializeObject<Dictionary<string, string>>(
                                        File.ReadAllText(args[++i])));
                                break;
                        }

                    if (string.IsNullOrWhiteSpace(configFilename) || hostProcessId == -1)
                        Environment.Exit(-24);

                    AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

                    var config = JsonConvert.DeserializeObject<WindowsPatcherConfig>(File.ReadAllText(configFilename));
                    var hostProcess = Process.GetProcessById(hostProcessId);
                    var updater = new UpdaterCore(config, hostProcess, new WindowsPatcherTranslation(language));

                    if (config.RunSilently)
                        updater.Update(CancellationToken.None);
                    else
                    {
                        if (customUpdaterUiFile != null)
                        {
                            var updaterUiDirectory = new DirectoryInfo("updaterUi");
                            try
                            {
                                var assembly = Assembly.LoadFrom(Path.Combine(updaterUiDirectory.FullName, "CustomUi.dll"));

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                        }

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new InstallForm(updater));
                    }
                    return;
                case "rollback":
                    var rollbackFile = new FileInfo("rollbackInfo.json");
                    if (!rollbackFile.Exists)
                    {
                        MessageBox.Show(
                            "Cannot initiate rollback process because the file \"rollbackInfo.json\" was not found in the application folder.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var rollbackInfo =
                        JsonConvert.DeserializeObject<RollbackInfo>(File.ReadAllText(rollbackFile.FullName));
                    rollbackInfo.Rollback();

                    updater = new UpdaterCore(rollbackInfo.PatcherConfig, null, WindowsPatcherTranslation.Default);
                    updater.CleanupTempDirectory();
                    updater.CompleteUpdateProcess(true);
                    return;
            }
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            if (File.Exists("rollbackInfo.json"))
            {
                MessageBox.Show(
                    "A critical exception occurred when trying to apply the update.\r\n" +
                    unhandledExceptionEventArgs.ExceptionObject +
                    "\r\n\r\nThis application will now restart and initiate a rollback.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Process.Start(Application.ExecutablePath, "rollback");
            }
            else if (Environment.GetCommandLineArgs()[1] == "patch")
            {
                var commandLineArgs = Environment.GetCommandLineArgs();
                MessageBox.Show(
                    "A critical exception occurred when trying to initate the update.\r\n" +
                    unhandledExceptionEventArgs.ExceptionObject +
                    "\r\n\r\nThis application will now restart and initiate a rollback.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                var config = JsonConvert.DeserializeObject<WindowsPatcherConfig>(File.ReadAllText(commandLineArgs[Array.IndexOf(commandLineArgs, "/config") + 1]));
                var updater = new UpdaterCore(config, null, WindowsPatcherTranslation.Default);
                updater.CompleteUpdateProcess(true);
                return;
            }

            Environment.Exit(-19);
        }
    }
}