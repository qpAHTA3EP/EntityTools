using Astral.Quester.Classes;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ConditionList = System.Collections.Generic.List<Astral.Quester.Classes.Condition>;
using Astral.Logic.UCC.Classes;
using EntityTools.Tools;

namespace EntityTools.Forms
{
    public partial class ConditionListForm : XtraForm //*/Form
    {
        private Condition conditionCopy;

        public ConditionListForm()
        {
            InitializeComponent();
        }

        private void bntAdd_Click(object sender, EventArgs e)
        {
            if (AddAction.Show(typeof(UCCCondition)) is UCCCondition condition)
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
            if (Conditions.SelectedItem is Condition cond)
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
                Condition cond = CopyHelper.CreateDeepCopy(conditionCopy);
                //CustomUCCCondition cond = CopyHelper.CreateXmlCopy(conditionCopy); // Этот метод дольше
                Conditions.Items.Add(cond); // Этот метод быстрее

                Conditions.SelectedItem = cond;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (Conditions.Items.Count > 0
                && Conditions.SelectedItem is Condition cond)
            {
                if (cond.IsValid)
                    XtraMessageBox.Show(string.Concat(cond, "\nResult: True"));
                else
                    XtraMessageBox.Show(string.Concat(cond, "\nResult: False"));

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
            if (Conditions.SelectedItem is UCCCondition condition)
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
                foreach (Condition condition in conditions)
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
                    if (item is Condition condition)
                        newConditions.Add(condition);
                }

                return newConditions;
            }

            return conditions;
        }
    }
}
