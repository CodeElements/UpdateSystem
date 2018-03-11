namespace CodeElements.UpdateSystem.Windows.WinForms
{
    /// <summary>
    ///     The patcher for Windows Forms
    /// </summary>
    public class WinFormsPatcher : WindowsPatcher
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="WinFormsPatcher" /> using the <see cref="WindowsFormsApplicationCloser" />
        /// </summary>
        public WinFormsPatcher() : base(new WindowsFormsApplicationCloser()) { }
    }
}