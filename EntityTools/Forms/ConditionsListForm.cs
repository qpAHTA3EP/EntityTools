using Astral.Quester.Classes;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using EntityTools.Tools;
using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using EntityTools.UCC.Conditions;
using ConditionList = System.Collections.Generic.List<EntityTools.UCC.Conditions.CustomUCCCondition>;

namespace EntityTools.Forms
{
    public partial class ConditionListForm : XtraForm //*/Form
    {
        private CustomUCCCondition conditionCopy;

        public ConditionListForm()
        {
            InitializeComponent();
        }

        private void bntAdd_Click(object sender, EventArgs e)
        {
            CustomUCCCondition condition = AddAction.Show(typeof(CustomUCCCondition)) as CustomUCCCondition;
            if (condition != null)
            {
                Conditions.Items.Add(condition);
                Conditions.SelectedItem = condition;
            }

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

            if (Conditions.SelectedIndex >= 0
                && XtraMessageBox.Show("Are you sure to remove selected condition ?", "Remove Condition ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Conditions.Items.RemoveAt(Conditions.SelectedIndex);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            conditionCopy = null;
            CustomUCCCondition cond = Conditions.SelectedItem as CustomUCCCondition;
            if (cond != null)
            {
                conditionCopy = CopyHelper.CreateDeepCopy(cond); // Этот метод быстрее
                //conditionCopy = CopyHelper.CreateXmlCopy(cond); // Этот метод дольше
                XtraMessageBox.Show($"Condition '{conditionCopy.ToString()}' copied");
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (conditionCopy != null)
            {
                CustomUCCCondition cond = CopyHelper.CreateDeepCopy(conditionCopy);
                //CustomUCCCondition cond = CopyHelper.CreateXmlCopy(conditionCopy); // Этот метод дольше
                Conditions.Items.Add(cond); // Этот метод быстрее

                Conditions.SelectedItem = cond;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            CustomUCCCondition cond = (Conditions.Items.Count > 0) ? Conditions.SelectedItem as CustomUCCCondition : null;
            if (cond != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(cond.ToString()).AppendLine();
                sb.Append("Result: ").Append(cond.IsOK().ToString());

                //MessageBox.Show(sb.ToString());
                XtraMessageBox.Show(sb.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void bntCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Conditions_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.SelectedObject = Conditions.SelectedItem;
        }

        private void Conditions_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Condition condition = Conditions.SelectedItem as Condition;
            if (condition != null)
            {
                condition.Locked = (e.NewValue == CheckState.Checked);
            }
        }

        private void Properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Locked")
                Conditions.SetItemChecked(Conditions.SelectedIndex, e.ChangedItem.Value.Equals(true));
        }

        public ConditionList GetConditionList(ConditionList conditions = null)
        {
            Conditions.Items.Clear();
            if (conditions != null)
            {
                // Отображаем список условий
                foreach (CustomUCCCondition condition in conditions)
                {
                    int ind = Conditions.Items.Add(CopyHelper.CreateDeepCopy(condition));
                    Conditions.SetItemChecked(ind, condition.Locked);
                }
            }

            ShowDialog();

            if (DialogResult == DialogResult.OK)
            {
                // Формируем новый список условий
                ConditionList newConditions = new ConditionList();
                foreach (object item in Conditions.Items)
                {
                    CustomUCCCondition condition = item as CustomUCCCondition;
                    if (condition != null)
                        newConditions.Add(condition);
                }

                return newConditions;
            }

            return conditions;
        }
    }
}
