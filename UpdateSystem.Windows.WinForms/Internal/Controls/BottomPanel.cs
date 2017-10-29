using System.Drawing;
using System.Windows.Forms;

namespace CodeElements.UpdateSystem.Windows.WinForms.Internal.Controls
{
    internal class BottomPanel : Panel
    {
        public BottomPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer,
                true);
            Size = new Size(50, 29);
            BackColor = SystemColors.Control;
            Paint += BottomPanel_Paint;
            Dock = DockStyle.Bottom;
        }

        private void BottomPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Brushes.LightGray, 1f), 0, 1, Width, 1);
        }
    }
}