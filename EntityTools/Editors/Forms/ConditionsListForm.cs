using Astral.Quester.Classes;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using EntityTools.Tools;
using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using EntityTools.UCC.Conditions;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;
using Astral.Logic.UCC.Classes;
using Astral.Controllers;
using System.Collections.Generic;
using EntityTools.Editors.Forms;

namespace EntityTools.Forms
{
    public partial class ConditionListForm : XtraForm //*/Form
    {
        private static UCCCondition conditionCopy;

        public ConditionListForm()
        {
            InitializeComponent();
        }

        private void bntAdd_Click(object sender, EventArgs e)
        {
            //if (AddAction.Show(typeof(UCCCondition)) is UCCCondition condition)
            if(UnoSelectForm.GetAnItem<UCCCondition>(false) is UCCCondition condition)
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
            if (Conditions.SelectedItem is UCCCondition cond)
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
                UCCCondition cond = CopyHelper.CreateDeepCopy(conditionCopy);
                //CustomUCCCondition cond = CopyHelper.CreateXmlCopy(conditionCopy); // Этот метод дольше
                Conditions.Items.Add(cond); // Этот метод быстрее

                Conditions.SelectedItem = cond;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            bool result = false;
            string mess = string.Empty;
            if (Conditions.Items.Count > 0)
            {
                UCCCondition c = Conditions.SelectedItem as UCCCondition;
                if (c is ICustomUCCCondition iCond)
                {
                    result = iCond.IsOK();
                    mess = iCond.TestInfos();
                }
                else
                {
                    result = c.IsOK(null);
                    mess = $"{c.Target} {c.Tested} : {c.Value}";
                }

                XtraMessageBox.Show($"{mess}\nResult: {result}");
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
                foreach (UCCCondition condition in conditions)
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
                    if (item is UCCCondition condition)
                        newConditions.Add(condition);
                }

                return newConditions;
            }

            return conditions;
        }
    }
}
