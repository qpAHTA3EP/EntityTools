using System;
using System.Text;
using System.Windows.Forms;
using Infrastructure.Reflection;
using Astral.Logic.UCC.Classes;
using DevExpress.XtraEditors;
using EntityTools.Forms;
using EntityTools.Enums;
using EntityTools.Tools.Extensions;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;
using ConditionList = System.Collections.ObjectModel.ObservableCollection<Astral.Logic.UCC.Classes.UCCCondition>;
using UCCEditor = Astral.Logic.UCC.Forms.Editor;

namespace EntityTools.Forms
{
    public partial class ConditionListForm : XtraForm
    {
        private static UCCCondition _conditionCopy;
        // Индекс элемента списка условий Conditions.Items
        // в котором допускается изменение состояния "Checked"
        private int allowConditionsItemCheckedChangeInd = -1;
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
                // Отключение кнопок тестирования, если окно редактора не найдено
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
                foreach (UCCCondition cnd in conditions)
                {
                    @this.allowConditionsItemCheckedChangeInd = @this.lsbxConditions.Items.Add(cnd is ICustomUCCCondition cstCnd ? (UCCCondition)cstCnd.Clone() : cnd.Clone());
                    @this.lsbxConditions.SetItemChecked(@this.allowConditionsItemCheckedChangeInd, cnd.Locked);
                }
            }

            if (@this.ShowDialog() == DialogResult.OK)
            {
                // Формируем новый список условий
                ConditionList newConditionList = new ConditionList();
                foreach (object item in @this.lsbxConditions.Items)
                {
                    if (item is UCCCondition cnd)
                        newConditionList.Add(cnd);
                }

                return newConditionList;
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
                foreach (UCCCondition cnd in conditions)
                {
                    // BUG Использование CopyHelper.CreateDeepCopy приводит к StackOverflow
                    @this.allowConditionsItemCheckedChangeInd = @this.lsbxConditions.Items.Add(cnd is ICustomUCCCondition cstCnd ? (UCCCondition)cstCnd.Clone() : cnd.Clone());
                    @this.lsbxConditions.SetItemChecked(@this.allowConditionsItemCheckedChangeInd, cnd.Locked);
                }
            }
            @this.cbLogic.SelectedItem = logic;
            @this.cheNegation.Checked = negation;

            if (@this.ShowDialog() == DialogResult.OK)
            {
                //BUG : Неправильное копирование последнего условия из списка
                // Формируем новый список условий
                ConditionList newConditions = new ConditionList();
                foreach (object item in @this.lsbxConditions.Items)
                {
                    if (item is UCCCondition cnd)
                        newConditions.Add(cnd);
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
                foreach (UCCCondition cnd in conditionPack.Conditions)
                {
                    @this.allowConditionsItemCheckedChangeInd = @this.lsbxConditions.Items.Add(cnd is ICustomUCCCondition cstCnd ? (UCCCondition)cstCnd.Clone() : cnd.Clone());
                    @this.lsbxConditions.SetItemChecked(@this.allowConditionsItemCheckedChangeInd, cnd.Locked);
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
                    if (item is UCCCondition cnd)
                        newConditions.Add(cnd);
                }
                conditionPack = newConditionPack;
                return true;
            }

            return false;
        }

        #region Interface event
        private void handler_Add(object sender, EventArgs e)
        {
            if (ItemSelectForm.GetAnInstance(out UCCCondition condition, false))
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
                _conditionCopy = cond.CreateXmlCopy(); 
                XtraMessageBox.Show($"Condition '{_conditionCopy}' copied");
            }
        }

        private void handler_Paste(object sender, EventArgs e)
        {
            if (_conditionCopy != null)
            {
                UCCCondition cond = _conditionCopy.CreateXmlCopy();
                allowConditionsItemCheckedChangeInd = lsbxConditions.Items.Add(cond, cond.Locked);
                lsbxConditions.SetItemChecked(allowConditionsItemCheckedChangeInd, cond.Locked);
                lsbxConditions.SelectedItem = cond;

                lsbxConditions.Refresh();
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
            Properties.SelectedObject = lsbxConditions.SelectedItem;
        }

        private void handler_LockCondition(object sender, ItemCheckEventArgs e)
        {
            // изменение состояния Checked у элемента разрешено, 
            // если его индекс совпадает с AllowConditionsItemChechedChangeInd
            if (e.Index != allowConditionsItemCheckedChangeInd
                || allowConditionsItemCheckedChangeInd < 0)
            {
                // запрет изменения состояния Чекбокса
                e.NewValue = e.CurrentValue;
            }
            allowConditionsItemCheckedChangeInd = -1;
        }

        private void handler_ConditionPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Locked")
            {
                allowConditionsItemCheckedChangeInd = lsbxConditions.SelectedIndex;
                lsbxConditions.SetItemChecked(lsbxConditions.SelectedIndex, e.ChangedItem.Value.Equals(true));
            }
            lsbxConditions.Refresh();
        }
        #endregion
    }
}
