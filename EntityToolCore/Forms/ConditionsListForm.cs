using DevExpress.XtraEditors;
using System;
using System.Text;
using System.Windows.Forms;
using EntityTools.UCC.Conditions;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;
using Astral.Logic.UCC.Classes;
using EntityTools.Extensions;
using UCCEditor = Astral.Logic.UCC.Forms.Editor;
using EntityTools.UCC.Actions;
using AcTp0Tools.Reflection;
using EntityTools.Enums;

namespace EntityCore.Forms
{
    public partial class ConditionListForm : XtraForm
    {
        private static UCCCondition _conditionCopy;
        // Индекс элемента списка условий Conditions.Items
        // в котором допускается изменение состояния "Checked"
        private int allowConditionsItemChechedChangeInd = -1;
        // Редактор UCC
        private readonly PropertyAccessor<UCCAction> currentUccAction;

        public ConditionListForm()
        {
            InitializeComponent();

            var editor = Application.OpenForms.Find<UCCEditor>();
            if(editor != null)
                currentUccAction = editor.GetProperty<UCCAction>("CurrentAction");
            if (currentUccAction == null)
            {
                // Отключение кнопок тестирования, если окно редактора не нейдено
                btnTest.Enabled = false;
                btnTestAll.Enabled = false;
            }

            cbLogic.Properties.Items.AddRange(Enum.GetValues(typeof(LogicRule)));
        }

        public static ConditionList UserRequest(ConditionList conditions = null)
        {
            ConditionListForm @this = new ConditionListForm();

            @this.lsbxConditions.Items.Clear();
            if (conditions != null)
            {
                // Отображаем список условий
                foreach (UCCCondition condition in conditions)
                {
                    @this.allowConditionsItemChechedChangeInd = @this.lsbxConditions.Items.Add(CopyHelper.CreateDeepCopy(condition));
                    @this.lsbxConditions.SetItemChecked(@this.allowConditionsItemChechedChangeInd, condition.Locked);
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

        public static bool UserRequest(ref ConditionList conditions, ref LogicRule logic, ref bool negation)
        {
            ConditionListForm @this = new ConditionListForm();

            @this.lsbxConditions.Items.Clear();
            if (conditions != null)
            {
                // Отображаем список условий
                foreach (UCCCondition condition in conditions)
                {
                    @this.allowConditionsItemChechedChangeInd = @this.lsbxConditions.Items.Add(CopyHelper.CreateDeepCopy(condition));
                    @this.lsbxConditions.SetItemChecked(@this.allowConditionsItemChechedChangeInd, condition.Locked);
                }
            }
            @this.cbLogic.SelectedItem = logic;
            @this.cheNegation.Checked = negation;

            if (@this.ShowDialog() == DialogResult.OK)
            {
                // Формируем новый список условий
                ConditionList newConditions = new ConditionList(@this.lsbxConditions.Items.Count);
                foreach (object item in @this.lsbxConditions.Items)
                {
                    if (item is UCCCondition condition)
                        newConditions.Add(condition);
                }

                logic = (LogicRule) @this.cbLogic.SelectedItem;
                negation = @this.cheNegation.Checked;
                conditions = newConditions;

                return true;
            }

            return false;
        }

        public static bool UserRequest(ref UCCConditionPack conditionPack)
        {
            ConditionListForm @this = new ConditionListForm
            {
                cbLogic = {Visible = false},
                cheNegation = {Visible = false},
                lsbxConditions = {Dock = DockStyle.Fill}
            };


            @this.lsbxConditions.Items.Clear();
            if (conditionPack != null)
            {
                @this.cbLogic.SelectedItem = conditionPack.TestRule;
                @this.cheNegation.Checked = conditionPack.Not;
                // Отображаем список условий
                foreach (UCCCondition condition in conditionPack.Conditions)
                {
                    @this.allowConditionsItemChechedChangeInd = @this.lsbxConditions.Items.Add(CopyHelper.CreateDeepCopy(condition));
                    @this.lsbxConditions.SetItemChecked(@this.allowConditionsItemChechedChangeInd, condition.Locked);
                }
            }

            if (@this.ShowDialog() == DialogResult.OK)
            {
                // Формируем новый список условий
                var newConditionPack = new UCCConditionPack
                {
                    Not = @this.cheNegation.Checked,
                    TestRule = (LogicRule)@this.cbLogic.SelectedItem
                };
                var newConditions = newConditionPack.Conditions;
                foreach (object item in @this.lsbxConditions.Items)
                {
                    if (item is UCCCondition condition)
                        newConditions.Add(condition);
                }
                conditionPack = newConditionPack;
                return true;
            }

            return false;
        }

        #region Interface event
        private void handler_Add(object sender, EventArgs e)
        {
            //if (AddAction.Show(typeof(UCCCondition)) is UCCCondition condition)
            if (ItemSelectForm.GetAnInstanceOfType(out UCCCondition condition, false))
            {
                lsbxConditions.Items.Add(condition);
                lsbxConditions.SelectedItem = condition;
            }
        }

        private void handler_Remove(object sender, EventArgs e)
        {

            if (lsbxConditions.SelectedIndex >= 0
                && XtraMessageBox.Show("Are you sure to remove selected condition ?", "Remove Condition ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lsbxConditions.Items.RemoveAt(lsbxConditions.SelectedIndex);
            }
        }

        private void handler_Copy(object sender, EventArgs e)
        {
            _conditionCopy = null;
            if (lsbxConditions.SelectedItem is UCCCondition cond)
            {
                _conditionCopy = CopyHelper.CreateDeepCopy(cond); 
                XtraMessageBox.Show($"Condition '{_conditionCopy}' copied");
            }
        }

        private void handler_Paste(object sender, EventArgs e)
        {
            if (_conditionCopy != null)
            {
                UCCCondition cond = CopyHelper.CreateDeepCopy(_conditionCopy);
                lsbxConditions.Items.Add(cond);

                lsbxConditions.SelectedItem = cond;
            }
        }

        private void handler_Test(object sender, EventArgs e)
        {
            if (currentUccAction != null && lsbxConditions.Items.Count > 0)
            {
                UCCAction action = currentUccAction;
                if (action is SpecializedUCCAction specializedUccAction)
                    action = specializedUccAction.ManagedAction;

                if (lsbxConditions.SelectedItem is UCCCondition cond)
                {
                    bool result;
                    string msg;
                    if (cond is ICustomUCCCondition iCond)
                    {
                        result = iCond.IsOK(action);
                        msg = iCond.TestInfos(action);
                    }
                    else
                    {
                        result = cond.IsOK(action);
                        msg = $"{cond.Target} {cond.Tested} : {cond.Value}";
                    }
                    XtraMessageBox.Show($"{msg}\nResult: {result}");
                }
                else XtraMessageBox.Show("Can't test selected object!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void handler_TestAll(object sender, EventArgs e)
        {
            if (currentUccAction != null && lsbxConditions.Items.Count > 0)
            {
                UCCAction action = currentUccAction;
                if (action is SpecializedUCCAction specializedUccAction)
                    action = specializedUccAction.ManagedAction;

                StringBuilder sb = new StringBuilder();
                foreach (UCCCondition cond in lsbxConditions.Items)
                {
                    if (cond.Locked)
                        sb.Append("[L] ");
                    else sb.Append("[U] ");
                    if (cond is ICustomUCCCondition iCond)
                        sb.Append(iCond).Append(" | Result: ").Append(iCond.IsOK(action)).AppendLine();
                    else sb.Append(cond).Append(" | Result: ").Append(cond.IsOK(action)).AppendLine();
                }
                XtraMessageBox.Show(sb.ToString());
            }
            else XtraMessageBox.Show("The list 'Conditions' is empty");

        }

        private void handler_Save(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void handler_Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void handler_SelectedConditionChanged(object sender, EventArgs e)
        {
            //if(Conditions.SelectedIndex >= 0  && Conditions.SelectedIndex < Conditions.Items.Count )
            //    Properties.SelectedObject = Conditions.Items[Conditions.SelectedIndex];
            Properties.SelectedObject = lsbxConditions.SelectedItem;
        }

        private void handler_LockCondition(object sender, ItemCheckEventArgs e)
        {
            // изменение состояния Checked у элемента разрешено, 
            // если его индекс совпадает с AllowConditionsItemChechedChangeInd
            if (e.Index != allowConditionsItemChechedChangeInd
                || allowConditionsItemChechedChangeInd < 0)
            {
                // запрет изменения состояния Чекбокса
                e.NewValue = e.CurrentValue;
            }
            allowConditionsItemChechedChangeInd = -1;
        }

        private void handler_ConditionPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Locked")
            {
                allowConditionsItemChechedChangeInd = lsbxConditions.SelectedIndex;
                //Conditions.ItemCheck -= Conditions_ItemCheck;
                lsbxConditions.SetItemChecked(lsbxConditions.SelectedIndex, e.ChangedItem.Value.Equals(true));
                //Conditions.ItemCheck += Conditions_ItemCheck;
            }
            lsbxConditions.Refresh();
        }
        #endregion
    }
}
