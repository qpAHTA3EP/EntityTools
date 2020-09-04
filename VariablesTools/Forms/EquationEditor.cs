using VariableTools.Expressions;
using System;
using System.Text;
using System.Windows.Forms;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using VariableTools.Classes;

namespace VariableTools.Forms
{
    public partial class EquationEditor : XtraForm //*/Form
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

            equationEditor.tbExpression.Text = expression.Text;

            DialogResult dResult = equationEditor.ShowDialog();
            if (dResult == DialogResult.OK)
                return expression;

            return null;
        }

        public EquationEditor()
        {
            InitializeComponent();
        }

        #region GUI Events
        #region Insert Events
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
            VariableCollection.VariableKey key = VariablesSelectForm.GetVariable();

            if (key != null)
            {
                tbExpression.AppendText(" " + key.ToString());
            }
        }

        private void btnInsNow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tbExpression.AppendText($" {Parser.Predicates.Now}()");
        }

        private void btnInsDays_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tbExpression.AppendText($" {Parser.Predicates.Days}()");
        }

        private void btnInsHours_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tbExpression.AppendText($" {Parser.Predicates.Hours}()");
        }

        private void btnInsMinutes_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tbExpression.AppendText($" {Parser.Predicates.Minutes}()");
        }

        private void btnInsSeconds_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tbExpression.AppendText($" {Parser.Predicates.Seconds}()");
        }
        #endregion

        private void btnValidate_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string caption;
            bool ok = NumberExpression.Validate(tbExpression.Text, out string description);
            if (ok)
            {
                sb.AppendLine($"Equation is correct!");
                caption = "Suceedeed";
            }
            else
            {
                sb.AppendLine($"Equation is incorrect!");
                caption = "Fail";
            }
            sb.Append(description);
            sb.AppendLine();
            if (ok)
                XtraMessageBox.Show(this, sb.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            else XtraMessageBox.Show(this, sb.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            expression.Text = tbExpression.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}
