using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ValiablesAstralExtention.Forms
{
    public partial class ScalarAddonPanel : Astral.Forms.BasePanel
    {
        public ScalarAddonPanel() :base ("VariablesAddon")
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Variable is " + VariablesAddon.GetValue(String.Empty).ToString());
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            VariablesAddon.SetValue(String.Empty, (int)numUdbValue.Value);
        }

        private void lblAuthor_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            VariablesAddon.SetValue(String.Empty, (int)numUdbValue.Value);
        }
    }
}
