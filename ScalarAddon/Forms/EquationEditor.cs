using AstralVariables.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Astral.Quester.Forms;

namespace AstralVariables.Forms
{
    public partial class EquationEditor : Form
    {
        private static NumberExpression expression;
        private static EquationEditor equationEditor;

        public static NumberExpression GetExpression(NumberExpression val = null)
        {
            if (val != null)
                expression = val;

            if (expression == null)
                expression = new NumberExpression();

            if (equationEditor == null)
                equationEditor = new EquationEditor();

            equationEditor.tbExpression.Text = expression.Expression;

            DialogResult dResult = equationEditor.ShowDialog();
            if (dResult == DialogResult.OK)
                return expression;

            return null;
        }

        public EquationEditor()
        {
            InitializeComponent();
        }

        private void btnInsItmCnt_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetAnItem.ListItem item = GetAnItem.Show();
            if (item != null)
                tbExpression.AppendText($" {Parser.Predicates.CountItem}({item.ItemId})");
        }

        private void btnInsNumCnt_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string numeric = GetAnId.GetANumeric(null);
            if (!string.IsNullOrEmpty(numeric))
                tbExpression.AppendText($" {Parser.Predicates.CountNumeric}({numeric})");
        }

        private void btnInsRnd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tbExpression.AppendText($" {Parser.Predicates.Random}()");
        }

        private void btnInsVarible_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string var_name = VariablesEditor.GetVariable("");
            if (!string.IsNullOrEmpty(var_name))
                tbExpression.AppendText(" "+var_name);
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (expression.Parse(tbExpression.Text))
                sb.AppendLine($"Parse suceedeed!");
            else
                sb.AppendLine($"Parse faild!");

            sb.Append(expression.Description()).AppendLine();

            if(expression.IsValid)
                MessageBox.Show(this,sb.ToString(), "Suceedeed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            else MessageBox.Show(this, sb.ToString(), "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            expression.Expression = tbExpression.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
