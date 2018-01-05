using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CodeElements.UpdateSystem.Windows.RollbackApp
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var rollbackFile = new FileInfo("CodeElements.UpdateSystem.RollbackApp.Info.xml");
            if (!rollbackFile.Exists)
            {
                MessageBox.Show($"\"{rollbackFile.Name}\" was not found.");
                return;
            }

            if (MessageBox.Show(
                    "It seems like someting went wrong when you tried to update this application. Do you want to undo the changes?",
                    "Update failed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var rollbackInfo = (RollbackAppInfo)
                new XmlSerializer(typeof(RollbackAppInfo)).Deserialize(
                    new StringReader(File.ReadAllText(rollbackFile.FullName)));

            var startInfo = new ProcessStartInfo(rollbackInfo.PatcherPath) {Arguments = "rollback"};
            if (rollbackInfo.RequireAdministratorPrivileges)
                startInfo.Verb = "runas";
            Process.Start(startInfo);
        }
    }
}