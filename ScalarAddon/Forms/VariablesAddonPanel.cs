using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ValiablesAstralExtention.Classes;

namespace ValiablesAstralExtention.Forms
{
    public partial class VariablesAddonPanel : Astral.Forms.BasePanel
    {
        public VariablesAddonPanel() :base ("VariablesAddon")
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            VariablesAddon.Variables.Clear();

            VariablesAddon.Variables.Add(VariableItem.Make("Int", 99));
            VariablesAddon.Variables.Add(VariableItem.Make("intSt", "101"));
            VariablesAddon.Variables.Add(VariableItem.Make("bl", true));
            VariablesAddon.Variables.Add(VariableItem.Make("bls", "false"));
            VariablesAddon.Variables.Add(VariableItem.Make("dt", DateTime.UtcNow));
            VariablesAddon.Variables.Add(VariableItem.Make("dts", "20.01.2019"));
            VariablesAddon.Variables.Add(VariableItem.Make("vss", "Super"));
            VariablesAddon.Variables.Add(VariableItem.Make("12", "Count[Artifactfood]"));
            VariablesAddon.Variables.Add(VariableItem.Make("15", "ItemsCount[Gemfood]"));
            VariablesAddon.Variables.Add(VariableItem.Make(VariableTypes.Boolean));
            VariablesAddon.Variables.Add(VariableItem.Make(VariableTypes.Integer));
            VariablesAddon.Variables.Add(VariableItem.Make(VariableTypes.DateTime));

            //StringBuilder strBldr = new StringBuilder();
            //foreach (VariableItem item in VariablesAddon.Variables)
            //{
            //    strBldr.AppendLine($"Variables[{item.Key}] = {item.Value} type of {item.VarType}. It represent as string like '{item.ToString()}'");
            //}

            //MessageBox.Show(strBldr.ToString());   

            VariableItem var = VariablesEditor.GetVariable(VariablesAddon.Variables);

            if (var != null)
                MessageBox.Show($"Variables[{var.Key}] = {var.Value} type of {var.VarType}.\n It represent as string like '{var.ToString()}'");
        }
    }
}
