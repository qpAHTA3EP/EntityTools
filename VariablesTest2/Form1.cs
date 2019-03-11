#define TEST3

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using AstralVars;
using AstralVars.Classes;
using AstralVars.Forms;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace VariablesTest2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cbType.DataSource = VariablesParcer.varTypes;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder strBldr = new StringBuilder();

#region [Тест 1] Проверка работы шаблонов      
#if TEST1
            string text = "ItemsCount(Aaafd)";

            strBldr.Clear();
            if (Regex.IsMatch(text, VariablesParcer.counterPattern))
                strBldr.AppendLine($"String '{text}' matches to pattern [{VariablesParcer.counterPattern}]");
            else strBldr.AppendLine($"String '{text}' does not matche to pattern [{VariablesParcer.counterPattern}]");

            string trimedText = Regex.Replace(text, VariablesParcer.counterTrimPattern, string.Empty);
            strBldr.AppendLine($"Trimed string '{text}' is '{trimedText}'");
            
            MessageBox.Show(strBldr.ToString());
#endif
#endregion


#region [Тест 2] Проверка работы фабрики объектов-переменных
#if TEST2
            VariablesAddon.Variables.Clear();

            VariablesAddon.Variables.Add(Variable.Make("Int", 99));
            VariablesAddon.Variables.Add(Variable.Make("intSt", "101"));
            VariablesAddon.Variables.Add(Variable.Make("bl", true));
            VariablesAddon.Variables.Add(Variable.Make("bls", "false"));
            VariablesAddon.Variables.Add(Variable.Make("dt", DateTime.UtcNow));
            VariablesAddon.Variables.Add(Variable.Make("dts", "20.01.2019"));
            VariablesAddon.Variables.Add(Variable.Make("vss", "Super"));
            VariablesAddon.Variables.Add(Variable.Make("12", "Count[Artifactfood]"));
            VariablesAddon.Variables.Add(Variable.Make("15", "Counter(Gemfood)"));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.Boolean));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.Integer));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.DateTime));

            strBldr.Clear();
            foreach(Variable item in VariablesAddon.Variables)
            {
                strBldr.AppendLine($"Variables[{item.Key}] = {item.Value} type of {item.VarType}. It represent as string like '{item.ToString()}'");
            }
            MessageBox.Show(strBldr.ToString());
#endif
#endregion

#region [Тест 3] Проверка формы редактора переменных
#if TEST3
            VariablesAddon.Variables.Clear();

            VariablesAddon.Variables.Add(Variable.Make("Int", 99));
            VariablesAddon.Variables.Add(Variable.Make("intSt", "101"));
            VariablesAddon.Variables.Add(Variable.Make("bl", true));
            VariablesAddon.Variables.Add(Variable.Make("bls", "false"));
            VariablesAddon.Variables.Add(Variable.Make("dt", DateTime.UtcNow));
            VariablesAddon.Variables.Add(Variable.Make("dts", "20.01.2019"));
            VariablesAddon.Variables.Add(Variable.Make("vss", "Super"));
            VariablesAddon.Variables.Add(Variable.Make("12", "Count[Artifactfood]"));
            VariablesAddon.Variables.Add(Variable.Make("15", "Counter(Gemfood)"));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.Boolean));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.Integer));
            VariablesAddon.Variables.Add(Variable.Make(VarTypes.DateTime));

            Variable var = VariablesEditor.GetVariable(VariablesAddon.Variables);
#endif
#endregion            
        }
    }
}
