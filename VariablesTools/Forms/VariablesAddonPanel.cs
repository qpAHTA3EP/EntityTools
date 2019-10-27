using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AstralVariables.Classes;
using AstralVariables.Expressions;

namespace AstralVariables.Forms
{
    public partial class VariablesAddonPanel : /* UserControl // */ Astral.Forms.BasePanel
    {
        public VariablesAddonPanel() :base ("Variables Tools")
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            NumberExpression numExpr = EquationEditor.GetExpression();

            if (numExpr != null)
                MessageBox.Show(numExpr.Expression, numExpr.Description());
        }
    }
}
