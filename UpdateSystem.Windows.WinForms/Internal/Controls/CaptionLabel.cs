using System.Drawing;
using System.Windows.Forms;

namespace CodeElements.UpdateSystem.Windows.WinForms.Internal.Controls
{
    [ToolboxBitmap(typeof(Label))]
    internal class CaptionLabel : Label
    {
        public CaptionLabel()
        {
            Font = new Font("Segoe UI", 12f);
            ForeColor = Color.FromArgb(0, 51, 153);
        }
    }
}