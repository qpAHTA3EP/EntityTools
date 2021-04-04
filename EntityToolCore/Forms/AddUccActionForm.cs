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
using System.Collections.Generic;

namespace EntityCore.Forms
{
    public partial class AddUccActionForm : XtraForm //*/Form
    {
        // Редактор UCC
        private readonly UCCEditor editor = null;
        private readonly InstancePropertyAccessor<UCCAction> currentUccAction = null;

        private static readonly List<Type> uccActionTypes = new List<Type>();
        private static Dictionary<Type, UCCAction> uccActions = new Dictionary<Type, UCCAction>();

        static AddUccActionForm()
        {
            // Формируем список типов uccAction;
            EntityTools.Patches.HarmonyPatch_XmlSerializer_GetExtraTypes.GetExtraTypes(out List<Type> uccTypes, 1);
            if(uccTypes != null && uccTypes.Count > 0)
            {
                foreach (Type t in uccTypes)
                    if (t.BaseType == typeof(UCCAction))
                        uccActionTypes.Add(t);
            }
        }

        public AddUccActionForm()
        {
            InitializeComponent();

            editor = Application.OpenForms.Find<UCCEditor>();
            if(editor != null)
                currentUccAction = editor.GetProperty<UCCAction>("CurrentAction");

            listOfActionTypes.DataSource = uccActionTypes;
            listOfActionTypes.DisplayMember = "Name";
        }

        public static bool GUIRequest(out UCCAction action)
        {
            AddUccActionForm @this = new AddUccActionForm();

#if disabled_20200527_1929
            if (action != null)
                @this.listOfActionTypes.SelectedItem = action.GetType();

#endif
            if (@this.ShowDialog() == DialogResult.OK
                && @this.listOfActionTypes.SelectedIndex >= 0)
            {
                if (@this.listOfActionTypes.SelectedItem is Type actionType
                    && uccActions.TryGetValue(actionType, out UCCAction newAction))
                {
                    action = newAction.Clone();
                    action.Target = newAction.Target;
                    return action != null;
                }
            }
            action = null;
            return false;
        }

        #region Interface event
#if false
        private void bntAdd_Click(object sender, EventArgs e)
        {
            //if (AddAction.Show(typeof(UCCCondition)) is UCCCondition condition)
            if (ItemSelectForm.GetAnInstanceOfType(out UCCCondition condition, false))
            {
                lsbxActions.Items.Add(condition);
                lsbxActions.SelectedItem = condition;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

            if (lsbxActions.SelectedIndex >= 0
                && XtraMessageBox.Show(/*Form.ActiveForm, */"Are you sure to remove selected condition ?", "Remove Condition ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lsbxActions.Items.RemoveAt(lsbxActions.SelectedIndex);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            conditionCopy = null;
            if (lsbxActions.SelectedItem is UCCCondition cond)
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
                lsbxActions.Items.Add(cond); // Этот метод быстрее

                lsbxActions.SelectedItem = cond;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            bool result = false;
            string mess = string.Empty;
            if (currentUccAction != null && lsbxActions.Items.Count > 0)
            {
                UCCAction action = currentUccAction;
                if (action is SpecializedUCCAction specializedUCCAction)
                    action = specializedUCCAction.ManagedAction;

                if (lsbxActions.SelectedItem is UCCCondition cond)
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
            if (currentUccAction != null && lsbxActions.Items?.Count > 0)
            {
                UCCAction action = currentUccAction;
                if (action is SpecializedUCCAction specializedUCCAction)
                    action = specializedUCCAction.ManagedAction;

                StringBuilder sb = new StringBuilder();
                foreach (UCCCondition cond in lsbxActions.Items)
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
#endif

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void bntCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ActionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listOfActionTypes.SelectedItem != null
                && listOfActionTypes.SelectedItem is Type uccActionType)
            {
                if (uccActions.TryGetValue(uccActionType, out UCCAction selecteAction))
                {
                    actionProperties.SelectedObject = selecteAction;
                    return;
                }
                else
                {
                    UCCAction action = Activator.CreateInstance(uccActionType) as UCCAction;
                    if (action != null)
                    {
                        uccActions.Add(uccActionType, action);
                        actionProperties.SelectedObject = action;
                        return;
                    }
                    else XtraMessageBox.Show($"Fail to create an Action '{uccActionType.Name}'");
                }
            }
            actionProperties.SelectedObject = null;
        }
        #endregion
    }
}
