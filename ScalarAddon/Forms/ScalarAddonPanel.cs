using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScalarAddon.Forms
{
    public partial class ScalarAddonPanel : Astral.Forms.BasePanel
    {
        public ScalarAddonPanel() :base ("ScalarAddon")
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Variable is " + AstralScalars.GetValue(String.Empty).ToString());
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            AstralScalars.SetValue(String.Empty, (int)numUdbValue.Value);
        }

        private void lblAuthor_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            AstralScalars.SetValue(String.Empty, (int)numUdbValue.Value);
        }
    }
}
