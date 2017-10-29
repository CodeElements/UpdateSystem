using System.Windows.Forms;

namespace CodeElements.UpdateSystem.Windows.WinForms
{
    /// <summary>
    ///     A terminator for a Windows Forms app. Invokes <see cref="Application.Exit()" />
    /// </summary>
    public class WindowsFormsApplicationCloser : IApplicationCloser
    {
        void IApplicationCloser.ExitApplication()
        {
            Application.Exit();
        }
    }
}