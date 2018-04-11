using System.Windows;

namespace CodeElements.UpdateSystem.Windows.Wpf
{
    /// <summary>
    ///     A terminator for a Wpf app. Invokes <see cref="Application.Current.Shutdown()" />
    /// </summary>
    public class WpfApplicationCloser : IApplicationCloser
    {
        void IApplicationCloser.ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}