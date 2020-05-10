using DevExpress.XtraEditors;
using System;
using System.Text;
using System.Windows.Forms;
using EntityTools.UCC.Conditions;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;
using Astral.Logic.UCC.Classes;
using EntityTools.Extensions;
using EntityTools.Reflection;
using UCCEditor = Astral.Logic.UCC.Forms.Editor;
using EntityTools.UCC.Actions;

namespace EntityCore.Forms
{
    public partial class ConditionListForm : XtraForm //*/Form
    {
        private static UCCCondition conditionCopy;
        // Индекс элемента списка условий Conditions.Items
        // в котором допускается изменение состояния "Checked"
        private int AllowConditionsItemChechedChangeInd = -1;
        // Редактор UCC
        private readonly UCCEditor editor = null;
        private readonly InstancePropertyAccessor<UCCEditor, UCCAction> currentUccAction = null;

        public ConditionListForm()
        {
            InitializeComponent();

            editor = Application.OpenForms.Find<UCCEditor>();
            if(editor != null)
                currentUccAction = editor.GetInstanceProperty<UCCEditor, UCCAction>("CurrentAction");
            if(currentUccAction == null)
            {
                // Отключение кнопок тестирования, если окно редактора не нейдено
                btnTest.Enabled = false;
                btnTestAll.Enabled = false;
            }
        }

        public static ConditionList GUIRequest(ConditionList conditions = null)
        {
            ConditionListForm @this = new ConditionListForm();

            @this.lsbxConditions.Items.Clear();
            if (conditions != null)
            {
                // Отображаем список условий
                foreach (UCCCondition condition in conditions)
                {
                    @this.AllowConditionsItemChechedChangeInd = @this.lsbxConditions.Items.Add(CopyHelper.CreateDeepCopy(condition));
                    @this.lsbxConditions.SetItemChecked(@this.AllowConditionsItemChechedChangeInd, condition.Locked);
                }
            }

            if (@this.ShowDialog() == DialogResult.OK)
            {
                // Формируем новый список условий
                ConditionList newConditions = new ConditionList(@this.lsbxConditions.Items.Count);
                foreach (object item in @this.lsbxConditions.Items)
                {
                    if (item is UCCCondition condition)
                        newConditions.Add(condition);
                }

                return newConditions;
            }

            return conditions;
        }

        #region Interface event
        private void bntAdd_Click(object sender, EventArgs e)
        {
            //if (AddAction.Show(typeof(UCCCondition)) is UCCCondition condition)
            if (ItemSelectForm.GetAnInstanceOfType(out UCCCondition condition, false))
            {
                lsbxConditions.Items.Add(condition);
                lsbxConditions.SelectedItem = condition;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

            if (lsbxConditions.SelectedIndex >= 0
                && XtraMessageBox.Show(/*Form.ActiveForm, */"Are you sure to remove selected condition ?", "Remove Condition ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lsbxConditions.Items.RemoveAt(lsbxConditions.SelectedIndex);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            conditionCopy = null;
            if (lsbxConditions.SelectedItem is UCCCondition cond)
            {
                conditionCopy = CopyHelper.CreateDeepCopy(cond); // Этот метод быстрее
                //conditionCopy = CopyHelper.CreateXmlCopy(cond); // Этот метод дольше
                XtraMessageBox.Show(/*Form.ActiveForm, */$"Condition '{conditionCopy.ToString()}' copied");
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (conditionCopy != null)
            {
                UCCCondition cond = CopyHelper.CreateDeepCopy(conditionCopy);
                //CustomUCCCondition cond = CopyHelper.CreateXmlCopy(conditionCopy); // Этот метод дольше
                lsbxConditions.Items.Add(cond); // Этот метод быстрее

                lsbxConditions.SelectedItem = cond;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            bool result = false;
            string mess = string.Empty;
            if (currentUccAction != null && lsbxConditions.Items.Count > 0)
            {
                UCCAction action = currentUccAction;
                if (action is SpecializedUCCAction specializedUCCAction)
                    action = specializedUCCAction.ManagedAction;

                if (lsbxConditions.SelectedItem is UCCCondition cond)
                {
                    if (cond is ICustomUCCCondition iCond)
                    {
                        result = iCond.IsOK(action);
                        mess = iCond.TestInfos(action);
                    }
                    else
                    {
                        result = cond.IsOK(action);
                        mess = $"{cond.Target} {cond.Tested} : {cond.Value}";
                    }
                    XtraMessageBox.Show(/*Form.ActiveForm, */$"{mess}\nResult: {result}");
                }
                else XtraMessageBox.Show(/*Form.ActiveForm, */"Can't test selected object!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTestAll_Click(object sender, EventArgs e)
        {
            if (currentUccAction != null && lsbxConditions.Items?.Count > 0)
            {
                UCCAction action = currentUccAction;
                if (action is SpecializedUCCAction specializedUCCAction)
                    action = specializedUCCAction.ManagedAction;

                StringBuilder sb = new StringBuilder();
                foreach (UCCCondition cond in lsbxConditions.Items)
                {
                    if (cond.Locked)
                        sb.Append("[L] ");
                    else sb.Append("[U] ");
                    if (cond is ICustomUCCCondition iCond)
                        sb.Append(iCond.ToString()).Append(" | Result: ").Append(iCond.IsOK(action)).AppendLine();
                    else sb.Append(cond.ToString()).Append(" | Result: ").Append(cond.IsOK(action)).AppendLine();
                }
                XtraMessageBox.Show(sb.ToString());
            }
            else XtraMessageBox.Show("The list 'Conditions' is empty");

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
            //if(Conditions.SelectedIndex >= 0  && Conditions.SelectedIndex < Conditions.Items.Count )
            //    Properties.SelectedObject = Conditions.Items[Conditions.SelectedIndex];
            Properties.SelectedObject = lsbxConditions.SelectedItem;
        }

        private void Conditions_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // изменение состояния Checked у элемента разрешено, 
            // если его индекс совпадает с AllowConditionsItemChechedChangeInd
            if (e.Index != AllowConditionsItemChechedChangeInd
                || AllowConditionsItemChechedChangeInd < 0)
            {
                // запрет изменения состояния Чекбокса
                e.NewValue = e.CurrentValue;
            }
            AllowConditionsItemChechedChangeInd = -1;
        }

        private void Properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Locked")
            {
                AllowConditionsItemChechedChangeInd = lsbxConditions.SelectedIndex;
                //Conditions.ItemCheck -= Conditions_ItemCheck;
                lsbxConditions.SetItemChecked(lsbxConditions.SelectedIndex, e.ChangedItem.Value.Equals(true));
                //Conditions.ItemCheck += Conditions_ItemCheck;
            }
            lsbxConditions.Refresh();
        }
        #endregion
    }
}
