namespace CodeElements.UpdateSystem.Windows.Wpf
{
    /// <summary>
    ///     The patcher for Wpf
    /// </summary>
    public class WpfPatcher : WindowsPatcher
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="WpfPatcher" /> using the <see cref="WpfApplicationCloser" />
        /// </summary>
        public WpfPatcher() : base(new WpfApplicationCloser())
        {
        }
    }
}