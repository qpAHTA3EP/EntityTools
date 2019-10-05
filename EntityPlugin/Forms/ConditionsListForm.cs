using Astral.Quester.Classes;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using EntityTools.Tools;
using System;
using System.Text;
using System.Windows.Forms;
using ConditionList = System.Collections.Generic.List<Astral.Quester.Classes.Condition>;

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
            Condition condition = AddAction.Show(typeof(Condition)) as Condition;
            if (condition != null)
            {
                Conditions.Items.Add(condition);
                Conditions.SelectedItem = condition;
            }
            
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (Conditions.SelectedIndex >= 0 
                && MessageBox.Show("Are you sure to delete selected condition ?", "Deleted Conditions ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Conditions.Items.RemoveAt(Conditions.SelectedIndex);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            conditionCopy = null;
            Condition cond = Conditions.SelectedItem as Condition;
            if (cond != null)
                conditionCopy = CopyHelper.CreateDeepCopy(cond); // Этот метод быстрее
                //conditionCopy = CopyHelper.CreateXmlCopy(cond); // Этот метод дольше
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (conditionCopy != null)
                Conditions.Items.Add(CopyHelper.CreateDeepCopy(conditionCopy)); // Этот метод быстрее
                //Conditions.Items.Add(CopyHelper.CreateXmlCopy(conditionCopy)); // Этот метод дольше
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Condition cond = Conditions.SelectedItem as Condition;
            if(cond != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(cond.TestInfos).AppendLine();
                sb.Append("Result: ").Append(cond.IsValid);

                MessageBox.Show(sb.ToString());
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
            if(condition != null)
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
            if(conditions != null)
            {
                // Отображаем список условий
                foreach (Condition condition in conditions)
                {
                    int ind = Conditions.Items.Add(condition);
                    Conditions.SetItemChecked(ind, condition.Locked);
                }
            }

            ShowDialog();

            if(DialogResult == DialogResult.OK)
            {
                // Формируем новый список условий
                ConditionList newConditions = new ConditionList();
                foreach(object item in Conditions.Items)
                {
                    Condition condition = item as Condition;
                    if (condition != null)
                        newConditions.Add(condition);
                }

                return newConditions;
            }

            return conditions;
        }

    }
}
