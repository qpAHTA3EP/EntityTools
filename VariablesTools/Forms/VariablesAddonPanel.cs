using System;
using System.Windows.Forms;

namespace VariableTools.Forms
{
    public partial class VariablesAddonPanel : /* UserControl // */ Astral.Forms.BasePanel
    {
        public VariablesAddonPanel() :base ("Variables Tools")
        {
            InitializeComponent();
        }

        private void btnVariables_Click(object sender, EventArgs e)
        {
            VariablesEditor.Show(true);
        }
    }
}
