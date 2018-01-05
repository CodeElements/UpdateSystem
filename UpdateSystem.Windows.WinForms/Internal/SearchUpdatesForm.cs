using System.Windows.Forms;

namespace CodeElements.UpdateSystem.Windows.WinForms.Internal
{
    public partial class SearchUpdatesForm : Form
    {
        public SearchUpdatesForm()
        {
            InitializeComponent();
            Text = Properties.Resources.SearchUpdatesForm_Title;
            checkingLabel.Text = Properties.Resources.SearchUpdatesForm_CheckingForUpdates;
        }
    }
}