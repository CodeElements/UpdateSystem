using System.Drawing;
using System.Windows.Forms;

namespace CodeElements.UpdateSystem.Windows.Patcher.UI.Controls
{
    [ToolboxBitmap(typeof(Label))]
    internal class CaptionLabel : Label
    {
        // Token: 0x06000003 RID: 3 RVA: 0x000020D1 File Offset: 0x000002D1
        public CaptionLabel()
        {
            Font = new Font("Segoe UI", 12f);
            ForeColor = Color.FromArgb(0, 51, 153);
        }
    }
}