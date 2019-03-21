using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AstralVars.Classes;

namespace AstralVars.Forms
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

            VariablesAddon.Variables.Add(Variable.Make("Int", 99));
            VariablesAddon.Variables.Add(Variable.Make("intSt", "101"));
            VariablesAddon.Variables.Add(Variable.Make("bl", true));
            VariablesAddon.Variables.Add(Variable.Make("bls", "false"));
            VariablesAddon.Variables.Add(Variable.Make("dt", DateTime.UtcNow));
            VariablesAddon.Variables.Add(Variable.Make("dts", "20.01.2019"));
            VariablesAddon.Variables.Add(Variable.Make("vss", "Super"));
            VariablesAddon.Variables.Add(Variable.Make("12", "Count[Artifactfood]"));
            VariablesAddon.Variables.Add(Variable.Make("15", "Counter[Gemfood]"));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.Boolean));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.Number));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.DateTime));

            //StringBuilder strBldr = new StringBuilder();
            //foreach (Variable item in VariablesAddon.Variables)
            //{
            //    strBldr.AppendLine($"Variables[{item.Key}] = {item.Value} type of {item.VarType}. It represent as string like '{item.ToString()}'");
            //}

            //MessageBox.Show(strBldr.ToString());   

            Variable var = VariablesEditor.GetVariable(VariablesAddon.Variables);

            if (var != null)
                MessageBox.Show($"Variables[{var.Key}] = {var.Value} type of {var.VarType}.\n" +
                                $" It represent as string like '{var.ToString()}'");
        }
    }
}
