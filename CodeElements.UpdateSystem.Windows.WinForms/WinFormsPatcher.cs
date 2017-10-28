namespace CodeElements.UpdateSystem.Windows.WinForms
{
    public class WinFormsPatcher : WindowsPatcher
    {
        public WinFormsPatcher() : base(new WindowsFormsApplicationCloser())
        {
        }
    }
}
